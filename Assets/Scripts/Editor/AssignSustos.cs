using UnityEditor;
using UnityEngine;

/// <summary>
/// Asigna los clips de susto (Assets/Audio/Sustos) al prefab HauntedFurniture
/// sin borrar su configuración. Útil si el prefab se regeneró y quedó sin sonidos.
///
/// Uso: menú  Tools > Arcano XV > Asignar sonidos de susto al prefab
/// </summary>
public static class AssignSustos
{
    private const string PrefabPath = "Assets/Prefabs/Ambience/HauntedFurniture.prefab";

    [MenuItem("Tools/Arcano XV/Asignar sonidos de susto al prefab")]
    public static void Assign()
    {
        GameObject root = PrefabUtility.LoadPrefabContents(PrefabPath);
        if (root == null)
        {
            Debug.LogError("[Arcano XV] No existe el prefab: " + PrefabPath + ". Genera los prefabs primero.");
            return;
        }

        HauntedObject haunted = root.GetComponent<HauntedObject>();
        if (haunted == null)
        {
            Debug.LogError("[Arcano XV] El prefab " + PrefabPath + " no tiene HauntedObject.");
            PrefabUtility.UnloadPrefabContents(root);
            return;
        }

        SustosSounds.AssignTo(haunted);

        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        PrefabUtility.UnloadPrefabContents(root);
        Debug.Log("[Arcano XV] Prefab " + PrefabPath + " actualizado con los sonidos de susto.");
    }
}