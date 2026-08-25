using UnityEngine;
using UnityEditor;

/// <summary>
/// Genera 6 prefabs de objetivos de habitación, uno por cada tipo.
/// Herramienta: Tools > Arcano XV > Generar Prefabs Objetivos
/// </summary>
public class GenerarPrefabsObjetivos : EditorWindow
{
    private static void Generar()
    {
        string carpeta = "Assets/Prefabs/Objetivos";
        if (!AssetDatabase.IsValidFolder(carpeta))
        {
            AssetDatabase.CreateFolder("Assets/Prefabs", "Objetivos");
        }

        CrearPrefab("Objetivo_BuscarTarjeta", ObjetivoHabitacion.TipoObjetivo.BuscarTarjeta, carpeta, 0f, 0);
        CrearPrefab("Objetivo_SobrevivirTiempo", ObjetivoHabitacion.TipoObjetivo.SobrevivirTiempo, carpeta, 30f, 0);
        CrearPrefab("Objetivo_MatarEnemigos", ObjetivoHabitacion.TipoObjetivo.MatarEnemigos, carpeta, 0f, 5);
        CrearPrefab("Objetivo_AtrapaVelas", ObjetivoHabitacion.TipoObjetivo.AtrapaVelas, carpeta, 0f, 0);
        CrearPrefab("Objetivo_PrendeLinternas", ObjetivoHabitacion.TipoObjetivo.PrendeLinternas, carpeta, 0f, 0);
        CrearPrefab("Objetivo_DestruyeCajas", ObjetivoHabitacion.TipoObjetivo.DestruyeCajas, carpeta, 0f, 0);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Arcano XV] 6 prefabs de objetivos generados en " + carpeta);
    }

    private static void CrearPrefab(string nombre, ObjetivoHabitacion.TipoObjetivo tipo, string carpeta, float tiempo, int enemigos)
    {
        GameObject go = new GameObject(nombre);
        go.AddComponent<BoxCollider>().isTrigger = true;

        ObjetivoHabitacion obj = go.AddComponent<ObjetivoHabitacion>();
        obj.tipoObjetivo = tipo;
        obj.tiempoSobrevivir = tiempo;
        obj.cantidadEnemigos = enemigos;

        string ruta = $"{carpeta}/{nombre}.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, ruta);
        Object.DestroyImmediate(go);
    }
}
