using UnityEngine;

public class RoomTimer : MonoBehaviour
{
    public static RoomTimer Instance { get; private set; }

    [Tooltip("Tiempo que debe permanecer el jugador dentro de la habitación.")]
    public float tiempoNecesario = 240f;

    public float TiempoDentro { get; private set; }

    public bool TiempoCompletado => TiempoDentro >= tiempoNecesario;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (TiempoCompletado)
            return;

        TiempoDentro += Time.deltaTime;

        if (TiempoDentro >= tiempoNecesario)
        {
            TiempoDentro = tiempoNecesario;
            Debug.Log("[H13] El jugador completó los 2 minutos dentro de la habitación.");
        }
    }
}