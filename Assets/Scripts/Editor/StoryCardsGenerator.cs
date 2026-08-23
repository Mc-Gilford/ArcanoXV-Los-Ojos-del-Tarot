using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class StoryCardsGenerator : EditorWindow
{
    // 9 cartas de historia - una por habitación (excepto La Huida, la final)
    private static readonly string[] _habitaciones = {
        "El Olvido", "El Castigo", "El Generador", "El Ritual", "La Reliquia",
        "La Cacería", "La Investigación", "El Almacén", "La Corrupción"
    };

    private static readonly string[] _titulos = {
        "El Origen del Don",
        "La Reina del Tarot",
        "La Cima del Éxito",
        "La Profecía del Dolor",
        "El Llamado del Arcano XV",
        "El Último Espectáculo",
        "El Sacrificio y la Traición",
        "La Infestación de la Mansión",
        "El Nacimiento del Acechador"
    };

    private static readonly string[] _textos = {
        // 1 - El Olvido
        "Mi abuela me lo dijo la noche antes de morir: \"El tarot no predice el futuro, niña. Lo escribe.\"\n\nNo entendí sus palabras hasta que encontré el mazo en el desván. Cartas que brillaban con luz propia, que susurraban al tocarlas. La primera vez que saqué una, el aire se heló y vi... vi cosas que no deberían verse.\n\nAhora sé que el don no es un regalo. Es una condena. Y mi hermana Hazel la ha heredado.",

        // 2 - El Castigo
        "La llamaban La Reina del Tarot en el circo ambulante. Madame Zara, la mujer que leía el destino por tres monedas.\n\nPero su verdadero poder no estaba en las cartas, sino en lo que hacía con quienes la cruzaban. Los encerraba en sus propios miedos, tejía maldiciones en la tinta de sus arcanos.\n\nHazel la buscó. La encontró. Y ahora... ahora Hazel es su aprendiz. O su prisionera.",

        // 3 - El Generador
        "El circo llegó a la cima. Teatros llenos, nobles y reyes pagando fortunas por una lectura de Madame Zara.\n\nPero el éxito tiene un precio. El generador que alimentaba las luces del espectáculo zumbaba con algo más que electricidad. Alimentaba el ritual. Cada función, cada aplauso, cada moneda... todo era energía para el Arcano XV.\n\nMi hermana estaba en el centro. Brillando. Rota.",

        // 4 - El Ritual
        "La noche del solsticio, el círculo se cerró. Trece velas. Trece cartas. Trece susurros.\n\n\"El Arcano XV despertará cuando la sangre de la linaje toque el pentagrama.\"\n\nYo no estuve allí. Pero las cartas me lo mostraron. Vi a Hazel de rodillas, las manos sangrando sobre el altar. Vi a Madame Zara sonriendo con dientes afilados. Vi el momento en que el mundo se partió en dos.",

        // 5 - La Reliquia
        "El relicario late. Lo siento en los huesos, en la sangre, en cada carta que toco.\n\nEl Arcano XV no es una carta. Es una puerta. Y mi hermana es la llave.\n\nMadame Zara lo planeó todo. Años. Décadas. La casa, el circo, el generador, el ritual... todo para crear un recipiente. Hazel. Su poder, su linaje, su don... todo converge en el Arcano XV.",

        // 6 - La Cacería
        "El cuerno suena. La cacería comienza.\n\nMadame Zara nos observa desde su palco. Sus criaturas —los que fueron público, los que fueron artistas— nos persiguen por los pasillos. Cada trofeo en esta pared fue alguien. Alguien que buscó la verdad. Alguien que falló.\n\nEl espectáculo no ha terminado. El público exige sangre.",

        // 7 - La Investigación
        "Notas. Miles de notas. Mi letra. No mi letra. La de Hazel. La de Madame Zara. Todas mezcladas.\n\nLa verdad duele: nuestra abuela no murió de vieja. La sacrificó Zara para sellar el primer círculo. Y mi madre... mi madre lo sabía. Lo permitió. Por poder. Por el don.\n\nHazel no es mi hermana. Es mi reemplazo. La siguiente en la línea.",

        // 8 - El Almacén
        "La nevera zumba. El ventilador gira. Cajas y más cajas de recuerdos ajenos.\n\nEsta casa no siempre fue una mansión. Fue un orfanato. Un hospital. Un teatro. Un matadero. Cada capa atrapada en las paredes. Los espíritus no son fantasmas. Son ecos. Grabaciones en la materia.\n\nLa infestación no viene de fuera. Viene de dentro. Del ritual que nunca terminó.",

        // 9 - La Corrupción
        "La carne cuelga. Las moscas zumban. El goteo marca el tiempo que no existe.\n\nAquí nació. El Acechador. La manifestación física de la cordura rota. Cuando mi mente se fracturó por primera vez al usar las cartas, él nació de mis grietas.\n\nEs invulnerable porque es MÍO. Mi miedo. Mi culpa. Mi duda. Me persigue porque soy su creador."
    };

    private static readonly string[] _imagenes = {
        "Historias/01_el_origen_del_don",
        "Historias/02_la_reina_del_tarot",
        "Historias/03_la_cima_del_exito",
        "Historias/04_la_profecia_del_dolor",
        "Historias/05_el_llamado_del_arcano_xv",
        "Historias/06_el_ultimo_espectaculo",
        "Historias/07_el_sacrificio_y_la_traicion",
        "Historias/08_la_infestacion_de_la_mansion",
        "Historias/09_el_nacimiento_del_acechador"
    };

    [MenuItem("Tools/Arcano XV/Generar Cartas de Historia (9)")]
    public static void GenerarTodas()
    {
        // Buscar o crear carpeta de prefabs
        string prefabFolder = "Assets/Prefabs/CartasHistoria";
        if (!AssetDatabase.IsValidFolder(prefabFolder))
        {
            string parent = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(parent))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            AssetDatabase.CreateFolder(parent, "CartasHistoria");
        }

        // Eliminar cartas viejas si existen
        for (int i = 1; i <= 9; i++)
        {
            string path = $"{prefabFolder}/Carta_{i:D2}_{SanitizarNombre(_titulos[i-1])}.prefab";
            if (File.Exists(path))
                AssetDatabase.DeleteAsset(path);
        }

        for (int i = 0; i < 9; i++)
        {
            CrearCartaHistoria(i, prefabFolder);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("========================================");
        Debug.Log($"✓ 9 cartas de historia creadas en {prefabFolder}");
        Debug.Log("✓ Cada prefab tiene HintCard con texto e imagen asignados");
        Debug.Log("✓ Arrastra cada prefab a su habitación correspondiente");
        Debug.Log("========================================");
    }

    private static void CrearCartaHistoria(int index, string folder)
    {
        // Crear Quad
        GameObject carta = GameObject.CreatePrimitive(PrimitiveType.Quad);
        carta.name = $"Carta_{index + 1:D2}_{SanitizarNombre(_titulos[index])}";
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
        hint.textoPista = _textos[index];

        // Cargar textura
        Texture2D tex = Resources.Load<Texture2D>(_imagenes[index]);
        if (tex != null)
        {
            var renderer = carta.GetComponent<Renderer>();
            // Crear material nuevo para el prefab
            Material mat = new Material(Shader.Find("Standard"));
            mat.mainTexture = tex;
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            renderer.sharedMaterial = mat;
        }
        else
        {
            Debug.LogWarning($"[StoryCards] No se encontró imagen: {_imagenes[index]}");
        }

        // Guardar como prefab
        string nombreArchivo = $"Carta_{index + 1:D2}_{SanitizarNombre(_titulos[index])}";
        string prefabPath = $"{folder}/{nombreArchivo}.prefab";
        PrefabUtility.SaveAsPrefabAsset(carta, prefabPath);
        Object.DestroyImmediate(carta);

        Debug.Log($"[StoryCards] Creada: {_titulos[index]} ({_habitaciones[index]})");
    }

    private static string SanitizarNombre(string nombre)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            nombre = nombre.Replace(c, '_');
        return nombre.Replace(" ", "_");
    }
}
