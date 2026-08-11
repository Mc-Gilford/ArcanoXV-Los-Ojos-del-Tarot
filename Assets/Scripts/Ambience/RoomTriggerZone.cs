using UnityEngine;

/// <summary>
/// Volumen invisible de una habitación. Detecta la entrada/salida del jugador
/// y lo reporta al RoomTracker.
/// </summary>
[RequireComponent(typeof(Collider))]
public class RoomTriggerZone : MonoBehaviour
{
    [Tooltip("Nombre lógico de la habitación (ej: 'El Ritual', 'La Cacería').")]
    public string roomName = "Habitación";

    [Tooltip("Tag con la que se detecta al jugador.")]
    public string playerTag = "Player";

    /// <summary>Jugador detectado dentro, si lo hay.</summary>
    public Transform CachedPlayer { get; private set; }

    private bool _inside;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        Transform root = other.transform.root;
        CachedPlayer = root;

        if (_inside) return;
        _inside = true;

        if (RoomTracker.Instance != null)
            RoomTracker.Instance.RegisterPlayerEnter(this, root);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        CachedPlayer = null;

        if (!_inside) return;
        _inside = false;

        if (RoomTracker.Instance != null)
            RoomTracker.Instance.RegisterPlayerExit(this);
    }

    private bool IsPlayer(Collider other)
    {
        if (other.CompareTag(playerTag)) return true;

        RoomTracker tracker = RoomTracker.Instance;
        if (tracker != null && tracker.playerOverride != null)
            return other.transform.root == tracker.playerOverride.root;

        return false;
    }

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnDrawGizmosSelected()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.35f);
        Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        Gizmos.color = new Color(0.2f, 1f, 0.4f, 0.9f);
        Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}