using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Carga los clips de susto desde Assets/Audio/Sustos/Near y /Far
/// y los asigna al HauntedObject. Usado por el generador de prefabs,
/// el de escena de prueba y el menú "Asignar sonidos...".
/// </summary>
public static class SustosSounds
{
    public const string NearFolder = "Assets/Audio/Sustos/Near";
    public const string FarFolder = "Assets/Audio/Sustos/Far";

    public static AudioClip[] NearClips() => LoadClips(NearFolder);
    public static AudioClip[] FarClips() => LoadClips(FarFolder);

    public static void AssignTo(HauntedObject h)
    {
        if (h == null) return;

        h.nearSounds = NearClips();
        h.farSounds = FarClips();
        if (h.nearSounds.Length > 0 || h.farSounds.Length > 0)
        {
            Debug.Log($"[Arcano XV] Sustos asignados: {h.nearSounds.Length} cerca, {h.farSounds.Length} lejos.");
        }
        else
        {
            Debug.LogWarning("[Arcano XV] No se encontraron clips. Revisa que haya audio en " + NearFolder + " / " + FarFolder);
        }
    }

    private static AudioClip[] LoadClips(string folder)
    {
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folder });
        var clips = new List<AudioClip>();
        foreach (string guid in guids)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(AssetDatabase.GUIDToAssetPath(guid));
            if (clip != null) clips.Add(clip);
        }
        return clips.ToArray();
    }
}