using UnityEngine;

public class SafeRoom : MonoBehaviour
{
    private Jugador playerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<Jugador>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerScript.isInsideSafeRoom = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerScript.isInsideSafeRoom = false;
        }
    }
}
