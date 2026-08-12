using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controlador de movimiento SOLO para la escena de prueba (probar ambience).
/// NO es el controlador real del juego: se usa para verificar que el sistema
/// detecta al jugador y dispara sonidos/animaciones. Puede eliminarse después.
/// WASD para moverse, ratón para mirar, Shift (izq/der) para correr.
/// </summary>
public class DebugPlayerMover : MonoBehaviour
{
    [Tooltip("Velocidad de movimiento caminando (m/s).")]
    public float moveSpeed = 5f;
    [Tooltip("Multiplicador de velocidad al correr (mantener Shift).")]
    public float sprintMultiplier = 2.2f;
    [Tooltip("Sensibilidad de la cámara.")]
    public float lookSpeed = 0.1f;
    [Tooltip("Altura a la que va la cámara respecto al jugador.")]
    public float cameraHeight = 1.6f;

    [Header("Bloqueo y modificadores (los usa el sistema de cartas)")]
    [Tooltip("Congela SOLO la traslación WASD; la cámara/ratón sigue funcionando.")]
    public bool lockMovement = false;
    [Tooltip("Multiplicador externo de velocidad (0.7 / 0.8 / 1.6).")]
    public float speedMultiplier = 1f;
    [Tooltip("Si false, pierdes el sprint (maldición).")]
    public bool canSprint = true;

    private Transform _cam;
    private float _pitch;

    private void Start()
    {
        // Reutiliza la cámara existente o crea una nueva como hija del jugador.
        _cam = Camera.main != null ? Camera.main.transform : CreateCamera();
        _cam.SetParent(transform, false);
        _cam.localPosition = new Vector3(0f, cameraHeight, 0f);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Keyboard kb = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (kb == null || mouse == null) return;

        // Movimiento WASD relativo a la orientación del jugador.
        Vector2 move = Vector2.zero;
        if (kb.wKey.isPressed) move.y += 1f;
        if (kb.sKey.isPressed) move.y -= 1f;
        if (kb.aKey.isPressed) move.x -= 1f;
        if (kb.dKey.isPressed) move.x += 1f;

        // Correr con Shift izquierdo o derecho (puede bloquearse con canSprint).
        bool sprinting = canSprint && (kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed);
        float speed = moveSpeed * speedMultiplier * (sprinting ? sprintMultiplier : 1f);

        // Movimiento WASD (se congela al seleccionar carta, la cámara no).
        if (!lockMovement)
        {
            Vector3 wish = (transform.right * move.x + transform.forward * move.y).normalized;
            transform.position += wish * speed * Time.deltaTime;
        }

        // Mirar con el ratón.
        Vector2 look = mouse.delta.ReadValue() * lookSpeed;
        transform.Rotate(0f, look.x, 0f, Space.Self);
        _pitch = Mathf.Clamp(_pitch - look.y, -89f, 89f);
        _cam.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private Transform CreateCamera()
    {
        GameObject go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.AddComponent<Camera>();
        go.AddComponent<AudioListener>();
        return go.transform;
    }
}