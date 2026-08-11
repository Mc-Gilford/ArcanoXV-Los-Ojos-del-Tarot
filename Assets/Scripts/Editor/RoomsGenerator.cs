using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Genera las 10 habitaciones reales del GDD a partir de RoomsLayouts.
/// Cada habitación es una escena en Assets/Scenes/Habitaciones/ con:
///   - Estructura (piso, paredes con vano de puerta, techo, luz tenue de terror).
///   - RoomTracker + una RoomTriggerZone con el nombre de la sala (sistema de sonido).
///   - El mobiliario con los MODELOS REALES de Assets/Models (o primitiva si el
///     modelo aún no importó: falta que Blender/glTFast estén listos y re-generar).
///   - Sonido por contacto (ObjectAmbience) + susto aleatorio (HauntedObject) en cada mueble.
///   - El jugador temporal (WASD + pasos de madera).
///
/// Uso: Tools > Arcano XV > Generar habitaciones  (también sale con "Generar TODO")
/// </summary>
public static class RoomsGenerator
{
    private const string RoomsFolder = "Assets/Scenes/Habitaciones";
    private const float DoorWidth = 2.2f;

    [MenuItem("Tools/Arcano XV/Generar habitaciones")]
    public static void GenerateAllRooms()
    {
        if (!Directory.Exists(RoomsFolder))
            Directory.CreateDirectory(RoomsFolder);

        RoomLayout[] rooms = RoomsLayouts.All();
        foreach (RoomLayout room in rooms)
            GenerateRoom(room);

        AssetDatabase.SaveAssets();
        Debug.Log($"[Arcano XV] {rooms.Length} habitaciones generadas en {RoomsFolder}." +
            "\nRevisa la Consola: si ves 'Modelo no importado aún', los modelos reales " +
            "se usarán al repetir el comando cuando Blender/glTFast estén listos.");
    }

    // ------------------------------------------------------------ estructura

    private static void GenerateRoom(RoomLayout room)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        float h = room.size.y;
        float hx = room.size.x;
        float hz = room.size.z;

        // Piso
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Piso";
        floor.transform.localScale = new Vector3(hx, 0.2f, hz);
        floor.transform.position = new Vector3(0f, -0.1f, 0f);
        floor.GetComponent<Renderer>().sharedMaterial = LitMaterial("Mat_Piso_" + room.folderName, room.floorColor);

        // Paredes + techo
        BuildWalls(room, h, hx, hz);

        // Luz: direccional tenue + punto central cálido (terror)
        GameObject lightGO = new GameObject("Luz");
        Light directional = lightGO.AddComponent<Light>();
        directional.type = LightType.Directional;
        directional.intensity = 0.35f;
        directional.color = new Color(1f, 0.9f, 0.8f);
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        GameObject pointGO = new GameObject("Luz_Central");
        Light lamp = pointGO.AddComponent<Light>();
        lamp.type = LightType.Point;
        lamp.range = 8f;
        lamp.intensity = 0.6f;
        lamp.color = new Color(1f, 0.75f, 0.5f);
        pointGO.transform.position = new Vector3(0f, h - 0.5f, 0f);

        // RoomTracker (uno por escena)
        GameObject trackerGO = new GameObject("RoomTracker");
        trackerGO.AddComponent<RoomTracker>();

        // Zona de sonido: volumen trigger que cubre toda la sala
        GameObject zoneGO = new GameObject("Zona_" + room.name);
        BoxCollider col = zoneGO.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(hx - 0.8f, h - 0.6f, hz - 0.8f);
        col.center = new Vector3(0f, h / 2f, 0f);
        RoomTriggerZone zone = zoneGO.AddComponent<RoomTriggerZone>();
        zone.roomName = room.name;

        // Mobiliario (componente padre para orden en la jerarquía)
        GameObject furnitureRoot = new GameObject("Mobiliario");
        foreach (FurnitureDef f in room.furniture)
            SpawnFurniture(f, furnitureRoot.transform);

        // Jugador temporal (WASD + pasos)
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Jugador";
        player.tag = "Player";
        player.transform.position = room.playerSpawn;
        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        player.AddComponent<DebugPlayerMover>();
        player.AddComponent<PlayerFootsteps>();

        // Asignar clips de contacto y de pasos automáticamente
        TrophyRoomGenerator.AssignObjectSoundsInScene();
        TrophyRoomGenerator.AssignFootstepsInScene();

        string path = Path.Combine(RoomsFolder, room.folderName + ".unity").Replace('\\', '/');
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        Debug.Log($"[Arcano XV] Habitación generada: {path}");
    }

    private static void BuildWalls(RoomLayout room, float h, float hx, float hz)
    {
        const float t = 0.3f; // grosor de pared
        Material wallMat = BuildWallMaterial(room);

        // Pared trasera (-Z), izquierda (-X), derecha (+X)
        CreateWall("Pared_Trasera", new Vector3(hx, h, t), new Vector3(0f, h / 2f, -hz / 2f), wallMat);
        CreateWall("Pared_Izquierda", new Vector3(t, h, hz), new Vector3(-hx / 2f, h / 2f, 0f), wallMat);
        CreateWall("Pared_Derecha", new Vector3(t, h, hz), new Vector3(hx / 2f, h / 2f, 0f), wallMat);

        // Pared frontal (+Z) con vano de puerta al centro
        float segW = (hx - DoorWidth) / 2f;
        CreateWall("Pared_Delantera_Izq", new Vector3(segW, h, t),
            new Vector3(-(hx / 2f - segW / 2f), h / 2f, hz / 2f), wallMat);
        CreateWall("Pared_Delantera_Der", new Vector3(segW, h, t),
            new Vector3(hx / 2f - segW / 2f, h / 2f, hz / 2f), wallMat);

        // Techo
        CreateWall("Techo", new Vector3(hx, t, hz), new Vector3(0f, h - t / 2f, 0f), wallMat);
    }

    private static void CreateWall(string name, Vector3 scale, Vector3 pos, Material mat)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.localScale = scale;
        wall.transform.position = pos;
        wall.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private static Material BuildWallMaterial(RoomLayout room)
    {
        if (!string.IsNullOrEmpty(room.wallTexturePath))
        {
            Texture2D tx = AssetDatabase.LoadAssetAtPath<Texture2D>(room.wallTexturePath);
            if (tx != null)
            {
                Material m = new Material(LitShader()) { name = "Mat_Pared_Textura" };
                m.SetTexture("_BaseMap", tx);
                return m;
            }
        }
        return LitMaterial("Mat_Pared_" + room.folderName, room.wallColor);
    }

    // ------------------------------------------------------------ mobiliario

    /// <summary>
    /// Instancia un mueble: usa el modelo real de Assets/Models si importó;
    /// si no, una primitiva (para que la sala igual funcione y se reemplace sola
    /// al volver a generar cuando los modelos estén importados).
    /// </summary>
    private static void SpawnFurniture(FurnitureDef f, Transform parent)
    {
        GameObject go = null;

        if (!string.IsNullOrEmpty(f.modelPath))
        {
            GameObject model = LoadModelAsset(f.modelPath);
            if (model != null)
            {
                try { go = (GameObject)PrefabUtility.InstantiatePrefab(model); }
                catch (System.Exception) { go = null; }
            }
        }

        if (go == null)
        {
            if (!string.IsNullOrEmpty(f.modelPath))
                Debug.LogWarning($"[Arcano XV] Modelo no importado aún: {f.modelPath} → se usa primitiva. " +
                    $"Cuando Blender/glTFast estén listos, vuelve a 'Generar habitaciones'.");
            go = GameObject.CreatePrimitive(f.fallbackShape);
            go.transform.localScale = f.fallbackScale;
            go.GetComponent<Renderer>().sharedMaterial = LitMaterial("Mat_" + f.id, f.fallbackColor);
        }

        go.name = f.id;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = f.pos;
        go.transform.localRotation = Quaternion.Euler(f.rot);
        go.transform.localScale = Vector3.Scale(go.transform.localScale, f.scl);

        // Collider aproximado si el modelo no trae
        if (go.GetComponent<Collider>() == null)
            go.AddComponent<BoxCollider>();

        // SONIDO POR CONTACTO (suena al tocarlo) — solo si tiene categoría de audio
        if (!string.IsNullOrEmpty(f.audioCategory))
        {
            ObjectAmbience amb = go.AddComponent<ObjectAmbience>();
            amb.audioCategory = f.audioCategory;
            amb.baseVolume = 0.9f;
            amb.playDuration = 3f;
            amb.triggerRadius = 1.5f;
        }

        // SUSTO ALEATORIO (nueva mecánica): suena solo sin importar la distancia.
        // Intervalo aleatorio por mueble para que no suenen todos sincronizados.
        HauntedObject haunted = go.AddComponent<HauntedObject>();
        haunted.nearSounds = SustosSounds.NearClips();
        haunted.farSounds = SustosSounds.FarClips();
        haunted.minIntervalScare = Random.Range(15f, 30f);
        haunted.maxIntervalScare = haunted.minIntervalScare + Random.Range(15f, 25f);
    }

    /// <summary>
    /// Carga el GameObject de un modelo. glTFast a veces genera el prefab como
    /// asset aparte (mismo nombre, extensión .prefab) junto al .glb/.gltf, así
    /// que si no está el asset directo, se prueba con el prefab generado.
    /// </summary>
    private static GameObject LoadModelAsset(string path)
    {
        GameObject m = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (m != null) return m;
        string prefabPath = path.Replace(".glb", ".prefab").Replace(".gltf", ".prefab");
        return AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
    }

    // ------------------------------------------------------------ util

    private static Shader LitShader()
    {
        Shader s = Shader.Find("Universal Render Pipeline/Lit");
        return s != null ? s : Shader.Find("Standard");
    }

    private static Material LitMaterial(string name, Color color)
    {
        return new Material(LitShader()) { name = name, color = color };
    }
}