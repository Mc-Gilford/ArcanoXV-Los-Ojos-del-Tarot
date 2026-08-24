using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador del carro en la escena "Carro y salida".
/// Se mueve automáticamente desde la parte del tablero hacia el centro,
/// y el jugador debe presionar X 10 veces para salir y cargar PrincipalScene.
///
/// Reutiliza la misma lógica y patrones que Jugador.cs:
/// - PlayerInput para el sistema de input
/// - InputAction para detectar teclas
/// - Mismo estilo de naming y estructura
/// </summary>
public class CarroSalidaController : MonoBehaviour
{
    [Header("Configuración Movimiento")]
    [Tooltip("Posición inicial del carro (parte del tablero)")]
    public Vector3 puntoInicio = new Vector3(-50f, 0.5f, 0f);
    [Tooltip("Posición final donde se detiene el carro")]
    public Vector3 puntoFin = new Vector3(0f, 0.5f, 0f);
    [Tooltip("Velocidad de movimiento del carro (se recalcula automáticamente para que el trayecto dure 'duracionTrayecto' segundos)")]
    public float velocidadMovimiento = 5f;
    [Tooltip("Duración del trayecto del carro en segundos")]
    public float duracionTrayecto = 14f;

    [Header("Configuración Salida")]
    [Tooltip("Número de veces que se debe presionar X para salir")]
    public int teclasRequeridas = 10;
    [Tooltip("Nombre de la escena a cargar al completar")]
    public string nombreEscenaSiguiente = "PrincipalScene";

    [Header("UI")]
    [Tooltip("Texto que muestra el contador de presiones")]
    public Text textoContador;

    [Header("Transición de Escena")]
    [Tooltip("Duración del fade out/in en segundos")]
    public float duracionFade = 1.5f;
    [Tooltip("Color del fade (normalmente negro)")]
    public Color colorFade = Color.black;

    // Estados internos
    private int _teclasPresionadas = 0;
    private bool _llegadaDestino = false;
    private bool _salidaCompletada = false;
    private PlayerInput _inputJugador;
    private Image _fadeImage;
    private Canvas _fadeCanvas;
    private Coroutine _timerCoroutine;

    private void Awake()
    {
        // Red de seguridad: si inicio y fin coinciden, usar la posición actual como inicio
        if ((puntoInicio - puntoFin).sqrMagnitude < 0.01f)
        {
            Debug.LogWarning("[CarroSalida] puntoInicio y puntoFin son iguales — usando la posición actual del carro como punto de inicio.");
            puntoInicio = transform.position;
        }

        // Posicionar en inicio
        transform.position = puntoInicio;

        // Calcular velocidad para que el recorrido dure exactamente 'duracionTrayecto' segundos
        float distanciaTrayecto = Vector3.Distance(puntoInicio, puntoFin);
        if (distanciaTrayecto > 0.01f && duracionTrayecto > 0.1f)
        {
            velocidadMovimiento = distanciaTrayecto / duracionTrayecto;
        }
    }

    private void Start()
    {
        // FALLBACK: Si no hay cámara en la escena, crearla en runtime
        CrearCamaraFallback();

        // Buscar el PlayerInput en este mismo GameObject (el carro)
        _inputJugador = GetComponent<PlayerInput>();

        // Si no hay PlayerInput, añadirlo (fallback)
        if (_inputJugador == null)
        {
            _inputJugador = gameObject.AddComponent<PlayerInput>();
            _inputJugador.actions = Resources.Load<InputActionAsset>("PlayerActions");
            if (_inputJugador.actions == null)
            {
                #if UNITY_EDITOR
                _inputJugador.actions = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Scenes/PlayerActions.inputactions");
                #endif
            }
            _inputJugador.defaultActionMap = "Jugador";
        }

        // Habilitar el mapa de acciones
        if (_inputJugador != null && _inputJugador.actions != null)
        {
            _inputJugador.actions.Enable();
        }

        // Crear sistema de fade para transición suave
        CrearSistemaFade();

        // Actualizar UI inicial
        ActualizarUI();
        // Iniciar temporizador de salida automático (15 s)
        _timerCoroutine = StartCoroutine(TimerSalida());

        // Cambiar el color del carro a negro
        try { CambiarColorCarroNegro(); }
        catch (System.Exception e) { Debug.LogWarning("[CarroSalida] Error al cambiar color: " + e.Message); }

        // Asignar cámara al RainMaker (evita NullReferenceException en BaseRainScript)
        AsignarCamaraARainMaker();
    }

    private void CrearCamaraFallback()
    {
        // 1) Buscar cámara existente AUNQUE esté desactivada (evita "No cameras rendering")
        Camera[] todas = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Camera principal = null;
        foreach (Camera c in todas)
        {
            if (c.CompareTag("MainCamera")) { principal = c; break; }
        }
        if (principal == null && todas.Length > 0) principal = todas[0];

        if (principal != null)
        {
            bool reactivada = false;
            if (!principal.gameObject.activeSelf) { principal.gameObject.SetActive(true); reactivada = true; }
            if (!principal.enabled) { principal.enabled = true; reactivada = true; }
            if (!principal.CompareTag("MainCamera")) principal.tag = "MainCamera";

            // Asegurar datos URP
            if (principal.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>() == null)
            {
                var data = principal.gameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                data.renderType = UnityEngine.Rendering.Universal.CameraRenderType.Base;
            }

            // Asegurar seguimiento de tercera persona SOLO si la cámara es externa al carro
            // (la cámara en primera persona es hija del carro y no debe llevar follow)
            bool esInterior = principal.transform.IsChildOf(transform);
            if (!esInterior)
            {
                CarCameraFollow followExistente = principal.GetComponent<CarCameraFollow>();
                if (followExistente == null) followExistente = principal.gameObject.AddComponent<CarCameraFollow>();
                followExistente.SetTarget(transform);
            }

            if (reactivada)
                Debug.LogWarning("[CarroSalida] La cámara estaba desactivada — se reactivó automáticamente.");
            else
                Debug.Log("[CarroSalida] Cámara existente encontrada: " + principal.gameObject.name);
            return;
        }

        // 2) No hay ninguna cámara: crearla en runtime
        // Crear cámara de tercera persona en runtime
        Debug.LogWarning("[CarroSalida] No se encontró Main Camera — creando cámara en runtime.");
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";

        // Posicionar detrás y arriba del carro
        camObj.transform.position = transform.position + new Vector3(0f, 5f, 12f);
        camObj.transform.LookAt(transform.position + Vector3.up * 1.5f);

        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;
        cam.fieldOfView = 60f;
        cam.depth = 0;

        camObj.AddComponent<AudioListener>();

        // URP data
        var urpData = camObj.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
        urpData.renderType = UnityEngine.Rendering.Universal.CameraRenderType.Base;
        urpData.renderShadows = true;

        // Seguimiento tercera persona
        CarCameraFollow follow = camObj.AddComponent<CarCameraFollow>();
        follow.SetTarget(transform);
    }

    private void AsignarCamaraARainMaker()
    {
        Camera cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        if (cam == null) return;

        // Buscar todos los scripts de RainMaker en la escena y asignarles la cámara
        var rainScripts = FindObjectsByType<DigitalRuby.RainMaker.BaseRainScript>(FindObjectsSortMode.None);
        foreach (var rain in rainScripts)
        {
            if (rain.Camera == null)
            {
                rain.Camera = cam;
                Debug.Log("[CarroSalida] Cámara asignada al RainMaker: " + rain.gameObject.name);
            }
        }
    }

    private void CambiarColorCarroNegro()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            // Usar sharedMaterials para obtener referencia real, luego crear instancias
            Material[] mats = r.materials; // ya crea instancias
            for (int i = 0; i < mats.Length; i++)
            {
                Material m = mats[i];
                string matName = m.name.ToLower();
                // Cambiar materiales de pintura y carrocería (no vidrios, luces, etc.)
                if (matName.Contains("paint") || matName.Contains("body") ||
                    matName.Contains("bumper") || matName.Contains("door"))
                {
                    // URP usa _BaseColor, Built-in usa _Color
                    if (m.HasProperty("_BaseColor"))
                        m.SetColor("_BaseColor", new Color(0.02f, 0.02f, 0.02f, 1f));
                    if (m.HasProperty("_Color"))
                        m.SetColor("_Color", new Color(0.02f, 0.02f, 0.02f, 1f));
                    // Metallic para look más realista
                    if (m.HasProperty("_Metallic"))
                        m.SetFloat("_Metallic", 0.8f);
                    if (m.HasProperty("_Smoothness"))
                        m.SetFloat("_Smoothness", 0.9f);
                }
            }
            r.materials = mats; // Reasignar las instancias modificadas
        }
    }

    private System.Collections.IEnumerator TimerSalida()
    {
        // Esperar la duración del trayecto (respeta Time.timeScale → se pausa con el juego)
        float elapsed = 0f;
        while (elapsed < duracionTrayecto && !_salidaCompletada)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (!_salidaCompletada)
        {
            // Primero: detener el carro
            _llegadaDestino = true;
            velocidadMovimiento = 0f;
            Debug.Log("[CarroSalida] Trayecto de " + duracionTrayecto.ToString("0") + "s terminado. Carro detenido.");

            // Esperar 2 segundos con el carro quieto antes de la transición
            yield return new WaitForSeconds(2f);

            // Ahora sí: iniciar la transición
            if (!_salidaCompletada)
            {
                CompletarSalida();
            }
        }
    }
    private void CrearSistemaFade()
    {
        // Crear canvas para el fade (Screen Space - Overlay, siempre visible)
        GameObject fadeCanvasObj = new GameObject("FadeCanvas");
        _fadeCanvas = fadeCanvasObj.AddComponent<Canvas>();
        _fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _fadeCanvas.sortingOrder = 1000; // Por encima de todo
        fadeCanvasObj.AddComponent<GraphicRaycaster>();

        // Panel de imagen para el fade
        GameObject fadeImageObj = new GameObject("FadeImage");
        fadeImageObj.transform.SetParent(fadeCanvasObj.transform, false);

        _fadeImage = fadeImageObj.AddComponent<Image>();
        _fadeImage.color = new Color(colorFade.r, colorFade.g, colorFade.b, 0f); // Empezar transparente
        _fadeImage.raycastTarget = false;

        // Configurar RectTransform para cubrir toda la pantalla
        RectTransform rect = fadeImageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        // Empezar con fade in (de negro a transparente)
        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float tiempo = 0f;
        Color colorInicial = new Color(colorFade.r, colorFade.g, colorFade.b, 1f);
        Color colorFinal = new Color(colorFade.r, colorFade.g, colorFade.b, 0f);

        while (tiempo < duracionFade)
        {
            tiempo += Time.deltaTime;
            float t = Mathf.Clamp01(tiempo / duracionFade);
            // Usar curva suave (ease in-out)
            t = t * t * (3f - 2f * t);
            _fadeImage.color = Color.Lerp(colorInicial, colorFinal, t);
            yield return null;
        }
        _fadeImage.color = colorFinal;
    }

    private void OnDestroy()
    {
        // Limpiar input actions
        if (_inputJugador != null && _inputJugador.actions != null)
        {
            _inputJugador.actions.Disable();
        }
    }

    private void Update()
    {
        if (_salidaCompletada) return;

        // 1. Movimiento automático del carro hacia el punto final
        if (!_llegadaDestino)
        {
            MoverCarro();
        }

        // 2. Detectar tecla X DURANTE TODO EL TRAYECTO (no solo al llegar)
        DetectarTeclaX();
    }

    private void MoverCarro()
    {
        float distancia = Vector3.Distance(transform.position, puntoFin);

        if (distancia > 0.1f)
        {
            Vector3 direccion = (puntoFin - transform.position).normalized;
            transform.position += direccion * velocidadMovimiento * Time.deltaTime;

            // Rotar suavemente para mirar al destino
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 5f);
        }
        else
        {
            _llegadaDestino = true;
            transform.position = puntoFin;
            Debug.Log("[CarroSalida] Carro llegó al destino. Presiona X 10 veces para salir.");
        }
    }

    private void DetectarTeclaX()
    {
        // Detectar tecla X directamente usando Keyboard.current (Input System)
        // X no está mapeado en PlayerActions, usamos detección directa como en HintCard/IntroCard
        // Funciona DURANTE TODO EL TRAYECTO, no solo al llegar
        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
        {
            _teclasPresionadas++;
            ActualizarUI();
            Debug.Log($"[CarroSalida] X presionada: {_teclasPresionadas}/{teclasRequeridas}");

            if (_teclasPresionadas >= teclasRequeridas)
            {
                CompletarSalida();
            }
        }
    }

    private void ActualizarUI()
    {
        if (textoContador != null)
        {
            textoContador.text = $"Presiona X: {_teclasPresionadas}/{teclasRequeridas}";
        }
        OnProgresoX?.Invoke(_teclasPresionadas, teclasRequeridas);
    }

    // Evento para elementos visuales del HUD (barra segmentada). No afecta la lógica.
    public static event System.Action<int, int> OnProgresoX;

    private void CompletarSalida()
    {
        if (_salidaCompletada) return;
        _salidaCompletada = true;

        Debug.Log("[CarroSalida] ¡Salida completada! Iniciando transición a " + nombreEscenaSiguiente + "...");

        // Deshabilitar input
        if (_inputJugador != null && _inputJugador.actions != null)
        {
            _inputJugador.actions.Disable();
        }

        // Transición completa (fundido a negro → carga → fundido de vuelta)
        // Se delega a un objeto DontDestroyOnLoad para sobrevivir al cambio de escena
        GameObject transicion = new GameObject("TransicionEscena");
        TransicionEscenaRunner runner = transicion.AddComponent<TransicionEscenaRunner>();
        runner.Iniciar(duracionFade, colorFade, nombreEscenaSiguiente);
    }
}

/// <summary>
/// Ejecuta la transición entre escenas: funde a negro, carga la escena destino
/// de forma asíncrona y funde de vuelta dentro de la nueva escena.
/// Vive en un objeto DontDestroyOnLoad para no morir con la escena anterior.
/// </summary>
public class TransicionEscenaRunner : MonoBehaviour
{
    private float _duracionFade;
    private Color _colorFade;
    private string _escenaDestino;
    private Image _imagen;

    public void Iniciar(float duracionFade, Color colorFade, string escenaDestino)
    {
        _duracionFade = duracionFade;
        _colorFade = colorFade;
        _escenaDestino = escenaDestino;

        // Canvas overlay que cubre toda la pantalla (por encima de todo)
        GameObject canvasObj = new GameObject("CanvasTransicion");
        canvasObj.transform.SetParent(transform, false);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imgObj = new GameObject("ImagenFundido");
        imgObj.transform.SetParent(canvasObj.transform, false);
        _imagen = imgObj.AddComponent<Image>();
        _imagen.color = new Color(_colorFade.r, _colorFade.g, _colorFade.b, 0f);
        _imagen.raycastTarget = false;

        RectTransform rect = _imagen.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        DontDestroyOnLoad(gameObject);
        StartCoroutine(Secuencia());
    }

    private System.Collections.IEnumerator Secuencia()
    {
        // Fundido a negro
        Debug.Log("[Transicion] Fundido a negro...");
        yield return Fundir(0f, 1f);

        // Pequeña pausa en negro para efecto cinematográfico
        yield return new WaitForSecondsRealtime(0.3f);

        // Cargar la escena destino de forma asíncrona
        AsyncOperation carga = SceneManager.LoadSceneAsync(_escenaDestino);
        carga.allowSceneActivation = false;

        // Esperar a que la carga llegue al 90% (Unity no permite activar hasta 0.9f)
        while (carga.progress < 0.9f)
        {
            yield return null;
        }

        // Activar la escena
        carga.allowSceneActivation = true;
        yield return carga;
        yield return null; // dejar renderizar un frame de la nueva escena detrás del negro
        Debug.Log("[Transicion] Escena cargada, fundido de vuelta...");

        // Fundido de vuelta (de negro a transparente) ya dentro de la nueva escena
        yield return Fundir(1f, 0f);

        Debug.Log("[Transicion] Completada.");
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator Fundir(float alphaInicial, float alphaFinal)
    {
        float tiempo = 0f;
        Color colorInicial = new Color(_colorFade.r, _colorFade.g, _colorFade.b, alphaInicial);
        Color colorFinal = new Color(_colorFade.r, _colorFade.g, _colorFade.b, alphaFinal);

        while (tiempo < _duracionFade)
        {
            // unscaledDeltaTime: sigue funcionando aunque Time.timeScale sea 0
            tiempo += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(tiempo / _duracionFade);
            // Curva suave (ease in-out)
            t = t * t * (3f - 2f * t);
            _imagen.color = Color.Lerp(colorInicial, colorFinal, t);
            yield return null;
        }
        _imagen.color = colorFinal;
    }
}