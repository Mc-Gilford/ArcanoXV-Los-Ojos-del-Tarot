using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Muestra un objetivo/alerta al entrar a la habitación.
/// Se coloca en un GameObject con BoxCollider (isTrigger).
/// Selecciona el tipo de objetivo desde el inspector y ajusta los parámetros.
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

    [Header("Parámetros (solo se usan según el tipo)")]
    [Tooltip("Tiempo en segundos que debe sobrevivir (Tipo 2)")]
    public float tiempoSobrevivir = 30f;

    [Tooltip("Cantidad de enemigos a matar (Tipo 3)")]
    public int cantidadEnemigos = 5;

    [Header("Alerta")]
    [Tooltip("Duración que se muestra el mensaje (segundos)")]
    public float duracionAlerta = 6f;

    [Tooltip("Segundos antes de mostrar la alerta al entrar")]
    public float delayInicial = 1f;

    private Canvas _canvas;
    private Text _textoObjetivo;
    private Text _textoTipo;
    private CanvasGroup _grupoCanvas;
    private bool _mostrado;

    private const float ANCHO_PANEL = 600f;
    private const float ALTO_PANEL = 120f;

    private void Start()
    {
        CrearUI();
        _canvas.gameObject.SetActive(false);
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
            case TipoObjetivo.BuscarTarjeta:    return new Color(0.957f, 0.788f, 0.365f); // dorado
            case TipoObjetivo.SobrevivirTiempo:  return new Color(0.3f, 0.8f, 0.9f);      // azul claro
            case TipoObjetivo.MatarEnemigos:     return new Color(0.9f, 0.3f, 0.3f);      // rojo
            case TipoObjetivo.AtrapaVelas:       return new Color(1f, 0.6f, 0.2f);        // naranja
            case TipoObjetivo.PrendeLinternas:   return new Color(0.9f, 0.9f, 0.4f);      // amarillo
            case TipoObjetivo.DestruyeCajas:     return new Color(0.8f, 0.5f, 0.2f);      // marrón
            default:                             return FuentesJuego.Dorado;
        }
    }

    private void ActualizarTexto()
    {
        if (_textoTipo != null)
        {
            _textoTipo.text = ObtenerEtiquetaTipo();
            _textoTipo.color = ObtenerColorTipo();
        }
        if (_textoObjetivo != null)
            _textoObjetivo.text = ObtenerTextoObjetivo();
    }

    private IEnumerator MostrarAlerta()
    {
        yield return new WaitForSeconds(delayInicial);

        _canvas.gameObject.SetActive(true);
        _grupoCanvas.alpha = 0f;

        // Fade in
        float t = 0f;
        while (t < 0.4f)
        {
            t += Time.deltaTime;
            _grupoCanvas.alpha = Mathf.Lerp(0f, 1f, t / 0.4f);
            yield return null;
        }
        _grupoCanvas.alpha = 1f;

        // Esperar
        yield return new WaitForSeconds(duracionAlerta);

        // Fade out
        t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            _grupoCanvas.alpha = Mathf.Lerp(1f, 0f, t / 0.5f);
            yield return null;
        }
        _grupoCanvas.alpha = 0f;
        _canvas.gameObject.SetActive(false);
    }

    private void CrearUI()
    {
        // Canvas
        GameObject canvasGo = new GameObject("HUD_Objetivo_" + tipoObjetivo);
        _canvas = canvasGo.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 900;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();
        _grupoCanvas = canvasGo.AddComponent<CanvasGroup>();

        // Panel principal (parte superior centrada)
        GameObject panelGo = CrearRect("PanelObjetivo", canvasGo.transform);
        RectTransform panelRect = panelGo.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -60);
        panelRect.sizeDelta = new Vector2(ANCHO_PANEL, ALTO_PANEL);
        Image panelImg = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.04f, 0.08f, 0.92f);

        // Borde dorado
        Outline borde = panelGo.AddComponent<Outline>();
        borde.effectColor = new Color(0.957f, 0.788f, 0.365f, 0.6f);
        borde.effectDistance = new Vector2(2, -2);

        // Texto tipo (etiqueta arriba)
        GameObject tipoGo = CrearRect("TextoTipo", panelGo.transform);
        RectTransform tipoRect = tipoGo.GetComponent<RectTransform>();
        tipoRect.anchorMin = new Vector2(0.05f, 0.55f);
        tipoRect.anchorMax = new Vector2(0.95f, 0.95f);
        tipoRect.offsetMin = Vector2.zero;
        tipoRect.offsetMax = Vector2.zero;
        _textoTipo = tipoGo.AddComponent<Text>();
        FuentesJuego.Aplicar(_textoTipo, 20, ObtenerColorTipo(), true, true);
        _textoTipo.alignment = TextAnchor.MiddleCenter;
        _textoTipo.text = ObtenerEtiquetaTipo();

        // Texto objetivo (descripción abajo)
        GameObject objGo = CrearRect("TextoObjetivo", panelGo.transform);
        RectTransform objRect = objGo.GetComponent<RectTransform>();
        objRect.anchorMin = new Vector2(0.05f, 0.05f);
        objRect.anchorMax = new Vector2(0.95f, 0.55f);
        objRect.offsetMin = Vector2.zero;
        objRect.offsetMax = Vector2.zero;
        _textoObjetivo = objGo.AddComponent<Text>();
        FuentesJuego.Aplicar(_textoObjetivo, 28, FuentesJuego.TextoSecundario, false, false);
        _textoObjetivo.alignment = TextAnchor.MiddleCenter;
        _textoObjetivo.text = ObtenerTextoObjetivo();
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
