using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Genera (o regenera) los prefabs del sistema de ambience paranormal:
///   - Assets/Prefabs/Ambience/RoomTracker.prefab
///   - Assets/Prefabs/Ambience/RoomZone.prefab
///   - Assets/Prefabs/Ambience/HauntedFurniture.prefab
///
/// Uso: menú  Tools > Arcano XV > Generar prefabs de ambience
/// Útil si el equipo prefiere prefabs generados por el propio Unity (o necesita
/// regenerarlos tras cambios en los scripts).
/// </summary>
public static class AmbiencePrefabGenerator
{
    private const string Folder = "Assets/Prefabs/Ambience";

    // Ya no es menú propio: lo invoca el único menú Tools > Arcano XV > Generar TODO.
    public static void GenerateAll()
    {
        Directory.CreateDirectory(Folder);

        Create<RoomTracker>("RoomTracker", go => { });
        Create<RoomTriggerZone>("RoomZone", go =>
        {
            BoxCollider col = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(10f, 3f, 10f);
            col.center = new Vector3(0f, 1.5f, 0f);
        });
        Create<HauntedObject>("HauntedFurniture", go =>
            SustosSounds.AssignTo(go.GetComponent<HauntedObject>()));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[Arcano XV] Prefabs de ambience generados en {Folder}");
    }

    private static void Create<T>(string name, System.Action<GameObject> configure) where T : Component
    {
        GameObject go = new GameObject(name);
        go.AddComponent<T>();

        if (configure != null)
            configure(go);

        string path = Folder + "/" + name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        Debug.Log($"[Arcano XV] {path} ok");
    }
}