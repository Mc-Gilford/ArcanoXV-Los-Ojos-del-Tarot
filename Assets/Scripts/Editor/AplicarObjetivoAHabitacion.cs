using UnityEngine;
using UnityEditor;

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
    private float duracionAlerta = 6f;

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

        // Prefab
        prefabHabitacion = (GameObject)EditorGUILayout.ObjectField(
            "Prefab de habitación", prefabHabitacion, typeof(GameObject), false);

        EditorGUILayout.Space(5);

        // Tipo de objetivo
        tipoSeleccionado = (ObjetivoHabitacion.TipoObjetivo)EditorGUILayout.EnumPopup(
            "Tipo de objetivo", tipoSeleccionado);

        // Parámetros según tipo
        EditorGUILayout.Space(5);
        if (tipoSeleccionado == ObjetivoHabitacion.TipoObjetivo.SobrevivirTiempo)
        {
            tiempoSobrevivir = EditorGUILayout.FloatField("Tiempo (segundos)", tiempoSobrevivir);
        }
        else if (tipoSeleccionado == ObjetivoHabitacion.TipoObjetivo.MatarEnemigos)
        {
            cantidadEnemigos = EditorGUILayout.IntField("Cantidad de enemigos", cantidadEnemigos);
        }

        duracionAlerta = EditorGUILayout.FloatField("Duración alerta (seg)", duracionAlerta);

        EditorGUILayout.Space(10);

        // Botón aplicar
        GUI.enabled = prefabHabitacion != null;
        if (GUILayout.Button("Aplicar", GUILayout.Height(32)))
        {
            AplicarObjetivo();
        }
        GUI.enabled = true;

        EditorGUILayout.Space(10);

        // Instrucciones
        EditorGUILayout.HelpBox(
            "1. Arrastra el prefab de la habitación (ej: H1 El Olvido)\n" +
            "2. Selecciona el tipo de objetivo\n" +
            "3. Ajusta los parámetros si es necesario\n" +
            "4. Click 'Aplicar'\n" +
            "5. Prueba en Unity: Play → entra a la habitación",
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

        // Cargar prefab como editable
        GameObject prefab = PrefabUtility.LoadPrefabContents(ruta);
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Error", "No se pudo cargar el prefab.", "OK");
            return;
        }

        // Verificar si ya tiene ObjetivoHabitacion
        ObjetivoHabitacion existente = prefab.GetComponent<ObjetivoHabitacion>();
        if (existente != null)
        {
            Undo.RecordObject(existente, "Actualizar Objetivo");
            existente.tipoObjetivo = tipoSeleccionado;
            existente.tiempoSobrevivir = tiempoSobrevivir;
            existente.cantidadEnemigos = cantidadEnemigos;
            existente.duracionAlerta = duracionAlerta;
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
            EditorUtility.SetDirty(prefab);
        }

        // Guardar
        PrefabUtility.SaveAsPrefabAsset(prefab, ruta);
        PrefabUtility.UnloadPrefabContents(prefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        string nombre = prefabHabitacion.name;
        Debug.Log($"[Arcano XV] Objetivo '{tipoSeleccionado}' aplicado a '{nombre}' ({duracionAlerta}s alerta)");
        EditorUtility.DisplayDialog("Listo",
            $"Objetivo '{tipoSeleccionado}' aplicado a '{nombre}'.\nPrueba en Unity: Play → entra a la habitación.",
            "OK");
    }
}
