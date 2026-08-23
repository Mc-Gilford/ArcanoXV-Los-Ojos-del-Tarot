using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class CartasPrefabsGenerator : EditorWindow
{
    [MenuItem("Tools/Arcano XV/Generar Prefabs Cartas Intro y Pista")]
    public static void GenerarPrefabsCartas()
    {
        // Crear carpeta si no existe
        string prefabFolder = "Assets/Prefabs/CartasEspeciales";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            string parent = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder(parent, "CartasEspeciales");
        }

        // Crear CartaIntro
        CrearCartaIntro(prefabFolder);

        // Crear CartaPista
        CrearCartaPista(prefabFolder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("========================================");
        Debug.Log($"✓ Prefabs creados en {prefabFolder}");
        Debug.Log("✓ CartaIntro.prefab - usa IntroCard.cs");
        Debug.Log("✓ CartaPista.prefab - usa HintCard.cs");
        Debug.Log("========================================");
    }

    private static void CrearCartaIntro(string folder)
    {
        // Eliminar si existe
        string path = $"{folder}/CartaIntro.prefab";
        if (File.Exists(path))
            AssetDatabase.DeleteAsset(path);

        // Crear Quad
        GameObject carta = GameObject.CreatePrimitive(PrimitiveType.Quad);
        carta.name = "CartaIntro";
        carta.transform.localScale = Vector3.one * 1.6f;

        // Limpiar collider
        Collider col = carta.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);

        // BoxCollider
        BoxCollider bc = carta.AddComponent<BoxCollider>();
        bc.size = new Vector3(0.7f, 0.9f, 0.2f);
        bc.center = new Vector3(0f, 0.45f, 0f);

        // IntroCard
        IntroCard intro = carta.AddComponent<IntroCard>();
        intro.nombreImagen = "intro";
        intro.distanciaInteraccion = 3f;
        intro.tiempoTransicion = 0.5f;

        // Cargar textura
        Texture2D tex = Resources.Load<Texture2D>("Historias/intro");
        if (tex != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = tex;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            carta.GetComponent<Renderer>().sharedMaterial = mat;
        }
        else
        {
            Debug.LogWarning("[CartaIntro] No se encontró imagen 'intro' en Resources/Historias");
        }

        // Guardar como prefab
        PrefabUtility.SaveAsPrefabAsset(carta, path);
        Object.DestroyImmediate(carta);

        Debug.Log($"[CartasPrefabs] Creada: CartaIntro.prefab");
    }

    private static void CrearCartaPista(string folder)
    {
        // Eliminar si existe
        string path = $"{folder}/CartaPista.prefab";
        if (File.Exists(path))
            AssetDatabase.DeleteAsset(path);

        // Crear Quad
        GameObject carta = GameObject.CreatePrimitive(PrimitiveType.Quad);
        carta.name = "CartaPista";
        carta.transform.localScale = Vector3.one * 1.6f;

        // Limpiar collider
        Collider col = carta.GetComponent<Collider>();
        if (col != null) Object.DestroyImmediate(col);

        // BoxCollider
        BoxCollider bc = carta.AddComponent<BoxCollider>();
        bc.size = new Vector3(0.7f, 0.9f, 0.2f);
        bc.center = new Vector3(0f, 0.45f, 0f);

        // HintCard
        HintCard hint = carta.AddComponent<HintCard>();
        hint.distanciaInteraccion = 3f;
        hint.tiempoTransicion = 0.5f;
        hint.tamanoPanel = new Vector2(600, 800);
        hint.tamanoTextoArea = new Vector2(500, 700);
        hint.textoPista = "Edita este texto en el Inspector para agregar la pista.";

        // Cargar textura
        Texture2D tex = Resources.Load<Texture2D>("Historias/Carta quemada");
        if (tex != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = tex;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            carta.GetComponent<Renderer>().sharedMaterial = mat;
        }
        else
        {
            Debug.LogWarning("[CartaPista] No se encontró 'Carta quemada' en Resources/Historias");
        }

        // Guardar como prefab
        PrefabUtility.SaveAsPrefabAsset(carta, path);
        Object.DestroyImmediate(carta);

        Debug.Log($"[CartasPrefabs] Creada: CartaPista.prefab");
    }

    [MenuItem("Tools/Arcano XV/Crear Escena de Prueba (Cartas)")]
    public static void CrearEscenaPrueba()
    {
        // Crear carpeta si no existe
        string sceneFolder = "Assets/Scenes/Tests";
        if (!AssetDatabase.IsValidFolder(sceneFolder))
        {
            string parent = "Assets/Scenes";
            if (!AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            AssetDatabase.CreateFolder(parent, "Tests");
        }

        string scenePath = $"{sceneFolder}/TestCartas.unity";

        // Crear nueva escena
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        scene.name = "TestCartas";

        // Crear jugador temporal (cubo para pruebas)
        GameObject jugador = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        jugador.name = "Player";
        jugador.tag = "Player";
        jugador.transform.position = new Vector3(0, 1, 0);

        // Luz direccional
        Light dirLight = Object.FindObjectOfType<Light>();
        if (dirLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Crear carta intro
        GameObject cartaIntro = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cartaIntro.name = "CartaIntro";
        cartaIntro.transform.position = new Vector3(-3, 1.2f, 3);
        cartaIntro.transform.localScale = Vector3.one * 1.6f;

        Collider colIntro = cartaIntro.GetComponent<Collider>();
        if (colIntro != null) Object.DestroyImmediate(colIntro);

        BoxCollider bcIntro = cartaIntro.AddComponent<BoxCollider>();
        bcIntro.size = new Vector3(0.7f, 0.9f, 0.2f);
        bcIntro.center = new Vector3(0f, 0.45f, 0f);

        IntroCard introCard = cartaIntro.AddComponent<IntroCard>();
        introCard.nombreImagen = "intro";
        introCard.distanciaInteraccion = 3f;

        Texture2D texIntro = Resources.Load<Texture2D>("Historias/intro");
        if (texIntro != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = texIntro;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            cartaIntro.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Crear carta pista
        GameObject cartaPista = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cartaPista.name = "CartaPista";
        cartaPista.transform.position = new Vector3(3, 1.2f, 3);
        cartaPista.transform.localScale = Vector3.one * 1.6f;

        Collider colPista = cartaPista.GetComponent<Collider>();
        if (colPista != null) Object.DestroyImmediate(colPista);

        BoxCollider bcPista = cartaPista.AddComponent<BoxCollider>();
        bcPista.size = new Vector3(0.7f, 0.9f, 0.2f);
        bcPista.center = new Vector3(0f, 0.45f, 0f);

        HintCard hintCard = cartaPista.AddComponent<HintCard>();
        hintCard.distanciaInteraccion = 3f;
        hintCard.textoPista = "Esta es una carta de prueba.\n\nEdítala en el Inspector.";

        Texture2D texPista = Resources.Load<Texture2D>("Historias/Carta quemada");
        if (texPista != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = texPista;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            cartaPista.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Crear carta de historia de ejemplo
        GameObject cartaHistoria = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cartaHistoria.name = "CartaHistoria_Ejemplo";
        cartaHistoria.transform.position = new Vector3(0, 1.2f, 3);
        cartaHistoria.transform.localScale = Vector3.one * 1.6f;

        Collider colHist = cartaHistoria.GetComponent<Collider>();
        if (colHist != null) Object.DestroyImmediate(colHist);

        BoxCollider bcHist = cartaHistoria.AddComponent<BoxCollider>();
        bcHist.size = new Vector3(0.7f, 0.9f, 0.2f);
        bcHist.center = new Vector3(0f, 0.45f, 0f);

        HintCard histCard = cartaHistoria.AddComponent<HintCard>();
        histCard.distanciaInteraccion = 3f;
        histCard.textoPista = "Mi abuela me lo dijo la noche antes de morir: \"El tarot no predice el futuro, niña. Lo escribe.\"\n\nNo entendí sus palabras hasta que encontré el mazo en el desván.";

        Texture2D texHist = Resources.Load<Texture2D>("Historias/01_el_origen_del_don");
        if (texHist != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = texHist;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            cartaHistoria.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Guardar escena
        EditorSceneManager.SaveScene(scene, scenePath);

        Debug.Log("========================================");
        Debug.Log($"✓ Escena de prueba creada: {scenePath}");
        Debug.Log("✓ 3 cartas: Intro, Pista, Historia");
        Debug.Log("✓ Jugador con tag 'Player'");
        Debug.Log("CONTROLES: X=abrir, E=voltear, V=cerrar");
        Debug.Log("========================================");
    }
}
