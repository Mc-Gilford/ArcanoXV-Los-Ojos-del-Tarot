using UnityEngine;

[System.Serializable]
public class CardDef
{
    [Header("Identidad")]
    public string nombre;

    [TextArea]
    public string poderDesc;

    [TextArea]
    public string maldicionDesc;

    public Color color = Color.white;

    [Header("Imagen")]
    public Sprite imagen;


    [Header("Poder (temporal)")]
    public float velocidadMult = 1f;
    public float duracionPoder = 12f;


    [Header("Maldición (temporal)")]
    public float maldicionVelocidadMult = 1f;
    public bool maldicionSinSprint;
    public float duracionMaldicion = 12f;


    public static CardDef[] Defaults()
    {
        return new CardDef[]
        {
            new CardDef
            {
                nombre = "Inversion Espectral",
                color = new Color(0.15f, 0.55f, 0.25f),

                poderDesc = "Eres inmune al daño",
                maldicionDesc = "Tus controles se invierten",

                velocidadMult = 1.6f,
                duracionPoder = 12f,

                maldicionVelocidadMult = 1f,
                maldicionSinSprint = true,
                duracionMaldicion = 10f
            },

            new CardDef
            {
                nombre = "Destruccion Espectral",
                color = new Color(0.20f, 0.45f, 0.85f),

                poderDesc = "Destruyes a los enemigos de la sala",
                maldicionDesc = "Pierdes la mitad de tu vida",

                velocidadMult = 1.25f,
                duracionPoder = 12f,

                maldicionVelocidadMult = 0.7f,
                maldicionSinSprint = false,
                duracionMaldicion = 1f
            },

            new CardDef
            {
                nombre = "Corazon Guia",
                color = new Color(0.92f, 0.92f, 0.92f),

                poderDesc = "Teletransporta al jugador a la zona segura",
                maldicionDesc = "Pierdes todas las municiones que no esten dentro del cargador de tu arma",

                velocidadMult = 2f,
                duracionPoder = 8f,

                maldicionVelocidadMult = 0.5f,
                maldicionSinSprint = true,
                duracionMaldicion = 1f
            }
        };
    }
}