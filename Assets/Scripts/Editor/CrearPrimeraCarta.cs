using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CrearPrimeraCarta : EditorWindow
{
    [MenuItem("Tools/Arcano XV/Crear Primera Carta (Test)")]
    public static void Crear()
    {
        // Verificar que hay una escena abierta
        if (EditorSceneManager.GetActiveScene().name == "")
        {
            Debug.LogError("❌ No hay escena abierta. Abre la escena donde quieras la carta.");
            return;
        }

        // Eliminar carta de prueba vieja si existe
        GameObject vieja = GameObject.Find("Carta_01_El_Origen_del_Don");
        if (vieja != null) Object.DestroyImmediate(vieja);

        // Crear Quad
        GameObject carta = GameObject.CreatePrimitive(PrimitiveType.Quad);
        carta.name = "Carta_01_El_Origen_del_Don";
        carta.transform.position = new Vector3(0, 1.2f, 2);
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
        hint.textoPista = "Mi abuela me lo dijo la noche antes de morir: \"El tarot no predice el futuro, niña. Lo escribe.\"\n\nNo entendí sus palabras hasta que encontré el mazo en el desván. Cartas que brillaban con luz propia, que susurraban al tocarlas. La primera vez que saqué una, el aire se heló y vi... vi cosas que no deberían verse.\n\nAhora sé que el don no es un regalo. Es una condena. Y mi hermana Hazel la ha heredado.";

        // Cargar textura con sharedMaterial
        Texture2D tex = Resources.Load<Texture2D>("Historias/01_el_origen_del_don");
        if (tex != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = tex;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            carta.GetComponent<Renderer>().sharedMaterial = mat;
            Debug.Log("✓ Textura '01_el_origen_del_don' cargada");
        }
        else
        {
            Debug.LogError("❌ No se encontró 'Historias/01_el_origen_del_don' en Resources");
        }

        // Seleccionar el objeto
        Selection.activeGameObject = carta;

        Debug.Log("========================================");
        Debug.Log("✓ Primera carta creada en la escena actual");
        Debug.Log("✓ Posición: (0, 1.2, 2) - frente al jugador");
        Debug.Log("✓ Nombre: Carta_01_El_Origen_del_Don");
        Debug.Log("✓ HintCard configurado con texto e imagen");
        Debug.Log("========================================");
        Debug.Log("CONTROLES: Acércate → X para abrir → E para voltear → V para cerrar");
    }
}