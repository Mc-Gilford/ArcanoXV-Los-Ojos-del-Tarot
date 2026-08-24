 using UnityEngine;
 using System.Collections.Generic;

public class CandleRoom : MonoBehaviour
{
    public GameObject candleCard;

    //Para validar la cantidad de luces encendidas, la cantidad a encender que este dentro de la habitacion
    [SerializeField] private int round; //Para las luces encendidas de la habitacion

    [SerializeField] private float candleTimer=20; //El tiempo antes de que la vela cambie de posicion

    [SerializeField] private float candleTimeBeforeTP = 0f;

    [SerializeField] private int indexPosition=0; //Con este se sabra a que posicion mover la vela

    private int totalRounds = 4; //Esta para establecer la cantidad de luces en la habitación

    [SerializeField] private bool isInsideCandleRoom = false; //Para validar que siga en la habitación de las luces

    [SerializeField] private bool candleCardPicked = false; //Validar que la carta haya sido recogida o no

    [SerializeField] private bool roomCompleted = false; //Validar que el cuarto fue completado
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public List<GameObject> tpPoints;

    [SerializeField] private bool canMove; 

    public GameObject candle;

    void Start()
    {
        round = 1;
        isInsideCandleRoom = false;
        totalRounds = 4;
        indexPosition = 0;
        candleTimer = 20f; 
        candleTimeBeforeTP =0;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInsideCandleRoom && !roomCompleted)
        {
           MovingCandle();
        }
    }

    private void InitialPos()
    {
        if(!roomCompleted)
        {
            indexPosition = Random.Range(0, tpPoints.Count);
            candle.SetActive(true);
            candle.transform.position = tpPoints[indexPosition].transform.position;
        }
        else
        {
            candle.SetActive(false);
        }
    }

    public void MovingCandle()
    {
        if(canMove)
        {
            candleTimeBeforeTP += Time.deltaTime;
            if(candleTimeBeforeTP >= candleTimer)
            {
                canMove = false;
                changePosition(indexPosition);
            }
        }
    }

    public void changePosition(int currentPos)
    {
        if(!canMove)
        {
            while(indexPosition == currentPos)
            {
                indexPosition = Random.Range(0, tpPoints.Count);
            }
            candleTimeBeforeTP = 0;
            GameObject teleportPosition = tpPoints[indexPosition];
            candle.transform.position = teleportPosition.transform.position;
            canMove = true;
        }
    }

    public void NextLevel()
    {
        if(isInsideCandleRoom && !roomCompleted)
        {
            round++;
            canMove = false;
            candleTimer -=5f;
            changePosition(indexPosition);

            if (round >= totalRounds)
            {
                CandleRoomCompleted();
            }   
        }
    }

    public void CandleRoomCompleted()
    {
        roomCompleted = true;
        candleCard.SetActive(true);
        candle.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideCandleRoom = true;
            InitialPos();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideCandleRoom = false;
            if(!roomCompleted)
            {
                ResetValues();
            }
        }
    }

    void ResetValues()
    {
        round = 0;
        indexPosition = 0;
        candleTimer = 20f; 
        candleTimeBeforeTP =0;
        canMove = true;
        candle.SetActive(false);
    }
}
