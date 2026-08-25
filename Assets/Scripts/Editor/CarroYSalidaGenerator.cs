using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/// <summary>
/// Generador de la escena "Carro y salida" para la rama develop.
/// El carro se mueve automáticamente desde (-50,0.5,0) al origen (0,0.5,0).
/// Cámara en primera persona desde el asiento del conductor (vista piloto).
/// El jugador presiona X 10 veces durante el trayecto para cargar PrincipalScene con fade.
/// Usa los mismos assets/ambiente de PrincipalScene (terreno, nubes, skybox, iluminación, lobby).
/// </summary>
public class CarroYSalidaGenerator : EditorWindow
{
    public static void GenerarEscenaCarroYSalida()
    {
        // Crear carpeta de escenas si no existe
        string sceneFolder = "Assets/Scenes";
        if (!AssetDatabase.IsValidFolder(sceneFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }

        string scenePath = $"{sceneFolder}/Carro y salida.unity";

        // Crear nueva escena SIN DefaultGameObjects (para no crear cámara por defecto)
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        scene.name = "Carro y salida";

        // Configurar render settings para que coincida con PrincipalScene (ambiente)
        ConfigurarRenderSettings();

        // Configurar iluminación para que coincida con PrincipalScene
        ConfigurarIluminacion();

        // Instanciar ambiente de PrincipalScene (terreno, nubes, lobby)
        InstanciarAmbientePrincipalScene();

        // Crear el carro (usar el prefab existente RMCar26_D, escalado 3x)
        GameObject carro = CrearCarro();

        // Crear el script de control del carro (usa CarroSalidaController.cs existente)
        CarroSalidaController controller = carro.AddComponent<CarroSalidaController>();

        // Configurar puntos de inicio y fin
        controller.puntoInicio = new Vector3(-50f, 0.5f, 0f);  // Parte del tablero (lejos)
        controller.puntoFin = new Vector3(0f, 0.5f, 0f);       // Posición final
        controller.velocidadMovimiento = 5f;                    // Velocidad del carro
        controller.teclasRequeridas = 10;                       // 10 veces X
        controller.nombreEscenaSiguiente = "PrincipalScene";    // Escena a cargar

        // Crear cámara en primera persona DENTRO del carro (vista de piloto por parabrisas)
        GameObject camObj = CrearCamaraEnCarro(carro);

        // Crear UI HUD overlay (Screen Space Overlay) para mostrar contador
        CrearUIHUD(controller);

        // Guardar escena
        EditorSceneManager.SaveScene(scene, scenePath);

        Debug.Log("========================================");
        Debug.Log($"✓ Escena creada: {scenePath}");
        Debug.Log("✓ Carro RMCar26_D (scale 3x) en (-50,0.5,0) -> (0,0.5,0)");
        Debug.Log("✓ Cámara primera persona: asiento conductor, mira hacia adelante");
        Debug.Log("✓ Contador X (10 requeridas) - HUD Screen Space Overlay");
        Debug.Log("✓ X contable DURANTE el trayecto, no solo al llegar");
        Debug.Log("✓ Fade in/out + LoadSceneAsync allowSceneActivation");
        Debug.Log("✓ Ambiente: Skybox BOXOPHOBIC, Terrain_A, Nubes, Lobby, Luz direccional");
        Debug.Log("✓ UniversalAdditionalCameraData en cámara (fix Game view freeze)");
        Debug.Log("========================================");
    }

    private static void ConfigurarRenderSettings()
    {
        // Skybox de PrincipalScene (GUID: 1cf6f4c6e6e0518419839d3f5448dd99)
        // Path real: Assets/External Assets/BOXOPHOBIC/Skybox Cubemap Extended/Demo/Materials/Skybox Cubemap Extended Blend.mat
        var skyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/External Assets/BOXOPHOBIC/Skybox Cubemap Extended/Demo/Materials/Skybox Cubemap Extended Blend.mat");

        if (skyboxMaterial == null)
        {
            // Fallback: buscar por GUID
            string guid = "1cf6f4c6e6e0518419839d3f5448dd99";
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
            {
                skyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
            }
        }

        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
            Debug.Log($"[CarroYSalida] Skybox asignado: {AssetDatabase.GetAssetPath(skyboxMaterial)}");
        }
        else
        {
            Debug.LogWarning("[CarroYSalida] No se encontró Skybox material de PrincipalScene");
        }

        // Configuración de Fog (igual que PrincipalScene - desactivado)
        RenderSettings.fog = false;
        RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.01f;
        RenderSettings.fogStartDistance = 0f;
        RenderSettings.fogEndDistance = 300f;

        // Configuración de Ambient Lighting (igual que PrincipalScene)
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientSkyColor = new Color(0.212f, 0.227f, 0.259f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.114f, 0.125f, 0.133f, 1f);
        RenderSettings.ambientGroundColor = new Color(0.047f, 0.043f, 0.035f, 1f);
        RenderSettings.ambientIntensity = 0.95f;

        // Configuración de Reflection
        RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
        RenderSettings.defaultReflectionResolution = 128;
        RenderSettings.reflectionBounces = 1;
        RenderSettings.reflectionIntensity = 1f;
    }

    private static void ConfigurarIluminacion()
    {
        // Luz direccional tipo luz de luna para ambiente nocturno visible
        GameObject lightObj = new GameObject("Directional Light");
        Light dirLight = lightObj.AddComponent<Light>();
        dirLight.type = LightType.Directional;

        // Rotación similar a PrincipalScene
        lightObj.transform.position = new Vector3(43.6f, 107f, -75.6f);
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Luz de luna tenue azulada para que se vea la escena nocturna
        dirLight.color = new Color(0.4f, 0.45f, 0.6f, 1f);
        dirLight.intensity = 0.8f;
        dirLight.shadows = LightShadows.Soft;
        dirLight.shadowBias = 0.05f;
        dirLight.shadowNormalBias = 0.4f;
        dirLight.bounceIntensity = 0.3f;

        // URP data
        var universalLightData = lightObj.AddComponent<UniversalAdditionalLightData>();

        Debug.Log("[CarroYSalida] Luz direccional configurada (luz de luna nocturna)");
    }

    private static void InstanciarAmbientePrincipalScene()
    {
        // 1. TERRAIN_A - PrincipalScene: pos (-513.5, -45.6, -107.1), scale 10, tag Ground
            // Terrain_A – place at origin for visibility
            GameObject terrainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/External Assets/_Bad_Raccoon/_3D Realistic Terrain Free/Terrains/Terrain_A.prefab");
            if (terrainPrefab != null)
            {
                GameObject terrain = PrefabUtility.InstantiatePrefab(terrainPrefab) as GameObject;
                terrain.name = "Terrain_A";
                terrain.transform.position = Vector3.zero;
                terrain.transform.localScale = Vector3.one * 10f;
                terrain.tag = "Ground";
                Debug.Log("[CarroYSalida] Terrain_A instanciado at origin");
            }
            else
            {
                Debug.LogWarning("[CarroYSalida] No se encontró Terrain_A.prefab");
            }

        // 2. PARTICLE_CLOUD_ATMOS_B - PrincipalScene: pos (-150.93971, -5.5305185, -32.656208)
        GameObject cloudsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/External Assets/_Bad_Raccoon/_3D Realistic Terrain Free/Prefabs/Particle_Cloud_Atmos_B.prefab");
        if (cloudsPrefab != null)
        {
            GameObject clouds = PrefabUtility.InstantiatePrefab(cloudsPrefab) as GameObject;
            clouds.name = "Particle_Cloud_Atmos_B";
            clouds.transform.position = new Vector3(-150.93971f, -5.5305185f, -32.656208f);
            Debug.Log("[CarroYSalida] Particle_Cloud_Atmos_B instanciado");
        }
        else
        {
            Debug.LogWarning("[CarroYSalida] No se encontró Particle_Cloud_Atmos_B.prefab");
        }

        // 3. LOBBY PREFAB - PrincipalScene: bajo Home, local (-23.366152, -24.883682, 58.14746), rot Y180
        // Home está en (-3.2550735, 14.4, 274.55255)
        GameObject lobbyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Models/Habitaciones/Lobby/Lobby Prefab.prefab");
        if (lobbyPrefab != null)
        {
            // Crear padre "Home" como en PrincipalScene
            GameObject homeObj = new GameObject("Home");
            homeObj.transform.position = new Vector3(-3.2550735f, 14.4f, 274.55255f);
            homeObj.transform.rotation = Quaternion.identity;
            homeObj.tag = "Home";

            GameObject lobby = PrefabUtility.InstantiatePrefab(lobbyPrefab) as GameObject;
            lobby.name = "Lobby";
            lobby.transform.SetParent(homeObj.transform);
            lobby.transform.localPosition = new Vector3(-23.366152f, -24.883682f, 58.14746f);
            lobby.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            Debug.Log("[CarroYSalida] Lobby instanciado bajo Home");
        }
        else
        {
            Debug.LogWarning("[CarroYSalida] No se encontró Lobby Prefab.prefab");
        }

        // 4. PASILLOS (opcional, para más ambiente)
        GameObject pasillosPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Models/Habitaciones/Pasillo/Pasillos.prefab");
        if (pasillosPrefab != null)
        {
            GameObject pasillos = PrefabUtility.InstantiatePrefab(pasillosPrefab) as GameObject;
            pasillos.name = "Pasillos";
            // Posición similar a PrincipalScene (cerca del Home/Lobby)
            pasillos.transform.position = new Vector3(-3.255f, 14.4f, 274.55f);
            Debug.Log("[CarroYSalida] Pasillos instanciado");
        }
        else
        {
            Debug.LogWarning("[CarroYSalida] No se encontró Pasillos.prefab");
        }

        // 5. RAIN PREFAB - Lluvia ambiental (RainMaker)
        GameObject rainPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/External Assets/RainMaker/Prefab/RainPrefab.prefab");
        if (rainPrefab != null)
        {
            GameObject rain = PrefabUtility.InstantiatePrefab(rainPrefab) as GameObject;
            rain.name = "RainPrefab";
            rain.transform.position = new Vector3(-25f, 20f, 0f);

            // Asignar la cámara principal al RainMaker para evitar NullReferenceException
            var rainScript = rain.GetComponent<DigitalRuby.RainMaker.BaseRainScript>();
            if (rainScript == null)
                rainScript = rain.GetComponentInChildren<DigitalRuby.RainMaker.BaseRainScript>();
            if (rainScript != null)
            {
                Camera mainCam = Camera.main;
                if (mainCam == null)
                    mainCam = Object.FindFirstObjectByType<Camera>();
                if (mainCam != null)
                    rainScript.Camera = mainCam;
            }

            Debug.Log("[CarroYSalida] RainPrefab instanciado con cámara asignada");
        }
        else
        {
            Debug.LogWarning("[CarroYSalida] No se encontró RainPrefab.prefab");
        }
    }

    private static GameObject CrearCarro()
    {
        // Cargar el prefab del carro existente (el mismo que PrincipalScene usa)
        GameObject carroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/External Assets/RealisticMobileCars - Pro3DModels/RMCar26/Prefabs/RMCar26_D.prefab");

        if (carroPrefab == null)
        {
            // Fallback: buscar cualquier variante RMCar26
            string[] guids = AssetDatabase.FindAssets("RMCar26 t:Prefab");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                carroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }

        GameObject carro;
        if (carroPrefab != null)
        {
            carro = PrefabUtility.InstantiatePrefab(carroPrefab) as GameObject;
            carro.name = "CarroSalida";
            // Escalar x3 como en PrincipalScene
            carro.transform.localScale = Vector3.one * 3f;
            Debug.Log($"[CarroYSalida] Carro instanciado: {AssetDatabase.GetAssetPath(carroPrefab)} scale 3x");
        }
        else
        {
            // Fallback: crear un cubo representando el carro
            carro = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carro.name = "CarroSalida";
            carro.transform.localScale = new Vector3(4f, 1.5f, 8f);
            Debug.LogWarning("[CarroYSalida] No se encontró prefab RMCar26, usando cubo placeholder");
        }

        // Posición inicial (fuera de vista, en la "parte del tablero")
        carro.transform.position = new Vector3(-50f, 0.5f, 0f);
        // Rotación Y=180 para mirar hacia el origen (0,0,0) - forward del carro apunta al origen
        carro.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Añadir Rigidbody kinematic para movimiento suave
        if (carro.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = carro.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        return carro;
    }

    private static GameObject CrearCamaraEnCarro(GameObject carro)
    {
        // Crear cámara como objeto INDEPENDIENTE (NO hija del carro)
        // para que en Game view se vea el carro moverse
        GameObject camObj = new GameObject("Main Camera");

        // Tag EXACTO que Unity usa para Camera.main
        camObj.tag = "MainCamera";

        // Posicionar detrás y arriba del carro (tercera persona)
        Vector3 carroPos = carro.transform.position;
        Vector3 offset = new Vector3(0f, 5f, 12f); // +Z = detrás (carro mira -Z con rot Y=180)
        camObj.transform.position = carroPos + offset;
        camObj.transform.LookAt(carroPos + Vector3.up * 1.5f);

        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 1000f;
        cam.fieldOfView = 60f;
        cam.depth = 0; // Asegurar que sea la cámara principal (depth 0)

        // AudioListener en la cámara
        camObj.AddComponent<AudioListener>();

        // CRÍTICO: Añadir UniversalAdditionalCameraData para URP
        var universalCamData = camObj.AddComponent<UniversalAdditionalCameraData>();
        universalCamData.renderType = CameraRenderType.Base;
        universalCamData.renderShadows = true;

        // Seguimiento de cámara en tercera persona
        CarCameraFollow follow = camObj.AddComponent<CarCameraFollow>();
        follow.SetTarget(carro.transform);

        // Añadir PlayerInput AL CARRO para detectar la tecla X
        PlayerInput input = carro.AddComponent<PlayerInput>();
        input.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Scenes/PlayerActions.inputactions");
        input.defaultActionMap = "Jugador";

        Debug.Log($"[CarroYSalida] Cámara creada en {camObj.transform.position}, tag={camObj.tag}");
        return camObj;
    }

    private static void CrearUIHUD(CarroSalidaController controller)
    {
        // HUD en Screen Space Overlay (no World Space) - siempre visible en Game view
        GameObject canvasObj = new GameObject("HUD_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100; // Sobre el juego, bajo el fade (1000)
        canvasObj.AddComponent<GraphicRaycaster>();

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // Panel de fondo semi-transparente para el contador
        GameObject panelObj = new GameObject("Panel_Contador");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0f, -30f);
        panelRect.sizeDelta = new Vector2(300f, 60f);

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.6f); // Negro semi-transparente
        panelImage.raycastTarget = false;

        // Texto del contador
        GameObject textoObj = new GameObject("Texto_Contador");
        textoObj.transform.SetParent(panelObj.transform, false);

        Text texto = textoObj.AddComponent<Text>();
          texto.font = FuentesJuego.Principal;
        texto.fontSize = 36;
        texto.fontStyle = FontStyle.Bold;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.color = Color.white;
        texto.text = "Presiona X: 0/10";

        RectTransform textoRect = textoObj.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;

        // Outline para legibilidad
        Outline outline = textoObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 1f);
        outline.effectDistance = new Vector2(2f, -2f);

        // Asignar referencia al controller
        controller.textoContador = texto;
    }

    // Helper method to save a GameObject as prefab
    private static void SaveAsPrefab(GameObject obj, string prefabPath)
    {
        // Ensure folder exists
        string folder = System.IO.Path.GetDirectoryName(prefabPath);
        if (!AssetDatabase.IsValidFolder(folder))
        {
            string[] parts = folder.Split('/');
            string current = "Assets";
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
        // Create or replace prefab
        PrefabUtility.SaveAsPrefabAssetAndConnect(obj, prefabPath, InteractionMode.UserAction);
    }
}