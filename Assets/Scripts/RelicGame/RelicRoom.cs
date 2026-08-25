using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RelicRoom : MonoBehaviour
{
    public GameObject relic;

    private Relic relicScript;

    public GameObject clue;

    public GameObject relicCardBox;

    private float hintTime;

    [SerializeField] private bool isInsideRelicRoom;

    [SerializeField] private float clueTimer;

    [SerializeField] private bool isDestroyed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hintTime = 10f;
        relicCardBox.SetActive(false);
        relicScript = relic.GetComponent<Relic>();
        clue = GameObject.Find("RoomClue");
        clue.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isInsideRelicRoom)
        {
            clueTimer +=Time.deltaTime;
            if(clueTimer >= hintTime)
            {
                TriggerHint();
            }
        }
    }

    public void TriggerHint()
    {
        clueTimer=0f;
        relicScript.PlayClue();
    }

    public void DestroyRelic()
    {
        isDestroyed = true;
        relicCardBox.SetActive(true);
        relic.SetActive(false);
    }

    IEnumerator InitialSign()
    {
        clue.SetActive(true);
        yield return new WaitForSeconds(5);
        clue.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideRelicRoom = true;
            if(!isDestroyed)
            {
                StartCoroutine(InitialSign()); 
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideRelicRoom = true;
            if(isDestroyed)
            {
                relicCardBox.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            relicCardBox.SetActive(false);
        }
    }
}
