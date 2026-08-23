using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class HintCardGenerator : EditorWindow
{
    [MenuItem("Tools/Arcano XV/Generar Carta Pista")]
    public static void GenerateHintCard()
    {
        // Buscar escena HabitacionTrofeo
        string[] escenas = AssetDatabase.FindAssets("HabitacionTrofeo t:Scene");
        if (escenas.Length == 0)
        {
            Debug.LogError("[HintCard] No se encontró HabitacionTrofeo.unity");
            return;
        }

        string escenaPath = AssetDatabase.GUIDToAssetPath(escenas[0]);
        EditorSceneManager.OpenScene(escenaPath);

        // Buscar carta pista existente y eliminarla
        GameObject cartaVieja = GameObject.Find("CartaPista");
        if (cartaVieja != null)
            GameObject.DestroyImmediate(cartaVieja);

        // Crear Quad
        GameObject carta = GameObject.CreatePrimitive(PrimitiveType.Quad);
        carta.name = "CartaPista";

        // Posición (diferente a CartaIntro)
        carta.transform.position = new Vector3(5, 1.2f, 35f);
        carta.transform.localScale = Vector3.one * 1.6f;

        // Limpiar collider default
        Collider defaultCollider = carta.GetComponent<Collider>();
        if (defaultCollider != null)
            GameObject.DestroyImmediate(defaultCollider);

        // Añadir BoxCollider
        BoxCollider bc = carta.AddComponent<BoxCollider>();
        bc.size = new Vector3(0.7f, 0.9f, 0.2f);
        bc.center = new Vector3(0f, 0.45f, 0f);

        // Añadir componente HintCard
        HintCard hintCard = carta.AddComponent<HintCard>();
        hintCard.distanciaInteraccion = 3f;
        hintCard.tiempoTransicion = 0.5f;
        hintCard.tamanoPanel = new Vector2(600, 800);
        hintCard.tamanoTextoArea = new Vector2(500, 700);
        // El texto de pista se puede editar en el Inspector después

        // Cargar textura usando sharedMaterial para evitar leaks
        Texture2D tex = Resources.Load<Texture2D>("Historias/Carta quemada");
        if (tex != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = tex;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            carta.GetComponent<Renderer>().sharedMaterial = mat;
        }

        // Guardar cambios
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        Debug.Log("========================================");
        Debug.Log("✓ Carta Pista creada en HabitacionTrofeo");
        Debug.Log("✓ Posición: (5, 1.2, 35)");
        Debug.Log("✓ Edita el texto en el Inspector");
        Debug.Log("========================================");
    }
}