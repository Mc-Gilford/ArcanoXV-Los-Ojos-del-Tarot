using UnityEngine;

/// <summary>
/// Sonido de susto por contacto: al TOCAR el objeto (entrar en su área de ~1.5 m)
/// reproduce su clip durante `playDuration` segundos (por defecto 3 s) y se apaga.
///
/// - Al inicio NO suena nada: solo arranca cuando el jugador toca el objeto.
/// - Aunque te alejes a mitad, termina los 3 segundos y luego se apaga.
/// - Para oírlo otra vez: sal del área y vuelve a tocarlo.
///
/// Así el objeto queda en silencio hasta que lo tocas → asusta de un momento a otro.
/// La categoría de audio (`audioCategory`) la usa el editor para buscar el clip en
/// Assets/Audio/Objetos/<audioCategory>/ y asignarlo automáticamente.
/// </summary>
public class ObjectAmbience : MonoBehaviour
{
    [Header("Sónico")]
    [Tooltip("Clip que suena al tocar el objeto. Vacío = silencioso (mesa, silla).")]
    public AudioClip clip;
    [Tooltip("Se usa en el editor para buscar el clip en Assets/Audio/Objetos/<audioCategory>/.")]
    public string audioCategory = "";
    [Tooltip("Volumen del sonido.")]
    [Range(0f, 1f)] public float baseVolume = 1f;
    [Tooltip("Segundos que suena desde que tocas el objeto, aunque te alejes.")]
    public float playDuration = 3f;
    [Tooltip("Radio (m) del área de contacto: al entrar, reproduce el sonido.")]
    public float triggerRadius = 1.5f;

    private AudioSource _source;
    private float _playTimer;

    private void Awake()
    {
        // Área de contacto: cuando entra el jugador, dispara el sonido.
        // Se agrega SIEMPRE un trigger nuevo, sin tocar el collider sólido del objeto.
        SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = Mathf.Max(0.1f, triggerRadius);

        _source = GetComponent<AudioSource>();
        if (_source == null) _source = gameObject.AddComponent<AudioSource>();

        _source.playOnAwake = false;
        _source.loop = false;          // un disparo de playDuration, no bucle continuo
        _source.spatialBlend = 1f;     // audio posicional: sale del propio objeto
        _source.rolloffMode = AudioRolloffMode.Logarithmic;
        _source.minDistance = 0.5f;
        _source.maxDistance = 15f;
        _source.volume = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;
        PlayOnce();
    }

    // OnTriggerExit a propósito NO corta el sonido: si te alejas
    // a mitad, los 3 segundos terminan y luego se apaga.

    private void Update()
    {
        if (clip == null) return;

        // Corta el sonido al cumplirse los playDuration segundos.
        if (_source.isPlaying)
        {
            _playTimer -= Time.deltaTime;
            if (_playTimer <= 0f)
            {
                _source.Stop();
                _source.volume = 0f;
            }
        }
    }

    private void PlayOnce()
    {
        if (clip == null) return;
        _source.clip = clip;
        _source.volume = baseVolume;
        _playTimer = playDuration;
        _source.Play();
    }

    private bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player")) return true;

        RoomTracker tracker = RoomTracker.Instance;
        return tracker != null && tracker.playerOverride != null
            && other.transform.root == tracker.playerOverride.root;
    }
}