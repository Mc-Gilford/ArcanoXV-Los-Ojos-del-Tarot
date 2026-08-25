using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

/// <summary>
/// Genera la escena "FinPartida" con:
/// - Fondo oscuro violáceo
/// - Texto "FIN DE LA PARTIDA"
/// - Texto del tiempo jugado
/// - Campo de nombre (InputField)
/// - Botón Guardar
/// - Lista de ranking
/// - Botón Volver al Menú
/// También agrega la escena a Build Settings si falta.
/// </summary>
public static class GenerarEscenaFinPartida
{
    private const string NombreEscena = "FinPartida";
    private const string RutaEscena = "Assets/Scenes/" + NombreEscena + ".unity";

    [MenuItem("Tools/Arcano XV/Generar Escena Fin Partida")]
    public static void Generar()
    {
        // Crear escena vacía
        var escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---- EventSystem (requerido por InputField) ----
        GameObject esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
        esGo.AddComponent<StandaloneInputModule>();

        // ---- Cámara (necesaria para evitar "Display cameras rendering") ----
        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        Camera cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.03f, 0.07f);
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.cullingMask = 0;
        cam.depth = -100;

        // ---- Canvas ----
        GameObject canvasGo = new GameObject("Canvas_FinPartida");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // ---- Fondo oscuro ----
        GameObject fondo = CrearRect("Fondo", canvasGo.transform);
        Image imgFondo = fondo.AddComponent<Image>();
        imgFondo.color = new Color(0.04f, 0.03f, 0.07f, 1f); // negro violáceo muy oscuro

        // ---- Título ----
        CrearTexto(canvasGo.transform, "TextoTitulo", "FIN DE LA PARTIDA",
            64, Color.white, new Vector2(0.5f, 0.85f), FontStyle.Bold);

        // ---- Texto tiempo (etiqueta + valor) ----
        CrearTexto(canvasGo.transform, "TextoTiempoLabel", "Tu tiempo:",
            32, new Color(0.8f, 0.75f, 0.65f), new Vector2(0.5f, 0.72f), FontStyle.Normal);

        Text valorTiempo = CrearTexto(canvasGo.transform, "TextoTiempoValor", "00:00.00",
            56, new Color(0.957f, 0.788f, 0.365f), new Vector2(0.5f, 0.65f), FontStyle.Bold);

        // ---- Campo nombre ----
        CrearTexto(canvasGo.transform, "TextoInputLabel", "Escribe tu nombre:",
            28, new Color(0.7f, 0.7f, 0.75f), new Vector2(0.5f, 0.55f), FontStyle.Normal);

        GameObject campoGO = CrearRect("CampoNombre", canvasGo.transform);
        RectTransform campoRect = campoGO.GetComponent<RectTransform>();
        campoRect.anchorMin = new Vector2(0.5f, 0.55f);
        campoRect.anchorMax = new Vector2(0.5f, 0.55f);
        campoRect.pivot = new Vector2(0.5f, 0.5f);
        campoRect.anchoredPosition = new Vector2(0, -35);
        campoRect.sizeDelta = new Vector2(400, 45);

        Image imgCampo = campoGO.AddComponent<Image>();
        imgCampo.color = new Color(0.12f, 0.10f, 0.18f, 0.9f);

        InputField input = campoGO.AddComponent<InputField>();
        input.textComponent = CrearTextoTMP(campoGO.transform, "Text", "", 24, Color.white);
        input.placeholder = CrearTextoTMP(campoGO.transform, "Placeholder", "Tu nombre aquí...", 24, new Color(0.5f, 0.5f, 0.55f));
        input.characterLimit = 20;

        // ---- Botón Guardar ----
        GameObject btnGuardar = CrearBoton(canvasGo.transform, "BotonGuardar",
            "GUARDAR", new Vector2(0.5f, 0.42f), new Color(0.2f, 0.17f, 0.30f));

        // ---- Texto estado ----
        CrearTexto(canvasGo.transform, "TextoEstado", "",
            24, new Color(0.6f, 0.9f, 0.6f), new Vector2(0.5f, 0.37f), FontStyle.Bold);

        // ---- Ranking ----
        GameObject panelRanking = CrearRect("PanelRanking", canvasGo.transform);
        RectTransform rankRect = panelRanking.GetComponent<RectTransform>();
        rankRect.anchorMin = new Vector2(0.3f, 0.05f);
        rankRect.anchorMax = new Vector2(0.7f, 0.33f);
        rankRect.offsetMin = Vector2.zero;
        rankRect.offsetMax = Vector2.zero;

        CrearTexto(panelRanking.transform, "TextoRankingHeader", "RANKING MEJORES TIEMPOS",
            28, new Color(0.8f, 0.7f, 0.5f), new Vector2(0.5f, 0.95f), FontStyle.Bold);

        CrearTexto(panelRanking.transform, "TextoRanking", "Aún no hay registros",
            24, Color.white, new Vector2(0.5f, 0.45f), FontStyle.Normal);

        // ---- Botón Volver ----
        CrearBoton(canvasGo.transform, "BotonVolver",
            "VOLVER AL MENÚ", new Vector2(0.5f, 0.03f), new Color(0.15f, 0.13f, 0.22f));

        // ---- Componente MostrarTiempo ----
        MostrarTiempo mt = canvasGo.AddComponent<MostrarTiempo>();

        // ---- Guardar escena ----
        if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        EditorSceneManager.SaveScene(escena, RutaEscena);
        Debug.Log("[FinPartida] Escena creada: " + RutaEscena);

        AgregarABuildSettings();
    }

    private static GameObject CrearRect(string nombre, Transform padre)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        return go;
    }

    private static Text CrearTexto(Transform padre, string nombre, string contenido,
        int fontSize, Color color, Vector2 anchor, FontStyle style)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = anchor;
        r.anchorMax = anchor;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(800, 80);

        Text t = go.AddComponent<Text>();
        t.text = contenido;
        t.font = FuentesJuego.Principal;
        t.fontSize = fontSize;
        t.fontStyle = style;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.raycastTarget = false;

        Outline contorno = go.AddComponent<Outline>();
        contorno.effectColor = new Color(0f, 0f, 0f, 0.8f);
        contorno.effectDistance = new Vector2(1.5f, -1.5f);

        return t;
    }

    private static Text CrearTextoTMP(Transform padre, string nombre, string contenido,
        int fontSize, Color color)
    {
        // Texto interno del InputField (no necesita Outline)
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = new Vector2(10, 0);
        r.offsetMax = new Vector2(-10, 0);

        Text t = go.AddComponent<Text>();
        t.text = contenido;
        t.font = FuentesJuego.Principal;
        t.fontSize = fontSize;
        t.color = color;
        t.supportRichText = false;
        t.alignment = TextAnchor.MiddleLeft;
        return t;
    }

    private static GameObject CrearBoton(Transform padre, string nombre, string texto,
        Vector2 anchor, Color colorFondo)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform r = go.GetComponent<RectTransform>();
        r.anchorMin = anchor;
        r.anchorMax = anchor;
        r.pivot = new Vector2(0.5f, 0.5f);
        r.anchoredPosition = Vector2.zero;
        r.sizeDelta = new Vector2(280, 50);

        Image img = go.AddComponent<Image>();
        img.color = colorFondo;

        Button btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        // Texto del botón
        GameObject txtGO = new GameObject("Label", typeof(RectTransform));
        txtGO.transform.SetParent(go.transform, false);
        RectTransform tr = txtGO.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero;
        tr.offsetMax = Vector2.zero;

        Text t = txtGO.AddComponent<Text>();
        t.text = texto;
        t.font = FuentesJuego.Principal;
        t.fontSize = 28;
        t.fontStyle = FontStyle.Bold;
        t.color = new Color(0.957f, 0.788f, 0.365f); // dorado
        t.alignment = TextAnchor.MiddleCenter;
        t.raycastTarget = false;

        return go;
    }

    private static void AgregarABuildSettings()
    {
        EditorBuildSettingsScene[] escenasActuales = EditorBuildSettings.scenes;
        string rutaAbsoluta = Path.GetFullPath(RutaEscena).Replace("\\", "/");

        // Verificar si ya está
        foreach (var s in escenasActuales)
        {
            if (s != null && s.path == RutaEscena) return;
        }

        // Agregar al final
        var lista = new System.Collections.Generic.List<EditorBuildSettingsScene>(escenasActuales);
        lista.Add(new EditorBuildSettingsScene(RutaEscena, true));
        EditorBuildSettings.scenes = lista.ToArray();

        Debug.Log("[FinPartida] Escena agregada a Build Settings.");
    }
}
