using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Definición de un mueble/prop dentro de una habitación.
/// - modelPath: ruta al modelo real en Assets/Models (gltf o blend). Vacío = primitiva.
/// - fallback*: si el modelo aún no está importado (falta Blender/glTFast), se usa una
///   primitiva para que la habitación igual se arme y suene bien. Se reemplaza sola
///   volviendo a "Generar habitaciones" cuando el modelo importe.
/// - audioCategory: vacío = silencioso; si no, suena al tocarlo (Assets/Audio/Objetos/<cat>).
/// - pos/rot/scl: ubicación en la habitación. scl es multiplicador sobre la escala del modelo.
/// </summary>
public class FurnitureDef
{
    public string id;
    public string modelPath = "";
    public PrimitiveType fallbackShape = PrimitiveType.Cube;
    public Vector3 fallbackScale = Vector3.one;
    public Color fallbackColor = new Color(0.42f, 0.38f, 0.34f);
    public string audioCategory = "";
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scl = Vector3.one;

    public FurnitureDef(string id, Vector3 pos, string modelPath = "",
        Vector3 rot = default, Vector3 scl = default,
        PrimitiveType fallbackShape = PrimitiveType.Cube, Vector3 fallbackScale = default,
        Color fallbackColor = default, string audioCategory = "")
    {
        this.id = id;
        this.pos = pos;
        this.modelPath = modelPath;
        this.rot = (rot == default) ? Vector3.zero : rot;
        this.scl = (scl == default) ? Vector3.one : scl;
        this.fallbackShape = fallbackShape;
        this.fallbackScale = (fallbackScale == default) ? Vector3.one : fallbackScale;
        this.fallbackColor = (fallbackColor == default) ? new Color(0.42f, 0.38f, 0.34f) : fallbackColor;
        this.audioCategory = audioCategory;
    }
}

/// <summary>Una habitación del GDD: medidas, zona, jugador y su mobiliario.</summary>
public class RoomLayout
{
    public string name;              // nombre visible (va en la zona de sonido)
    public string folderName;        // nombre del archivo de escena (sin .unity)
    public Vector3 size = new Vector3(14f, 3.2f, 14f);
    public Color floorColor = new Color(0.24f, 0.19f, 0.16f);
    public Color wallColor = new Color(0.16f, 0.14f, 0.13f);
    public Vector3 playerSpawn = new Vector3(0f, 1f, 4f);
    public string wallTexturePath = ""; // textura opcional para las paredes (La Huida)
    public List<FurnitureDef> furniture = new List<FurnitureDef>();
}

/// <summary>
/// Catálogo de las 10 habitaciones del GDD con los modelos reales de Assets/Models
/// ya asignados a cada una (y primitivas con sonido como respaldo para lo que
/// no tiene modelo). Es la fuente del generador Tools > Arcano XV > Generar habitaciones.
/// </summary>
public static class RoomsLayouts
{
    public const string ModelsRoot = "Assets/Models/";
    public const string TextureHuida = "Assets/Textures/Huida/corredor_blanco.jpg";

    // Colores de suelo por habitación (extraídos del catálogo del GDD).
    private static readonly Color Olvido   = new Color(0.55f, 0.55f, 0.62f);
    private static readonly Color Castigo  = new Color(0.45f, 0.22f, 0.22f);
    private static readonly Color Generador= new Color(0.5f, 0.48f, 0.28f);
    private static readonly Color Ritual   = new Color(0.4f, 0.28f, 0.5f);
    private static readonly Color Reliquia = new Color(0.55f, 0.45f, 0.28f);
    private static readonly Color Caceria  = new Color(0.3f, 0.42f, 0.28f);
    private static readonly Color Invest   = new Color(0.42f, 0.42f, 0.55f);
    private static readonly Color Almacen  = new Color(0.5f, 0.4f, 0.28f);
    private static readonly Color Corrup   = new Color(0.3f, 0.38f, 0.3f);
    private static readonly Color HuidaCol = new Color(0.4f, 0.55f, 0.6f);

    // Colores de los props de respaldo (carpintería, metal, etc.)
    private static readonly Color Madera  = new Color(0.45f, 0.3f, 0.2f);
    private static readonly Color Madera2 = new Color(0.5f, 0.38f, 0.26f);
    private static readonly Color Metal   = new Color(0.42f, 0.4f, 0.38f);
    private static readonly Color Negro   = new Color(0.12f, 0.12f, 0.15f);

    public static RoomLayout[] All()
    {
        return new RoomLayout[]
        {
            Habitacion1_Olvido(),
            Habitacion2_Castigo(),
            Habitacion3_Generador(),
            Habitacion4_Ritual(),
            Habitacion5_Reliquia(),
            Habitacion6_Caceria(),
            Habitacion7_Investigacion(),
            Habitacion8_Almacen(),
            Habitacion9_Corrupcion(),
            Habitacion10_Huida(),
        };
    }

    // ---------------------------------------------------------------- 1 - El Olvido
    private static RoomLayout Habitacion1_Olvido()
    {
        var r = new RoomLayout
        {
            name = "1 - El Olvido",
            folderName = "Habitacion_1_El_Olvido",
            floorColor = Olvido,
        };
        r.furniture.AddRange(new[]
        {
            // Reloj de pared real (reloj2 = reloj de pie, RelojPared)
            new FurnitureDef("Espejo_embanado", new Vector3(0f, 1.8f, -6.3f),
                ModelsRoot + "GLB/Olvido/ornate_mirror_01_4k.glb",
                new Vector3(0f, 180f, 0f)),
            new FurnitureDef("Reloj_de_pie", new Vector3(-4.8f, 0f, -6f),
                ModelsRoot + "GLB/Olvido/vintage_grandfather_clock_01_4k.glb",
                new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f), PrimitiveType.Cube, new Vector3(1f, 2f, 0.6f), Madera2, "RelojPared"),
            new FurnitureDef("Mesa_de_madera", new Vector3(0f, 0f, -3.5f),
                ModelsRoot + "GLB/Base/dining_table_4k.glb"),
            new FurnitureDef("Reloj_sobremesa_1", new Vector3(-0.9f, 0.85f, -3.5f),
                ModelsRoot + "GLB/Olvido/alarm_clock_01_4k.glb", Vector3.zero, new Vector3(0.8f, 0.8f, 0.8f)),
            new FurnitureDef("Reloj_sobremesa_2", new Vector3(1.2f, 1.1f, -3.5f),
                ModelsRoot + "GLB/Olvido/mantel_clock_01_4k.glb", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Sillon", new Vector3(4.5f, 0f, 1.5f),
                ModelsRoot + "GLB/Base/mid_century_lounge_chair_4k.glb", new Vector3(0f, -90f, 0f)),
            new FurnitureDef("Alfombra", new Vector3(0f, 0.05f, 0.8f),
                ModelsRoot + "Base/Alfombra/scene.gltf"),
            new FurnitureDef("Televisor_CRT", new Vector3(-4.8f, 0.4f, -3.5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1.2f, 0.8f, 0.7f), Negro, "Televisor"),
        });
        return r;
    }

    // ---------------------------------------------------------------- 2 - El Castigo
    private static RoomLayout Habitacion2_Castigo()
    {
        var r = new RoomLayout
        {
            name = "2 - El Castigo",
            folderName = "Habitacion_2_El_Castigo",
            floorColor = Castigo,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Cadenas_colgantes", new Vector3(0f, 2.4f, -5.5f),
                ModelsRoot + "Castigo/Castigo_cadenas/scene.gltf", Vector3.zero, new Vector3(1.2f, 1.2f, 1.2f), PrimitiveType.Capsule, new Vector3(0.4f, 2.2f, 0.4f), Metal, "Cadenas"),
            new FurnitureDef("Hacha", new Vector3(-4.5f, 1.7f, -6.3f),
                ModelsRoot + "GLB/Castigo/wooden_axe_03_4k.glb",
                new Vector3(0f, 180f, 0f), new Vector3(0.8f, 0.8f, 0.8f)),
            new FurnitureDef("Llave_mordaza", new Vector3(4.8f, 1.6f, -6.3f),
                ModelsRoot + "GLB/Castigo/tongue_groove_pliers_4k.glb",
                new Vector3(0f, 180f, 0f), new Vector3(1.2f, 1.2f, 1.2f)),
            new FurnitureDef("Grilletes", new Vector3(2f, 1.2f, -6.3f), "",
                new Vector3(0f, 180f, 0f), Vector3.one, PrimitiveType.Sphere, new Vector3(0.6f, 0.6f, 0.6f), Metal, "Grilletes"),
            new FurnitureDef("Mesa_de_trabajo", new Vector3(-1f, 0f, -4f),
                ModelsRoot + "GLB/Investigacion/metal_office_desk_4k.glb", new Vector3(0f, 180f, 0f)),
            new FurnitureDef("Yunque", new Vector3(-4.5f, 0.35f, -3f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1f, 0.7f, 0.6f), Metal),
        });
        return r;
    }

    // ---------------------------------------------------------------- 3 - El Generador
    private static RoomLayout Habitacion3_Generador()
    {
        var r = new RoomLayout
        {
            name = "3 - El Generador",
            folderName = "Habitacion_3_El_Generador",
            floorColor = Generador,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Generador_portatil", new Vector3(0f, 0f, -4f),
                ModelsRoot + "GLB/Generador/portable_generator_4k.glb", Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1.1f, 0.8f, 0.7f), new Color(0.45f, 0.45f, 0.2f), "Generador"),
            new FurnitureDef("Caja_energia", new Vector3(-4.5f, 1.4f, -6.3f),
                ModelsRoot + "GLB/Generador/utility_box_02_4k.glb",
                new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f), PrimitiveType.Cube, new Vector3(0.8f, 1.3f, 0.3f), Metal, "Electricidad"),
            new FurnitureDef("Palanca_emergencia", new Vector3(4.5f, 1.7f, -6.3f),
                ModelsRoot + "GLB/Generador/fire_alarm_4k.glb",
                new Vector3(0f, 180f, 0f), new Vector3(0.8f, 0.8f, 0.8f), PrimitiveType.Cube, new Vector3(0.5f, 0.5f, 0.2f), new Color(0.7f, 0.15f, 0.1f), "Electricidad"),
            new FurnitureDef("Tuberias_engranajes", new Vector3(0f, 1.6f, 0.5f),
                ModelsRoot + "GLB/Generador/modular_pipes_4k.glb", Vector3.zero, new Vector3(0.8f, 0.8f, 0.8f)),
            new FurnitureDef("Bombilla_colgante", new Vector3(0f, 3f, 0f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Sphere, new Vector3(0.4f, 0.4f, 0.4f), new Color(0.95f, 0.9f, 0.6f), "Bombilla"),
            new FurnitureDef("Cables", new Vector3(-3f, 0.4f, -4f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cylinder, new Vector3(1.2f, 0.15f, 1.2f), Negro),
        });
        return r;
    }

    // ---------------------------------------------------------------- 4 - El Ritual
    private static RoomLayout Habitacion4_Ritual()
    {
        var r = new RoomLayout
        {
            name = "4 - El Ritual",
            folderName = "Habitacion_4_El_Ritual",
            floorColor = Ritual,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Altar", new Vector3(0f, 0f, -5f),
                ModelsRoot + "Ritual/Altar_cabeza/scene.gltf", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Candelabros_velas", new Vector3(-2f, 0f, -4f),
                ModelsRoot + "GLB/Ritual/brass_candleholders_4k.glb", Vector3.zero, new Vector3(0.8f, 0.8f, 0.8f), PrimitiveType.Cylinder, new Vector3(0.4f, 1f, 0.4f), new Color(0.95f, 0.85f, 0.6f), "Velas"),
            new FurnitureDef("Copas_ceremoniales", new Vector3(1.5f, 0.35f, -4.5f),
                ModelsRoot + "GLB/Cocina/brass_goblets_4k.glb", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Vasija", new Vector3(2.5f, 0.5f, -3.5f),
                ModelsRoot + "GLB/Cocina/ceramic_pot_4k.glb", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Pentagrama", new Vector3(0f, 0.05f, 0f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(2.5f, 0.05f, 2.5f), new Color(0.55f, 0.4f, 0.65f)),
            new FurnitureDef("Servicio_te", new Vector3(-4f, 0.35f, -2.5f),
                ModelsRoot + "GLB/Cocina/tea_set_01_4k.glb", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
        });
        return r;
    }

    // ---------------------------------------------------------------- 5 - La Reliquia
    private static RoomLayout Habitacion5_Reliquia()
    {
        var r = new RoomLayout
        {
            name = "5 - La Reliquia",
            folderName = "Habitacion_5_La_Reliquia",
            floorColor = Reliquia,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Estatua_gotica", new Vector3(0f, 0f, -5f),
                ModelsRoot + "GLB/Reliquia/gothic_statue_4k.glb", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f), PrimitiveType.Cube, new Vector3(0.8f, 1.6f, 0.8f), new Color(0.7f, 0.6f, 0.4f), "Reliquia"),
            new FurnitureDef("Vitrina_estante", new Vector3(-4.5f, 1f, -4f),
                ModelsRoot + "GLB/Reliquia/worn_metal_rack_4k.glb", new Vector3(0f, 45f, 0f), new Vector3(0.9f, 0.9f, 0.9f), PrimitiveType.Cube, new Vector3(1.2f, 1.6f, 0.5f), Metal),
            new FurnitureDef("Libros_antiguos", new Vector3(3f, 0.2f, -4.5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1.2f, 0.4f, 0.8f), Madera2),
            new FurnitureDef("Busto", new Vector3(4.5f, 0.5f, -5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Sphere, new Vector3(0.5f, 0.6f, 0.5f), new Color(0.82f, 0.8f, 0.75f)),
        });
        return r;
    }

    // ---------------------------------------------------------------- 6 - La Cacería
    private static RoomLayout Habitacion6_Caceria()
    {
        var r = new RoomLayout
        {
            name = "6 - La Cacería",
            folderName = "Habitacion_6_La_Caceria",
            floorColor = Caceria,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Cuadro_grande", new Vector3(-5.5f, 1.8f, -6.3f),
                ModelsRoot + "GLB/Caceria/fancy_picture_frame_01_4k.glb", new Vector3(0f, 180f, 0f)),
            new FurnitureDef("Cuadro_caballero_1", new Vector3(-3.2f, 0.9f, -5.8f),
                ModelsRoot + "GLB/Caceria/standing_picture_frame_02_4k.glb", Vector3.zero),
            new FurnitureDef("Cuadro_caballero_2", new Vector3(3.2f, 0.9f, -5.8f),
                ModelsRoot + "GLB/Caceria/standing_picture_frame_01_4k.glb", Vector3.zero),
            new FurnitureDef("Cuadro_colgado", new Vector3(6.3f, 1.8f, 0f),
                ModelsRoot + "GLB/Caceria/hanging_picture_frame_02_4k.glb", new Vector3(0f, -90f, 0f)),
            new FurnitureDef("Trofeo_toro", new Vector3(-4.2f, 1.8f, -6.3f),
                ModelsRoot + "GLB/Caceria/bull_head_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Trofeo_caballo", new Vector3(4.8f, 1.8f, -6.3f),
                ModelsRoot + "GLB/Caceria/horse_head_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Trofeo_leon", new Vector3(1.8f, 1.8f, -6.3f),
                ModelsRoot + "GLB/Caceria/lion_head_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.8f, 0.8f, 0.8f)),
            new FurnitureDef("Estatua_ballena", new Vector3(-6f, 0.6f, -1.5f),
                ModelsRoot + "GLB/Caceria/bronze_whale_statue_4k.glb", Vector3.zero, new Vector3(0.8f, 0.8f, 0.8f)),
            new FurnitureDef("Estatua_tiburon", new Vector3(-6f, 0.6f, 2f),
                ModelsRoot + "GLB/Caceria/bronze_shark_statue_4k.glb", Vector3.zero, new Vector3(0.8f, 0.8f, 0.8f)),
            new FurnitureDef("Cuerno_caza", new Vector3(6.3f, 1.5f, -3f), "",
                new Vector3(0f, 90f, 0f), Vector3.one, PrimitiveType.Cylinder, new Vector3(0.8f, 0.9f, 0.5f), Madera, "CuernoCaza"),
        });
        return r;
    }

    // ---------------------------------------------------------------- 7 - La Investigación
    private static RoomLayout Habitacion7_Investigacion()
    {
        var r = new RoomLayout
        {
            name = "7 - La Investigación",
            folderName = "Habitacion_7_La_Investigacion",
            floorColor = Invest,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Escritorio", new Vector3(0f, 0f, -4f),
                ModelsRoot + "Investigacion/Escritorio_mas_papeles/scene.gltf", new Vector3(0f, 180f, 0f), Vector3.one, PrimitiveType.Cube, new Vector3(1.5f, 0.1f, 0.9f), Madera2),
            new FurnitureDef("Maquina_escribir", new Vector3(-0.7f, 0.8f, -4f), "",
                new Vector3(0f, 180f, 0f), Vector3.one, PrimitiveType.Cube, new Vector3(0.8f, 0.45f, 0.6f), Negro, "MaquinaEscribir"),
            new FurnitureDef("Telefono", new Vector3(0.9f, 0.8f, -4f), "",
                new Vector3(0f, 180f, 0f), Vector3.one, PrimitiveType.Sphere, new Vector3(0.6f, 0.5f, 0.6f), Negro, "Telefono"),
            new FurnitureDef("Archivadores", new Vector3(-4.8f, 0f, -2.5f),
                ModelsRoot + "GLB/Investigacion/book_encyclopedia_set_01_4k.glb", Vector3.zero, new Vector3(1.1f, 1.1f, 1.1f)),
            new FurnitureDef("Mesa_metal", new Vector3(3.5f, 0f, -1f),
                ModelsRoot + "GLB/Investigacion/metal_office_desk_4k.glb"),
            new FurnitureDef("Pizarra", new Vector3(4.5f, 1.6f, -6.3f), "",
                new Vector3(0f, 180f, 0f), Vector3.one, PrimitiveType.Cube, new Vector3(1.3f, 1f, 0.1f), new Color(0.75f, 0.78f, 0.85f)),
            new FurnitureDef("Silla_lectura", new Vector3(-2f, 0f, 1f),
                ModelsRoot + "GLB/Base/metal_stool_03_4k.glb", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
        });
        return r;
    }

    // ---------------------------------------------------------------- 8 - El Almacén
    private static RoomLayout Habitacion8_Almacen()
    {
        var r = new RoomLayout
        {
            name = "8 - El Almacén",
            folderName = "Habitacion_8_El_Almacen",
            floorColor = Almacen,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Pack_almacen", new Vector3(0f, 0f, 0f),
                ModelsRoot + "Almacen/scene.gltf", Vector3.zero, new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Nevera", new Vector3(-4.5f, 0.95f, -5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1f, 1.9f, 0.9f), new Color(0.8f, 0.82f, 0.85f), "Nevera"),
            new FurnitureDef("Ventilador", new Vector3(4.5f, 0.55f, -4.5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cylinder, new Vector3(0.5f, 1.1f, 0.5f), Metal, "Ventilador"),
            new FurnitureDef("Caja_madera_1", new Vector3(-2f, 0.35f, -5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(0.9f, 0.7f, 0.9f), Madera),
            new FurnitureDef("Caja_madera_2", new Vector3(-2.6f, 0.35f, -3.5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(0.9f, 0.7f, 0.9f), Madera),
            new FurnitureDef("Barril", new Vector3(2f, 0.5f, -5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cylinder, new Vector3(0.7f, 1f, 0.7f), new Color(0.4f, 0.36f, 0.3f)),
            new FurnitureDef("Estanteria", new Vector3(3.5f, 0.8f, -5f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1.2f, 1.6f, 0.5f), Metal),
        });
        return r;
    }

    // ---------------------------------------------------------------- 9 - La Corrupción
    private static RoomLayout Habitacion9_Corrupcion()
    {
        var r = new RoomLayout
        {
            name = "9 - La Corrupción",
            folderName = "Habitacion_9_La_Corrupcion",
            floorColor = Corrup,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Armario_gotico", new Vector3(-5f, 0f, -5f),
                ModelsRoot + "GLB/Base/GothicCabinet_01_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Sillon", new Vector3(4.5f, 0f, 1.5f),
                ModelsRoot + "GLB/Base/mid_century_lounge_chair_4k.glb", new Vector3(0f, -90f, 0f)),
            new FurnitureDef("Sofa", new Vector3(-1f, 0f, 1f),
                ModelsRoot + "GLB/Base/sofa_02_4k.glb", new Vector3(0f, -90f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Carne_colgando", new Vector3(2f, 1.8f, -6f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Capsule, new Vector3(0.4f, 1.6f, 0.4f), new Color(0.55f, 0.25f, 0.25f), "Moscas"),
            new FurnitureDef("Goteo_oscuro", new Vector3(4.5f, 1.7f, -6.2f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Capsule, new Vector3(0.3f, 1.4f, 0.3f), new Color(0.15f, 0.2f, 0.25f), "Goteo"),
            new FurnitureDef("Mancha_negra", new Vector3(0f, 0.05f, 2f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1.4f, 0.02f, 1.4f), Negro),
            new FurnitureDef("Huesos", new Vector3(0.5f, 0.2f, -4f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cylinder, new Vector3(0.6f, 0.4f, 0.6f), new Color(0.8f, 0.8f, 0.75f)),
        });
        return r;
    }

    // ---------------------------------------------------------------- 10 - La Huida
    private static RoomLayout Habitacion10_Huida()
    {
        var r = new RoomLayout
        {
            name = "10 - La Huida",
            folderName = "Habitacion_10_La_Huida",
            floorColor = HuidaCol,
            wallTexturePath = TextureHuida,
        };
        r.furniture.AddRange(new[]
        {
            new FurnitureDef("Puerta_salida", new Vector3(0f, 1.1f, 6.7f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(1.4f, 2.2f, 0.3f), Madera2, "Viento"),
            new FurnitureDef("Banco", new Vector3(-4.5f, 0f, -4f),
                ModelsRoot + "GLB/Base/sofa_02_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Mesa_central", new Vector3(3f, 0f, -3f),
                ModelsRoot + "GLB/Base/dining_table_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Farola", new Vector3(-1f, 0f, -3f),
                ModelsRoot + "GLB/Base/street_lamp_02_4k.glb", Vector3.zero, new Vector3(0.7f, 0.7f, 0.7f)),
            new FurnitureDef("Armario", new Vector3(5f, 0f, -5f),
                ModelsRoot + "GLB/Base/GothicCabinet_01_4k.glb", new Vector3(0f, 180f, 0f), new Vector3(0.9f, 0.9f, 0.9f)),
            new FurnitureDef("Linterna", new Vector3(0.5f, 1.2f, -6.3f), "",
                new Vector3(0f, 180f, 0f), Vector3.one, PrimitiveType.Cylinder, new Vector3(0.3f, 0.6f, 0.3f), new Color(0.85f, 0.7f, 0.35f)),
            new FurnitureDef("Maleta", new Vector3(1.5f, 0.3f, -4f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cube, new Vector3(0.9f, 0.5f, 0.6f), new Color(0.4f, 0.32f, 0.25f)),
            new FurnitureDef("Escalera", new Vector3(-6f, 0.8f, -6f), "",
                Vector3.zero, Vector3.one, PrimitiveType.Cylinder, new Vector3(1.1f, 2f, 0.5f), Madera, "EscaleraMadera"),
        });
        return r;
    }
}