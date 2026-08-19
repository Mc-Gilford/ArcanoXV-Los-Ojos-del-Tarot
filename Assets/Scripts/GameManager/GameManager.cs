using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    [SerializeField] private int keyCards;

    //public CardCollector keyCardCollection;
    [SerializeField] private int cardsBeforeStalker;
    [SerializeField] private bool isAwaked;

    //Booleanos para comprobar los tipos de cartas de especiales que se tienen
    [SerializeField] private bool deathCard=false;
    [SerializeField] private bool safeCard=false;
    [SerializeField] private bool drunkCard=false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //keyCardCollection = GameObject.Find("CardPickup").GetComponent<CardPickup>();
        cardsBeforeStalker = 5;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetKeyCard()
    {
        keyCards++;
        if(keyCards >= cardsBeforeStalker && !isAwaked)
        {   
            Debug.Log("Ella esta aqui OnO");
            isAwaked = true;
            // Aqui va la funcion para hacer que el acosador se mantenga activo apartir de este momento

        }
    }

    public void SpectralDestruction()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject entity in monsters)
        {
            Destroy(entity);
        }
    }

    public void GetDeathCard()
    {
        deathCard = true;
    }

    public void GetDrunkCard()
    {
        drunkCard = true;
    }

    public void GetSafeCard()
    {
        safeCard = true;
    }
}
