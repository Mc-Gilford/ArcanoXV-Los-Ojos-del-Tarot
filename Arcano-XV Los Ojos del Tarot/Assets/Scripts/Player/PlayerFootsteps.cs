using UnityEngine;

/// <summary>
/// Reproduce sonidos de pasos sobre suelo de madera vieja al caminar
/// (estilo película de terror). Se adjunta al jugador.
///
/// Detecta el movimiento por posición horizontal (funciona con el
/// DebugPlayerMover de la escena de prueba). Cada `stepDistance` metros
/// recorridos reproduce un crujido al azar con una pequeña variación de tono.
///
/// Los clips los asigna el editor desde Assets/Audio/Pasos/ (menú
/// "Tools > Arcano XV > Reasignar pasos (escena)").
/// </summary>
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Sónico")]
    [Tooltip("Crujidos de madera vieja. Se eligen al azar en cada paso.")]
    public AudioClip[] stepClips;
    [Tooltip("Distancia recorrida (m) entre cada paso.")]
    public float stepDistance = 0.75f;
    [Tooltip("Volumen de los pasos.")]
    [Range(0f, 1f)] public float volume = 0.7f;
    [Tooltip("Variación de tono entre pasos para que no parezca grabado.")]
    public float pitchJitter = 0.12f;

    private AudioSource _source;
    private Vector3 _lastPos;
    private float _accumulated;
    private float _lastMovedTime = -1f;      // cuándo caminó por última vez
    private const float _stopGrace = 0.3f;   // gracia para cortar el crujido al detenerse

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (_source == null) _source = gameObject.AddComponent<AudioSource>();

        _source.playOnAwake = false;
        _source.spatialBlend = 0f; // los pasos son del propio jugador: audio 2D
        _lastPos = transform.position;
    }

    private void Update()
    {
        if (stepClips == null || stepClips.Length == 0) return;

        Vector3 pos = transform.position;
        float walkDist = Vector3.Distance(
            new Vector3(pos.x, 0f, pos.z),
            new Vector3(_lastPos.x, 0f, _lastPos.z));
        _lastPos = pos;

        if (walkDist <= 0.0001f)
        {
            // Dejó de caminar: corta el crujido en curso tras una pequeña gracia,
            // para que no siga sonando como si aún estuviera andando.
            if (_source.isPlaying && Time.time - _lastMovedTime > _stopGrace)
                _source.Stop();
            return;
        }

        _lastMovedTime = Time.time;
        _accumulated += walkDist;
        if (_accumulated < stepDistance) return;

        if (_source.isPlaying)
        {
            // Un crujido a la vez: espera a que termine el paso actual antes de
            // que suene el siguiente (nada de pasos encimados). Se mantiene el
            // paso "pendiente" para que no se pierda al acabar el anterior.
            _accumulated = stepDistance;
            return;
        }

        _accumulated = 0f;
        PlayStep();
    }

    private void PlayStep()
    {
        AudioClip clip = stepClips[Random.Range(0, stepClips.Length)];
        _source.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        _source.PlayOneShot(clip, volume);
    }
}