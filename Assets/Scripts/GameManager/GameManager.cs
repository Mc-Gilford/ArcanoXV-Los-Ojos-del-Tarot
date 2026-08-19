using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    private int cardCooldown;

    [SerializeField] private int keyCards;

    //public CardCollector keyCardCollection;
    [SerializeField] private int cardsBeforeStalker;
    [SerializeField] private bool isAwaked;

    //Booleanos para comprobar los tipos de cartas de especiales que se tienen
    [SerializeField] private bool deathCard=false;
    [SerializeField] private bool safeCard=false;
    [SerializeField] private bool drunkCard=false;
    [SerializeField] private bool canUseCard=false;
    [SerializeField] private string cardSelected="";

    public Jugador playerScript;
    public ShootBullets gunScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //keyCardCollection = GameObject.Find("CardPickup").GetComponent<CardPickup>();
        cardsBeforeStalker = 5;
        cardCooldown = 5;
        playerScript = GameObject.Find("Player").GetComponent<Jugador>();
        gunScript = GameObject.Find("Gun").GetComponent<ShootBullets>();
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
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy");

        
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

    /*public void UseCard(InputAction.CallbackContext context)
    {
        if(context.started && (cardSelected != "" && cardSelected != "none") && canUseCard)
        {
            Debug.Log("Usando carta");
            CardEffect();
        }
    }*/

    public void CardEffect(int card)
    {
        switch(card)
        {
            case 0:
                playerScript.LetsDrink();
                break;
            case 1:
                SpectralDestruction();
                playerScript.LoseHalfHP();
                break;
            case 2:
                playerScript.GoToSafeRoom();
                gunScript.EmptyBullets();
                break;
        }
    }

    /*IEnumerator CardWaitTime()
    {
        Debug.Log("Espera antes de usar otra carta");
        canUseCard = false;
        yield return new WaitForSeconds(cardCooldown);
        canUseCard = true;
    } */
}
