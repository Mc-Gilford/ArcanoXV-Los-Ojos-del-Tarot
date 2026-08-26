using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Notes : MonoBehaviour
{
    [Header("Interaccion")]
    [SerializeField] private float interactionDistance = 10f;

    [Header("Flotacion")]
    [SerializeField] private float floatHeight = 0.10f;
    [SerializeField] private float floatSpeed = 1.4f;
    [SerializeField] private float tiltAmount = 2.5f;
    [SerializeField] private float tiltSpeed = 1.1f;

    [Header("Animacion")]
    [SerializeField] private float openDuration = 0.40f;
    [SerializeField] private float closeDuration = 0.25f;

    private GameObject player;

    private SpriteRenderer noteSpriteRenderer;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private bool playerNear = false;
    private bool noteOpen = false;
    private bool isAnimating = false;

    private Canvas noteCanvas;
    private CanvasGroup documentGroup;

    private Image darkBackground;
    private Image noteImage;

    private RectTransform noteImageRT;

    private Text interactionText;
    private Text closeText;

    private Coroutine animationCoroutine;

    void Start()
    {
        // NUEVA FEATURE: Obtiene automaticamente el SpriteRenderer del mismo objeto
        noteSpriteRenderer = GetComponent<SpriteRenderer>();

        if (noteSpriteRenderer == null)
        {
            Debug.LogError("Notes: Este objeto necesita un SpriteRenderer.");
            enabled = false;
            return;
        }

        // NUEVA FEATURE: Busca automaticamente al jugador
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Notes: No se encontro un objeto con Tag Player.");
        }

        startPosition = transform.position;
        startRotation = transform.rotation;

        CreateCanvas();
        HideNoteInstant();
    }

    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // NUEVA FEATURE: La nota flota mientras no esta abierta
        if (!noteOpen)
        {
            FloatNote();
        }

        CheckPlayerDistance();

        // NUEVA FEATURE: X abre la nota
        if (playerNear && !noteOpen && !isAnimating && Keyboard.current.xKey.wasPressedThisFrame)
        {
            OpenNote();
            return;
        }

        // NUEVA FEATURE: V cierra la nota
        if (noteOpen && !isAnimating && Keyboard.current.vKey.wasPressedThisFrame)
        {
            CloseNote();
        }
    }

    // ---------------------------------------------------------
    // DISTANCIA DEL PLAYER
    // ---------------------------------------------------------

    private void CheckPlayerDistance()
    {
        if (player == null)
        {
            playerNear = false;
            interactionText.gameObject.SetActive(false);
            return;
        }

        float distance = Vector3.Distance(transform.position, player.transform.position);

        playerNear = distance <= interactionDistance;

        interactionText.gameObject.SetActive(playerNear && !noteOpen);
    }

    // ---------------------------------------------------------
    // FLOTACION
    // ---------------------------------------------------------

    private void FloatNote()
    {
        // NUEVA FEATURE: Movimiento vertical suave
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // NUEVA FEATURE: Balanceo leve
        float tiltX = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;
        float tiltZ = Mathf.Cos(Time.time * tiltSpeed * 0.8f) * tiltAmount;

        Quaternion floatingRotation = startRotation * Quaternion.Euler(tiltX, 0f, tiltZ);

        transform.rotation = Quaternion.Slerp(transform.rotation, floatingRotation, Time.deltaTime * 2.5f);
    }

    // ---------------------------------------------------------
    // ABRIR NOTA
    // ---------------------------------------------------------

    private void OpenNote()
    {
        noteOpen = true;

        interactionText.gameObject.SetActive(false);
        documentGroup.gameObject.SetActive(true);

        // NUEVA FEATURE: Pausa el juego
        Time.timeScale = 0f;

        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(OpenAnimation());
    }

    private IEnumerator OpenAnimation()
    {
        isAnimating = true;

        documentGroup.alpha = 0f;

        // NUEVA FEATURE: Entrada pequeña e inclinada
        noteImageRT.localScale = new Vector3(0.72f, 0.72f, 1f);
        noteImageRT.localRotation = Quaternion.Euler(0f, 0f, -4f);

        float timer = 0f;

        while (timer < openDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / openDuration);

            // NUEVA FEATURE: Suavizado estilo cinematico
            float smooth = 1f - Mathf.Pow(1f - progress, 3f);

            documentGroup.alpha = smooth;

            noteImageRT.localScale = Vector3.Lerp(new Vector3(0.72f, 0.72f, 1f), new Vector3(1.04f, 1.04f, 1f), smooth);

            noteImageRT.localRotation = Quaternion.Lerp(Quaternion.Euler(0f, 0f, -4f), Quaternion.identity, smooth);

            yield return null;
        }

        // NUEVA FEATURE: Pequeño rebote final
        timer = 0f;

        while (timer < 0.14f)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / 0.14f);

            noteImageRT.localScale = Vector3.Lerp(new Vector3(1.04f, 1.04f, 1f), Vector3.one, progress);

            yield return null;
        }

        noteImageRT.localScale = Vector3.one;
        noteImageRT.localRotation = Quaternion.identity;

        documentGroup.alpha = 1f;

        closeText.gameObject.SetActive(true);

        isAnimating = false;
    }

    // ---------------------------------------------------------
    // CERRAR NOTA
    // ---------------------------------------------------------

    private void CloseNote()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        animationCoroutine = StartCoroutine(CloseAnimation());
    }

    private IEnumerator CloseAnimation()
    {
        isAnimating = true;

        closeText.gameObject.SetActive(false);

        float timer = 0f;

        Vector3 currentScale = noteImageRT.localScale;

        while (timer < closeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(timer / closeDuration);

            documentGroup.alpha = 1f - progress;

            noteImageRT.localScale = Vector3.Lerp(currentScale, new Vector3(0.80f, 0.80f, 1f), progress);

            noteImageRT.localRotation = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0f, 0f, 3f), progress);

            yield return null;
        }

        documentGroup.alpha = 0f;
        documentGroup.gameObject.SetActive(false);

        noteOpen = false;
        isAnimating = false;

        // NUEVA FEATURE: Reanuda el juego
        Time.timeScale = 1f;

        noteImageRT.localScale = Vector3.one;
        noteImageRT.localRotation = Quaternion.identity;

        CheckPlayerDistance();
    }

    // ---------------------------------------------------------
    // CREAR CANVAS
    // ---------------------------------------------------------

    private void CreateCanvas()
    {
        GameObject canvasObject = new GameObject("NoteCanvas");

        noteCanvas = canvasObject.AddComponent<Canvas>();
        noteCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        noteCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        RectTransform canvasRT = canvasObject.GetComponent<RectTransform>();

        // NUEVA FEATURE: Texto X separado del documento
        CreateInteractionText(canvasRT);

        // NUEVA FEATURE: Grupo completo de lectura
        GameObject documentObject = new GameObject("DocumentView");
        documentObject.transform.SetParent(canvasRT, false);

        RectTransform documentRT = documentObject.AddComponent<RectTransform>();

        documentRT.anchorMin = Vector2.zero;
        documentRT.anchorMax = Vector2.one;
        documentRT.offsetMin = Vector2.zero;
        documentRT.offsetMax = Vector2.zero;

        documentGroup = documentObject.AddComponent<CanvasGroup>();

        CreateBackground(documentRT);
        CreateNoteImage(documentRT);
        CreateCloseText(documentRT);
    }

    // ---------------------------------------------------------
    // FONDO
    // ---------------------------------------------------------

    private void CreateBackground(RectTransform padre)
    {
        GameObject backgroundObject = new GameObject("DarkBackground");
        backgroundObject.transform.SetParent(padre, false);

        RectTransform backgroundRT = backgroundObject.AddComponent<RectTransform>();

        backgroundRT.anchorMin = Vector2.zero;
        backgroundRT.anchorMax = Vector2.one;
        backgroundRT.offsetMin = Vector2.zero;
        backgroundRT.offsetMax = Vector2.zero;

        darkBackground = backgroundObject.AddComponent<Image>();

        // NUEVA FEATURE: Fondo oscuro pero aun permite ver ligeramente el juego
        darkBackground.color = new Color(0f, 0f, 0f, 0.90f);
        darkBackground.raycastTarget = false;
    }

    // ---------------------------------------------------------
    // IMAGEN GRANDE DE LA NOTA
    // ---------------------------------------------------------

    private void CreateNoteImage(RectTransform padre)
    {
        GameObject noteObject = new GameObject("NoteImage");
        noteObject.transform.SetParent(padre, false);

        noteImageRT = noteObject.AddComponent<RectTransform>();

        noteImageRT.anchorMin = new Vector2(0.5f, 0.5f);
        noteImageRT.anchorMax = new Vector2(0.5f, 0.5f);
        noteImageRT.pivot = new Vector2(0.5f, 0.5f);

        noteImageRT.anchoredPosition = new Vector2(0f, 10f);

        // NUEVA FEATURE: Casi todo el alto de pantalla
        noteImageRT.sizeDelta = new Vector2(1050f, 1000f);

        noteImage = noteObject.AddComponent<Image>();

        // NUEVA FEATURE: Usa automaticamente el mismo SpriteRenderer del prefab
        noteImage.sprite = noteSpriteRenderer.sprite;

        noteImage.color = Color.white;
        noteImage.preserveAspect = true;
        noteImage.raycastTarget = false;
    }

    // ---------------------------------------------------------
    // TEXTO V CERRAR
    // ---------------------------------------------------------

    private void CreateCloseText(RectTransform padre)
    {
        GameObject closeObject = new GameObject("CloseText");
        closeObject.transform.SetParent(padre, false);

        RectTransform closeRT = closeObject.AddComponent<RectTransform>();

        closeRT.anchorMin = new Vector2(0.5f, 0f);
        closeRT.anchorMax = new Vector2(0.5f, 0f);
        closeRT.pivot = new Vector2(0.5f, 0f);

        closeRT.anchoredPosition = new Vector2(0f, 20f);
        closeRT.sizeDelta = new Vector2(700f, 50f);

        closeText = closeObject.AddComponent<Text>();

        closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        closeText.text = "[ V ] Cerrar";
        closeText.fontSize = 25;
        closeText.alignment = TextAnchor.MiddleCenter;
        closeText.color = new Color(1f, 1f, 1f, 0.9f);
        closeText.raycastTarget = false;
    }

    // ---------------------------------------------------------
    // TEXTO X LEER
    // ---------------------------------------------------------

    private void CreateInteractionText(RectTransform padre)
    {
        GameObject interactionObject = new GameObject("InteractionText");
        interactionObject.transform.SetParent(padre, false);

        RectTransform interactionRT = interactionObject.AddComponent<RectTransform>();

        interactionRT.anchorMin = new Vector2(0.5f, 0f);
        interactionRT.anchorMax = new Vector2(0.5f, 0f);
        interactionRT.pivot = new Vector2(0.5f, 0f);

        interactionRT.anchoredPosition = new Vector2(0f, 100f);
        interactionRT.sizeDelta = new Vector2(800f, 65f);

        interactionText = interactionObject.AddComponent<Text>();

        interactionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        interactionText.text = "[ X ] Leer nota";
        interactionText.fontSize = 28;
        interactionText.alignment = TextAnchor.MiddleCenter;
        interactionText.color = Color.white;
        interactionText.raycastTarget = false;

        interactionText.gameObject.SetActive(false);
    }

    // ---------------------------------------------------------
    // OCULTAR AL INICIO
    // ---------------------------------------------------------

    private void HideNoteInstant()
    {
        documentGroup.alpha = 0f;
        documentGroup.gameObject.SetActive(false);

        closeText.gameObject.SetActive(false);
        interactionText.gameObject.SetActive(false);
    }

    // ---------------------------------------------------------
    // SEGURIDAD
    // ---------------------------------------------------------

    private void OnDisable()
    {
        if (noteOpen)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
        if (noteCanvas != null)
        {
            Destroy(noteCanvas.gameObject);
        }

        if (noteOpen)
        {
            Time.timeScale = 1f;
        }
    }
}