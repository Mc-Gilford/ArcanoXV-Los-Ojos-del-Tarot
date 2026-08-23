using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightRoom : MonoBehaviour
{
    //Referencia a la carta del cuarto de las luces
    public GameObject lightCard;

    //Para validar la cantidad de luces encendidas, la cantidad a encender que este dentro de la habitacion
    [SerializeField] private int lightsOn; //Para las luces encendidas de la habitacion

    private int totalLights = 4; //Esta para establecer la cantidad de luces en la habitación

    [SerializeField] private bool isInsideLightRoom = false; //Para validar que siga en la habitación de las luces

    [SerializeField] private bool lightCardPicked = false; //Validar que la carta haya sido recogida o no

    [SerializeField] private bool roomCompleted = false; //Validar que la carta haya sido recogida o no

    void Start()
    {
        isInsideLightRoom = false;
        totalLights = 4;
    }
    
    public void validateLamps()
    {
        if (lightsOn < totalLights)
        {
            lightsOn = 0;
            TurnOffLights();
        }
    }

    public void TurnOnLight()
    {
        if(isInsideLightRoom)
        {
            lightsOn++;
            if (lightsOn >= totalLights)
            {
                LightRoomCompleted();
            }   
        }
    }

    private void TurnOffLights()
    {
        GameObject[] lamps = GameObject.FindGameObjectsWithTag("Lamp");

        foreach(GameObject lamp in lamps)
        {
            Lamp lampScript = lamp.GetComponent<Lamp>();
            if(lampScript.LampIsOn())
            {
                lampScript.TurnOffLamp();
            }
        }
    }

    public void LightRoomCompleted()
    {
        if (!lightCardPicked)
        {
            lightCard.SetActive(true);
            roomCompleted = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideLightRoom = true;
            if(roomCompleted)
            {
                LightRoomCompleted();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        isInsideLightRoom=false;
        if(other.CompareTag("Player"))
        {
            validateLamps();


            if(roomCompleted && !lightCardPicked)
            {
                lightCard.SetActive(false);
            }    
        }
        
    }
}
