using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System;

public class Ghost : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Rigidbody ghostRb;
    private Vector3 teletransportPosition;
    private bool isTeletransported = false;
    private bool freezeGhost;
    private Jugador playerScript;
    private bool hasAlmostAllKeys = false;
    private Renderer ghostRenderer;
    private Collider ghostCollider;
    private float randomTimeScare;
    private AudioSource scareAudio;
    private bool isHidden = true;
    private float[] teleportDistance = { 2, 4, 6, 10 };
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
        else
        {
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
        if (player != null && playerScript != null)
        {
            // NUEVA FEATURE: isInsane debe ser true cuando la cordura sea menor a 3
            bool corduraPlayer = playerScript.isInsane;

            // Aquí posteriormente puedes actualizar hasAlmostAllKeys con las tarjetas del jugador
            // hasAlmostAllKeys = playerScript...

            // NUEVA FEATURE: Cordura < 3 O casi todas las tarjetas = Ghost siempre activo
            if ((corduraPlayer || hasAlmostAllKeys) && !isTeletransported)
            {
                Debug.Log("Ghost always active");

                isGhostAlwaysActive = true;
                isTeletransported = true;

                if (isHidden)
                {
                    ShowGhost();
                }

                // NUEVA FEATURE: Estando activo puede teletransportarse nuevamente cada 60-90 segundos
                randomTimeScare = UnityEngine.Random.Range(60, 90);
                StartCoroutine(WaitAction(randomTimeScare, ScarePlayer));
            }
            // NUEVA FEATURE: Cordura >= 3 Y sin casi todas las tarjetas = apariciones cortas
            else if ((!corduraPlayer && !hasAlmostAllKeys) && !isTeletransported)
            {
                Debug.Log("Ghost scare mode");

                isGhostAlwaysActive = false;
                isTeletransported = true;

                randomTimeScare = UnityEngine.Random.Range(10, 20);
                StartCoroutine(WaitAction(randomTimeScare, ScarePlayer));
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

        // NUEVA FEATURE: Permite programar una nueva aparición después de ocultarse
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
        // NUEVA FEATURE: El Ghost no intenta seguir al jugador mientras está oculto
        if (isHidden || player == null)
        {
            return;
        }

        Debug.Log("Follow");
        FollowPlayer(ghostRb);
    }

    private void Teletransport()
    {
        Debug.Log("Makes the teletransport");

        if (player == null)
        {
            return;
        }

        if (isHidden)
        {
            ShowGhost();
        }

        // NUEVA FEATURE: Selecciona una dirección aleatoria alrededor del jugador
        Vector3 directionToPlayer = UnityEngine.Random.insideUnitSphere;
        directionToPlayer.y = 0f;

        // NUEVA FEATURE: Evita una dirección prácticamente vacía
        if (directionToPlayer.sqrMagnitude < 0.01f)
        {
            directionToPlayer = Vector3.forward;
        }

        directionToPlayer.Normalize();

        int index = UnityEngine.Random.Range(0, teleportDistance.Length);

        // NUEVA FEATURE: El Ghost aparece alrededor del jugador sin depender de su posición anterior
        teletransportPosition = player.transform.position + directionToPlayer * teleportDistance[index];

        // NUEVA FEATURE: Mantiene la altura del jugador para permitir apariciones en otros pisos
        teletransportPosition.y = player.transform.position.y;

        ghostRb.linearVelocity = Vector3.zero;
        ghostRb.angularVelocity = Vector3.zero;
        ghostRb.position = teletransportPosition;

        isTeletransported = true;

        // Si no está en modo permanente, desaparece después de 20 segundos
        if (!isGhostAlwaysActive)
        {
            StartCoroutine(WaitAction(20, HideGhost, isTeletransported));
        }
        else
        {
            // NUEVA FEATURE: En modo permanente permite programar otro teletransporte
            isTeletransported = false;
        }
    }

    private void ScarePlayer()
    {
        Debug.Log("Scare Player");
        Teletransport();
    }

    private IEnumerator WaitAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }

    private IEnumerator WaitAction(float waitTime, Action<bool> action, bool activateTeletransport)
    {
        yield return new WaitForSeconds(waitTime);
        action(activateTeletransport);
    }
}