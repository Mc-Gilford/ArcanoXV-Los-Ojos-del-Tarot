using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Catálogo de objetos por habitación, según el GDD (10 habitaciones temáticas).
///
/// Cada objeto tiene una categoría de audio: si está vacía, el objeto es
/// SILENCIOSO (mesa, silla, caja...). Si no está vacía, el sistema buscará el clip
/// en Assets/Audio/Objetos/<audioCategory>/ y lo reproducirá con ObjectAmbience
/// (sonido 3D: sube de volumen conforme te acercas).
///
/// La "habitación trofeo" (Assets/Scripts/Editor/TrophyRoomGenerator.cs) coloca un
/// trofeo de cada objeto de ESTA lista, de modo que al entrar tengas todos los
/// objetos que usarán las demás habitaciones.
/// </summary>
public static class RoomObjectCatalog
{
    public const string AudioRoot = "Assets/Audio/Objetos";
    public const string PasosFolder = "Assets/Audio/Pasos";

    public sealed class ObjectDef
    {
        public string name = "";
        public PrimitiveType shape = PrimitiveType.Cube;
        public Vector3 scale = Vector3.one;
        public Color color = Color.gray;
        public string audioCategory = ""; // "" = silencioso
        public string soundHint = "";     // qué sonido buscar (para documentación)
    }

    public sealed class RoomDef
    {
        public string name = "";
        public Color roomColor = Color.gray;
        public List<ObjectDef> objects = new List<ObjectDef>();
    }

    private static ObjectDef Obj(string name, PrimitiveType shape, Vector3 scale, Color color,
        string audioCategory = "", string soundHint = "")
    {
        return new ObjectDef { name = name, shape = shape, scale = scale, color = color, audioCategory = audioCategory, soundHint = soundHint };
    }

    public static RoomDef[] Rooms => new RoomDef[]
    {
        new RoomDef
        {
            name = "1 - El Olvido",
            roomColor = new Color(0.70f, 0.70f, 0.78f),
            objects = new List<ObjectDef>
            {
                Obj("Televisor CRT viejo", PrimitiveType.Cube, new Vector3(1.2f, 0.8f, 0.7f), new Color(0.25f,0.25f,0.28f), "Televisor", "estática/pantalla encendida"),
                Obj("Reloj de pared", PrimitiveType.Cylinder, new Vector3(1f, 1f, .9f), new Color(0.55f,0.4f,0.25f), "RelojPared", "tic-tac"),
                Obj("Espejo empañado", PrimitiveType.Cube, new Vector3(1f, 1.4f, 0.15f), new Color(0.6f,0.75f,0.8f), "", ""),
                Obj("Mesa de madera", PrimitiveType.Cube, new Vector3(1.6f, 0.1f, 1f), new Color(0.45f,0.3f,0.2f), "", ""),
                Obj("Vitrina polvorienta", PrimitiveType.Cube, new Vector3(1.1f, 1.5f, 0.5f), new Color(0.5f,0.42f,0.32f), "", ""),
            }
        },
        new RoomDef
        {
            name = "2 - El Castigo",
            roomColor = new Color(0.62f, 0.3f, 0.3f),
            objects = new List<ObjectDef>
            {
                Obj("Cadenas colgantes", PrimitiveType.Capsule, new Vector3(.4f, 2.2f, .4f), new Color(0.4f,0.4f,0.45f), "Cadenas", "golpes de metal"),
                Obj("Grilletes en pared", PrimitiveType.Sphere, new Vector3(.7f,.7f,.7f), new Color(0.45f,0.45f,0.5f), "Grilletes", "metal que choca"),
                Obj("Yunque de fragua", PrimitiveType.Cube, new Vector3(1f,.7f,.6f), new Color(0.35f,0.35f,0.4f), "", ""),
                Obj("Mesa de trabajo", PrimitiveType.Cube, new Vector3(1.4f,.1f,.8f), new Color(0.5f,0.28f,0.2f), "", ""),
            }
        },
        new RoomDef
        {
            name = "3 - El Generador",
            roomColor = new Color(0.72f, 0.7f, 0.4f),
            objects = new List<ObjectDef>
            {
                Obj("Generador", PrimitiveType.Cube, new Vector3(1.1f, .8f, .7f), new Color(0.45f,0.45f,0.2f), "Generador", "zumbido de motor"),
                Obj("Panel eléctrico", PrimitiveType.Cube, new Vector3(.8f, 1.3f, .3f), new Color(0.45f,0.42f,0.3f), "Electricidad", "zumbido eléctrico"),
                Obj("Bombilla colgante", PrimitiveType.Sphere, new Vector3(.5f,.5f,.5f), new Color(0.95f,0.9f,0.6f), "Bombilla", "zumbido/parpadeo"),
                Obj("Cables enredados", PrimitiveType.Cylinder, new Vector3(1.2f,.15f,1.2f), new Color(0.2f,0.2f,0.2f), "", ""),
            }
        },
        new RoomDef
        {
            name = "4 - El Ritual",
            roomColor = new Color(0.55f, 0.4f, 0.65f),
            objects = new List<ObjectDef>
            {
                Obj("Altar de ritual", PrimitiveType.Cube, new Vector3(1.4f,.15f,.9f), new Color(0.35f,0.25f,0.4f), "", ""),
                Obj("Velas encendidas", PrimitiveType.Cylinder, new Vector3(.4f,1f,.4f), new Color(0.95f,0.85f,0.6f), "Velas", "crepitar de llama"),
                Obj("Pentagrama en el suelo", PrimitiveType.Cube, new Vector3(1.2f,.02f,1.2f), new Color(0.3f,0.25f,0.35f), "", ""),
                Obj("Copas ceremoniales", PrimitiveType.Sphere, new Vector3(.5f,.4f,.5f), new Color(0.55f,0.5f,0.4f), "", ""),
            }
        },
        new RoomDef
        {
            name = "5 - La Reliquia",
            roomColor = new Color(0.7f, 0.6f, 0.4f),
            objects = new List<ObjectDef>
            {
                Obj("Relicario maldito", PrimitiveType.Sphere, new Vector3(.6f,.6f,.6f), new Color(0.8f,0.7f,0.3f), "Reliquia", "zumbido ominoso bajo"),
                Obj("Vitrina de cristal", PrimitiveType.Cube, new Vector3(1f,1.5f,.7f), new Color(0.55f,0.7f,0.75f), "", ""),
                Obj("Libros antiguos", PrimitiveType.Cube, new Vector3(.9f,.4f,.7f), new Color(0.4f,0.3f,0.2f), "", ""),
            }
        },
        new RoomDef
        {
            name = "6 - La Cacería",
            roomColor = new Color(0.45f, 0.6f, 0.4f),
            objects = new List<ObjectDef>
            {
                Obj("Cuerno de caza", PrimitiveType.Cylinder, new Vector3(.8f,.9f,.5f), new Color(0.75f,0.65f,0.4f), "CuernoCaza", "cuerno lejano"),
                Obj("Muro de trofeos", PrimitiveType.Cube, new Vector3(1.5f,1.2f,.4f), new Color(0.5f,0.45f,0.35f), "", ""),
                Obj("Escopeta de caza", PrimitiveType.Cylinder, new Vector3(1.3f,.15f,.15f), new Color(0.3f,0.28f,0.26f), "", ""),
                Obj("Trampa de oso", PrimitiveType.Sphere, new Vector3(.6f,.4f,.6f), new Color(0.4f,0.4f,0.45f), "", ""),
            }
        },
        new RoomDef
        {
            name = "7 - La Investigación",
            roomColor = new Color(0.6f, 0.6f, 0.75f),
            objects = new List<ObjectDef>
            {
                Obj("Máquina de escribir", PrimitiveType.Cube, new Vector3(.8f,.45f,.6f), new Color(0.25f,0.25f,0.28f), "MaquinaEscribir", "tecleo"),
                Obj("Teléfono de disco", PrimitiveType.Sphere, new Vector3(.6f,.5f,.6f), new Color(0.3f,0.28f,0.26f), "Telefono", "timbre antiguo"),
                Obj("Escritorio con notas", PrimitiveType.Cube, new Vector3(1.5f,.1f,.9f), new Color(0.5f,0.38f,0.26f), "", ""),
                Obj("Pizarra de casos", PrimitiveType.Cube, new Vector3(1.3f,1f,.1f), new Color(0.75f,0.78f,0.85f), "", ""),
            }
        },
        new RoomDef
        {
            name = "8 - El Almacén",
            roomColor = new Color(0.65f, 0.55f, 0.4f),
            objects = new List<ObjectDef>
            {
                Obj("Nevera industrial", PrimitiveType.Cube, new Vector3(1f,1.9f,.9f), new Color(0.8f,0.82f,0.85f), "Nevera", "zumbido de motor"),
                Obj("Ventilador de piso", PrimitiveType.Cylinder, new Vector3(.5f,1.1f,.5f), new Color(0.35f,0.35f,0.4f), "Ventilador", "hélices girando"),
                Obj("Cajas de madera", PrimitiveType.Cube, new Vector3(.9f,.7f,.9f), new Color(0.55f,0.44f,0.3f), "", ""),
                Obj("Barril oxidado", PrimitiveType.Cylinder, new Vector3(.7f,1f,.7f), new Color(0.4f,0.36f,0.3f), "", ""),
                Obj("Estantería metálica", PrimitiveType.Cube, new Vector3(1.2f,1.6f,.5f), new Color(0.5f,0.5f,0.55f), "", ""),
            }
        },
        new RoomDef
        {
            name = "9 - La Corrupción",
            roomColor = new Color(0.45f, 0.55f, 0.45f),
            objects = new List<ObjectDef>
            {
                Obj("Carne colgando", PrimitiveType.Capsule, new Vector3(.4f,1.6f,.4f), new Color(0.55f,0.25f,0.25f), "Moscas", "zumbido de moscas"),
                Obj("Goteo de agua oscura", PrimitiveType.Capsule, new Vector3(.3f,1.4f,.3f), new Color(0.15f,0.2f,0.25f), "Goteo", "gotas cayendo"),
                Obj("Mancha negra", PrimitiveType.Cube, new Vector3(1.4f,.02f,1.4f), new Color(0.08f,0.08f,0.1f), "", ""),
                Obj("Huesos", PrimitiveType.Cylinder, new Vector3(.6f,.4f,.6f), new Color(0.8f,0.8f,0.75f), "", ""),
            }
        },
        new RoomDef
        {
            name = "10 - La Huida",
            roomColor = new Color(0.5f, 0.65f, 0.7f),
            objects = new List<ObjectDef>
            {
                Obj("Puerta de salida", PrimitiveType.Cube, new Vector3(1.2f,2.2f,.3f), new Color(0.5f,0.4f,0.3f), "Viento", "viento/chirrido"),
                Obj("Linterna vieja", PrimitiveType.Cylinder, new Vector3(.3f,.6f,.3f), new Color(0.85f,0.7f,0.35f), "", ""),
                Obj("Maleta abandonada", PrimitiveType.Cube, new Vector3(.9f,.5f,.6f), new Color(0.4f,0.32f,0.25f), "", ""),
                Obj("Escalera de madera", PrimitiveType.Cylinder, new Vector3(1.1f,2f,.5f), new Color(0.5f,0.4f,0.3f), "EscaleraMadera", "(pasos ya cubiertos por PlayerFootsteps)"),
            }
        },
    };

    /// <summary>Devuelve todos los objetos de todas las habitaciones (para la habitación trofeo).</summary>
    public static IEnumerable<ObjectDef> AllObjects()
    {
        foreach (RoomDef room in Rooms)
            foreach (ObjectDef obj in room.objects)
                yield return obj;
    }
}