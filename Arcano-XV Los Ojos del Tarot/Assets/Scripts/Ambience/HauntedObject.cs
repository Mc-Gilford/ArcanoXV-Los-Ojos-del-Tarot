using System.Collections;
using UnityEngine;

/// <summary>
/// Objeto embrujado: detecta al jugador y reacciona con sonidos y animaciones aleatorias.
/// - Jugador CERCA del objeto  → reacciones 'near' frecuentes (sonidos + animación).
/// - Jugador en la MISMA habitación pero LEJOS → sonidos inesperados 'far' (sustos).
///
/// Detección de "misma habitación":
///  - Si hay RoomTriggerZone en el objeto (o en su padre... GetComponentInParent) y el
///    RoomTracker del escenario, se compara la zona real.
///  - Si no hay zonas configuradas, se aproxima por distancia (roomRadiusFallback).
/// Lo que permite usar el sistema tanto con "zonas por habitación" como de forma simple.
/// </summary>
public class HauntedObject : MonoBehaviour
{
    [Header("Jugador")]
    [Tooltip("Opcional. Si está vacío, se busca al jugador por tag 'Player' y/o vía RoomTracker.")]
    public Transform playerOverride;
    public string playerTag = "Player";

    [Header("Proximidad")]
    [Tooltip("Distancia a la que el jugador se considera 'cerca'. Dibuja un gizmo naranja en el editor.")]
    public float proximityDistance = 5f;

    [Header("Sonidos")]
    [Tooltip("Clips que suenan cuando el jugador está cerca.")]
    public AudioClip[] nearSounds;
    [Tooltip("Clips que suenan cuando el jugador está lejos pero en la misma habitación (sustos inesperados).")]
    public AudioClip[] farSounds;
    [Tooltip("Activa los sonidos 'lejos' cuando el jugador está en la misma habitación.")]
    public bool enableFarSounds = true;

    [Header("Temporización aleatoria")]
    [Tooltip("Intervalo aleatorio entre reacciones cuando el jugador está cerca.")]
    public float minIntervalNear = 3f;
    public float maxIntervalNear = 8f;
    [Tooltip("Intervalo aleatorio entre sonidos 'lejos'.")]
    public float minIntervalFar = 8f;
    public float maxIntervalFar = 20f;

    [Header("Animaciones")]
    [Tooltip("Animator del objeto (opcional). Se disparan triggers aleatorios.")]
    public Animator animator;
    [Tooltip("Nombres de triggers del Animator a disparar al azar.")]
    public string[] animationTriggers;
    [Tooltip("Si no hay Animator, el objeto hace una sacudida procedural (movimiento de susto).")]
    public bool proceduralShake = true;
    public float shakeMagnitude = 10f;
    public float shakeDuration = 0.35f;

    [Header("Detección sin zonas (fallback)")]
    [Tooltip("Si el objeto no está dentro de ninguna RoomTriggerZone, se asume 'misma habitación' si el jugador está a menos de esta distancia.")]
    public float roomRadiusFallback = 30f;

    [Header("Volumen")]
    [Range(0f, 1f)] public float nearVolume = 1f;
    [Range(0f, 1f)] public float farVolume = 0.55f;

    private AudioSource _source;
    private RoomTriggerZone _zone;
    private Transform _player;
    private Transform _tagPlayer;
    private Coroutine _shake;

    // Estado evaluado cada frame en Update.
    private bool _playerNear;
    private bool _playerSameRoom;

    private void Awake()
    {
        _zone = GetComponentInParent<RoomTriggerZone>();
        _source = GetComponent<AudioSource>();
        if (_source == null)
            _source = gameObject.AddComponent<AudioSource>();

        _source.playOnAwake = false;
        _source.spatialBlend = 1f; // audio 3D: se oye más fuerte cuanto más cerca
        _source.rolloffMode = AudioRolloffMode.Logarithmic;
    }

    private void Start()
    {
        StartCoroutine(NearLoop());
        StartCoroutine(FarLoop());
    }

    private void Update()
    {
        _player = ResolvePlayer();
        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);
        _playerNear = dist <= proximityDistance;
        _playerSameRoom = IsSameRoom(dist);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.6f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, proximityDistance);
    }

    // ---- Lógica ------------------------------------------------------------

    private bool IsSameRoom(float currentDistance)
    {
        RoomTracker tracker = RoomTracker.Instance;

        // Sin tracker o sin habitación actual: aproximación por distancia.
        if (tracker == null || tracker.CurrentRoom == null)
            return _player != null && currentDistance <= roomRadiusFallback;

        // El objeto pertenece a una zona: comparación exacta de zonas.
        if (_zone != null)
            return tracker.CurrentRoom == _zone;

        // Objeto suelto (fuera de cualquier zona): fallback con jugador en alguna habitación.
        return currentDistance <= roomRadiusFallback;
    }

    private IEnumerator NearLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minIntervalNear, maxIntervalNear));
            if (_player != null && _playerNear)
                React("near");
        }
    }

    private IEnumerator FarLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minIntervalFar, maxIntervalFar));
            if (_player != null && enableFarSounds && _playerSameRoom && !_playerNear)
                React("far");
        }
    }

    private void React(string zone)
    {
        if (zone == "near")
            PlayRandomClip(nearSounds, nearVolume);
        else
            PlayRandomClip(farSounds, farVolume);

        // Animación aleatoria (Animator) o sacudida procedural.
        if (animator != null && animationTriggers != null && animationTriggers.Length > 0)
        {
            animator.SetTrigger(animationTriggers[Random.Range(0, animationTriggers.Length)]);
        }
        else if (proceduralShake)
        {
            float mag = zone == "near" ? shakeMagnitude : shakeMagnitude * 0.5f;
            if (_shake != null) StopCoroutine(_shake);
            _shake = StartCoroutine(ShakeRoutine(mag));
        }
    }

    private void PlayRandomClip(AudioClip[] clips, float volume)
    {
        if (clips == null || clips.Length == 0) return;
        _source.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }

    private Transform ResolvePlayer()
    {
        if (playerOverride != null) return playerOverride;

        RoomTracker tracker = RoomTracker.Instance;
        if (tracker != null && tracker.Player != null) return tracker.Player;

        if (_tagPlayer == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(playerTag);
            if (go != null) _tagPlayer = go.transform;
        }
        return _tagPlayer;
    }

    // Movimiento procedural de "susto": pequeña sacudida hacia/atrás.
    private IEnumerator ShakeRoutine(float magnitude)
    {
        Vector3 basePos = transform.localPosition;
        Quaternion baseRot = transform.localRotation;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float strength = Mathf.Lerp(magnitude, 0f, t / shakeDuration);

            transform.localPosition = basePos + (Vector3)(Random.insideUnitSphere * strength * 0.05f);
            transform.localRotation = baseRot * Quaternion.Euler(Random.insideUnitSphere * strength);

            yield return null;
        }

        transform.localPosition = basePos;
        transform.localRotation = baseRot;
        _shake = null;
    }
}