using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Carta de historia: flota, gira, se abre con E y se voltea con V.
/// </summary>
public class StoryCard : MonoBehaviour
{
    [Header("Historia")]
    public int numeroHistoria;
    public string nombreImagen;
    [TextArea(5, 10)]
    public string textoHistoria;

    [Header("Interacción")]
    public float distanciaInteraccion = 3f;

    [Header("Flotación")]
    public float alturaFlotacion = 0.15f;
    public float velocidadRotacion = 70f;

    [Header("Movimiento con Mouse")]
    public float velocidadScroll = 0.5f;
    public float alturaMaxima = 3f;
    public float alturaMinima = 0.5f;

    // Estados: 0 = flotando, 1 = abierta (imagen), 2 = volteada (reverso)
    private int _estado = 0;
    private Vector3 _basePos;
    private float _offsetY = 0f;
    private Renderer _renderer;
    private Texture2D _texturaAnverso;
    private Texture2D _texturaReverso;

    // UI
    private GameObject _panelAbierto;    // Imagen grande
    private GameObject _panelReverso;    // Carta quemada + texto
    private CanvasGroup _canvasGroupAbierto;
    private CanvasGroup _canvasGroupReverso;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer == null)
        {
            Debug.LogError($"[StoryCard] {gameObject.name} no tiene Renderer.");
            return;
        }

        _basePos = transform.position;
        CargarTexturas();
        CrearUIAbierto();
        CrearUIReverso();
    }

    private void CargarTexturas()
    {
        _texturaAnverso = Resources.Load<Texture2D>("Historias/" + nombreImagen);
        if (_texturaAnverso != null)
        {
            _renderer.material.mainTexture = _texturaAnverso;
        }
        else
        {
            Debug.LogWarning($"[StoryCard] No se encontró: {nombreImagen}");
        }

        _texturaReverso = Resources.Load<Texture2D>("Historias/Carta quemada");
        if (_texturaReverso == null)
        {
            Debug.LogWarning("[StoryCard] No se encontró Carta quemada.png");
        }

        // Rendering por ambos lados
        _renderer.material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
    }

    private void CrearUIAbierto()
    {
        // Panel grande con la imagen de la historia
        GameObject canvasObj = new GameObject("UI_Abierto");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, 0, -0.5f);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rect = canvasObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(3, 4);
        rect.anchoredPosition = Vector2.zero;

        _canvasGroupAbierto = canvasObj.AddComponent<CanvasGroup>();
        _canvasGroupAbierto.alpha = 0f;

        // Imagen grande
        GameObject imagenObj = new GameObject("Imagen");
        imagenObj.transform.SetParent(canvasObj.transform);

        Image imagen = imagenObj.AddComponent<Image>();
        if (_texturaAnverso != null)
        {
            Sprite sprite = Sprite.Create(_texturaAnverso,
                new Rect(0, 0, _texturaAnverso.width, _texturaAnverso.height),
                Vector2.one * 0.5f);
            imagen.sprite = sprite;
        }

        RectTransform imagenRect = imagenObj.GetComponent<RectTransform>();
        imagenRect.anchorMin = Vector2.zero;
        imagenRect.anchorMax = Vector2.one;
        imagenRect.offsetMin = Vector2.zero;
        imagenRect.offsetMax = Vector2.zero;

        _panelAbierto = canvasObj;
    }

    private void CrearUIReverso()
    {
        // Panel con Carta quemada + texto
        GameObject canvasObj = new GameObject("UI_Reverso");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = new Vector3(0, 0, -0.5f);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        RectTransform rect = canvasObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(3, 4);
        rect.anchoredPosition = Vector2.zero;

        _canvasGroupReverso = canvasObj.AddComponent<CanvasGroup>();
        _canvasGroupReverso.alpha = 0f;

        // Fondo (Carta quemada)
        GameObject fondoObj = new GameObject("Fondo");
        fondoObj.transform.SetParent(canvasObj.transform);

        Image fondo = fondoObj.AddComponent<Image>();
        if (_texturaReverso != null)
        {
            Sprite sprite = Sprite.Create(_texturaReverso,
                new Rect(0, 0, _texturaReverso.width, _texturaReverso.height),
                Vector2.one * 0.5f);
            fondo.sprite = sprite;
        }

        RectTransform fondoRect = fondoObj.GetComponent<RectTransform>();
        fondoRect.anchorMin = Vector2.zero;
        fondoRect.anchorMax = Vector2.one;
        fondoRect.offsetMin = Vector2.zero;
        fondoRect.offsetMax = Vector2.zero;

        // Texto de la historia
        GameObject textoObj = new GameObject("Texto");
        textoObj.transform.SetParent(canvasObj.transform);

        Text texto = textoObj.AddComponent<Text>();
        texto.text = textoHistoria;
        texto.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        texto.fontSize = 8;
        texto.fontStyle = FontStyle.Normal;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = new Color(0.9f, 0.85f, 0.75f);
        texto.verticalOverflow = VerticalWrapMode.Truncate;
        texto.horizontalOverflow = HorizontalWrapMode.Wrap;

        RectTransform textoRect = textoObj.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = new Vector2(100, 100);
        textoRect.offsetMax = new Vector2(-100, -100);

        _panelReverso = canvasObj;
    }

    private void Update()
    {
        // Solo flotar y girar si está en estado inicial (0)
        if (_estado == 0)
        {
            float onda = (Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f);
            transform.position = _basePos + Vector3.up * (onda * alturaFlotacion) + new Vector3(0, _offsetY, 0);
            transform.Rotate(0f, velocidadRotacion * Time.deltaTime, 0f, Space.World);
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null) return;

        float distancia = Vector3.Distance(jugador.transform.position, transform.position);

        if (distancia <= distanciaInteraccion)
        {
            // E = abrir (si está cerrada)
            if (Input.GetKeyDown(KeyCode.E) && _estado == 0)
            {
                Abrir();
            }

            // V = voltear (si está abierta)
            if (Input.GetKeyDown(KeyCode.V) && _estado == 1)
            {
                Voltear();
            }

            // Scroll para mover la carta
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0f)
            {
                _offsetY += scroll * velocidadScroll;
                _offsetY = Mathf.Clamp(_offsetY, alturaMinima - _basePos.y, alturaMaxima - _basePos.y);
            }
        }
        else
        {
            // Si el jugador se aleja, cerrar todo
            if (_estado != 0)
            {
                Cerrar();
            }

            if (Mathf.Abs(_offsetY) > 0.01f)
            {
                _offsetY = Mathf.Lerp(_offsetY, 0f, Time.deltaTime * 3f);
            }
        }
    }

    private void Abrir()
    {
        _estado = 1;
        _canvasGroupAbierto.alpha = 1f;
        Debug.Log($"[StoryCard] Abierta: {nombreImagen}");
    }

    private void Voltear()
    {
        _estado = 2;
        _canvasGroupAbierto.alpha = 0f;
        _canvasGroupReverso.alpha = 1f;
        Debug.Log($"[StoryCard] Reverso: {nombreImagen}");
    }

    private void Cerrar()
    {
        _estado = 0;
        _canvasGroupAbierto.alpha = 0f;
        _canvasGroupReverso.alpha = 0f;
        Debug.Log($"[StoryCard] Cerrada: {nombreImagen}");
    }
}