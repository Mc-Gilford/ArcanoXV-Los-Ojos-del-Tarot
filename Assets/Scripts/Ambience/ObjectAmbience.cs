using UnityEngine;

public class ObjectAmbience : MonoBehaviour
{
    [Header("Sónico")]
    public AudioClip clip;
    public string audioCategory = "";
    [Range(0f, 1f)] public float baseVolume = 1f;
    public float playDuration = 3f;
    public float triggerRadius = 1.5f;
    public float minAudioDistance = 2f;
    public float maxAudioDistance = 15f;

    private Collider _solidCollider;
    private Transform _player;
    private Transform _audioEmitter;
    private AudioSource _source;
    private float _playTimer;
    private bool _yaSono;
    private bool _debugLogged;

    private void Awake()
    {
        _solidCollider = GetComponent<Collider>();
        if (_solidCollider == null)
            _solidCollider = GetComponentInChildren<Collider>();

        GameObject emitterGo = new GameObject("AudioEmitter");
        emitterGo.transform.SetParent(transform, false);
        _audioEmitter = emitterGo.transform;

        _source = emitterGo.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = 1f;
        _source.rolloffMode = AudioRolloffMode.Logarithmic;
        _source.minDistance = minAudioDistance;
        _source.maxDistance = maxAudioDistance;
        _source.volume = 0f;
    }

    private void Start()
    {
        _player = FindPlayer();

        if (!_debugLogged)
        {
            _debugLogged = true;
            Debug.Log($"[ObjectAmbience] {gameObject.name} | clip={(clip != null ? clip.name : "NULL")} | player={(_player != null ? _player.name : "NULL")} | radius={triggerRadius} | collider={(_solidCollider != null ? _solidCollider.GetType().Name : "NULL")}");
        }
    }

    private void Update()
    {
        if (clip == null) return;

        if (_source.isPlaying)
        {
            if (_player != null)
                UpdateEmitterPosition();

            _playTimer -= Time.deltaTime;
            if (_playTimer <= 0f)
            {
                _source.Stop();
                _source.volume = 0f;
            }
            return;
        }

        if (_player == null)
        {
            _player = FindPlayer();
            if (_player == null) return;
        }

        float dist = GetDistanceToPlayer();

        if (dist <= triggerRadius && !_yaSono)
        {
            _yaSono = true;
            Debug.Log($"[ObjectAmbience] {gameObject.name} SONANDO (dist={dist:F2})");
            PlayOnce();
        }
        else if (dist > triggerRadius)
        {
            _yaSono = false;
        }
    }

    private Transform FindPlayer()
    {
        if (RoomTracker.Instance != null && RoomTracker.Instance.Player != null)
            return RoomTracker.Instance.Player;

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null) return go.transform;

        if (Camera.main != null)
            return Camera.main.transform.root;

        return null;
    }

    private float GetDistanceToPlayer()
    {
        if (_solidCollider != null)
        {
            Vector3 closest = _solidCollider.ClosestPoint(_player.position);
            return Vector3.Distance(closest, _player.position);
        }
        return Vector3.Distance(transform.position, _player.position);
    }

    private void UpdateEmitterPosition()
    {
        if (_solidCollider != null)
            _audioEmitter.position = _solidCollider.ClosestPoint(_player.position);
        else
            _audioEmitter.position = transform.position;
    }

    private void PlayOnce()
    {
        if (clip == null) return;

        UpdateEmitterPosition();

        _source.clip = clip;
        _source.volume = baseVolume;
        _source.minDistance = minAudioDistance;
        _source.maxDistance = maxAudioDistance;
        _playTimer = playDuration;
        _source.Play();
    }
}