using System.Collections;
using UnityEngine;
using TMPro;

public class Lamp : MonoBehaviour
{
     public float detectionDistance = 2f;

    private Transform player;
    private TextMeshProUGUI lightText;

    private GameObject crosshair;
    public bool isOn = false;
    private bool wasNear = false;

    private LightRoom lampManager;

    public GameObject lightRoom;

    public GameObject light;

    // Lampara que actualmente controla el texto
    private static Lamp activeLamp;

    void Start()
    {
        detectionDistance = 6f;
        light.SetActive(false);
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
	    lampManager = lightRoom.GetComponent<LightRoom>();

        if (playerObject != null)
        {
            player = playerObject.transform;
            //Debug.Log(gameObject.name + ": Player encontrado");
        }
        else
        {
            //Debug.LogError(gameObject.name + ": No se encontró Player");
        }

        GameObject textObject = GameObject.Find("LightText");
        crosshair = GameObject.Find("Crosshair");

        if (textObject != null)
        {
            lightText = textObject.GetComponent<TextMeshProUGUI>();
            

            if (lightText != null)
            {
                lightText.enabled = false;
                //Debug.Log(gameObject.name + ": DoorText encontrado");
            }
        }
        else
        {
            //Debug.LogError(gameObject.name + ": No se encontró DoorText");
        }
    }

    void Update()
    {
        if (player == null || lightText == null)
            return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool playerNear = distance <= detectionDistance;

        
        if (playerNear && !isOn)
        {
            activeLamp = this;

            if (!wasNear)
            {
                //Debug.Log(gameObject.name + ": Player cerca");
                wasNear = true;
            }

            lightText.text = "[ M ] Activar Luz";
            lightText.enabled = true;

            crosshair.SetActive(false);

            if (Input.GetKeyDown(KeyCode.M))
            {
                //Debug.Log(gameObject.name + ": M presionada");

                lightText.enabled = false;
                TurnOnLamp();
            }
        }
        else
        {
            if (wasNear && !playerNear)
            {
                //Debug.Log(gameObject.name + ": Player se alejó");
                wasNear = false;
                crosshair.SetActive(true);
            }

            // Solo puede ocultar el texto si ESTA puerta lo estaba usando
            if (activeLamp == this)
            {
                lightText.enabled = false;

                if (!playerNear)
                    activeLamp = null;
            }
        }
    }

    private void TurnOnLamp()
    {
	    lampManager.TurnOnLight();
	    isOn = true;
        light.SetActive(true);
    }

    public void TurnOffLamp()
    {
	    isOn = false;
        light.SetActive(false);
    }

    public bool LampIsOn()
    {
        return isOn;
    }   
}
