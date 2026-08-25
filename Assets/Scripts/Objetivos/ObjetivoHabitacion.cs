using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Muestra un objetivo/alerta al entrar a la habitación.
/// Estilo RelicRoom: si asignas un GameObject de clue, solo lo activa/desactiva.
/// Si no, crea la UI automáticamente.
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

    [Header("UI (estilo RelicRoom)")]
    [Tooltip("Si asignas un GameObject con Text, se activa/desactiva como RoomClue. Si lo dejas vacío, se crea la UI automáticamente.")]
    public GameObject clueUI;

    [Tooltip("Objeto hijo con el Text del tipo de objetivo (opcional, se busca por nombre 'TextoTipo' si no se asigna)")]
    public Text textoTipo;

    [Tooltip("Objeto hijo con el Text de la descripción (opcional, se busca por nombre 'TextoObjetivo' si no se asigna)")]
    public Text textoObjetivo;

    [Header("Parámetros")]
    [Tooltip("Tiempo en segundos que debe sobrevivir (Tipo 2)")]
    public float tiempoSobrevivir = 30f;

    [Tooltip("Cantidad de enemigos a matar (Tipo 3)")]
    public int cantidadEnemigos = 5;

    [Header("Alerta")]
    [Tooltip("Duración que se muestra el mensaje (segundos)")]
    public float duracionAlerta = 5f;

    [Tooltip("Segundos antes de mostrar la alerta al entrar")]
    public float delayInicial = 1f;

    private bool _mostrado;
    private Canvas _canvasAuto;
    private CanvasGroup _grupoCanvas;

    private const float ANCHO_PANEL = 600f;
    private const float ALTO_PANEL = 120f;

    private void Start()
    {
        // Si tiene clueUI pre-existente (estilo RelicRoom), solo ocultarlo
        if (clueUI != null)
        {
            clueUI.SetActive(false);
            // Si tiene TextoTipo y TextoObjetivo, actualizarlos
            if (textoTipo == null)
            {
                Transform t = clueUI.transform.Find("TextoTipo");
                if (t != null) textoTipo = t.GetComponent<Text>();
            }
            if (textoObjetivo == null)
            {
                Transform t = clueUI.transform.Find("TextoObjetivo");
                if (t != null) textoObjetivo = t.GetComponent<Text>();
            }
            ActualizarTexto();
        }
        else
        {
            // Sin clueUI: crear canvas automáticamente
            CrearUI();
            _canvasAuto.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_mostrado)
        {
            _mostrado = true;
            ActualizarTexto();
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
                return $"Sobrevive {tiempoSobrevivir:F0} segundos en la habitación";
            case TipoObjetivo.MatarEnemigos:
                return $"Mata {cantidadEnemigos} enemigos";
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

    private string ObtenerEtiquetaTipo()
    {
        switch (tipoObjetivo)
        {
            case TipoObjetivo.BuscarTarjeta:    return "OBJETIVO";
            case TipoObjetivo.SobrevivirTiempo:  return "TIEMPO";
            case TipoObjetivo.MatarEnemigos:     return "COMBATE";
            case TipoObjetivo.AtrapaVelas:       return "VELAS";
            case TipoObjetivo.PrendeLinternas:   return "LINTERNA";
            case TipoObjetivo.DestruyeCajas:     return "DESTRUCCIÓN";
            default:                             return "OBJETIVO";
        }
    }

    private Color ObtenerColorTipo()
    {
        switch (tipoObjetivo)
        {
            case TipoObjetivo.BuscarTarjeta:    return new Color(0.957f, 0.788f, 0.365f);
            case TipoObjetivo.SobrevivirTiempo:  return new Color(0.3f, 0.8f, 0.9f);
            case TipoObjetivo.MatarEnemigos:     return new Color(0.9f, 0.3f, 0.3f);
            case TipoObjetivo.AtrapaVelas:       return new Color(1f, 0.6f, 0.2f);
            case TipoObjetivo.PrendeLinternas:   return new Color(0.9f, 0.9f, 0.4f);
            case TipoObjetivo.DestruyeCajas:     return new Color(0.8f, 0.5f, 0.2f);
            default:                             return FuentesJuego.Dorado;
        }
    }

    private void ActualizarTexto()
    {
        if (textoTipo != null)
        {
            textoTipo.text = ObtenerEtiquetaTipo();
            textoTipo.color = ObtenerColorTipo();
        }
        if (textoObjetivo != null)
            textoObjetivo.text = ObtenerTextoObjetivo();
    }

    private IEnumerator MostrarAlerta()
    {
        yield return new WaitForSeconds(delayInicial);

        if (clueUI != null)
        {
            // Estilo RelicRoom: solo SetActive
            clueUI.SetActive(true);
            yield return new WaitForSeconds(duracionAlerta);
            clueUI.SetActive(false);
        }
        else
        {
            // Canvas automático: fade in/out
            _canvasAuto.gameObject.SetActive(true);
            _grupoCanvas.alpha = 0f;

            float t = 0f;
            while (t < 0.4f)
            {
                t += Time.deltaTime;
                _grupoCanvas.alpha = Mathf.Lerp(0f, 1f, t / 0.4f);
                yield return null;
            }
            _grupoCanvas.alpha = 1f;

            yield return new WaitForSeconds(duracionAlerta);

            t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                _grupoCanvas.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
                yield return null;
            }
            _grupoCanvas.alpha = 0f;
            _canvasAuto.gameObject.SetActive(false);
        }
    }

    private void CrearUI()
    {
        GameObject canvasGo = new GameObject("HUD_Objetivo_" + tipoObjetivo);
        _canvasAuto = canvasGo.AddComponent<Canvas>();
        _canvasAuto.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvasAuto.sortingOrder = 900;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();
        _grupoCanvas = canvasGo.AddComponent<CanvasGroup>();

        GameObject panelGo = CrearRect("PanelObjetivo", canvasGo.transform);
        RectTransform panelRect = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -60);
        panelRect.sizeDelta = new Vector2(ANCHO_PANEL, ALTO_PANEL);
        Image panelImg = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.04f, 0.08f, 0.92f);

        Outline borde = panelGo.AddComponent<Outline>();
        borde.effectColor = new Color(0.957f, 0.788f, 0.365f, 0.6f);
        borde.effectDistance = new Vector2(2, -2);

        GameObject tipoGo = CrearRect("TextoTipo", panelGo.transform);
        RectTransform tipoRect = tipoGo.GetComponent<RectTransform>();
        tipoRect.anchorMin = new Vector2(0.05f, 0.55f);
        tipoRect.anchorMax = new Vector2(0.95f, 0.95f);
        tipoRect.offsetMin = Vector2.zero;
        tipoRect.offsetMax = Vector2.zero;
        textoTipo = tipoGo.AddComponent<Text>();
        FuentesJuego.Aplicar(textoTipo, 20, ObtenerColorTipo(), true, true);
        textoTipo.alignment = TextAnchor.MiddleCenter;
        textoTipo.text = ObtenerEtiquetaTipo();

        GameObject objGo = CrearRect("TextoObjetivo", panelGo.transform);
        RectTransform objRect = objGo.GetComponent<RectTransform>();
        objRect.anchorMin = new Vector2(0.05f, 0.05f);
        objRect.anchorMax = new Vector2(0.95f, 0.55f);
        objRect.offsetMin = Vector2.zero;
        objRect.offsetMax = Vector2.zero;
        textoObjetivo = objGo.AddComponent<Text>();
        FuentesJuego.Aplicar(textoObjetivo, 28, FuentesJuego.TextoSecundario, false, false);
        textoObjetivo.alignment = TextAnchor.MiddleCenter;
        textoObjetivo.text = ObtenerTextoObjetivo();
    }

    private GameObject CrearRect(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }
}
