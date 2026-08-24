using UnityEngine;
using System.Collections;
using UnityEngine;

public class Candle : MonoBehaviour
{
    private CandleRoom candleRoom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        candleRoom = GameObject.Find("CandleRoom").GetComponent<CandleRoom>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            candleRoom.NextLevel();
        }
    }
}
