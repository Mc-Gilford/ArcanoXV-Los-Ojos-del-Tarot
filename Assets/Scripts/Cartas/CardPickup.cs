using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Carta coleccionable: flota y gira en el sitio. Cuando el jugador está cerca
/// aparece una "E" sobre ella; al pulsar E se recoge (avisa a CardCollector) con
/// una pequeña animación y desaparece.
/// </summary>
public class CardPickup : MonoBehaviour
{
    [Tooltip("Distancia a la que se puede recoger con E.")]
    public float radioInteraccion = 2.5f;
    [Tooltip("Amplitud del vaivén vertical (flotación).")]
    public float alturaFlotacion = 0.15f;
    [Tooltip("Velocidad de rotación en grados/segundo.")]
    public float velocidadRotacion = 70f;

    private bool _recogida;
    private Vector3 _basePos;
    private TextMesh _etiqueta;

    private void Start()
    {
        _basePos = transform.position;
        _etiqueta = CrearEtiqueta();
        _etiqueta.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_recogida) return;

        // Flotación y rotación (sigue girando aunque no estés cerca).
        float onda = (Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f);
        transform.position = _basePos + Vector3.up * (onda * alturaFlotacion);
        transform.Rotate(0f, velocidadRotacion * Time.deltaTime, 0f, Space.World);

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador == null)
        {
            _etiqueta.gameObject.SetActive(false);
            return;
        }

        bool cerca = Vector3.Distance(jugador.transform.position, transform.position) <= radioInteraccion;
        _etiqueta.gameObject.SetActive(cerca);
        ApuntarEtiqueta();

        if (cerca && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Recoger();
    }

    private void Recoger()
    {
        _recogida = true;
        _etiqueta.gameObject.SetActive(false);
        CardCollector.Instance?.RecogerCarta();
        StartCoroutine(AnimacionRecogida());
    }

    private IEnumerator AnimacionRecogida()
    {
        Vector3 baseScale = transform.localScale;
        float t = 0f;
        while (t < 0.35f)
        {
            t += Time.deltaTime;
            transform.localScale = baseScale * (1f + t * 3f);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void ApuntarEtiqueta()
    {
        Camera cam = Camera.main;
        if (cam == null) return;
        _etiqueta.transform.rotation = Quaternion.LookRotation(_etiqueta.transform.position - cam.transform.position);
    }

    private TextMesh CrearEtiqueta()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject go = new GameObject("EtiquetaE");
        go.transform.SetParent(transform, false);
        go.transform.localPosition = Vector3.up * 1.1f;

        TextMesh tm = go.AddComponent<TextMesh>();
        tm.text = "E";
        tm.font = font;
        tm.fontSize = 80;
        tm.characterSize = 0.04f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = Color.white;
        return tm;
    }
}