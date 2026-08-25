using System.Collections;
using UnityEngine;
using TMPro;

public class Door : MonoBehaviour
{
    public float detectionDistance = 2.5f;
    public float openAngle = -90f;
    public float speed = 3f;
    public float closeDelay = 10f;

    private Transform player;
    private TextMeshProUGUI doorText;
    private bool isOpen = false;
    private bool wasNear = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    // Puerta que actualmente controla el texto
    private static Door activeDoor;

    private CardCollector cardCollector;

    // H13
    private const float h13RequiredTime = 120f;
    private static float h13Timer = 0f;
    private static bool h13TimerActive = false;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        GameObject textObject = GameObject.Find("DoorText");

        if (textObject != null)
        {
            doorText = textObject.GetComponent<TextMeshProUGUI>();

            if (doorText != null)
            {
                doorText.enabled = false;
            }
        }

        cardCollector = CardCollector.Instance != null ? CardCollector.Instance : FindFirstObjectByType<CardCollector>();

        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, 0, openAngle);
    }

    void Update()
    {
        // Empieza a contar después de abrir la entrada de H13
        if (h13TimerActive && h13Timer < h13RequiredTime)
        {
            h13Timer += Time.deltaTime;
        }

        OpenDoorAction();
    }

    private void OpenDoorAction()
    {
        if (player == null || doorText == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool playerNear = distance <= detectionDistance;

        if (playerNear && !isOpen)
        {
            activeDoor = this;

            if (!wasNear)
            {
                wasNear = true;
            }

            // Revisa si esta puerta tiene algún requisito especial
            if (!SpecialDoors())
            {
                doorText.enabled = true;
                return;
            }

            doorText.text = "[ X ] Abrir puerta";
            doorText.enabled = true;

            if (Input.GetKeyDown(KeyCode.X))
            {
                doorText.enabled = false;

                // Al abrir la entrada de H13 comienza el tiempo
                if (gameObject.name == "H13 Puerta Entrada" && !h13TimerActive)
                {
                    h13Timer = 0f;
                    h13TimerActive = true;

                    Debug.Log("Jugador entró a H13. Comienza el temporizador de 2 minutos.");
                }

                StartCoroutine(OpenDoor());
            }
        }
        else
        {
            if (wasNear && !playerNear)
            {
                wasNear = false;
            }

            // Solo esta puerta puede ocultar el texto que estaba usando
            if (activeDoor == this)
            {
                doorText.enabled = false;

                if (!playerNear)
                {
                    activeDoor = null;
                }
            }
        }
    }

    private bool SpecialDoors()
    {
        // H13 ENTRADA
        // Necesita tener todas las cartas
        if (gameObject.name == "H13 Puerta Entrada")
        {
            if (cardCollector == null || !cardCollector.TodasRecogidas)
            {
                doorText.text = "Necesitas todas las cartas";
                return false;
            }

            return true;
        }

        // H13 SALIDA
        // Necesita permanecer 2 minutos en H13
        if (gameObject.name == "H13 Salida")
        {
            if (!h13TimerActive)
            {
                doorText.text = "No puedes salir todavía";
                return false;
            }

            if (h13Timer < h13RequiredTime)
            {
                float remainingTime = h13RequiredTime - h13Timer;
                int seconds = Mathf.CeilToInt(remainingTime);
                int minutes = seconds / 60;
                int remainingSeconds = seconds % 60;

                doorText.text = "Tiempo restante: " + minutes.ToString("00") + ":" + remainingSeconds.ToString("00");

                return false;
            }

            return true;
        }

        // Todas las demás puertas funcionan normalmente
        return true;
    }

    private IEnumerator OpenDoor()
    {
        isOpen = true;

        if (activeDoor == this)
        {
            doorText.enabled = false;
        }

        while (Quaternion.Angle(transform.localRotation, openRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, openRotation, speed * Time.deltaTime);
            yield return null;
        }

        transform.localRotation = openRotation;

        yield return new WaitForSeconds(closeDelay);

        while (Quaternion.Angle(transform.localRotation, closedRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, closedRotation, speed * Time.deltaTime);
            yield return null;
        }

        transform.localRotation = closedRotation;
        isOpen = false;
    }
}