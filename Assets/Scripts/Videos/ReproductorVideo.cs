using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Reproduce un video a pantalla completa (cutscene/intro) sobre un Canvas propio.
/// - Se puede saltar con cualquier tecla/click si saltarConCualquierTecla = true
/// - Dispara alTerminar (UnityEvent) al acabar o al saltarlo: ahí engancha la carga de escena.
///
/// USO:
/// 1. El .mp4 debe estar en Assets/Videos (Unity lo importa como VideoClip)
/// 2. Crear un GameObject vacío en la escena y añadir este componente
/// 3. Arrastrar el VideoClip al campo "Clip" y dar Play
/// 4. En "Al Terminar" conectar por ejemplo GameManager.CargarSiguienteEscena
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class ReproductorVideo : MonoBehaviour
{
    [Tooltip("Video a reproducir (asset .mp4/.mov dentro del proyecto).")]
    public VideoClip clip;

    [Tooltip("Permitir saltar el video con cualquier tecla o click.")]
    public bool saltarConCualquierTecla = true;

    [Tooltip("Segundos de fundido de salida al terminar.")]
    public float fundidoSalida = 0.6f;

    [Tooltip("Se invoca cuando el video termina o es saltado.")]
    public UnityEvent alTerminar;

    // ---- MONTAJE AUTOMÁTICO DE LA INTRO ----
    // Se ejecuta una sola vez al abrir el juego (antes de la primera escena).
    // Busca SIEMPRE el clip en Resources/Videos/intro: para cambiar la intro
    // basta con reemplazar ese archivo .mp4 manteniendo el nombre.
    // Si NO hay video o algo falla => salta directo a la escena del carro.
    private const string EscenaSinVideo = "Carro y salida";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void MontarIntroAutomatica()
    {
        try
        {
            VideoClip intro = Resources.Load<VideoClip>("Videos/intro");
            if (intro == null)
            {
                Debug.LogWarning("[ReproductorVideo] No hay Assets/Resources/Videos/intro.mp4; saltando a '" + EscenaSinVideo + "'.");
                SceneManager.LoadScene(EscenaSinVideo);
                return;
            }

            GameObject go = new GameObject("VISUAL_IntroVideo");
            Object.DontDestroyOnLoad(go);
            ReproductorVideo reproductor = go.AddComponent<ReproductorVideo>();
            reproductor.clip = intro;
            reproductor.saltarConCualquierTecla = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ReproductorVideo] Error preparando la intro: " + e.Message + ". Saltando a '" + EscenaSinVideo + "'.");
            SceneManager.LoadScene(EscenaSinVideo);
        }
    }

    private VideoPlayer _player;
    private RawImage _pantalla;
    private CanvasGroup _grupo;
    private RenderTexture _rt;
    private bool _terminado;

    private void Start()
    {
        try
        {
            if (clip == null)
                throw new System.Exception("Sin clip asignado");

            ConstruirUI();
            ConfigurarPlayer();
        }
        catch (System.Exception e)
        {
            Debug.LogError("[ReproductorVideo] No se pudo reproducir el video: " + e.Message + ". Saltando a '" + EscenaSinVideo + "'.");
            SceneManager.LoadScene(EscenaSinVideo);
            Destroy(gameObject);
        }
    }

    private void ConstruirUI()
    {
        GameObject canvasGo = new GameObject("VISUAL_VideoCanvas");
        canvasGo.transform.SetParent(transform, false);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500; // encima de todo lo demás
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        _grupo = canvasGo.AddComponent<CanvasGroup>();

        GameObject imgGo = new GameObject("Pantalla");
        imgGo.transform.SetParent(canvasGo.transform, false);
        _pantalla = imgGo.AddComponent<RawImage>();
        _pantalla.rectTransform.anchorMin = Vector2.zero;
        _pantalla.rectTransform.anchorMax = Vector2.one;
        _pantalla.rectTransform.offsetMin = Vector2.zero;
        _pantalla.rectTransform.offsetMax = Vector2.zero;
    }

    private void ConfigurarPlayer()
    {
        // Textura de render del tamaño del video
        _rt = new RenderTexture((int)clip.width, (int)clip.height, 0);
        _pantalla.texture = _rt;

        _player = gameObject.AddComponent<VideoPlayer>();
        _player.clip = clip;
        _player.targetTexture = _rt;
        _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _player.SetTargetAudioSource(0, GetComponent<AudioSource>());
        _player.playOnAwake = false;
        _player.isLooping = false;
        _player.loopPointReached += OnVideoTermino;
        _player.Play();
    }

    private void Update()
    {
        if (_terminado) return;

        bool tecla = UnityEngine.InputSystem.Keyboard.current != null &&
                     UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame;
        var raton = UnityEngine.InputSystem.Mouse.current;
        bool click = raton != null &&
                     (raton.leftButton.wasPressedThisFrame || raton.rightButton.wasPressedThisFrame);

        if (saltarConCualquierTecla && (tecla || click))
        {
            Finalizar();
        }
    }

    private void OnVideoTermino(VideoPlayer vp)
    {
        if (!_terminado) Finalizar();
    }

    private void Finalizar()
    {
        _terminado = true;
        StartCoroutine(FundidoYEvento());
    }

    private System.Collections.IEnumerator FundidoYEvento()
    {
        float t = 0f;
        while (t < fundidoSalida)
        {
            t += Time.unscaledDeltaTime;
            _grupo.alpha = 1f - Mathf.Clamp01(t / fundidoSalida);
            yield return null;
        }

        alTerminar?.Invoke();

        // Autodestrucción para limpiar canvas/textura
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_player != null) _player.loopPointReached -= OnVideoTermino;
        if (_rt != null) { _rt.Release(); Destroy(_rt); }
    }
}
