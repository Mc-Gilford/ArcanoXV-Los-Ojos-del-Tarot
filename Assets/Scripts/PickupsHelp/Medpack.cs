using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Medpack : MonoBehaviour
{
    private int healPoints;
    [SerializeField] private float pickupTime;
    public Vector3 initPos; 

     private Renderer[] renders;
    
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float elevateHeight;
    [SerializeField] private float blinkInterval;
    [SerializeField] private float timeForBlink;

    [SerializeField] private bool isBlinking;
    private Jugador jugadorScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        healPoints = 1;
        pickupTime = 10f;
        rotationSpeed = 40f;
    }
    
    void Start()
    {
        initPos = transform.position;
        elevateHeight = 0.5f;
        blinkInterval = 0.1f;
        timeForBlink = 3f;
        isBlinking = false;
        renders = GetComponentsInChildren<Renderer>();
        jugadorScript = GameObject.Find("Player").GetComponent<Jugador>();
    }

    // Update is called once per frame
    void Update()
    {
        DissapearCountDown();
        RotateAnimation();
    }

    private void DissapearCountDown()
    {
        if(pickupTime <= timeForBlink && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(Dissapear());
        }
        else if(pickupTime >0)
        {
            pickupTime -= Time.deltaTime;
        }
        else if (pickupTime <= 0)
        {
            DestroyMedpack();
        }
    }

    private void RotateAnimation()
    {
        float wave = (Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f);
        transform.position = initPos + Vector3.up * (wave * elevateHeight);
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
    }

    IEnumerator Dissapear()
    {
        while(pickupTime > 0)
        {
            foreach (Renderer childRenders in renders)
            {
                childRenders.enabled = !childRenders.enabled;
            }
            yield return new WaitForSeconds(blinkInterval);
        }
    }

    private void DestroyMedpack()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("I need healing");
        if(collision.gameObject.CompareTag("Player"))
        {
            
            Destroy(gameObject);
            jugadorScript.HealHP();
        }
    }
}
