using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class CollectibleCardGenerator : EditorWindow
{
    [MenuItem("Tools/Arcano XV/Generar Prefabs Cartas Coleccionables (5)")]
    public static void GenerarCartasColeccionables()
    {
        // Crear carpeta si no existe
        string prefabFolder = "Assets/Prefabs/CartasColeccionables";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            string parent = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder(parent, "CartasColeccionables");
        }

        // Buscar el modelo GLTF
        GameObject modelo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Cartas/scene.gltf");
        if (modelo == null)
        {
            Debug.LogError("[CollectibleCardGenerator] No se encontró el modelo en Assets/Models/Cartas/scene.gltf");
            return;
        }

        for (int i = 1; i <= 5; i++)
        {
            string path = $"{prefabFolder}/CartaColeccionable_{i}.prefab";
            if (File.Exists(path))
                AssetDatabase.DeleteAsset(path);

            // Instanciar el modelo
            GameObject carta = PrefabUtility.InstantiatePrefab(modelo) as GameObject;
            carta.name = $"CartaColeccionable_{i}";

            // Añadir componente CardPickup
            CardPickup pickup = carta.AddComponent<CardPickup>();
            pickup.radioInteraccion = 2.5f;
            pickup.alturaFlotacion = 0.15f;
            pickup.velocidadRotacion = 70f;

            // Asegurar tag
            if (carta.tag == "Untagged")
                carta.tag = "Untagged";

            // Guardar como prefab
            PrefabUtility.SaveAsPrefabAsset(carta, path);
            Object.DestroyImmediate(carta);

            Debug.Log($"[CollectibleCardGenerator] Creada: CartaColeccionable_{i}.prefab");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("========================================");
        Debug.Log($"✓ 5 cartas coleccionables creadas en {prefabFolder}");
        Debug.Log("✓ Usan modelo GLTF + CardPickup.cs");
        Debug.Log("✓ Colócalas en la escena para probar");
        Debug.Log("========================================");
    }

    [MenuItem("Tools/Arcano XV/Crear Escena Prueba Cartas Coleccionables")]
    public static void CrearEscenaPruebaColeccionables()
    {
        string sceneFolder = "Assets/Scenes/Tests";
        if (!AssetDatabase.IsValidFolder(sceneFolder))
        {
            string parent = "Assets/Scenes";
            if (!AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            AssetDatabase.CreateFolder(parent, "Tests");
        }

        string scenePath = $"{sceneFolder}/TestCartasColeccionables.unity";
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        scene.name = "TestCartasColeccionables";

        // Jugador
        GameObject jugador = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        jugador.name = "Player";
        jugador.tag = "Player";
        jugador.transform.position = new Vector3(0, 1, 0);

        // Luz
        Light dirLight = Object.FindObjectOfType<Light>();
        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // CardCollector en el jugador
        jugador.AddComponent<CardCollector>();

        // Modelo
        GameObject modelo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Models/Cartas/scene.gltf");
        if (modelo == null)
        {
            Debug.LogError("No se encontró el modelo");
            return;
        }

        // 5 cartas en anillo
        float radio = 5f;
        for (int i = 0; i < 5; i++)
        {
            float angulo = (360f / 5f) * i * Mathf.Deg2Rad;
            Vector3 pos = new Vector3(Mathf.Sin(angulo) * radio, 1f, Mathf.Cos(angulo) * radio);

            GameObject carta = PrefabUtility.InstantiatePrefab(modelo) as GameObject;
            carta.name = $"CartaColeccionable_{i + 1}";
            carta.transform.position = pos;
            carta.transform.localScale = Vector3.one;

            CardPickup pickup = carta.AddComponent<CardPickup>();
            pickup.radioInteraccion = 2.5f;
            pickup.alturaFlotacion = 0.15f;
            pickup.velocidadRotacion = 70f;
        }

        // Puerta de prueba
        GameObject puerta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        puerta.name = "PuertaJefeFinal";
        puerta.transform.position = new Vector3(0, 2, 10);
        puerta.transform.localScale = new Vector3(4, 4, 0.5f);
        BossDoor bossDoor = puerta.AddComponent<BossDoor>();

        EditorSceneManager.SaveScene(scene, scenePath);

        Debug.Log("========================================");
        Debug.Log($"✓ Escena creada: {scenePath}");
        Debug.Log("✓ 5 cartas coleccionables en anillo");
        Debug.Log("✓ Jugador con CardCollector");
        Debug.Log("✓ Puerta del jefe con BossDoor");
        Debug.Log("CONTROLES: Acércate a carta → E para recoger");
        Debug.Log("========================================");
    }
}