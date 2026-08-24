using UnityEngine;

/// <summary>
/// Sigue al carro desde tercera persona (detrás y arriba).
/// La cámara NO debe ser hija del carro; se posiciona de forma independiente.
/// Busca automáticamente al carro "CarroSalida" si no se asigna target.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CarCameraFollow : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Offset detrás y arriba del carro (espacio local del target)")]
    public Vector3 offset = new Vector3(0f, 5f, -12f);
    [Tooltip("Suavizado del seguimiento")]
    public float smoothSpeed = 5f;

    [Tooltip("Target a seguir (se busca automáticamente si está vacío)")]
    public Transform target;

    private void Start()
    {
        // Si no se asignó target, buscar el carro por nombre
        if (target == null)
        {
            GameObject carro = GameObject.Find("CarroSalida");
            if (carro != null)
            {
                target = carro.transform;
            }
            else
            {
                // Buscar por componente
                CarroSalidaController ctrl = FindFirstObjectByType<CarroSalidaController>();
                if (ctrl != null)
                    target = ctrl.transform;
            }
        }

        if (target == null)
        {
            Debug.LogWarning("[CarCameraFollow] No se encontró el carro para seguir.");
        }
    }

    /// <summary>
    /// Asigna el transform del carro a seguir.
    /// </summary>
    public void SetTarget(Transform t)
    {
        target = t;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // Posición deseada: detrás y arriba del carro
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Siempre mirar hacia el carro
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
