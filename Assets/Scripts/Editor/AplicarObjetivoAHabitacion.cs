using UnityEngine;
using UnityEditor;
using TMPro;

/// <summary>
/// Ventana para agregar ObjetivoHabitacion a un prefab de habitación existente.
/// Menú: Tools > Arcano XV > Aplicar Objetivo a Habitación
/// </summary>
public class AplicarObjetivoAHabitacion : EditorWindow
{
    private GameObject prefabHabitacion;
    private ObjetivoHabitacion.TipoObjetivo tipoSeleccionado = ObjetivoHabitacion.TipoObjetivo.BuscarTarjeta;
    private float tiempoSobrevivir = 30f;
    private int cantidadEnemigos = 5;
    private float duracionAlerta = 5f;

    // GUID de "Old Horror Films 1-0 SDF.asset"
    private const string FUENTE_HORROR_GUID = "88cb6dacb7ce2754ea4cc3c20c5b9da6";

    [MenuItem("Tools/Arcano XV/Aplicar Objetivo a Habitación")]
    private static void Abrir()
    {
        var win = GetWindow<AplicarObjetivoAHabitacion>("Aplicar Objetivo");
        win.minSize = new Vector2(380, 320);
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Agregar Objetivo a Habitación", EditorStyles.boldLabel);
        EditorGUILayout.Space(8);

        prefabHabitacion = (GameObject)EditorGUILayout.ObjectField(
            "Prefab de habitación", prefabHabitacion, typeof(GameObject), false);

        EditorGUILayout.Space(5);

        tipoSeleccionado = (ObjetivoHabitacion.TipoObjetivo)EditorGUILayout.EnumPopup(
            "Tipo de objetivo", tipoSeleccionado);

        EditorGUILayout.Space(5);
        if (tipoSeleccionado == ObjetivoHabitacion.TipoObjetivo.SobrevivirTiempo)
            tiempoSobrevivir = EditorGUILayout.FloatField("Tiempo (segundos)", tiempoSobrevivir);
        else if (tipoSeleccionado == ObjetivoHabitacion.TipoObjetivo.MatarEnemigos)
            cantidadEnemigos = EditorGUILayout.IntField("Cantidad de enemigos", cantidadEnemigos);

        duracionAlerta = EditorGUILayout.FloatField("Duración alerta (seg)", duracionAlerta);

        EditorGUILayout.Space(10);

        GUI.enabled = prefabHabitacion != null;
        if (GUILayout.Button("Aplicar", GUILayout.Height(32)))
            AplicarObjetivo();
        GUI.enabled = true;

        EditorGUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "1. Arrastra el prefab (ej: H1 El Olvido)\n" +
            "2. Selecciona el tipo de objetivo\n" +
            "3. Click 'Aplicar'\n" +
            "4. Play → entra a la habitación",
            MessageType.Info);
    }

    private void AplicarObjetivo()
    {
        string ruta = AssetDatabase.GetAssetPath(prefabHabitacion);
        if (string.IsNullOrEmpty(ruta) || !ruta.EndsWith(".prefab"))
        {
            EditorUtility.DisplayDialog("Error", "El objeto seleccionado no es un prefab.", "OK");
            return;
        }

        GameObject prefab = PrefabUtility.LoadPrefabContents(ruta);
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", "No se pudo cargar el prefab.", "OK");
            return;
        }

        // Buscar fuente Old Horror Films
        TMP_FontAsset fuente = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            AssetDatabase.GUIDToAssetPath(FUENTE_HORROR_GUID));

        ObjetivoHabitacion existente = prefab.GetComponent<ObjetivoHabitacion>();
        if (existente != null)
        {
            Undo.RecordObject(existente, "Actualizar Objetivo");
            existente.tipoObjetivo = tipoSeleccionado;
            existente.tiempoSobrevivir = tiempoSobrevivir;
            existente.cantidadEnemigos = cantidadEnemigos;
            existente.duracionAlerta = duracionAlerta;
            if (fuente != null) existente.fuenteObjetivo = fuente;
            EditorUtility.SetDirty(existente);
        }
        else
        {
            Undo.RecordObject(prefab, "Agregar ObjetivoHabitacion");
            ObjetivoHabitacion obj = prefab.AddComponent<ObjetivoHabitacion>();
            obj.tipoObjetivo = tipoSeleccionado;
            obj.tiempoSobrevivir = tiempoSobrevivir;
            obj.cantidadEnemigos = cantidadEnemigos;
            obj.duracionAlerta = duracionAlerta;
            if (fuente != null) obj.fuenteObjetivo = fuente;
            EditorUtility.SetDirty(prefab);
        }

        PrefabUtility.SaveAsPrefabAsset(prefab, ruta);
        PrefabUtility.UnloadPrefabContents(prefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string nombre = prefabHabitacion.name;
        string fuenteMsg = fuente != null ? " (fuente Old Horror Films)" : " (fuente por defecto)";
        Debug.Log($"[Arcano XV] Objetivo '{tipoSeleccionado}' aplicado a '{nombre}'{fuenteMsg}");
        EditorUtility.DisplayDialog("Listo",
            $"Objetivo '{tipoSeleccionado}' aplicado a '{nombre}'.{fuenteMsg}\nPlay → entra a la habitación.",
            "OK");
    }
}
