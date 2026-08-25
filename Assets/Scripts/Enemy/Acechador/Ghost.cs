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
    public bool hasAlmostAllKeys = false;
    private Renderer ghostRenderer;
    private Collider ghostCollider;
    private float randomTimeScare;
    private AudioSource scareAudio;
    private bool isHidden = true;
    private float[] teleportDistance = { 7, 10, 15, 20 };
    public bool isGhostAlwaysActive { get; set; }

    // NUEVA FEATURE: Ajusta la altura del Ghost respecto al Player
    [SerializeField] private float ghostHeightOffset = 1.0f;

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
            if (playerScript.playerGetsInroom)
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
        // NUEVA FEATURE: Si prácticamente no se está moviendo, mira al Player
        if (ghostRb.linearVelocity.magnitude < 0.1f)
        {
            LookPlayer();
        }
    }
    // NUEVA FEATURE: Si el Ghost está detenido gira para mirar al Player
    private void LookPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - ghostRb.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer.normalized, Vector3.up);
            ghostRb.MoveRotation(Quaternion.Slerp(ghostRb.rotation, targetRotation, 5f * Time.fixedDeltaTime));
        }
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

        // NUEVA FEATURE: Aparece alrededor del jugador
        teletransportPosition = player.transform.position + directionToPlayer * teleportDistance[index];

        // NUEVA FEATURE: Usa la altura del Player más el offset del pivote del Ghost
        teletransportPosition.y = player.transform.position.y + ghostHeightOffset;

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