using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Genera una escena de prueba lista para presionar Play y verificar el sistema
/// de ambience: RoomTracker + una zona + un mueble embrujado + jugador temporal
/// (WASD + ratón) + piso y luz.
///
/// Uso: menú  Tools > Arcano XV > Generar escena de prueba de ambience
/// La escena se guarda en Assets/Scenes/AmbienceTest.unity
/// </summary>
public static class AmbienceTestSceneGenerator
{
    [MenuItem("Tools/Arcano XV/Generar escena de prueba de ambience")]
    public static void Generate()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Material mat = CreateLitMaterial("MatPiso");

        // Piso
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Piso";
        floor.transform.localScale = new Vector3(40f, 0.2f, 40f);
        floor.transform.position = new Vector3(0f, -0.1f, 0f);
        floor.GetComponent<Renderer>().sharedMaterial = mat;

        // Luz direccional para que se vea algo
        GameObject lightGO = new GameObject("Luz");
        Light l = lightGO.AddComponent<Light>();
        l.type = LightType.Directional;
        lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // RoomTracker (uno por escena)
        GameObject trackerGO = new GameObject("RoomTracker");
        trackerGO.AddComponent<RoomTracker>();

        // Zona de prueba (trigger que cubre la sala)
        GameObject zoneGO = new GameObject("ZonaPrueba");
        BoxCollider col = zoneGO.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.center = new Vector3(0f, 2f, 0f);
        col.size = new Vector3(30f, 6f, 30f);
        RoomTriggerZone zone = zoneGO.AddComponent<RoomTriggerZone>();
        zone.roomName = "Sala de prueba";

        // Mueble embrujado: raíz con HauntedObject + cubo hijo visible
        GameObject furniture = new GameObject("MuebleEmbrujado");
        HauntedObject haunted = furniture.AddComponent<HauntedObject>();
        haunted.proximityDistance = 5f;

        GameObject mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mesh.name = "Malla";
        mesh.transform.SetParent(furniture.transform, false);
        mesh.transform.localPosition = new Vector3(2f, 1f, 2f);
        mesh.transform.localScale = Vector3.one * 1.2f;
        mesh.GetComponent<Renderer>().sharedMaterial = mat;

        // Jugador temporal (detecta la zona y las reacciones)
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "JugadorPrueba";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1f, 12f);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        player.AddComponent<DebugPlayerMover>();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        const string path = "Assets/Scenes/AmbienceTest.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), path);
        AssetDatabase.SaveAssets();

        Debug.Log($"[Arcano XV] Escena de prueba creada en {path}. Presiona Play: muévete con WASD y acércate/alejate del mueble.");
    }

    private static Material CreateLitMaterial(string name)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        return new Material(shader) { name = name };
    }
}