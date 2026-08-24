using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Carta intro: X = abrir, E = voltear, V = salir
/// Controles mostrados en pantalla
/// </summary>
public class IntroCard : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreImagen = "intro";
    public float distanciaInteraccion = 3f;
    public float tiempoTransicion = 0.5f;  // Duración de la transición

    private Renderer _renderer;
    private Texture2D _texturaIntro;
    private Texture2D _texturaReverso;

    // Estados: 0 = flotando, 1 = abierto, 2 = reverso
    private int _estado = 0;
    private Vector3 _basePos;
    private bool _enTransicion = false;
    private float _tiempoTransicionActual = 0f;

    // UI
    private CanvasGroup _canvasGroupAbierto;
    private CanvasGroup _canvasGroupReverso;
    private Text _textoControles;
    private ScrollRect _scrollRectAbierto;
    private ScrollRect _scrollRectReverso;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("[IntroCard] No tiene Renderer");
            return;
        }

        _basePos = transform.position;

        if (!CargarTexturas())
        {
            Debug.LogError("[IntroCard] No se cargaron las texturas");
            return;
        }

        CrearUIPaneles();
    }

    private bool CargarTexturas()
    {
        _texturaIntro = Resources.Load<Texture2D>("Historias/" + nombreImagen);
        if (_texturaIntro == null)
        {
            Debug.LogError($"[IntroCard] No se encontró: Historias/{nombreImagen}");
            return false;
        }

        _renderer.material.mainTexture = _texturaIntro;

        _texturaReverso = Resources.Load<Texture2D>("Historias/Carta quemada");
        if (_texturaReverso == null)
        {
            Debug.LogError("[IntroCard] No se encontró: Historias/Carta quemada");
            return false;
        }

        _renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        return true;
    }

    private void CrearUIPaneles()
    {
        // Panel Abierto (imagen intro) - ScreenSpaceOverlay
        GameObject canvasAbierto = new GameObject("UI_Abierto");
        canvasAbierto.transform.SetParent(transform);
        canvasAbierto.transform.localPosition = Vector3.zero;

        Canvas ca = canvasAbierto.AddComponent<Canvas>();
        ca.renderMode = RenderMode.ScreenSpaceOverlay;
        ca.overrideSorting = true;
        ca.sortingOrder = 1;

        // RectTransform para ocupar toda la pantalla
        RectTransform rectCanvasA = canvasAbierto.GetComponent<RectTransform>();
        rectCanvasA.anchorMin = Vector2.zero;
        rectCanvasA.anchorMax = Vector2.one;
        rectCanvasA.offsetMin = Vector2.zero;
        rectCanvasA.offsetMax = Vector2.zero;

        _canvasGroupAbierto = canvasAbierto.AddComponent<CanvasGroup>();
        _canvasGroupAbierto.alpha = 0f;

        // Fondo para panel abierto - centrado
        GameObject fondoA = new GameObject("Fondo");
        fondoA.transform.SetParent(canvasAbierto.transform);

        Image imgA = fondoA.AddComponent<Image>();
        Sprite spriteIntro = Sprite.Create(_texturaIntro,
            new Rect(0, 0, _texturaIntro.width, _texturaIntro.height),
            Vector2.one * 0.5f);
        imgA.sprite = spriteIntro;
        imgA.preserveAspect = true;

        RectTransform rectFondoA = fondoA.GetComponent<RectTransform>();
        rectFondoA.anchorMin = new Vector2(0.5f, 0.5f);
        rectFondoA.anchorMax = new Vector2(0.5f, 0.5f);
        rectFondoA.sizeDelta = new Vector2(600, 1600);  // El doble del viewport para que haya scroll
        rectFondoA.anchoredPosition = Vector2.zero;

        // ScrollRect para scroll en la imagen
        ScrollRect scrollRectA = canvasAbierto.AddComponent<ScrollRect>();
        scrollRectA.content = rectFondoA;
        scrollRectA.vertical = true;
        scrollRectA.horizontal = false;
        scrollRectA.inertia = false;  // Sin inercia, control manual con flechas
        _scrollRectAbierto = scrollRectA;

        // Viewport
        GameObject viewportA = new GameObject("Viewport");
        viewportA.transform.SetParent(canvasAbierto.transform);
        RectTransform viewportRectA = viewportA.AddComponent<RectTransform>();
        viewportRectA.anchorMin = new Vector2(0.5f, 0.5f);
        viewportRectA.anchorMax = new Vector2(0.5f, 0.5f);
        viewportRectA.sizeDelta = new Vector2(600, 800);
        viewportRectA.anchoredPosition = Vector2.zero;

        // IMPORTANTE: Agregar Mask al Viewport para que funcione el clipping
        Image maskImageA = viewportA.AddComponent<Image>();
        maskImageA.color = new Color(1, 1, 1, 0);  // Transparente
        Mask maskA = viewportA.AddComponent<Mask>();
        maskA.showMaskGraphic = false;

        scrollRectA.viewport = viewportRectA;

        // Panel Reverso (carta quemada) - ScreenSpaceOverlay
        GameObject canvasReverso = new GameObject("UI_Reverso");
        canvasReverso.transform.SetParent(transform);
        canvasReverso.transform.localPosition = Vector3.zero;

        Canvas cr = canvasReverso.AddComponent<Canvas>();
        cr.renderMode = RenderMode.ScreenSpaceOverlay;
        cr.overrideSorting = true;
        cr.sortingOrder = 2;  // Encima del frente (1)

        // RectTransform para ocupar toda la pantalla
        RectTransform rectCanvasR = canvasReverso.GetComponent<RectTransform>();
        rectCanvasR.anchorMin = Vector2.zero;
        rectCanvasR.anchorMax = Vector2.one;
        rectCanvasR.offsetMin = Vector2.zero;
        rectCanvasR.offsetMax = Vector2.zero;

        _canvasGroupReverso = canvasReverso.AddComponent<CanvasGroup>();
        _canvasGroupReverso.alpha = 0f;

        // Fondo para panel reverso - centrado
        GameObject fondoR = new GameObject("Fondo");
        fondoR.transform.SetParent(canvasReverso.transform);

        Image imgR = fondoR.AddComponent<Image>();
        Sprite spriteReverso = Sprite.Create(_texturaReverso,
            new Rect(0, 0, _texturaReverso.width, _texturaReverso.height),
            Vector2.one * 0.5f);
        imgR.sprite = spriteReverso;
        imgR.preserveAspect = true;

        RectTransform rectFondoR = fondoR.GetComponent<RectTransform>();
        rectFondoR.anchorMin = new Vector2(0.5f, 0.5f);
        rectFondoR.anchorMax = new Vector2(0.5f, 0.5f);
        rectFondoR.sizeDelta = new Vector2(600, 1600);  // El doble del viewport para scroll
        rectFondoR.anchoredPosition = Vector2.zero;

        // ScrollRect para scroll en el reverso
        ScrollRect scrollRectR = canvasReverso.AddComponent<ScrollRect>();
        scrollRectR.content = rectFondoR;
        scrollRectR.vertical = true;
        scrollRectR.horizontal = false;
        scrollRectR.inertia = false;  // Sin inercia, control manual con flechas
        _scrollRectReverso = scrollRectR;

        // Viewport para reverso
        GameObject viewportR = new GameObject("Viewport");
        viewportR.transform.SetParent(canvasReverso.transform);
        RectTransform viewportRectR = viewportR.AddComponent<RectTransform>();
        viewportRectR.anchorMin = new Vector2(0.5f, 0.5f);
        viewportRectR.anchorMax = new Vector2(0.5f, 0.5f);
        viewportRectR.sizeDelta = new Vector2(600, 800);
        viewportRectR.anchoredPosition = Vector2.zero;

        // Mask para el reverso (clipping)
        Image maskImageR = viewportR.AddComponent<Image>();
        maskImageR.color = new Color(1, 1, 1, 0);  // Transparente
        Mask maskR = viewportR.AddComponent<Mask>();
        maskR.showMaskGraphic = false;

        scrollRectR.viewport = viewportRectR;

        // Canvas para controles (siempre encima)
        GameObject canvasControles = new GameObject("UI_Controles");
        canvasControles.transform.SetParent(transform);
        canvasControles.transform.SetAsLastSibling();  // Render último = encima de todo

        Canvas cc = canvasControles.AddComponent<Canvas>();
        cc.renderMode = RenderMode.ScreenSpaceOverlay;
        cc.overrideSorting = true;
        cc.sortingOrder = 10;  // Encima de los otros canvas

        CanvasScaler scaler = canvasControles.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Texto de controles
        GameObject textoObj = new GameObject("Texto");
        textoObj.transform.SetParent(canvasControles.transform);

        _textoControles = textoObj.AddComponent<Text>();
        FuentesJuego.Aplicar(_textoControles, 20, FuentesJuego.Dorado, true, true);
        _textoControles.alignment = TextAnchor.MiddleCenter;
        _textoControles.text = "";

        RectTransform textoRect = textoObj.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = new Vector2(1, 0.3f);
        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;

        // Inicializar texto
        ActualizarTextoControles();
    }

    private void ActualizarTextoControles()
    {
        if (_textoControles == null) return;

        switch (_estado)
        {
            case 0:
                _textoControles.text = "Presiona (X) para abrir";
                break;
            case 1:
                _textoControles.text = "Presiona (E) para voltear\nPresiona (↑↓) para desplazarse\nPresiona (V) para salir";
                break;
            case 2:
                _textoControles.text = "Presiona (E) para volver\nPresiona (↑↓) para desplazarse\nPresiona (V) para salir";
                break;
        }
    }

    private void Update()
    {
        if (_canvasGroupAbierto == null || _canvasGroupReverso == null)
            return;

        // Actualizar transición suave (usar unscaledDeltaTime porque timeScale = 0)
        if (_enTransicion)
        {
            _tiempoTransicionActual += Time.unscaledDeltaTime;
            float progreso = Mathf.Clamp01(_tiempoTransicionActual / tiempoTransicion);

            if (_estado == 2)
            {
                // Transición frente → reverso
                _canvasGroupAbierto.alpha = Mathf.Lerp(1f, 0f, progreso);
                _canvasGroupReverso.alpha = Mathf.Lerp(0f, 1f, progreso);
            }
            else if (_estado == 1)
            {
                // Transición reverso → frente
                _canvasGroupAbierto.alpha = Mathf.Lerp(0f, 1f, progreso);
                _canvasGroupReverso.alpha = Mathf.Lerp(1f, 0f, progreso);
            }

            if (progreso >= 1f)
            {
                _enTransicion = false;
            }
        }

        // V = salir SIEMPRE (incluso durante transición)
        if (Input.GetKeyDown(KeyCode.V) && _estado != 0)
        {
            Cerrar();
            return;
        }

        // Flotar y girar solo si está cerrado
        if (_estado == 0)
        {
            float onda = (Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f);
            transform.position = _basePos + Vector3.up * (onda * 0.15f);
            transform.Rotate(0f, 70f * Time.deltaTime, 0f, Space.World);
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null)
        {
            _textoControles.text = "";
            return;
        }

        float distancia = Vector3.Distance(jugador.transform.position, transform.position);

        if (distancia <= distanciaInteraccion)
        {
            // X = abrir
            if (Input.GetKeyDown(KeyCode.X) && _estado == 0)
            {
                Abrir();
            }

            // E = voltear (permite cambiar entre estado 1 y 2)
            if (Input.GetKeyDown(KeyCode.E) && (_estado == 1 || _estado == 2))
            {
                if (_estado == 1)
                    Voltear();
                else if (_estado == 2)
                    VolverAlFrente();
            }

            // V = salir
            if (Input.GetKeyDown(KeyCode.V) && _estado != 0)
            {
                Cerrar();
            }

            // Control de scroll: touchpad, mouse y flechas
            if ((_estado == 1 && _scrollRectAbierto != null) || (_estado == 2 && _scrollRectReverso != null))
            {
                ScrollRect scrollActual = _estado == 1 ? _scrollRectAbierto : _scrollRectReverso;

                // Input.mouseScrollDelta funciona con mouse y touchpad
                float scrollDelta = Input.mouseScrollDelta.y;

                if (scrollDelta != 0)
                {
                    // Invertir: scrollDelta positivo = scroll arriba, move down
                    Vector2 newPos = scrollActual.normalizedPosition;
                    newPos.y = Mathf.Clamp01(newPos.y + scrollDelta * 0.02f);
                    scrollActual.normalizedPosition = newPos;
                }

                // Flechas como alternativa
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Vector2 newPos = scrollActual.normalizedPosition;
                    newPos.y = Mathf.Clamp01(newPos.y + 0.02f);
                    scrollActual.normalizedPosition = newPos;
                }

                if (Input.GetKey(KeyCode.DownArrow))
                {
                    Vector2 newPos = scrollActual.normalizedPosition;
                    newPos.y = Mathf.Clamp01(newPos.y - 0.02f);
                    scrollActual.normalizedPosition = newPos;
                }
            }

            ActualizarTextoControles();
        }
        else
        {
            if (_estado != 0) Cerrar();
            _textoControles.text = "";
        }
    }

    private void Abrir()
    {
        _estado = 1;
        _canvasGroupAbierto.alpha = 1f;
        Time.timeScale = 0f;  // Congelar el juego
        Debug.Log("[IntroCard] Panel abierto - Juego congelado");
    }

    private void Voltear()
    {
        _estado = 2;
        _enTransicion = true;
        _tiempoTransicionActual = 0f;
        Debug.Log("[IntroCard] Volteando...");
    }

    private void VolverAlFrente()
    {
        _estado = 1;
        _enTransicion = true;
        _tiempoTransicionActual = 0f;
        Debug.Log("[IntroCard] Volviendo al frente...");
    }

    private void Cerrar()
    {
        _estado = 0;
        _canvasGroupAbierto.alpha = 0f;
        _canvasGroupReverso.alpha = 0f;
        Time.timeScale = 1f;  // Descongelar el juego
        Debug.Log("[IntroCard] Cerrado - Juego reanudado");
    }
}