using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Habilidad activa de cartas del tarot (Tab): elige con J/K/L y sale con Tab.
/// Cada carta activa un Power Up.
/// La UI se construye por código.
/// </summary>
public class CardSelectionSystem : MonoBehaviour
{
    [Header("Cartas (3: J, K, L)")]
    [Tooltip("Las 3 cartas. Se usa CardDef.Defaults() si no están configuradas.")]
    public CardDef[] cartas = CardDef.Defaults();

    [Header("Tiempos")]
    public float cooldownHabilidad = 45f;
    public float tiempoAbrir = 0.4f;
    public float tiempoAnimacionElegir = 0.5f;
    public float tiempoResultado = 2.5f;
    public float tiempoFlash = 0.4f;

    [Header("Comportamiento")]
    [Tooltip("Si true, al abrir la selección se congela el WASD.")]
    public bool bloquearMovimientoAlElegir = true;

    public GameManager gameManager;

    private enum Estado
    {
        Idle,
        Seleccionando,
        Resolviendo,
        EnCooldown
    }

    private Estado _estado = Estado.Idle;

    private float _cooldownRestante;
    private bool _cartaElegida;

    public int IndexCartasSeleccionada { get; private set; } = -1;
    public bool isPowerUpSeleccionado { get; set; } = false;

    // Referencias UI
    private Font _fuente;
    private Image _fondo;
    private Text _titulo;
    private RectTransform _cartasRaiz;

    private readonly RectTransform[] _cartaRT = new RectTransform[3];
    private readonly CanvasGroup[] _cartaGrupo = new CanvasGroup[3];

    private Text _instrucciones;

    private GameObject _panelResultado;
    private CanvasGroup _grupoResultado;
    private Text _textoResultado;

    private Image _flash;
    private Text _textoCooldown;
    private Text _textoCartaActiva;

    // NUEVA FEATURE: Imagen del Power Up activo
    private Image _powerUpIcon;
    private RectTransform _powerUpIconRT;
    private Coroutine _powerUpAnimation;

    public bool showPowerUp = false;

    private void Awake()
    {
        if (cartas == null || cartas.Length == 0)
        {
            cartas = CardDef.Defaults();
        }

        ConstruirCanvas();

        GameObject gameManagerObject = GameObject.Find("GameManager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }
    }

    private void OnDestroy()
    {
        GameObject hud = _fondo != null ? _fondo.canvas.gameObject : null;

        if (hud != null && Application.isPlaying)
        {
            Destroy(hud);
        }
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;

        if (kb == null)
        {
            return;
        }

        if (_estado == Estado.Idle)
        {
            if (kb.tabKey.wasPressedThisFrame)
            {
                AbrirSeleccion();
            }
        }
        else if (_estado == Estado.Seleccionando && !_cartaElegida)
        {
            if (kb.jKey.wasPressedThisFrame)
            {
                ElegirCarta(0);
                return;
            }

            if (kb.kKey.wasPressedThisFrame)
            {
                ElegirCarta(1);
                return;
            }

            if (kb.lKey.wasPressedThisFrame)
            {
                ElegirCarta(2);
                return;
            }

            if (kb.tabKey.wasPressedThisFrame)
            {
                CerrarSinElegir();
            }
        }

        if (_estado == Estado.EnCooldown)
        {
            _cooldownRestante -= Time.deltaTime;

            if (_cooldownRestante <= 0f)
            {
                _estado = Estado.Idle;
            }
        }

        ActualizarTextos();
    }

    public bool SeleccionAbierta => _estado == Estado.Seleccionando;

    // ------------------------------------------------------------------
    // ESTADOS
    // ------------------------------------------------------------------

    private void AbrirSeleccion()
    {
        showPowerUp = false;
        _estado = Estado.Seleccionando;
        _cartaElegida = false;

        StartCoroutine(AnimacionAbrir());

        Debug.Log("[Cartas] Selección abierta (Tab).");
    }

    private void ElegirCarta(int indice)
    {
        if (indice < 0 || indice >= cartas.Length)
        {
            return;
        }

        CardDef c = cartas[indice];

        IndexCartasSeleccionada = indice;
        isPowerUpSeleccionado = true;
        _cartaElegida = true;
        _estado = Estado.Resolviendo;

        _textoCartaActiva.text = "Carta activa: " + c.nombre;

        // NUEVA FEATURE: Muestra la imagen de la carta seleccionada como Power Up
        _powerUpIcon.sprite = c.imagen;
        _powerUpIcon.color = c.imagen != null ? Color.white : c.color;
        _powerUpIcon.gameObject.SetActive(true);

        // NUEVA FEATURE: Si había una animación anterior la reinicia
        if (_powerUpAnimation != null)
        {
            StopCoroutine(_powerUpAnimation);
        }

        _powerUpAnimation = StartCoroutine(AnimarPowerUpIcon());

        StartCoroutine(AnimacionElegir(indice));

        if (gameManager != null)
        {
            gameManager.CardEffect(indice);
        }

        Debug.Log($"[Cartas] Elegida: {c.nombre}.");
    }

    private void CerrarSinElegir()
    {
        _cartaElegida = true;
        IniciarCooldown();
    }

    private void IniciarCooldown()
    {
        _estado = Estado.EnCooldown;
        _cooldownRestante = cooldownHabilidad;

        OcultarHUD();
    }

    // ------------------------------------------------------------------
    // ANIMACIONES
    // ------------------------------------------------------------------

    private IEnumerator AnimacionAbrir()
    {
        OcultarResultado();

        _fondo.gameObject.SetActive(true);
        _titulo.gameObject.SetActive(true);
        _cartasRaiz.gameObject.SetActive(true);
        _instrucciones.gameObject.SetActive(true);

        StartCoroutine(FadeImage(_fondo, 0.35f, 0.3f));

        for (int i = 0; i < 3; i++)
        {
            _cartaRT[i].localScale = new Vector3(0.7f, 0.7f, 1f);
            _cartaGrupo[i].alpha = 0f;

            StartCoroutine(AnimarEntrada(i));

            yield return new WaitForSeconds(tiempoAbrir / 3f);
        }
    }

    private IEnumerator AnimarEntrada(int i)
    {
        RectTransform rt = _cartaRT[i];
        CanvasGroup g = _cartaGrupo[i];

        float t = 0f;
        const float dur = 0.25f;

        while (t < dur)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / dur);

            rt.localScale = Vector3.Lerp(new Vector3(0.7f, 0.7f, 1f), Vector3.one, k);
            g.alpha = k;

            yield return null;
        }

        rt.localScale = Vector3.one;
        g.alpha = 1f;
    }

    private IEnumerator AnimacionElegir(int indice)
    {
        CardDef c = cartas[indice];

        StartCoroutine(PulsoCarta(_cartaRT[indice], 1.15f, 3));

        for (int j = 0; j < 3; j++)
        {
            if (j != indice)
            {
                StartCoroutine(FadeCanvasGroup(_cartaGrupo[j], 0f, 0.25f));
            }
        }

        StartCoroutine(FlashColor(c.color, tiempoFlash));

        yield return new WaitForSeconds(tiempoAnimacionElegir);

        yield return StartCoroutine(AnimacionResultado(c.nombre, "CARTA ACTIVADA: " + c.maldicionDesc, c.color));
    }

    private IEnumerator AnimacionResultado(string titulo, string mensaje, Color color)
    {
        _textoResultado.text = titulo + "\n" + mensaje;
        _textoResultado.color = color;

        _panelResultado.SetActive(true);
        _grupoResultado.alpha = 0f;

        float t = 0f;

        while (t < 0.2f)
        {
            t += Time.deltaTime;
            _grupoResultado.alpha = Mathf.Clamp01(t / 0.2f);

            yield return null;
        }

        _grupoResultado.alpha = 1f;

        yield return new WaitForSeconds(tiempoResultado);

        t = 0f;

        while (t < 0.3f)
        {
            t += Time.deltaTime;
            _grupoResultado.alpha = 1f - Mathf.Clamp01(t / 0.3f);

            yield return null;
        }

        _grupoResultado.alpha = 0f;

        OcultarHUD();
        IniciarCooldown();
    }

    private IEnumerator PulsoCarta(RectTransform rt, float factor, int veces)
    {
        Vector3 baseScale = Vector3.one;

        for (int v = 0; v < veces; v++)
        {
            float t = 0f;

            while (t < 0.3f)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(baseScale, new Vector3(factor, factor, 1f), t / 0.3f);

                yield return null;
            }

            t = 0f;

            while (t < 0.3f)
            {
                t += Time.deltaTime;
                rt.localScale = Vector3.Lerp(new Vector3(factor, factor, 1f), baseScale, t / 0.3f);

                yield return null;
            }
        }

        rt.localScale = baseScale;
    }

    // NUEVA FEATURE: Animación de aparición del Power Up
    private IEnumerator AnimarPowerUpIcon()
    {
        float t = 0f;
        float duration = 0.35f;

        _powerUpIconRT.localScale = Vector3.zero;

        Color iconColor = _powerUpIcon.color;
        iconColor.a = 0f;
        _powerUpIcon.color = iconColor;

        // NUEVA FEATURE: La carta aparece creciendo
        while (t < duration)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / duration);

            _powerUpIconRT.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.2f, 1.2f, 1f), k);

            iconColor.a = k;
            _powerUpIcon.color = iconColor;

            yield return null;
        }

        t = 0f;

        // NUEVA FEATURE: Pequeño rebote al terminar
        while (t < 0.2f)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / 0.2f);

            _powerUpIconRT.localScale = Vector3.Lerp(new Vector3(1.2f, 1.2f, 1f), Vector3.one, k);

            yield return null;
        }

        _powerUpIconRT.localScale = Vector3.one;

        iconColor.a = 1f;
        _powerUpIcon.color = iconColor;

        _powerUpAnimation = null;
    }

    private IEnumerator FlashColor(Color color, float duracion)
    {
        _flash.color = new Color(color.r, color.g, color.b, 0.25f);

        float t = 0f;

        while (t < duracion)
        {
            t += Time.deltaTime;

            Color c = _flash.color;
            c.a = Mathf.Lerp(0.25f, 0f, Mathf.Clamp01(t / duracion));

            _flash.color = c;

            yield return null;
        }

        _flash.color = new Color(color.r, color.g, color.b, 0f);
    }

    private IEnumerator FadeImage(Image img, float targetAlpha, float dur)
    {
        Color c = img.color;

        float a0 = c.a;
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(a0, targetAlpha, Mathf.Clamp01(t / dur));
            img.color = c;

            yield return null;
        }

        c.a = targetAlpha;
        img.color = c;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup g, float target, float dur)
    {
        float a0 = g.alpha;
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;

            g.alpha = Mathf.Lerp(a0, target, Mathf.Clamp01(t / dur));

            yield return null;
        }

        g.alpha = target;
    }

    // ------------------------------------------------------------------
    // TEXTOS
    // ------------------------------------------------------------------

    private void ActualizarTextos()
    {
        if (_textoCooldown == null)
        {
            return;
        }

        switch (_estado)
        {
            case Estado.Idle:
                _instrucciones.text = "";
                _textoCooldown.text = "";
                break;

            case Estado.Seleccionando:
                _instrucciones.text = "J / K / L para elegir   ·   Tab para cerrar";
                _textoCooldown.text = "";
                break;

            case Estado.EnCooldown:
                showPowerUp = true;
                _instrucciones.text = "";
                _textoCooldown.text = "";
                break;
        }
    }

    // ------------------------------------------------------------------
    // CONSTRUIR UI
    // ------------------------------------------------------------------

    private void ConstruirCanvas()
    {
        _fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject root = new GameObject("CardSelectionHUD");

        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform rootRT = (RectTransform)root.transform;

        _fondo = CrearImagenStretch(rootRT, "Fondo", new Color(0f, 0f, 0f, 0.35f));

        // NUEVA FEATURE: Título más separado de las cartas
        _titulo = CrearTexto(rootRT, "Titulo", "Elige tu carta", 40, Color.white, new Vector2(0f, 330f), new Vector2(1000f, 60f), TextAnchor.MiddleCenter);

        // NUEVA FEATURE: Zona de cartas
        _cartasRaiz = CrearRectangulo(rootRT, "Cartas", new Vector2(0f, 20f), new Vector2(1160f, 520f));

        for (int i = 0; i < 3; i++)
        {
            CrearCarta(i, cartas[i]);
        }

        // NUEVA FEATURE: J/K/L debajo de las cartas
        _instrucciones = CrearTexto(rootRT, "Instrucciones", "", 22, new Color(1f, 1f, 1f, 0.85f), new Vector2(0f, -310f), new Vector2(1400f, 50f), TextAnchor.MiddleCenter);

        RectTransform resRT = CrearRectangulo(rootRT, "Resultado", new Vector2(0f, -220f), new Vector2(900f, 180f));

        Image resBg = resRT.gameObject.AddComponent<Image>();
        resBg.color = new Color(0f, 0f, 0f, 0.8f);
        resBg.raycastTarget = false;

        _grupoResultado = resRT.gameObject.AddComponent<CanvasGroup>();
        _panelResultado = resRT.gameObject;

        _textoResultado = CrearTexto(resRT, "TextoResultado", "", 26, Color.white, Vector2.zero, new Vector2(860f, 160f), TextAnchor.MiddleCenter);

        _flash = CrearImagenStretch(rootRT, "Flash", new Color(1f, 1f, 1f, 0f));

        _textoCooldown = CrearTexto(rootRT, "Cooldown", "", 20, Color.white, new Vector2(30f, 40f), new Vector2(600f, 40f), TextAnchor.LowerLeft);

        _textoCartaActiva = CrearTexto(rootRT, "CartaActiva", "", 20, new Color(1f, 1f, 1f, 0.9f), new Vector2(30f, 80f), new Vector2(600f, 40f), TextAnchor.LowerLeft);

        // NUEVA FEATURE: Icono del Power Up activo
        _powerUpIcon = CrearImagen(rootRT, "PowerUpIcon", new Vector2(820f, 410f), new Vector2(120f, 160f), Color.white);
        _powerUpIcon.preserveAspect = true;
        _powerUpIconRT = _powerUpIcon.rectTransform;
        _powerUpIcon.gameObject.SetActive(false);

        OcultarHUD();
        OcultarResultado();
    }

    private void CrearCarta(int indice, CardDef def)
    {
        RectTransform raiz = CrearRectangulo(_cartasRaiz, "Carta_" + indice + "_" + def.nombre.Replace(" ", "_"), new Vector2(-400f + indice * 400f, 0f), new Vector2(360f, 500f));

        Image borde = raiz.gameObject.AddComponent<Image>();
        borde.color = new Color(0f, 0f, 0f, 0.9f);
        borde.raycastTarget = false;

        CanvasGroup grupo = raiz.gameObject.AddComponent<CanvasGroup>();

        _cartaRT[indice] = raiz;
        _cartaGrupo[indice] = grupo;

        // NUEVA FEATURE: Margen para que la imagen no se vea recortada
        Image relleno = CrearImagen(raiz, "Relleno", Vector2.zero, new Vector2(336f, 468f), Color.white);

        if (def.imagen != null)
        {
            relleno.sprite = def.imagen;
            relleno.type = Image.Type.Simple;
            relleno.preserveAspect = true;
        }
    }

    private void OcultarHUD()
    {
        _fondo.gameObject.SetActive(false);
        _titulo.gameObject.SetActive(false);
        _cartasRaiz.gameObject.SetActive(false);
        _instrucciones.gameObject.SetActive(false);

        // NUEVA FEATURE:
        // NO apagamos _powerUpIcon porque debe quedarse visible durante el juego.
    }

    private void OcultarResultado()
    {
        if (_panelResultado != null)
        {
            _panelResultado.SetActive(false);
        }
    }

    // ------------------------------------------------------------------
    // HELPERS UI
    // ------------------------------------------------------------------

    private RectTransform CrearRectangulo(Transform padre, string nombre, Vector2 pos, Vector2 tam)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);

        RectTransform rt = go.AddComponent<RectTransform>();

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.anchoredPosition = pos;
        rt.sizeDelta = tam;

        return rt;
    }

    private Image CrearImagen(RectTransform padre, string nombre, Vector2 pos, Vector2 tam, Color color)
    {
        RectTransform rt = CrearRectangulo(padre, nombre, pos, tam);

        Image img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;

        return img;
    }

    private Image CrearImagenStretch(RectTransform padre, string nombre, Color color)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);

        RectTransform rt = go.AddComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = false;

        return img;
    }

    private Text CrearTexto(RectTransform padre, string nombre, string contenido, int fontSize, Color color, Vector2 pos, Vector2 tam, TextAnchor alineacion)
    {
        RectTransform rt = CrearRectangulo(padre, nombre, pos, tam);

        Text t = rt.gameObject.AddComponent<Text>();

        t.text = contenido;
        t.font = _fuente;
        t.fontSize = fontSize;
        t.color = color;
        t.alignment = alineacion;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;

        return t;
    }
}