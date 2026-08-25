using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

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

    [SerializeField] private float timeInGame = 0f;

    [SerializeField] private bool isDead = false;

    [SerializeField] private bool gameFinished = false;

    [SerializeField] private int totalKeyCards;

    [SerializeField] private bool hasAllKeyCards = false;

    public GameObject finalCardBox;

    public GameObject finalMessage;

    public string timeWithFormat;

    public Jugador playerScript;
    public ShootBullets gunScript;

   // public GameObject gameOvermenu;
    private GameObject gameOverPanel;

    public TextMeshProUGUI timeMessage;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //keyCardCollection = GameObject.Find("CardPickup").GetComponent<CardPickup>();
        totalKeyCards = 11;
        hasAllKeyCards = false;
        finalCardBox.SetActive(false);
        finalMessage.SetActive(false);
        timeWithFormat="";
        timeMessage = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        TimeFormat();
        isDead = false;
        gameFinished = false;
        timeInGame = 0f;
        cardsBeforeStalker = 7;
        cardCooldown = 5;
        playerScript = GameObject.Find("Player").GetComponent<Jugador>();
        gunScript = GameObject.Find("Gun").GetComponent<ShootBullets>();
        gameOverPanel = GameObject.FindGameObjectWithTag("GameOver");
        if(gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
                Debug.Log("gameover");
            }
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameFinished && !isDead)
        {
            timeInGame+=Time.deltaTime;
        }

        TimeFormat();
    }

    private void TimeFormat()
    {
        int hours = Mathf.FloorToInt(timeInGame / 3600);
        int minutes = Mathf.FloorToInt((timeInGame % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInGame % 60);

        timeWithFormat = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        timeMessage.text = "Time in game: "+timeWithFormat;
    }

    public void GetKeyCard()
    {
        keyCards++;
        if(keyCards >= cardsBeforeStalker && !isAwaked)
        {
            GameObject ghostObjet = GameObject.FindGameObjectWithTag("Follower");
            Ghost ghostScript = ghostObjet.GetComponent<Ghost>();


            //Debug.Log("Ella esta aqui OnO");
            isAwaked = true;
            ghostScript.isGhostAlwaysActive = true;
            // Aqui va la funcion para hacer que el acosador se mantenga activo apartir de este momento

        }

        if(keyCards >= totalKeyCards && !hasAllKeyCards)
        {
            hasAllKeyCards = true;
            finalCardBox.SetActive(true);
            StartCoroutine(FinalSign()); 
        }
        else if(keyCards>=totalKeyCards)
        {
            gameFinished = true;
            FinishGame();
        }
    }

    private void FinishGame()
    {
        Debug.Log("Juego terminado!");
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

    IEnumerator FinalSign()
    {
        finalMessage.SetActive(true);
        yield return new WaitForSeconds(5);
        finalMessage.SetActive(false);
    }

    public void GameOver()
    {
        Debug.Log("Ya me mori");
        gameOverPanel.SetActive(true);
        isDead = false;
        //referenciaALaMira.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    /*IEnumerator CardWaitTime()
    {
        Debug.Log("Espera antes de usar otra carta");
        canUseCard = false;
        yield return new WaitForSeconds(cardCooldown);
        canUseCard = true;
    } */

    //Metodo para desplegar el Submenu del Canvas
   /* public void GameOver()
    {
        gameOverMenu.SetActive(true);
        referenciaALaMira.SetActive(false);
    }

    //Funcion para recargar la escena
    public void RestartGame()
    {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Funcion para regresar al menu principal
    public void Exit()
    {
        SceneManager.LoadScene(0);
    }*/
}






