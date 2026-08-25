using UnityEngine;
 using System.Collections.Generic;

public class BoxRoom : MonoBehaviour
{
    [SerializeField] private bool hasEntered=false;
    [SerializeField] private bool isInsideBoxRoom=false;

    public List<GameObject> boxPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInsideBoxRoom = false;
        hasEntered = false;
        MoveCardInsideBox();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveCardInsideBox()
    {
        
        int boxindex = Random.Range(0, boxPoints.Count);
        Debug.Log("Moviendo Carta a caja del indice: "+boxindex+" y posicion: "+boxPoints[boxindex].transform.position);
        boxPoints[boxindex].SetActive(true);
        //boxCard.transform.position = boxPoints[boxindex].transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideBoxRoom = true;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideBoxRoom = false;
        }
    }
}
