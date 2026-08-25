using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Muestra un objetivo/alerta al entrar a la habitación.
/// Lee los valores de RoomTimer y SpawnEnemies que ya existen en la escena.
/// Arrastra el prefab al scene, selecciona el tipo desde el inspector.
/// </summary>
public class ObjetivoHabitacion : MonoBehaviour
{
    public enum TipoObjetivo
    {
        BuscarTarjeta = 1,
        SobrevivirTiempo = 2,
        MatarEnemigos = 3,
        AtrapaVelas = 4,
        PrendeLinternas = 5,
        DestruyeCajas = 6
    }

    [Header("Objetivo")]
    public TipoObjetivo tipoObjetivo = TipoObjetivo.BuscarTarjeta;

    [Header("UI")]
    [Tooltip("Objeto pre-existente para activar/desactivar (estilo RelicRoom). Si está vacío, crea texto automático.")]
    public GameObject clueUI;

    [Tooltip("Fuente TMP (se auto-busca si está vacío)")]
    public TMP_FontAsset fuenteObjetivo;

    [Header("Alerta")]
    public float duracionAlerta = 5f;
    public float delayInicial = 1f;

    private bool _mostrado;
    private TextMeshProUGUI _tmp;
    private Canvas _canvas;
    private CanvasGroup _grupoCanvas;

    private void Start()
    {
        if (clueUI != null)
        {
            clueUI.SetActive(false);
        }
        else
        {
            CrearUITexto();
            _canvas.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_mostrado)
        {
            _mostrado = true;
            StartCoroutine(MostrarAlerta());
        }
    }

    private string ObtenerTextoObjetivo()
    {
        switch (tipoObjetivo)
        {
            case TipoObjetivo.BuscarTarjeta:
                return "Busca la tarjeta dentro de la habitación";

            case TipoObjetivo.SobrevivirTiempo:
            {
                // Leer de RoomTimer si existe
                float tiempo = 240f;
                RoomTimer rt = FindFirstObjectByType<RoomTimer>();
                if (rt != null) tiempo = rt.tiempoNecesario;
                return $"Sobrevive {tiempo:F0} segundos en la habitación";
            }

            case TipoObjetivo.MatarEnemigos:
            {
                // Leer de SpawnEnemies si existe
                int enemigos = 30;
                SpawnEnemies sp = FindFirstObjectByType<SpawnEnemies>();
                if (sp != null) enemigos = sp.limitEnemies;
                return $"Mata {enemigos} enemigos";
            }

            case TipoObjetivo.AtrapaVelas:
                return "Atrapa las velas";

            case TipoObjetivo.PrendeLinternas:
                return "Prende las linternas";

            case TipoObjetivo.DestruyeCajas:
                return "Destruye las cajas y encuentra la carta";

            default:
                return "Objetivo desconocido";
        }
    }

    private IEnumerator MostrarAlerta()
    {
        yield return new WaitForSeconds(delayInicial);

        if (clueUI != null)
        {
            clueUI.SetActive(true);
            yield return new WaitForSeconds(duracionAlerta);
            clueUI.SetActive(false);
        }
        else
        {
            if (_tmp != null)
                _tmp.text = ObtenerTextoObjetivo();

            _canvas.gameObject.SetActive(true);
            _grupoCanvas.alpha = 1f;

            yield return new WaitForSeconds(duracionAlerta);

            float t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                _grupoCanvas.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
                yield return null;
            }
            _grupoCanvas.alpha = 0f;
            _canvas.gameObject.SetActive(false);
        }
    }

    private void CrearUITexto()
    {
        GameObject canvasGo = new GameObject("HUD_Objetivo_" + tipoObjetivo);
        _canvas = canvasGo.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 900;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();
        _grupoCanvas = canvasGo.AddComponent<CanvasGroup>();

        GameObject textoGo = new GameObject("TextoObjetivo");
        textoGo.transform.SetParent(canvasGo.transform, false);
        _tmp = textoGo.AddComponent<TextMeshProUGUI>();

        if (fuenteObjetivo != null)
            _tmp.font = fuenteObjetivo;
        else
        {
            TMP_FontAsset[] fuentes = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
            foreach (TMP_FontAsset f in fuentes)
            {
                if (f.name.Contains("Old Horror Films"))
                {
                    _tmp.font = f;
                    break;
                }
            }
        }

        _tmp.text = ObtenerTextoObjetivo();
        _tmp.fontSize = 40;
        _tmp.fontStyle = FontStyles.Bold;
        _tmp.color = new Color(0.396f, 0f, 0f);
        _tmp.alignment = TextAlignmentOptions.Center;
        _tmp.enableWordWrapping = true;
        _tmp.overflowMode = TextOverflowModes.Ellipsis;
        _tmp.richText = true;

        RectTransform rt = textoGo.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 69);
        rt.sizeDelta = new Vector2(800, 60);
    }
}
