using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contador de cartas recogidas del jugador (coleccionables). Cuando se llega al
/// total (5 por defecto) dispara OnTodasRecogidas para abrir la puerta del jefe final.
/// Muestra un contador "Cartas: X/5" en la esquina inferior derecha.
/// </summary>
public class CardCollector : MonoBehaviour
{
    /// <summary>Acceso global para que las cartas y la puerta lo encuentren fácil.</summary>
    public static CardCollector Instance { get; private set; }

    [Tooltip("Cuántas cartas hay que recoger para abrir la puerta del jefe final.")]
    public int cartasNecesarias = 5;

    public int CartasObtenidas { get; private set; }
    public bool TodasRecogidas => CartasObtenidas >= cartasNecesarias;

    /// <summary>Se invoca al recoger una carta (obtenidas, necesarias).</summary>
    public event System.Action<int, int> OnCartaRecogida;
    /// <summary>Se invoca cuando ya están TODAS las cartas (abre la puerta).</summary>
    public event System.Action OnTodasRecogidas;

    private Text _hud;

    private void Awake()
    {
        Instance = this;
        CrearHud();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void RecogerCarta()
    {
        if (TodasRecogidas) return;
        CartasObtenidas = Mathf.Min(cartasNecesarias, CartasObtenidas + 1);

        OnCartaRecogida?.Invoke(CartasObtenidas, cartasNecesarias);
        ActualizarHud();
        Debug.Log($"[Cartas] Carta recogida: {CartasObtenidas}/{cartasNecesarias}.");

        if (TodasRecogidas)
        {
            Debug.Log("[Cartas] ¡Todas las cartas reunidas! Se abre la puerta del jefe final.");
            OnTodasRecogidas?.Invoke();
        }
    }

    private void CrearHud()
    {
        GameObject root = new GameObject("HUD_Cartas");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject txt = new GameObject("Contador");
        txt.transform.SetParent(root.transform, false);
        RectTransform rt = txt.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-30f, 40f);
        rt.sizeDelta = new Vector2(500f, 50f);

        _hud = txt.AddComponent<Text>();
        _hud.font = font;
        _hud.fontSize = 26;
        _hud.color = new Color(1f, 0.85f, 0.3f);   // dorado
        _hud.alignment = TextAnchor.MiddleRight;
        _hud.raycastTarget = false;

        ActualizarHud();
    }

    private void ActualizarHud()
    {
        if (_hud != null)
            _hud.text = "Cartas: " + CartasObtenidas + "/" + cartasNecesarias;
    }
}