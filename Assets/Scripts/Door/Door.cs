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

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            //Debug.Log(gameObject.name + ": Player encontrado");
        }
        else
        {
            //Debug.LogError(gameObject.name + ": No se encontró Player");
        }

        GameObject textObject = GameObject.Find("DoorText");

        if (textObject != null)
        {
            doorText = textObject.GetComponent<TextMeshProUGUI>();

            if (doorText != null)
            {
                doorText.enabled = false;
                //Debug.Log(gameObject.name + ": DoorText encontrado");
            }
        }
        else
        {
            //Debug.LogError(gameObject.name + ": No se encontró DoorText");
        }

        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, 0, openAngle);
    }

    void Update()
    {
        if (player == null || doorText == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool playerNear = distance <= detectionDistance;

        if (playerNear && !isOpen)
        if (playerNear && !isOpen)
        {
            activeDoor = this;

            if (!wasNear)
            {
                //Debug.Log(gameObject.name + ": Player cerca");
                wasNear = true;
            }

            doorText.text = "[ X ] Abrir puerta";
            doorText.enabled = true;

            if (Input.GetKeyDown(KeyCode.X))
            {
                //Debug.Log(gameObject.name + ": X presionada");

                doorText.enabled = false;
                StartCoroutine(OpenDoor());
            }
        }
        else
        {
            if (wasNear && !playerNear)
            {
                //Debug.Log(gameObject.name + ": Player se alejó");
                wasNear = false;
            }

            // Solo puede ocultar el texto si ESTA puerta lo estaba usando
            if (activeDoor == this)
            {
                doorText.enabled = false;

                if (!playerNear)
                    activeDoor = null;
            }
        }
    }

    private IEnumerator OpenDoor()
    {
        isOpen = true;

        if (activeDoor == this)
        {
            doorText.enabled = false;
        }

        //Debug.Log(gameObject.name + ": Abriendo");

        while (Quaternion.Angle(transform.localRotation, openRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                openRotation,
                speed * Time.deltaTime
            );

            yield return null;
        }

        transform.localRotation = openRotation;

        //Debug.Log(gameObject.name + ": Abierta");

        yield return new WaitForSeconds(closeDelay);

        //Debug.Log(gameObject.name + ": Cerrando");

        while (Quaternion.Angle(transform.localRotation, closedRotation) > 0.5f)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                closedRotation,
                speed * Time.deltaTime
            );

            yield return null;
        }

        transform.localRotation = closedRotation;
        isOpen = false;

        //Debug.Log(gameObject.name + ": Cerrada");
    }
}