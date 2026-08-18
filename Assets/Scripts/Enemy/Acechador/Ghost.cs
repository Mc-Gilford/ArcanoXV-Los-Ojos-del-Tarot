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
    private bool isGhostAlwaysActive; 

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
            bool corduraPlayer = playerScript.isInsane;
            //hasAlmostAllKeys = playerScript.getCordura() > 7;
            Debug.Log(corduraPlayer);

            if((corduraPlayer || hasAlmostAllKeys) && !isTeletransported)
            {
                //gameObject.SetActive(true);
                isGhostAlwaysActive = true;
                isTeletransported = true;
                ShowGhost();
                randomTimeScare = UnityEngine.Random.Range(60, 90);
                StartCoroutine(WaitAction(randomTimeScare, ScarePlayer));
            }

            else if((corduraPlayer && !hasAlmostAllKeys) &&!isTeletransported)
            {
                isTeletransported = true;
                isGhostAlwaysActive = false;
               
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
        if (!isGhostAlwaysActive)
        {
            StartCoroutine(WaitAction(20, HideGhost, isTeletransported));
        }
        else{
            isTeletransported = false;
        }


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
