using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System;
public class Ghost : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody ghostRb;
    private Vector3 teletransportPosition;
    private bool isTeletransported= false;
    private bool freezeGhost;
    private Jugador playerScript;
    private bool hasAlmostAllKeys = false;
    private Renderer ghostRenderer;
    private Collider ghostCollider;
    private float randomTimeScare;
    private AudioSource scareAudio;
    private bool isHidden = true;
    private float[] teleportDistance = {2,4,6,10};

    void Start()
    {
        ghostRb = GetComponent<Rigidbody>();
        ghostCollider = GetComponent<Collider>();
        ghostRenderer = GetComponent<Renderer>();
        player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.Log("Player not found");
        }
        else{
            playerScript = player.GetComponent<Jugador>();
        }
        HideGhost();
        initializeVariables();

    }

    private void initializeVariables()
    {
        health = 20;
        maxHealth = health;
        speed = 4;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            float corduraPlayer = playerScript.getCordura();
            //hasAlmostAllKeys = playerScript.getCordura() > 7;
            Debug.Log(corduraPlayer);

            if(corduraPlayer<=5 || hasAlmostAllKeys)
            {
                //gameObject.SetActive(true);
                ShowGhost();
            }
            else if(corduraPlayer>5 && !isTeletransported)
            {
                isTeletransported = true;
                randomTimeScare = UnityEngine.Random.Range(10, 20);
                StartCoroutine( WaitAction(randomTimeScare, ScarePlayer));
            }

        }
    }
    private void HideGhost()
    {
        isHidden = true;
        ghostRenderer.enabled = false;
        ghostCollider.enabled = false;
        ghostRb.linearVelocity = Vector3.zero;
        ghostRb.angularVelocity = Vector3.zero;
        ghostRb.isKinematic = true;
        
    }
    private void HideGhost(bool TeletransporActive)
    {
        isHidden = true;
        ghostRenderer.enabled = false;
        ghostCollider.enabled = false;
        ghostRb.linearVelocity = Vector3.zero;
        ghostRb.angularVelocity = Vector3.zero;
        ghostRb.isKinematic = true;
        if (isTeletransported)
        {
            isTeletransported = !TeletransporActive;
        }
    }

    private void ShowGhost()
    {
        isHidden = false;
        ghostRenderer.enabled = true;
        ghostCollider.enabled = true;
        ghostRb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        Debug.Log("Follow");
        FollowPlayer(ghostRb);

    }

    private void Teletransport()
    {
        Debug.Log("Makes the teletransport");
        if (isHidden)
        {
            ShowGhost();
        }
        Vector3 directionToPlayer =
            player.transform.position -  ghostRb.position;
        directionToPlayer.Normalize();
        int index = UnityEngine.Random.Range(0, teleportDistance.Length);
        teletransportPosition = player.transform.position - directionToPlayer * teleportDistance[index];
        ghostRb.position = teletransportPosition;
        isTeletransported = true;
        StartCoroutine(WaitAction(20, HideGhost, isTeletransported));


    }

    private void ScarePlayer()
    {
        Teletransport();
    }

    private IEnumerator WaitAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
    private IEnumerator WaitAction(float waitTime, Action<bool> action,bool activateTeletransport)
    {
        yield return new WaitForSeconds(waitTime);
        action(activateTeletransport);
    }
}
