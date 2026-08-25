using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Punto ÚNICO de tipografía del juego (todas las habitaciones y escenas).
///
/// COMO CAMBIAR LA FUENTE DE TODO EL JUEGO:
///   1. Arrastra tu archivo .ttf a:  Assets/Resources/Fuentes/
///   2. Renómbralo exactamente:      principal.ttf
///   Listo. Todos los textos que usen FuentesJuego.Principal cambian solos.
///   Si no existe ese archivo, se usa la fuente por defecto de Unity (LegacyRuntime).
///
/// Los scripts deben pedir SIEMPRE:   FuentesJuego.Principal
/// </summary>
public static class FuentesJuego
{
    private const string RutaFuentePropia = "Fuentes/principal";
    private static Font _cache;

    /// <summary>Dorado cálido oficial del HUD/cartas (#F4C95D).</summary>
    public static readonly Color Dorado = new Color(0.957f, 0.788f, 0.365f);

    /// <summary>Blanco cálido suave para texto de lectura secundario.</summary>
    public static readonly Color TextoSecundario = new Color(0.9f, 0.85f, 0.75f);

    /// <summary>Fuente principal del juego (con caché).</summary>
    public static Font Principal
    {
        get
        {
            if (_cache == null)
            {
                _cache = Resources.Load<Font>(RutaFuentePropia);
                if (_cache == null)
                    _cache = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            return _cache;
        }
    }

    /// <summary>Fuerza recargar la fuente (útil si reemplazan el ttf con Unity abierto).</summary>
    public static void Refrescar()
    {
        _cache = null;
    }

    /// <summary>Aplica la fuente principal y formato estándar a un texto UGUI.</summary>
    public static void Aplicar(Text texto, int tamano, Color? color = null, bool negrita = true, bool estiloArcano = false)
    {
        if (texto == null) return;
        texto.font = Principal;
        texto.fontSize = tamano;
        texto.fontStyle = negrita ? FontStyle.Bold : FontStyle.Normal;
        if (color.HasValue) texto.color = color.Value;

        if (estiloArcano)
        {
            Outline contorno = texto.GetComponent<Outline>();
            if (contorno == null) contorno = texto.gameObject.AddComponent<Outline>();
            contorno.effectColor = new Color(0f, 0f, 0f, 0.85f);
            contorno.effectDistance = new Vector2(2f, -2f);

            Shadow sombra = texto.GetComponent<Shadow>();
            if (sombra == null) sombra = texto.gameObject.AddComponent<Shadow>();
            sombra.effectColor = new Color(0f, 0f, 0f, 0.6f);
            sombra.effectDistance = new Vector2(3f, -3f);
        }
    }

    /// <summary>Aplica la fuente principal y formato estándar a un TextMesh (texto 3D en el mundo).</summary>
    public static void Aplicar(TextMesh texto, int tamano, Color? color = null, bool negrita = true)
    {
        if (texto == null) return;
        texto.font = Principal;
        texto.fontSize = tamano;
        texto.fontStyle = negrita ? FontStyle.Bold : FontStyle.Normal;
        if (color.HasValue) texto.color = color.Value;
    }
}
