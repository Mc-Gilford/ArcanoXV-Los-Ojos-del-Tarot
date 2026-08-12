using UnityEngine;

/// <summary>
/// Datos de una carta del tarot para la habilidad de selección.
/// Cada carta tiene un PODER temporal (beneficio) y una MALDICIÓN temporal
/// (repercusión), ambas de movimiento. La maldición se muestra ANTES como "???"
/// y se revela al elegir la carta.
/// </summary>
[System.Serializable]
public class CardDef
{
    [Header("Identidad")]
    public string nombre;             // ej. "El Carro"
    [TextArea] public string poderDesc;      // lo que se ve ANTES de elegir
    [TextArea] public string maldicionDesc;  // se revela al elegir (panel de resultado)
    public Color color = Color.white;        // verde / azul / blanco

    [Header("Poder (temporal)")]
    public float velocidadMult = 1f;   // 1.6 = +60% velocidad
    public float duracionPoder = 12f;  // segundos del beneficio

    [Header("Maldición (temporal)")]
    public float maldicionVelocidadMult = 1f; // 0.7 = -30%, 0.5 = -50%
    public bool maldicionSinSprint;           // true = pierde el sprint
    public float duracionMaldicion = 12f;     // segundos del castigo

    /// <summary>Las 3 cartas del GDD (valores por defecto).</summary>
    public static CardDef[] Defaults()
    {
        return new CardDef[]
        {
            new CardDef
            {
                nombre = "El Carro",
                color = new Color(0.15f, 0.55f, 0.25f),
                poderDesc = "Vas más rápido (+60% velocidad) durante 12 s.",
                maldicionDesc = "Pierdes el sprint durante 12 s.",
                velocidadMult = 1.6f,
                duracionPoder = 12f,
                maldicionVelocidadMult = 1f,
                maldicionSinSprint = true,
                duracionMaldicion = 12f
            },
            new CardDef
            {
                nombre = "La Estrella",
                color = new Color(0.20f, 0.45f, 0.85f),
                poderDesc = "Avanzas con paso firme (+25% velocidad) durante 12 s.",
                maldicionDesc = "Te ralentiza (-30% velocidad) durante 15 s.",
                velocidadMult = 1.25f,
                duracionPoder = 12f,
                maldicionVelocidadMult = 0.7f,
                maldicionSinSprint = false,
                duracionMaldicion = 15f
            },
            new CardDef
            {
                nombre = "El Ahorcado",
                color = new Color(0.92f, 0.92f, 0.92f),
                poderDesc = "Ráfaga de velocidad (+100%) durante 8 s.",
                maldicionDesc = "Pierdes el sprint y vas lento (-50% velocidad) durante 15 s.",
                velocidadMult = 2f,
                duracionPoder = 8f,
                maldicionVelocidadMult = 0.5f,
                maldicionSinSprint = true,
                duracionMaldicion = 15f
            }
        };
    }
}
