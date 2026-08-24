using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Configura el carro raíz de la escena "Carro y salida":
/// - Rigidbody cinemático + PlayerInput + CarroSalidaController SOLO en el objeto raíz
/// - Limpia componentes mal colocados de corridas anteriores (ruedas, hijos)
/// - Cámara en PRIMERA PERSONA dentro del carro (asiento del conductor)
/// - Ambiente nocturno igual a PrincipalScene (skybox BOXOPHOBIC + luz de luna)
/// Menú: Tools > Arcano XV > Configurar Carro Salida (Primera Persona)
/// </summary>
public static class ConfigurarCarroSalidaTool
{
    // Ajustes de la cámara primera persona (espacio local del carro)
    // Ligeramente elevada y hacia el lado del conductor (encuadre cinematográfico)
    private static readonly Vector3 PosicionAsiento = new Vector3(-0.38f, 0.80f, 0.22f);
    private const float NearClipPrimeraPersona = 0.02f;
    private const float FOVCinematografico = 62f;

    [MenuItem("Tools/Arcano XV/Configurar Carro Salida (Primera Persona)")]
    public static void Configurar()
    {
        // 1. Encontrar el carro RAÍZ por nombre (nunca ruedas ni hijos)
        GameObject carro = BuscarCarro();
        if (carro == null)
        {
            Debug.LogError("[ConfigurarCarro] No se encontró un objeto RAÍZ llamado RMCar26*. Revisa que el carro esté suelto en la jerarquía (no dentro de otro objeto).");
            return;
        }
        Debug.Log("[ConfigurarCarro] Carro raíz encontrado: '" + carro.name + "'");

        Undo.RegisterFullObjectHierarchyUndo(carro, "Configurar Carro Salida");

        // 2. Limpiar restos de configuraciones anteriores (componentes en hijos, cámaras viejas)
        LimpiarRestos(carro);

        // 3. Rigidbody cinemático en la RAÍZ (el movimiento lo hace el controller vía transform)
        Rigidbody rb = carro.GetComponent<Rigidbody>();
        if (rb == null) rb = carro.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // 4. PlayerInput con el asset de acciones del proyecto
        PlayerInput pi = carro.GetComponent<PlayerInput>();
        if (pi == null) pi = carro.AddComponent<PlayerInput>();
        pi.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Scenes/PlayerActions.inputactions");
        pi.defaultActionMap = "Jugador";

        // 5. Controlador con puntos de inicio/fin coherentes
        CarroSalidaController ctrl = carro.GetComponent<CarroSalidaController>();
        if (ctrl == null) ctrl = carro.AddComponent<CarroSalidaController>();
        Vector3 pos = carro.transform.position;
        ctrl.puntoInicio = pos;
        // Recorrido MÁS LARGO: cruza el origen y sigue la misma línea hasta ~x=+36
        // (la carretera cubre x ∈ [-64.75, +40.75]). El tiempo no cambia, así que
        // velocidad = distancia / duracionTrayecto sube automáticamente.
        Vector3 dirBase = new Vector3(0f, pos.y, 0f) - pos;
        float largoNuevo = Mathf.Min(dirBase.magnitude * 1.55f, 100f);
        ctrl.puntoFin = pos + dirBase.normalized * largoNuevo;
        ctrl.velocidadMovimiento = 8f;
        ctrl.teclasRequeridas = 10;
        ctrl.nombreEscenaSiguiente = "PrincipalScene";
        ctrl.duracionFade = 1.5f;
        ctrl.colorFade = Color.black;

        // 6. Cámara en primera persona DENTRO del carro (hija de la RAÍZ)
        CrearCamaraPrimeraPersona(carro);

        // 7. HUD del contador (usa el existente o crea uno overlay)
        ctrl.textoContador = BuscarOCrearHUD();

        // 8. Ambiente nocturno como PrincipalScene
        ConfigurarAmbiente();

        MarkSceneDirty();
        Selection.activeGameObject = carro;
        Debug.Log("[ConfigurarCarro] Listo: '" + carro.name + "' configurado. Guarda la escena (Ctrl+S) y da Play.");
    }

    private static GameObject BuscarCarro()
    {
        // Solo objetos RAÍZ de la escena (t.root == t) para no agarrar ruedas/hijos
        Transform[] todos = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform t in todos)
        {
            if (t.root != t || t.gameObject.scene.isLoaded == false) continue;
            string n = t.name.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");
            if (n.StartsWith("rmcar26")) return t.gameObject;
        }
        return null;
    }

    private static void LimpiarRestos(GameObject carro)
    {
        int eliminados = 0;

        // Componentes añadidos por error a hijos (ruedas, puertas, etc.)
        foreach (Transform hijo in carro.GetComponentsInChildren<Transform>(true))
        {
            if (hijo == carro.transform) continue;

            foreach (var c in hijo.GetComponents<CarroSalidaController>()) { Undo.DestroyObjectImmediate(c); eliminados++; }
            foreach (var c in hijo.GetComponents<PlayerInput>()) { Undo.DestroyObjectImmediate(c); eliminados++; }
            foreach (var c in hijo.GetComponents<Rigidbody>()) { Undo.DestroyObjectImmediate(c); eliminados++; }
        }

        // Cámaras dentro del carro: se recrean desde cero bien parentadas a la raíz
        foreach (var cam in carro.GetComponentsInChildren<Camera>(true))
        {
            Undo.DestroyObjectImmediate(cam.gameObject);
            eliminados++;
        }

        // Cámaras sueltas fuera del carro (evita duplicados y AudioListener doble)
        Camera[] externas = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Camera c in externas)
        {
            if (!c.transform.IsChildOf(carro.transform))
            {
                Undo.DestroyObjectImmediate(c.gameObject);
                eliminados++;
            }
        }

        if (eliminados > 0)
            Debug.Log("[ConfigurarCarro] Limpieza: " + eliminados + " objetos/componentes mal colocados eliminados.");
    }

    private static void CrearCamaraPrimeraPersona(GameObject carro)
    {
        GameObject camObj = new GameObject("Main Camera");
        Undo.RegisterCreatedObjectUndo(camObj, "Crear Cámara Primera Persona");
        camObj.transform.SetParent(carro.transform, false);

        camObj.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();

        UniversalAdditionalCameraData urpData = camObj.AddComponent<UniversalAdditionalCameraData>();
        urpData.renderType = CameraRenderType.Base;
        urpData.renderShadows = true;

        // Vista del piloto: sentado en el asiento, mirando hacia el frente (+Z local del carro)
        camObj.transform.localPosition = PosicionAsiento;
        camObj.transform.localRotation = Quaternion.identity;

        Camera camComponente = camObj.GetComponent<Camera>();
        camComponente.nearClipPlane = NearClipPrimeraPersona;
        camComponente.farClipPlane = 1000f;
        camComponente.fieldOfView = FOVCinematografico;
        camComponente.clearFlags = CameraClearFlags.Skybox;
        camComponente.depth = 0;

        Debug.Log("[ConfigurarCarro] Cámara primera persona creada bajo la RAÍZ de '" + carro.name +
                  "'. Si la vista sale muy arriba/abajo ajusta Local Position; si mira al revés pon rotación Y=180.");
    }

    private static Text BuscarOCrearHUD()
    {
        GameObject existente = GameObject.Find("Texto_Contador");
        GameObject panelObj = null;
        Text texto = null;

        // Si ya existe el HUD, reutilizarlo (y reestilizarlo abajo)
        if (existente != null)
        {
            texto = existente.GetComponent<Text>();
            if (texto != null && existente.transform.parent != null)
                panelObj = existente.transform.parent.gameObject;
        }

        if (texto == null)
        {
            GameObject canvasObj = new GameObject("HUD_Canvas");
            Undo.RegisterCreatedObjectUndo(canvasObj, "Crear HUD Contador");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObj.AddComponent<GraphicRaycaster>();

            panelObj = new GameObject("Panel_Contador");
            panelObj.transform.SetParent(canvasObj.transform, false);
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 1f);
            panelRect.anchorMax = new Vector2(0.5f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.anchoredPosition = new Vector2(0f, -40f);
            panelRect.sizeDelta = new Vector2(430f, 74f);
            panelObj.AddComponent<Image>();

            GameObject textoObj = new GameObject("Texto_Contador");
            textoObj.transform.SetParent(panelObj.transform, false);
            texto = textoObj.AddComponent<Text>();

            RectTransform textoRect = textoObj.GetComponent<RectTransform>();
            textoRect.anchorMin = Vector2.zero;
            textoRect.anchorMax = Vector2.one;
            textoRect.offsetMin = Vector2.zero;
            textoRect.offsetMax = Vector2.zero;
        }

        AplicarEstiloMistico(panelObj, texto);

        // Barra segmentada de 10 pips debajo del contador (solo visual, usa el evento existente)
        if (panelObj != null && panelObj.GetComponent<HUDBarraSegmentada>() == null)
            Undo.AddComponent<HUDBarraSegmentada>(panelObj);

        return texto;
    }

    /// <summary>
    /// Estética arcana para el HUD: fondo oscuro redondeado semitransparente,
    /// texto dorado grande con contorno y sombra suave.
    /// </summary>
    private static void AplicarEstiloMistico(GameObject panelObj, Text texto)
    {
        // ---- Fondo del panel ----
        if (panelObj != null)
        {
            Image fondo = panelObj.GetComponent<Image>();
            if (fondo != null)
            {
                // Totalmente transparente: solo se ven las letras
                fondo.color = new Color(0.07f, 0.05f, 0.12f, 0f);
                fondo.raycastTarget = false;
            }
        }

        // ---- Texto ----
        texto.font = FuentesJuego.Principal;
        texto.fontSize = 42;
        texto.fontStyle = FontStyle.Bold;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = new Color(0.96f, 0.83f, 0.45f); // dorado tenue
        texto.horizontalOverflow = HorizontalWrapMode.Overflow;
        texto.verticalOverflow = VerticalWrapMode.Overflow;
        texto.raycastTarget = false;

        // Contorno oscuro para legibilidad
        Outline outline = texto.GetComponent<Outline>();
        if (outline == null) outline = texto.gameObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);

        // Sombra desplazada para profundidad
        Shadow sombra = texto.GetComponent<Shadow>();
        if (sombra == null) sombra = texto.gameObject.AddComponent<Shadow>();
        sombra.effectColor = new Color(0f, 0f, 0f, 0.6f);
        sombra.effectDistance = new Vector2(3f, -3f);
    }

    private static void ConfigurarAmbiente()
    {
        // Skybox nocturno usado por PrincipalScene
        Material skybox = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/External Assets/BOXOPHOBIC/Skybox Cubemap Extended/Demo/Materials/Skybox Cubemap Extended Blend.mat");
        if (skybox == null)
            skybox = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath("1cf6f4c6e6e0518419839d3f5448dd99"));

        RenderSettings.skybox = skybox;
        RenderSettings.fog = false;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
        RenderSettings.ambientIntensity = 0.95f;
        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
        RenderSettings.defaultReflectionResolution = 128;
        RenderSettings.reflectionIntensity = 1f;

        // Luz direccional tipo luna (como PrincipalScene)
        Light[] luces = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in luces)
        {
            if (l.type != LightType.Directional) continue;
            l.color = new Color(0.4f, 0.45f, 0.6f, 1f);
            l.intensity = 0.8f;
            l.shadows = LightShadows.Soft;
            l.bounceIntensity = 0.3f;
        }

        DynamicGI.UpdateEnvironment();
        Debug.Log("[ConfigurarCarro] Ambiente: skybox BOXOPHOBIC + luz de luna aplicados" + (skybox == null ? " (AVISO: no se encontró el material de skybox)" : ""));
    }

    private static void MarkSceneDirty()
    {
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
}
