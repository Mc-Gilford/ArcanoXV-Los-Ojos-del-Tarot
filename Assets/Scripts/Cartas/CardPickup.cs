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
    private Transform _etiquetaRaiz;

    private void Start()
    {
        _basePos = transform.position;
        _etiquetaRaiz = CrearEtiqueta();
        _etiquetaRaiz.gameObject.SetActive(false);
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
            _etiquetaRaiz.gameObject.SetActive(false);
            return;
        }

        bool cerca = Vector3.Distance(jugador.transform.position, transform.position) <= radioInteraccion;
        _etiquetaRaiz.gameObject.SetActive(cerca);
        ApuntarEtiqueta();

        // Pulso sutil de escala para llamar la atención
        float pulso = 1f + Mathf.Sin(Time.time * 4f) * 0.08f;
        _etiquetaRaiz.localScale = Vector3.one * pulso;

        if (cerca && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            Recoger();
    }

    private void Recoger()
    {
        _recogida = true;
        _etiquetaRaiz.gameObject.SetActive(false);
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
        _etiquetaRaiz.rotation = Quaternion.LookRotation(_etiquetaRaiz.position - cam.transform.position);
    }

    // Dorado del HUD (#F4C95D) para coherencia visual
    private static readonly Color ColorE = new Color(0.957f, 0.788f, 0.365f);

    private Transform CrearEtiqueta()
    {
        GameObject raiz = new GameObject("EtiquetaE");
        raiz.transform.SetParent(transform, false);
        raiz.transform.localPosition = Vector3.up * 1.25f;

        // Sombra negra desplazada (legibilidad sobre fondos claros)
        CrearTexto(raiz.transform, "SombraE", new Color(0f, 0f, 0f, 0.9f), new Vector3(-0.045f, -0.045f, 0.01f));
        CrearTexto(raiz.transform, "TextoE", ColorE, Vector3.zero);
        return raiz.transform;
    }

    private void CrearTexto(Transform padre, string nombre, Color color, Vector3 posLocal)
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject go = new GameObject(nombre);
        go.transform.SetParent(padre, false);
        go.transform.localPosition = posLocal;

        TextMesh tm = go.AddComponent<TextMesh>();
        tm.text = "E";
        tm.font = font;
        tm.fontSize = 80;
        tm.characterSize = 0.075f; // antes 0.04 (se veía diminuto)
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.color = color;
        go.GetComponent<MeshRenderer>().sortingOrder = nombre == "SombraE" ? 0 : 1;
    }
}