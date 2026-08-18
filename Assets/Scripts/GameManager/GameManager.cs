using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    [SerializeField] private int keyCards;

    //public CardCollector keyCardCollection;
    [SerializeField] private int cardsBeforeStalker;
    [SerializeField] private bool isAwaked;
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
}
