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
    private RoomTimer roomTimer;

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

        roomTimer = RoomTimer.Instance != null ? RoomTimer.Instance : FindFirstObjectByType<RoomTimer>();

        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, 0, openAngle);
    }

    void Update()
    {
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

            // Revisa si la puerta tiene alguna condición especial
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
                StartCoroutine(OpenDoor());
            }
        }
        else
        {
            if (wasNear && !playerNear)
            {
                wasNear = false;
            }

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
        // =========================
        // H13 PUERTA ENTRADA
        // =========================

        if (gameObject.name == "H13 Puerta Entrada")
        {
            if (cardCollector == null || !cardCollector.TodasRecogidas)
            {
                doorText.text = "Necesitas todas las cartas";
                return false;
            }

            return true;
        }

        // =========================
        // H13 SALIDA
        // =========================

        if (gameObject.name == "H13 Salida")
        {
            if (roomTimer == null)
            {
                doorText.text = "No puedes salir todavía";
                return false;
            }

            if (!roomTimer.TiempoCompletado)
            {
                float remainingTime = roomTimer.tiempoNecesario - roomTimer.TiempoDentro;

                int seconds = Mathf.CeilToInt(remainingTime);
                int minutes = seconds / 60;
                int remainingSeconds = seconds % 60;

                doorText.text = "Tiempo restante: " + minutes.ToString("00") + ":" + remainingSeconds.ToString("00");

                return false;
            }

            return true;
        }

        // =========================
        // PUERTAS NORMALES
        // =========================

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