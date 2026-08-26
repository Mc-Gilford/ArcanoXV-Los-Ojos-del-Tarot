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
    [SerializeField] private int cardsBeforeStalker;
    [SerializeField] private bool isAwaked;

    [SerializeField] private bool deathCard = false;
    [SerializeField] private bool safeCard = false;
    [SerializeField] private bool drunkCard = false;
    [SerializeField] private bool canUseCard = false;
    [SerializeField] private string cardSelected = "";

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

    private GameObject gameOverPanel;

    public TextMeshProUGUI timeMessage;

    void Start()
    {
        totalKeyCards = 11;
        hasAllKeyCards = false;

        finalCardBox.SetActive(false);
        finalMessage.SetActive(false);

        timeWithFormat = "";

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

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("gameover");
        }
    }

    void Update()
    {
        if (!gameFinished && !isDead)
        {
            timeInGame += Time.deltaTime;
        }

        TimeFormat();
    }

    private void TimeFormat()
    {
        int hours = Mathf.FloorToInt(timeInGame / 3600);
        int minutes = Mathf.FloorToInt((timeInGame % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInGame % 60);

        timeWithFormat = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

        timeMessage.text = "Time in game: " + timeWithFormat;
    }

    public void GetKeyCard()
    {
        keyCards++;

        if (keyCards >= cardsBeforeStalker && !isAwaked)
        {
            GameObject ghostObjet = GameObject.FindGameObjectWithTag("Follower");
            Ghost ghostScript = ghostObjet.GetComponent<Ghost>();

            isAwaked = true;
            ghostScript.hasAlmostAllKeys = true;
        }

        if (keyCards >= totalKeyCards && !hasAllKeyCards)
        {
            hasAllKeyCards = true;

            finalCardBox.SetActive(true);

            StartCoroutine(FinalSign());
        }
        else if (keyCards >= totalKeyCards)
        {
            gameFinished = true;

            FinishGame();
        }
    }

    // NUEVA FEATURE: Al terminar el juego muestra primero el Ranking
    private void FinishGame()
    {
        Debug.Log("Juego terminado!");

        RankingManager rankingManager = FindFirstObjectByType<RankingManager>();

        if (rankingManager != null)
        {
            // Enviamos el tiempo numerico para guardar y ordenar los records
            rankingManager.SetTiempoFinal(timeInGame);

            // Mostramos primero el panel de Ranking
            rankingManager.ShowRankingPanel();

            return;
        }

        // NUEVA FEATURE:
        // Si por alguna razon no encuentra RankingManager,
        // muestra directamente el Game Over.
        //ShowGameOverPanel();
    }

    // NUEVA FEATURE:
    // Este metodo se usara cuando el jugador termine de ver el Ranking
    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        isDead = false;

        //referenciaALaMira.SetActive(false);
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

    public void CardEffect(int card)
    {
        switch (card)
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