using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Genera la "habitación trofeo": una sala donde está colocado UN trofeo de cada
/// objeto del catálogo (RoomObjectCatalog), de modo que puedas entrar y escuchar
/// todos los sonidos que tendrán las demás habitaciones del GDD.
///
/// También crea las carpetas de audio (Assets/Audio/Objetos/<Categoría> y
/// Assets/Audio/Pasos) y asignadores automáticos que llenan los ObjectAmbience
/// y el PlayerFootsteps con los clips que encuentre en esas carpetas.
///
/// Uso:  Tools > Arcano XV > Generar TODO (regenera todo de una vez)
/// </summary>
public static class TrophyRoomGenerator
{
    private const string ScenePath = "Assets/Scenes/HabitacionTrofeo.unity";
    private const string ObjectsPrefabsFolder = "Assets/Prefabs/Objetos";
    private const string PlayerPrefabsFolder = "Assets/Prefabs/Player";

    // ---------------------------------------------------------------- menús

    /// <summary>
    /// UN SOLO clic regenera TODO el sistema: prefabs de sustos, escena de prueba de
    /// sustos, carpetas de audio, prefabs de objetos y jugador, y la habitación trofeo
    /// (con sonidos y pasos ya asignados). Es el ÚNICO menú bajo Tools > Arcano XV.
    /// </summary>
    [MenuItem("Tools/Arcano XV/Generar TODO")]
    public static void GenerateAll()
    {
        AmbiencePrefabGenerator.GenerateAll(); // prefabs del sistema de sustos (HauntedObject)
        AmbienceTestSceneGenerator.Generate(); // escena de prueba de sustos
        CreateAudioFolders();                  // carpetas de audio por si faltan
        CreateObjectPrefabs();                 // prefabs de objetos para las habitaciones
        CreatePlayerPrefab();                  // jugador con pasos
        GenerateTrophyRoom();                  // escena de la habitación trofeo (asigna todo)
        RoomsGenerator.GenerateAllRooms();     // las 10 habitaciones reales amobladas
    }

    public static void GenerateTrophyRoom()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material floorMat = CreateLitMaterial("MatPiso", new Color(0.24f, 0.19f, 0.16f));
        Material pedestalMat = CreateLitMaterial("MatPedestal", new Color(0.42f, 0.38f, 0.34f));

        // Sala de exhibición CIRCULAR: piso en disco (radio 21 m) + luz
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        floor.name = "Piso";
        floor.transform.localScale = new Vector3(116f, 0.1f, 116f); // radio 58 m
        floor.transform.position = new Vector3(0f, -0.1f, 0f);
        floor.GetComponent<Renderer>().sharedMaterial = floorMat;

        GameObject lightGO = new GameObject("Luz");
        Light l = lightGO.AddComponent<Light>();
        l.type = LightType.Directional;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // RoomTracker + zona que cubre toda la sala
        GameObject trackerGO = new GameObject("RoomTracker");
        trackerGO.AddComponent<RoomTracker>();
        GameObject zoneGO = new GameObject("ZonaTrofeos");
        SphereCollider col = zoneGO.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0f, 2f, 0f);
        col.radius = 54f; // sigue el contorno de la sala circular
        RoomTriggerZone zone = zoneGO.AddComponent<RoomTriggerZone>();
        zone.roomName = "Habitación trofeo";

        // Un trofeo por cada objeto del catálogo. Se parentan bajo la zona solo por
        // orden en la jerarquía; el sonido ahora se dispara por contacto, sin depender
        // de la zona.
        List<ObjectDefHolder> trophies = new List<ObjectDefHolder>();
        List<RoomObjectCatalog.ObjectDef> allObjects = new List<RoomObjectCatalog.ObjectDef>(RoomObjectCatalog.AllObjects());
        for (int index = 0; index < allObjects.Count; index++)
        {
            RoomObjectCatalog.ObjectDef def = allObjects[index];
            GameObject trophy = CreateTrophy(def, index, allObjects.Count, pedestalMat, zoneGO.transform);
            trophies.Add(new ObjectDefHolder { def = def, go = trophy });
        }

        CreateTrophyLabels(trophies);

        // Jugador con pasos de madera
        GameObject player = CreatePlayer();

        // Cartas recogibles + puerta del jefe final (coleccionables)
        SpawnCartasRecogibles();
        SpawnPuertaJefe();

        // Los clips ya viven en Assets/Audio/Objetos/<Categoría> y Assets/Audio/Pasos:
        // asígnalos a los trofeos y a los pasos en el mismo paso (un solo clic).
        AssignObjectSoundsInScene();
        AssignFootstepsInScene();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        AssetDatabase.SaveAssets();

        int conSonido = 0, sinSonido = 0;
        foreach (ObjectDefHolder t in trophies)
        {
            if (string.IsNullOrEmpty(t.def.audioCategory)) sinSonido++;
            else conSonido++;
        }
        Debug.Log($"[Arcano XV] Habitación trofeo CIRCULAR creada: {trophies.Count} trofeos " +
            $"en un anillo de radio 50 m ({conSonido} con sonido, {sinSonido} silenciosos). " +
            $"Guardada en {ScenePath}\n" +
            "Los clips se asignaron solos desde Assets/Audio/Objetos. Cada trofeo suena " +
            "3 s SOLO al tocarlo y luego se apaga: susto por contacto.");
    }

    // ------------------------------------------------------------ trofeos

    private sealed class ObjectDefHolder
    {
        public RoomObjectCatalog.ObjectDef def;
        public GameObject go;
    }

    private static GameObject CreateTrophy(RoomObjectCatalog.ObjectDef def, int index, int totalCount, Material pedestalMat, Transform zoneRoot)
    {
        Vector3 pos = TrophyPosition(index, totalCount);

        // Pedestal
        GameObject pedestal = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pedestal.name = "Pedestal_" + def.name;
        pedestal.transform.SetParent(zoneRoot, false);
        pedestal.transform.localPosition = new Vector3(pos.x, 0.45f, pos.z);
        pedestal.transform.localScale = new Vector3(1f, 0.5f, 1f);
        pedestal.GetComponent<Renderer>().sharedMaterial = pedestalMat;

        // Trofeo (el objeto en sí), parentado bajo la zona para mantener orden.
        GameObject obj = CreateObjectVisual(def);
        obj.transform.SetParent(zoneRoot, false);
        obj.transform.localPosition = new Vector3(pos.x, 1.4f, pos.z);
        return obj;
    }

    /// <summary>
    /// Arma el GameObject de un objeto del catálogo (primitiva + material +
    /// ObjectAmbience de contacto + HauntedObject de susto aleatorio). Se usa tanto
    /// para los trofeos de la sala como para los prefabs que se llevan a las
    /// habitaciones reales: así TODOS los elementos suenan a intervalos aleatorios
    /// sin importar si el jugador está cerca o lejos.
    /// </summary>
    private static GameObject CreateObjectVisual(RoomObjectCatalog.ObjectDef def)
    {
        GameObject obj = GameObject.CreatePrimitive(def.shape);
        obj.name = def.name;
        obj.transform.localScale = def.scale;
        obj.GetComponent<Renderer>().sharedMaterial = CreateLitMaterial("Mat_" + def.name, def.color);

        if (!string.IsNullOrEmpty(def.audioCategory))
        {
            ObjectAmbience amb = obj.AddComponent<ObjectAmbience>();
            amb.audioCategory = def.audioCategory;
            amb.baseVolume = 0.9f;
            amb.playDuration = 3f;     // suena 3 s al tocarlo y se apaga
            amb.triggerRadius = 1.5f;  // área de contacto casi al tocar
        }

        // Susto aleatorio en CADA elemento (nueva mecánica): suena solo a
        // intervalos aleatorios, no importa la distancia. El primer intervalo es
        // aleatorio por objeto, así nunca arrancan todos sincronizados.
        HauntedObject haunted = obj.AddComponent<HauntedObject>();
        haunted.nearSounds = SustosSounds.NearClips();
        haunted.farSounds = SustosSounds.FarClips();
        haunted.minIntervalScare = Random.Range(15f, 30f);
        haunted.maxIntervalScare = haunted.minIntervalScare + Random.Range(15f, 30f);

        return obj;
    }

    private static Vector3 TrophyPosition(int index, int totalCount)
    {
        // Exhibición circular: los trofeos rodean el centro a radio 50 m
        // (separación entre vecinos ~7.5 m). El sonido sale SOLO al tocar el objeto
        // (área de contacto de 1.5 m), así que los vecinos nunca se mezclan.
        float radius = 50f;
        float angle = (index / (float)totalCount) * Mathf.PI * 2f;
        return new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
    }

    private static void CreateTrophyLabels(List<ObjectDefHolder> trophies)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        foreach (ObjectDefHolder t in trophies)
        {
            GameObject labelGO = new GameObject("Label_" + t.def.name);
            labelGO.transform.SetParent(t.go.transform, false);
            labelGO.transform.localPosition = Vector3.up * 0f + Vector3.down * 0.9f;
            labelGO.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            TextMesh tm = labelGO.AddComponent<TextMesh>();
            tm.text = t.def.name;
            tm.font = font;
            tm.fontSize = 36;
            tm.characterSize = 0.06f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = t.def.color;
        }
    }

    // ------------------------------------------------------------ jugador

    private static GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "JugadorPrueba";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, 0f);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        player.AddComponent<DebugPlayerMover>();
        player.AddComponent<PlayerFootsteps>();
        player.AddComponent<CardSelectionSystem>();
        player.AddComponent<CardCollector>();

        // Asigna los crujidos de madera si ya existen clips en Assets/Audio/Pasos
        PlayerFootsteps steps = player.GetComponent<PlayerFootsteps>();
        steps.stepClips = LoadAllClips(RoomObjectCatalog.PasosFolder);
        if (steps.stepClips.Length == 0)
            Debug.Log("[Arcano XV] PlayerFootsteps sin clips. Pon mp3 en Assets/Audio/Pasos/ y usa 'Habitación trofeo: Asignar pasos'.");
        else
            Debug.Log($"[Arcano XV] Pasos asignados: {steps.stepClips.Length} crujidos de madera.");

        return player;
    }

    // ------------------------------------------------------------ cartas y puerta del jefe

    /// <summary>
    /// Coloca las 5 cartas recogibles (modelo Assets/Models/Cartas) en un anillo
    /// alrededor del centro. Se recogen con E y alimentan a CardCollector.
    /// </summary>
    private static void SpawnCartasRecogibles()
    {
        const string modelPath = "Assets/Models/Cartas/scene.gltf";
        GameObject model = LoadModelAsset(modelPath);
        if (model == null)
        {
            Debug.LogWarning("[Arcano XV] Modelo de carta no importado aún: " + modelPath +
                " → las cartas recogibles no se colocaron. Abre el proyecto para que glTFast lo importe.");
            return;
        }

        const int total = 5;
        const float radius = 25f;
        for (int i = 0; i < total; i++)
        {
            float angle = (i / (float)total) * Mathf.PI * 2f;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, 1.2f, Mathf.Sin(angle) * radius);

            GameObject carta;
            try { carta = (GameObject)PrefabUtility.InstantiatePrefab(model); }
            catch (System.Exception) { carta = null; }
            if (carta == null) continue;

            carta.name = "CartaRecogible_" + (i + 1);
            carta.transform.position = pos;
            carta.transform.localScale = Vector3.one * 1.6f;

            BoxCollider bc = carta.AddComponent<BoxCollider>();
            bc.size = new Vector3(0.7f, 0.9f, 0.2f);
            bc.center = new Vector3(0f, 0.45f, 0f);

            carta.AddComponent<CardPickup>();
        }
        Debug.Log($"[Arcano XV] {total} cartas recogibles colocadas en la sala (pulsa E para recogerlas).");
    }

    /// <summary>
    /// Puerta del jefe final: nace bloqueada y se abre (BossDoor) cuando se reúnen
    /// todas las cartas. Es la evolución de la "llave" que abría el último cuarto.
    /// </summary>
    private static void SpawnPuertaJefe()
    {
        GameObject puerta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        puerta.name = "PuertaJefeFinal";
        puerta.transform.position = new Vector3(0f, 2.5f, 46f);
        puerta.transform.localScale = new Vector3(8f, 5f, 0.5f);
        puerta.GetComponent<Renderer>().sharedMaterial =
            CreateLitMaterial("MatPuertaJefe", new Color(0.6f, 0.1f, 0.1f));
        puerta.AddComponent<BossDoor>();
        Debug.Log("[Arcano XV] Puerta del jefe final colocada: se abre al reunir las 5 cartas.");
    }

    private static GameObject LoadModelAsset(string path)
    {
        GameObject m = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (m != null) return m;
        string prefabPath = path.Replace(".glb", ".prefab").Replace(".gltf", ".prefab");
        return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    // ------------------------------------------------------------ audio

    public static void CreateAudioFolders()
    {
        HashSet<string> categorias = new HashSet<string>();
        foreach (RoomObjectCatalog.ObjectDef def in RoomObjectCatalog.AllObjects())
            if (!string.IsNullOrEmpty(def.audioCategory))
                categorias.Add(def.audioCategory);

        Directory.CreateDirectory(RoomObjectCatalog.AudioRoot);
        Directory.CreateDirectory(RoomObjectCatalog.PasosFolder);
        foreach (string cat in categorias)
            Directory.CreateDirectory(Path.Combine(RoomObjectCatalog.AudioRoot, cat));

        AssetDatabase.Refresh();
        Debug.Log("[Arcano XV] Carpetas de audio listas en Assets/Audio/Objetos/<Categoría> y Assets/Audio/Pasos. " +
            "Pega ahí los clips que consigas, en .wav o .ogg preferiblemente.");
    }

    public static void AssignObjectSoundsInScene()
    {
        ObjectAmbience[] objects = Object.FindObjectsByType<ObjectAmbience>(FindObjectsSortMode.None);
        int asignados = 0, faltan = 0;
        foreach (ObjectAmbience amb in objects)
        {
            if (string.IsNullOrEmpty(amb.audioCategory))
            {
                amb.clip = null;
                continue;
            }
            AudioClip clip = LoadFirstClip(Path.Combine(RoomObjectCatalog.AudioRoot, amb.audioCategory));
            amb.clip = clip;
            EditorUtility.SetDirty(amb);
            if (clip != null) asignados++;
            else { faltan++; Debug.LogWarning($"[Arcano XV] Falta clip para '{amb.gameObject.name}' en {RoomObjectCatalog.AudioRoot}/{amb.audioCategory}/"); }
        }
        Debug.Log($"[Arcano XV] Objetos con sonido: {asignados} asignados, {faltan} sin clip aún.");
    }

    public static void AssignFootstepsInScene()
    {
        PlayerFootsteps steps = Object.FindFirstObjectByType<PlayerFootsteps>();
        if (steps == null) { Debug.LogWarning("[Arcano XV] No hay PlayerFootsteps en la escena."); return; }
        AudioClip[] clips = LoadAllClips(RoomObjectCatalog.PasosFolder);
        steps.stepClips = clips;
        EditorUtility.SetDirty(steps);
        Debug.Log(clips.Length == 0
            ? "[Arcano XV] No hay clips en Assets/Audio/Pasos/. Consigue crujidos de madera y vuelve a asignar."
            : $"[Arcano XV] Pasos asignados: {clips.Length} crujidos.");
    }

    // ------------------------------------------------------------ prefabs

    public static void CreateObjectPrefabs()
    {
        if (!Directory.Exists(ObjectsPrefabsFolder))
            Directory.CreateDirectory(ObjectsPrefabsFolder);

        int creados = 0, faltan = 0, eliminados = 0;
        foreach (RoomObjectCatalog.ObjectDef def in RoomObjectCatalog.AllObjects())
        {
            string nombre = def.name.Replace(" ", "_");
            string path = Path.Combine(ObjectsPrefabsFolder, nombre + ".prefab");

            if (string.IsNullOrEmpty(def.audioCategory))
            {
                // Sin sonido: NO se exporta prefab (solo van los que tienen sonido).
                // Si existía uno de antes, se elimina.
                if (File.Exists(path))
                {
                    AssetDatabase.DeleteAsset(path.Replace('\\', '/'));
                    eliminados++;
                }
                continue;
            }

            GameObject go = CreateObjectVisual(def);
            ObjectAmbience amb = go.GetComponent<ObjectAmbience>();
            amb.clip = LoadFirstClip(Path.Combine(RoomObjectCatalog.AudioRoot, def.audioCategory));
            if (amb.clip == null)
            {
                faltan++;
                Debug.LogWarning($"[Arcano XV] Prefab '{def.name}' sin clip: falta en {RoomObjectCatalog.AudioRoot}/{def.audioCategory}/");
            }

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            creados++;
        }

        AssetDatabase.Refresh();
        Debug.Log($"[Arcano XV] Prefabs de objetos CON SONIDO en {ObjectsPrefabsFolder}: {creados} creados, " +
            $"{eliminados} silenciosos quitados, {faltan} sin clip. Arrastra cada prefab a la habitación de tu escena.");
    }

    public static void CreatePlayerPrefab()
    {
        if (!Directory.Exists(PlayerPrefabsFolder))
            Directory.CreateDirectory(PlayerPrefabsFolder);

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Jugador_Pasos";
        player.tag = "Player";

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        player.AddComponent<DebugPlayerMover>();
        player.AddComponent<CardSelectionSystem>();
        player.AddComponent<CardCollector>();
        PlayerFootsteps steps = player.AddComponent<PlayerFootsteps>();
        steps.stepClips = LoadAllClips(RoomObjectCatalog.PasosFolder);

        string path = Path.Combine(PlayerPrefabsFolder, "Jugador_Pasos.prefab");
        PrefabUtility.SaveAsPrefabAsset(player, path);
        Object.DestroyImmediate(player);

        Debug.Log($"[Arcano XV] Prefab de jugador con pasos creado: {path}. " +
            "Puedes reemplazar DebugPlayerMover por tu controlador; lo importante es PlayerFootsteps.");
    }

    // ------------------------------------------------------------ util

    private static AudioClip LoadFirstClip(string folder)
    {
        AudioClip[] all = LoadAllClips(folder);
        return all.Length > 0 ? all[0] : null;
    }

    private static AudioClip[] LoadAllClips(string folder)
    {
        if (!Directory.Exists(folder)) return new AudioClip[0];
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folder });
        var clips = new List<AudioClip>();
        foreach (string guid in guids)
        {
            AudioClip c = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
            if (c != null) clips.Add(c);
        }
        return clips.ToArray();
    }

    private static Material CreateLitMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        return new Material(shader) { name = name, color = color };
    }
}