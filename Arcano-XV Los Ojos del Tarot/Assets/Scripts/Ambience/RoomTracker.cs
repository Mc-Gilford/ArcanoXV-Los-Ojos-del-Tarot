using System;
using UnityEngine;

/// <summary>
/// Rastrea en qué habitación (RoomTriggerZone) está el jugador actualmente.
/// Debe haber una sola instancia en la escena (singleton).
/// </summary>
public class RoomTracker : MonoBehaviour
{
    public static RoomTracker Instance { get; private set; }

    [Tooltip("Si el jugador no lleva la tag 'Player', asígnalo aquí.")]
    public Transform playerOverride;

    [Header("Detección")]
    [Tooltip("Tag con la que se detecta al jugador en los triggers.")]
    public string playerTag = "Player";

    /// <summary>Zona (habitación) donde está el jugador. null si está fuera de todas.</summary>
    public RoomTriggerZone CurrentRoom { get; private set; }

    /// <summary>Transform raíz del jugador actual, si se conoce.</summary>
    public Transform Player
    {
        get
        {
            if (playerOverride != null) return playerOverride;
            return _cachedPlayer;
        }
    }

    public event Action<RoomTriggerZone> PlayerEnteredRoom;
    public event Action<RoomTriggerZone> PlayerExitedRoom;

    private Transform _cachedPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void RegisterPlayerEnter(RoomTriggerZone zone, Transform player)
    {
        if (player != null) _cachedPlayer = player.root;
        if (CurrentRoom == zone) return;

        RoomTriggerZone previous = CurrentRoom;
        CurrentRoom = zone;
        PlayerEnteredRoom?.Invoke(zone);
        if (previous != null)
            PlayerExitedRoom?.Invoke(previous);
    }

    public void RegisterPlayerExit(RoomTriggerZone zone)
    {
        if (CurrentRoom != zone) return;
        CurrentRoom = null;
        PlayerExitedRoom?.Invoke(zone);
    }

    public bool IsPlayerInRoom(RoomTriggerZone zone) => CurrentRoom == zone;
}