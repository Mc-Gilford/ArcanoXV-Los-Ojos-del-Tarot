using UnityEngine;
using UnityEngine.UI;

public class HintCard : MonoBehaviour
{
    [Header("Configuracion")]
    public float distanciaInteraccion = 3f;
    public float tiempoTransicion = 0.5f;

    [Header("Texto de Pista (Editable)")]
    [TextArea(5, 15)]
    public string textoPista = "Escribe aqui la pista que vera el jugador.";
    public int tamanoLetra = 26;
    public Color colorTexto = new Color(0.12f, 0.08f, 0.04f, 1f);

    [Header("Tamano UI")]
    public Vector2 tamanoPanel = new Vector2(600, 800);
    public Vector2 tamanoTextoArea = new Vector2(500, 700);
    public Vector2 posicionPanel = Vector2.zero;
    public Vector2 posicionTexto = Vector2.zero;

    private Renderer _renderer;
    private Texture2D _texturaCartaQuemada;

    // Estados: 0 = flotando/cerrada, 1 = abierta (frente), 2 = abierta (reverso)
    private int _estado = 0;
    private Vector3 _basePos;
    private bool _enTransicion = false;
    private float _tiempoTransicionActual = 0f;

    private CanvasGroup _canvasGroupFrente;
    private CanvasGroup _canvasGroupReverso;
    private Text _textoControles;
    private Text _textoPistaUI;
    private RectTransform _rectFondoFrente;
    private RectTransform _rectFondoReverso;
    private RectTransform _rectTextoPista;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError("[HintCard] El objeto no tiene componente Renderer.");
            return;
        }

        _basePos = transform.position;

        if (!CargarTexturas())
        {
            Debug.LogError("[HintCard] No se pudo cargar la textura 'Carta quemada'.");
            return;
        }

        CrearUIPaneles();
    }

    private bool CargarTexturas()
    {
        // La imagen debe estar en: Assets/Resources/Historias/Carta quemada.png
        _texturaCartaQuemada = Resources.Load<Texture2D>("Historias/Carta quemada");
        if (_texturaCartaQuemada == null)
        {
            Debug.LogError("[HintCard] No se encontro 'Resources/Historias/Carta quemada'. Verifica ruta y nombre exacto.");
            return false;
        }

        _renderer.material.mainTexture = _texturaCartaQuemada;
        _renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        return true;
    }

    private void CrearUIPaneles()
    {
        // ================= PANEL FRENTE =================
        GameObject canvasFrente = new GameObject("UI_Frente");
        canvasFrente.transform.SetParent(transform);
        canvasFrente.transform.localPosition = Vector3.zero;

        Canvas cf = canvasFrente.AddComponent<Canvas>();
        cf.renderMode = RenderMode.ScreenSpaceOverlay;
        cf.overrideSorting = true;
        cf.sortingOrder = 1;

        CanvasScaler scalerF = canvasFrente.AddComponent<CanvasScaler>();
        scalerF.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scalerF.referenceResolution = new Vector2(1920, 1080);

        RectTransform rectCanvasF = canvasFrente.GetComponent<RectTransform>();
        rectCanvasF.anchorMin = Vector2.zero;
        rectCanvasF.anchorMax = Vector2.one;
        rectCanvasF.offsetMin = Vector2.zero;
        rectCanvasF.offsetMax = Vector2.zero;

        _canvasGroupFrente = canvasFrente.AddComponent<CanvasGroup>();
        _canvasGroupFrente.alpha = 0f;
        _canvasGroupFrente.blocksRaycasts = false;
        _canvasGroupFrente.interactable = false;

        GameObject fondoF = new GameObject("Fondo");
        fondoF.transform.SetParent(canvasFrente.transform);
        Image imgF = fondoF.AddComponent<Image>();
        imgF.sprite = Sprite.Create(
            _texturaCartaQuemada,
            new Rect(0, 0, _texturaCartaQuemada.width, _texturaCartaQuemada.height),
            Vector2.one * 0.5f);
        imgF.preserveAspect = true;

        RectTransform rectFondoF = fondoF.GetComponent<RectTransform>();
        rectFondoF.anchorMin = rectFondoF.anchorMax = new Vector2(0.5f, 0.5f);
        rectFondoF.sizeDelta = tamanoPanel;
        rectFondoF.anchoredPosition = posicionPanel;
        _rectFondoFrente = rectFondoF;

        // ================= PANEL REVERSO =================
        // Jerarquia: canvasReverso > Fondo > Viewport(Mask) > ContenedorTexto > TextoPista
        GameObject canvasReverso = new GameObject("UI_Reverso");
        canvasReverso.transform.SetParent(transform);
        canvasReverso.transform.localPosition = Vector3.zero;

        Canvas cr = canvasReverso.AddComponent<Canvas>();
        cr.renderMode = RenderMode.ScreenSpaceOverlay;
        cr.overrideSorting = true;
        cr.sortingOrder = 2;

        CanvasScaler scalerR = canvasReverso.AddComponent<CanvasScaler>();
        scalerR.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scalerR.referenceResolution = new Vector2(1920, 1080);

        RectTransform rectCanvasR = canvasReverso.GetComponent<RectTransform>();
        rectCanvasR.anchorMin = Vector2.zero;
        rectCanvasR.anchorMax = Vector2.one;
        rectCanvasR.offsetMin = Vector2.zero;
        rectCanvasR.offsetMax = Vector2.zero;

        _canvasGroupReverso = canvasReverso.AddComponent<CanvasGroup>();
        _canvasGroupReverso.alpha = 0f;
        _canvasGroupReverso.blocksRaycasts = false;
        _canvasGroupReverso.interactable = false;

        // Fondo (imagen carta quemada)
        GameObject fondoR = new GameObject("Fondo");
        fondoR.transform.SetParent(canvasReverso.transform);
        Image imgR = fondoR.AddComponent<Image>();
        imgR.sprite = Sprite.Create(
            _texturaCartaQuemada,
            new Rect(0, 0, _texturaCartaQuemada.width, _texturaCartaQuemada.height),
            Vector2.one * 0.5f);
        imgR.preserveAspect = true;

        RectTransform rectFondoR = fondoR.GetComponent<RectTransform>();
        rectFondoR.anchorMin = rectFondoR.anchorMax = new Vector2(0.5f, 0.5f);
        rectFondoR.sizeDelta = tamanoPanel;
        rectFondoR.anchoredPosition = posicionPanel;
        _rectFondoReverso = rectFondoR;

        // TextoPista: hermano del Fondo (hijo de canvasReverso), tamano y posicion independientes
        GameObject textoObj = new GameObject("TextoPista");
        textoObj.transform.SetParent(canvasReverso.transform);
        RectTransform rectTextoObj = textoObj.AddComponent<RectTransform>();
        rectTextoObj.anchorMin = rectTextoObj.anchorMax = new Vector2(0.5f, 0.5f);
        rectTextoObj.sizeDelta = tamanoTextoArea;
        rectTextoObj.anchoredPosition = posicionPanel + posicionTexto;
        _rectTextoPista = rectTextoObj;

        _textoPistaUI = textoObj.AddComponent<Text>();
        _textoPistaUI.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _textoPistaUI.fontSize = tamanoLetra;
        _textoPistaUI.fontStyle = FontStyle.Bold;
        _textoPistaUI.alignment = TextAnchor.UpperLeft;
        _textoPistaUI.color = colorTexto;
        _textoPistaUI.lineSpacing = 1.3f;
        _textoPistaUI.verticalOverflow = VerticalWrapMode.Overflow;
        _textoPistaUI.horizontalOverflow = HorizontalWrapMode.Wrap;
        _textoPistaUI.text = textoPista;
        _textoPistaUI.raycastTarget = false;

        // ================= CANVAS CONTROLES =================
        GameObject canvasControles = new GameObject("UI_Controles");
        canvasControles.transform.SetParent(transform);
        canvasControles.transform.SetAsLastSibling();

        Canvas cc = canvasControles.AddComponent<Canvas>();
        cc.renderMode = RenderMode.ScreenSpaceOverlay;
        cc.overrideSorting = true;
        cc.sortingOrder = 10;

        CanvasScaler scalerC = canvasControles.AddComponent<CanvasScaler>();
        scalerC.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scalerC.referenceResolution = new Vector2(1920, 1080);

        GameObject textoCtrlObj = new GameObject("TextoControles");
        textoCtrlObj.transform.SetParent(canvasControles.transform);
        _textoControles = textoCtrlObj.AddComponent<Text>();
        _textoControles.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        _textoControles.fontSize = 22;
        _textoControles.fontStyle = FontStyle.Bold;
        _textoControles.alignment = TextAnchor.MiddleCenter;
        _textoControles.color = Color.yellow;
        _textoControles.text = "";

        RectTransform textoCtrlRect = textoCtrlObj.GetComponent<RectTransform>();
        textoCtrlRect.anchorMin = Vector2.zero;
        textoCtrlRect.anchorMax = new Vector2(1f, 0.25f);
        textoCtrlRect.offsetMin = Vector2.zero;
        textoCtrlRect.offsetMax = Vector2.zero;

        ActualizarTextoControles();
    }

    private void ActualizarTextoControles()
    {
        if (_textoControles == null) return;

        switch (_estado)
        {
            case 0:
                _textoControles.text = "Presiona (X) para examinar";
                break;
            case 1:
                _textoControles.text = "Presiona (E) para voltear\nPresiona (V) para salir";
                break;
            case 2:
                _textoControles.text = "Presiona (E) para volver\nPresiona (V) para salir";
                break;
        }
    }

    private void ActualizarTextoPista()
    {
        if (_textoPistaUI == null) return;
        _textoPistaUI.text = textoPista;
        _textoPistaUI.fontSize = tamanoLetra;
        _textoPistaUI.color = colorTexto;
    }

    private void Update()
    {
        if (_canvasGroupFrente == null || _canvasGroupReverso == null) return;

        // --- Transicion de alpha entre frente y reverso ---
        if (_enTransicion)
        {
            _tiempoTransicionActual += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(_tiempoTransicionActual / tiempoTransicion);

            if (_estado == 2)
            {
                _canvasGroupFrente.alpha = Mathf.Lerp(1f, 0f, t);
                _canvasGroupReverso.alpha = Mathf.Lerp(0f, 1f, t);
            }
            else if (_estado == 1)
            {
                _canvasGroupFrente.alpha = Mathf.Lerp(0f, 1f, t);
                _canvasGroupReverso.alpha = Mathf.Lerp(1f, 0f, t);
            }

            if (t >= 1f) _enTransicion = false;
        }

        // --- Salir con V ---
        if (Input.GetKeyDown(KeyCode.V) && _estado != 0)
        {
            Cerrar();
            return;
        }

        // --- Flotacion cuando esta cerrada ---
        if (_estado == 0)
        {
            float onda = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f;
            transform.position = _basePos + Vector3.up * (onda * 0.15f);
            transform.Rotate(0f, 70f * Time.deltaTime, 0f, Space.World);
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null)
        {
            if (_textoControles != null) _textoControles.text = "";
            return;
        }

        float distancia = Vector3.Distance(jugador.transform.position, transform.position);

        if (distancia <= distanciaInteraccion)
        {
            if (Input.GetKeyDown(KeyCode.X) && _estado == 0)
            {
                Abrir();
            }

            if (Input.GetKeyDown(KeyCode.E) && (_estado == 1 || _estado == 2))
            {
                if (_estado == 1) Voltear();
                else VolverAlFrente();
            }

            ActualizarTextoControles();
        }
        else
        {
            if (_estado != 0) Cerrar();
            if (_textoControles != null) _textoControles.text = "";
        }
    }

    private void Abrir()
    {
        _estado = 1;
        _canvasGroupFrente.alpha = 1f;
        _canvasGroupFrente.blocksRaycasts = true;
        _canvasGroupFrente.interactable = true;
        _canvasGroupReverso.alpha = 0f;
        _canvasGroupReverso.blocksRaycasts = false;
        _canvasGroupReverso.interactable = false;

        ActualizarTextoPista();
        Time.timeScale = 0f;
        ActualizarTextoControles();
    }

    private void Voltear()
    {
        _estado = 2;
        _enTransicion = true;
        _tiempoTransicionActual = 0f;
        _canvasGroupReverso.blocksRaycasts = true;
        _canvasGroupReverso.interactable = true;

        ActualizarTextoPista();
        ActualizarTextoControles();
    }

    private void VolverAlFrente()
    {
        _estado = 1;
        _enTransicion = true;
        _tiempoTransicionActual = 0f;
        _canvasGroupReverso.blocksRaycasts = false;
        _canvasGroupReverso.interactable = false;

        ActualizarTextoControles();
    }

    private void Cerrar()
    {
        _estado = 0;
        _canvasGroupFrente.alpha = 0f;
        _canvasGroupFrente.blocksRaycasts = false;
        _canvasGroupFrente.interactable = false;
        _canvasGroupReverso.alpha = 0f;
        _canvasGroupReverso.blocksRaycasts = false;
        _canvasGroupReverso.interactable = false;

        Time.timeScale = 1f;
        _basePos = transform.position;
        ActualizarTextoControles();
    }

    private void OnValidate()
    {
        ActualizarTextoPista();

        if (_rectFondoFrente != null)
        {
            _rectFondoFrente.anchoredPosition = posicionPanel;
            _rectFondoFrente.sizeDelta = tamanoPanel;
        }
        if (_rectFondoReverso != null)
        {
            _rectFondoReverso.anchoredPosition = posicionPanel;
            _rectFondoReverso.sizeDelta = tamanoPanel;
        }
        if (_rectTextoPista != null)
        {
            _rectTextoPista.anchoredPosition = posicionPanel + posicionTexto;
            _rectTextoPista.sizeDelta = tamanoTextoArea;
        }
    }
}