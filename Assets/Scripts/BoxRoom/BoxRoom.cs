using UnityEngine;
 using System.Collections.Generic;

public class BoxRoom : MonoBehaviour
{
    [SerializeField] private bool hasEntered=false;
    [SerializeField] private bool isInsideBoxRoom=false;

    public List<GameObject> boxPoints;

    public GameObject boxCard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isInsideBoxRoom = false;
        hasEntered = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MoveCardInsideBox()
    {
        int boxindex = Random.Range(0, boxPoints.Count);
        boxCard.transform.position = boxPoints[boxindex].transform.position;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isInsideBoxRoom = true;
            
            if(!hasEntered)
            {
                MoveCardInsideBox();
                hasEntered = true;
            }
            
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
