using System.Collections;
using UnityEngine;

/// <summary>
/// Puerta del jefe final: nace bloqueada (roja) y se abre (se eleva y se pone
/// verde) cuando CardCollector reúne todas las cartas. Si ya están todas al
/// cargar (por ejemplo al reentrar), arranca abierta.
/// </summary>
public class BossDoor : MonoBehaviour
{
    [Tooltip("Altura a la que sube la puerta al abrirse.")]
    public float alturaAbrir = 5f;
    [Tooltip("Duración de la animación de apertura.")]
    public float duracionAbrir = 1.5f;

    public bool Abierta { get; private set; }

    private Vector3 _posCerrada;
    private Collider _collider;
    private Renderer _renderer;
    private Material _material;

    private void Start()
    {
        _posCerrada = transform.position;
        _collider = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _material = new Material(_renderer.sharedMaterial);
            _renderer.sharedMaterial = _material;
        }

        CardCollector colector = CardCollector.Instance != null
            ? CardCollector.Instance
            : FindFirstObjectByType<CardCollector>();
        if (colector != null)
        {
            colector.OnTodasRecogidas += Abrir;
            if (colector.TodasRecogidas)
                AbrirInstantaneo();
            else
                PintarBloqueada();
        }
    }

    private void OnDestroy()
    {
        if (CardCollector.Instance != null)
            CardCollector.Instance.OnTodasRecogidas -= Abrir;
    }

    public void Abrir()
    {
        if (Abierta) return;
        Abierta = true;
        if (_collider != null) _collider.enabled = false;
        StartCoroutine(AnimacionAbrir());
    }

    private void AbrirInstantaneo()
    {
        Abierta = true;
        if (_collider != null) _collider.enabled = false;
        transform.position = _posCerrada + Vector3.up * alturaAbrir;
        PintarAbierta();
    }

    private IEnumerator AnimacionAbrir()
    {
        Vector3 destino = _posCerrada + Vector3.up * alturaAbrir;
        float t = 0f;
        while (t < duracionAbrir)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / duracionAbrir);
            transform.position = Vector3.Lerp(_posCerrada, destino, k);
            yield return null;
        }
        transform.position = destino;
        PintarAbierta();
    }

    private void PintarBloqueada()
    {
        if (_material != null) _material.color = new Color(0.6f, 0.1f, 0.1f); // rojo
    }

    private void PintarAbierta()
    {
        if (_material != null) _material.color = new Color(0.15f, 0.6f, 0.2f); // verde
    }
}