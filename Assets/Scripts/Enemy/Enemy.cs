using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Enemy : Character
{
    [Header("References")]
    [SerializeField] public GameObject player;

    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] public float anger = 0.9f;
    [SerializeField] public float nearDistance = 25f;
    [SerializeField] public float slowDistance = 5f;
    [SerializeField] private float stoppingDistance = 1f;
    [SerializeField] private float detectionDistance = 150f; // NUEVO: distancia máxima para perseguir
    [SerializeField] private float wallForce = 5f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] public float maximumSpeed = 8f;
    [SerializeField] private float airAcceleration = 12f;
    [SerializeField] private float maximumFallSpeed = 12f;

    [Header("Wall Jump")]
    [SerializeField] private float wallDetachTime = 0.35f;
    [SerializeField] private float wallJumpForwardForce = 3f;
    [SerializeField] private float wallJumpUpForce = 2f;

    [Header("Landing")]
    [SerializeField] private float minimumAirTime = 0.25f;

    [Header("Random Ground Jump")]
    [SerializeField] private float minimumJumpTime = 2f;
    [SerializeField] private float maximumJumpTime = 8f;

    [Header("State")]
    [SerializeField] public bool isNear;
    [SerializeField] private bool isOnWall;
    [SerializeField] private bool isOnGround;
    [SerializeField] private bool isWaitingToJump;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isInRoom;
    [SerializeField] private bool IsAngerActive;

    public int damage { get; set; }

    private NavMeshAgent agent;
    private Rigidbody enemyrb;
    private Coroutine randomJumpCoroutine;
    private Vector3 surfaceNormal = Vector3.up;
    private float wallDetachTimer;
    private float airTimeRemaining;

    public float maxHealth;

    void Start()
    {
        enemyrb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            Debug.Log("Player not found");
        }

        initializeVariables();
        if (agent != null)
        {
            agent.speed = speed * anger;
            agent.stoppingDistance = stoppingDistance;
            agent.acceleration = 12f;
            agent.angularSpeed = 360f;
        }
        StartCoroutine(WaitAnger());
    }

    private void EnableNavMesh()
    {
        if (agent == null)
            return;

        if (!agent.enabled)
        {
            agent.enabled = true;
        }

        enemyrb.isKinematic = true;
        enemyrb.useGravity = false;
    }

    private void DisableNavMesh()
    {
        if (agent != null && agent.enabled)
        {
            agent.enabled = false;
        }

        enemyrb.isKinematic = false;
    }

    private void initializeVariables()
    {
        int damageRandom = UnityEngine.Random.Range(1, 3);
        damage = damageRandom;

        int lifeLevelRandom = UnityEngine.Random.Range(8, 10);
        health = lifeLevelRandom;

        maxHealth = health;
        speed = 3;
    }

    public void IncreaseAngry()
    {
        if (health <= maxHealth / 2 && anger <= 5)
        {
            anger = anger + 0.01f;
        }
    }
    private void FollowPlayerNavMesh()
    {
        if (agent == null)
            return;

        EnableNavMesh();

        if (!agent.isOnNavMesh)
            return;

        agent.speed = speed * anger;
        agent.stoppingDistance = stoppingDistance;

        agent.SetDestination(player.transform.position);
    }

    private void FixedUpdate()
    {
        if (player == null)
            return;

        // NUEVO:
        // Si el jugador está a más de 150 unidades,
        // el enemigo deja de perseguirlo.
        float playerDistance = Vector3.Distance(
            player.transform.position,
            enemyrb.position
        );

        if (playerDistance > detectionDistance && !gameObject.CompareTag("Follower"))
        {
            enemyrb.linearVelocity = Vector3.zero;
            CancelRandomJump();
            return;
        }
        if (!gameObject.CompareTag("Follower"))
        {
            isNear = isPlayerNear();
        }

        if (airTimeRemaining > 0f)
        {
            airTimeRemaining -= Time.fixedDeltaTime;
        }

        // Si está saltando, persigue al jugador en el aire
        if (isJumping)
        {
            /*
             * Durante unos instantes conserva el impulso
             * necesario para despegarse de la pared.
             */
            if (wallDetachTimer > 0f)
            {
                wallDetachTimer -= Time.fixedDeltaTime;
            }
            else
            {
                FollowPlayerInAir();
            }

            RotateFallingUpright();
        }

        // En la pared solamente salta cuando el jugador está cerca
        else if (isOnWall && isNear)
        {
            CancelRandomJump();
            jumpAttack();
        }

        // En la pared y lejos nunca salta
        else if (isOnWall && !isNear)
        {
            CancelRandomJump();
            FollowPlayerOnWall();
        }

        // Si no está en la pared, sigue al jugador
        else
        {
            //FollowPlayer(enemyrb);
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                FollowPlayerNavMesh();
            else
                FollowPlayer(enemyrb);
        }

        // Al tocar el suelo continúa girando hasta quedar de pie
        if (isOnGround && !isOnWall)
        {
            RotateFallingUpright();
        }

        // Los saltos aleatorios solamente ocurren en el suelo
        if (isOnGround && !isOnWall && !isWaitingToJump && !isJumping && !gameObject.CompareTag("Follower"))
        {
            randomJumpCoroutine = StartCoroutine(WaitJump());
        }

        IncreaseAngry();
        ControlMaximumVelocity();
    }

    private IEnumerator WaitAnger()
    {
        IsAngerActive = true;

        yield return new WaitForSeconds(300);

        if (anger <= 5)
        {
            anger++;
        }
    }

    private IEnumerator WaitJump()
    {
        isWaitingToJump = true;

        float timeJump = UnityEngine.Random.Range(
            minimumJumpTime,
            maximumJumpTime
        );

        yield return new WaitForSeconds(timeJump);

        /*
         * Solamente realiza el salto aleatorio si todavía
         * continúa en el suelo y no está tocando una pared.
         */
        if (isOnGround && !isOnWall && !isJumping)
        {
            jumpAttack();
        }

        isWaitingToJump = false;
        randomJumpCoroutine = null;
    }

    private void CancelRandomJump()
    {
        if (randomJumpCoroutine != null)
        {
            StopCoroutine(randomJumpCoroutine);
            randomJumpCoroutine = null;
        }

        isWaitingToJump = false;
    }

    private void jumpAttack()
    {
        DisableNavMesh();
        bool jumpingFromWall = isOnWall;

        enemyrb.useGravity = true;
        enemyrb.linearVelocity = Vector3.zero;
        enemyrb.angularVelocity = Vector3.zero;

        Vector3 directionToPlayer =
            player.transform.position - enemyrb.position;

        if (jumpingFromWall)
        {
            /*
             * Obtiene la dirección hacia el jugador,
             * pero solamente sobre la pared.
             */
            Vector3 directionAlongWall =
                Vector3.ProjectOnPlane(
                    directionToPlayer,
                    surfaceNormal
                ).normalized;

            /*
             * surfaceNormal lo separa de la pared.
             * directionAlongWall lo dirige hacia el jugador.
             * Vector3.up agrega altura.
             */
            Vector3 wallJumpVelocity =
                surfaceNormal * jumpForce +
                directionAlongWall * wallJumpForwardForce +
                Vector3.up * wallJumpUpForce;

            enemyrb.AddForce(
                wallJumpVelocity,
                ForceMode.VelocityChange
            );

            /*
             * Durante este tiempo el seguimiento aéreo
             * no puede cancelar el impulso de salida.
             */
            wallDetachTimer = wallDetachTime;
        }
        else
        {
            /*
             * Los saltos desde el suelo son aleatorios,
             * pero se dirigen horizontalmente hacia el jugador.
             */
            Vector3 horizontalDirection =
                Vector3.ProjectOnPlane(
                    directionToPlayer,
                    Vector3.up
                ).normalized;

            Vector3 groundJumpVelocity =
                Vector3.up * jumpForce +
                horizontalDirection * wallJumpForwardForce;

            enemyrb.AddForce(
                groundJumpVelocity,
                ForceMode.VelocityChange
            );

            wallDetachTimer = 0f;
        }

        isOnWall = false;
        isOnGround = false;
        isJumping = true;
        airTimeRemaining = minimumAirTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;

            /*
             * Si acaba de saltar, no termina el aterrizaje
             * inmediatamente aunque roce el suelo.
             */
            if (isJumping && airTimeRemaining > 0f)
                return;

            FinishLanding();
        }

        if ((collision.gameObject.CompareTag("Wall") ||
             collision.gameObject.CompareTag("Roof")) &&
            !isJumping && !gameObject.CompareTag("Follower"))
        {
            isOnWall = true;
            DisableNavMesh();

            // En una pared se cancela el salto aleatorio
            CancelRandomJump();
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(2);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Room") && !gameObject.CompareTag("Follower"))
        {
            //Destroy(gameObject);
            Die();
        }
    }

    public void FollowPlayer(Rigidbody enemyRb)
    {
        enemyRb.useGravity = true;

        Vector3 directionToPlayer =
            player.transform.position - enemyRb.position;

        // En el suelo solamente persigue horizontalmente
        directionToPlayer.y = 0f;

        float distanceToPlayer =
            directionToPlayer.magnitude;

        /*
         * Siempre intenta mirar al jugador, aunque ya
         * se encuentre dentro de stoppingDistance.
         */
        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            Vector3 lookDirection =
                directionToPlayer.normalized;

            RotateEnemy(
                lookDirection,
                Vector3.up,
                enemyRb
            );
        }

        // Si ya está cerca, se frena
        if (distanceToPlayer <= stoppingDistance)
        {
            Vector3 velocity =
                enemyRb.linearVelocity;

            velocity.x = 0f;
            velocity.z = 0f;

            enemyRb.linearVelocity = velocity;

            return;
        }

        /*
         * Reduce gradualmente la velocidad cuando
         * comienza a acercarse al jugador.
         */
        float distanceFactor =
            Mathf.InverseLerp(
                stoppingDistance,
                slowDistance,
                distanceToPlayer
            );

        float currentSpeed =
            speed * anger * distanceFactor;

        Vector3 targetPosition =
            player.transform.position;

        targetPosition.y =
            enemyRb.position.y;

        /*
         * MoveTowards evita que el enemigo se pase
         * de la posición del jugador.
         */
        Vector3 nextPosition =
            Vector3.MoveTowards(
                enemyRb.position,
                targetPosition,
                currentSpeed * Time.fixedDeltaTime
            );

        enemyRb.MovePosition(nextPosition);
    }

    private void FollowPlayerOnWall()
    {
        enemyrb.useGravity = false;

        Vector3 directionToPlayer =
            player.transform.position - enemyrb.position;

        // Evita que el enemigo se salga de la pared
        Vector3 wallDirection =
            Vector3.ProjectOnPlane(
                directionToPlayer,
                surfaceNormal
            ).normalized;

        MoveEnemy(wallDirection);

        // Sostiene al enemigo en la pared
        enemyrb.AddForce(
            -surfaceNormal * wallForce,
            ForceMode.Acceleration
        );

        // Se ajusta con la rotación de la pared
        RotateEnemy(
            wallDirection,
            surfaceNormal,
            enemyrb
        );
    }

    private void FollowPlayerInAir()
    {
        Vector3 directionToPlayer =
            player.transform.position - enemyrb.position;

        Vector3 horizontalDirection =
            Vector3.ProjectOnPlane(
                directionToPlayer,
                Vector3.up
            ).normalized;

        Vector3 currentVelocity =
            enemyrb.linearVelocity;

        Vector3 verticalVelocity =
            Vector3.up * currentVelocity.y;

        Vector3 horizontalVelocity =
            Vector3.ProjectOnPlane(
                currentVelocity,
                Vector3.up
            );

        Vector3 desiredVelocity =
            horizontalDirection * maximumSpeed;

        /*
         * Frena y cambia de dirección si sobrepasa
         * la posición del jugador.
         */
        horizontalVelocity =
            Vector3.MoveTowards(
                horizontalVelocity,
                desiredVelocity,
                airAcceleration * Time.fixedDeltaTime
            );

        enemyrb.linearVelocity =
            horizontalVelocity + verticalVelocity;
    }

    private void MoveEnemy(Vector3 direction)
    {
        // Calculamos el movimiento del enemigo
        float movementAmount =
            speed * anger * Time.fixedDeltaTime;

        Vector3 targetPosition =
            enemyrb.position +
            direction * movementAmount;

        Vector3 nextPosition =
            Vector3.MoveTowards(
                enemyrb.position,
                targetPosition,
                movementAmount
            );

        enemyrb.MovePosition(nextPosition);
    }

    private void ControlMaximumVelocity()
    {
        Vector3 velocity =
            enemyrb.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        // Evita que la velocidad horizontal aumente sin control
        if (horizontalVelocity.magnitude > maximumSpeed)
        {
            horizontalVelocity =
                horizontalVelocity.normalized *
                maximumSpeed;
        }

        // Evita que caiga demasiado rápido
        float verticalSpeed =
            Mathf.Max(
                velocity.y,
                -maximumFallSpeed
            );

        enemyrb.linearVelocity =
            new Vector3(
                horizontalVelocity.x,
                verticalSpeed,
                horizontalVelocity.z
            );
    }

    private void RotateEnemy(
        Vector3 direction,
        Vector3 upDirection,
        Rigidbody enemyRB)
    {
        // Evita calcular rotación si no existe dirección
        if (direction.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction,
                upDirection
            );

        Quaternion smoothRotation =
            Quaternion.Slerp(
                enemyRB.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        enemyRB.MoveRotation(smoothRotation);
    }

    private void RotateFallingUpright()
    {
        Vector3 directionToPlayer =
            player.transform.position - enemyrb.position;

        /*
         * Quita la inclinación vertical para que mire al
         * jugador sin caer de cabeza.
         */
        Vector3 horizontalDirection =
            Vector3.ProjectOnPlane(
                directionToPlayer,
                Vector3.up
            ).normalized;

        if (horizontalDirection.sqrMagnitude <= 0.01f)
        {
            horizontalDirection =
                Vector3.ProjectOnPlane(
                    transform.forward,
                    Vector3.up
                ).normalized;
        }

        if (horizontalDirection.sqrMagnitude <= 0.01f)
        {
            horizontalDirection = Vector3.forward;
        }

        // Mira al jugador y mantiene los pies hacia el suelo
        Quaternion uprightRotation =
            Quaternion.LookRotation(
                horizontalDirection,
                Vector3.up
            );

        Quaternion smoothRotation =
            Quaternion.Slerp(
                enemyrb.rotation,
                uprightRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

        enemyrb.MoveRotation(smoothRotation);
    }

    private void FinishLanding()
    {
        isJumping = false;
        isOnGround = true;
        isOnWall = false;

        wallDetachTimer = 0f;
        airTimeRemaining = 0f;

        enemyrb.angularVelocity = Vector3.zero;

        Vector3 directionToPlayer =
            player.transform.position - enemyrb.position;

        Vector3 horizontalDirection =
            Vector3.ProjectOnPlane(
                directionToPlayer,
                Vector3.up
            ).normalized;

        if (horizontalDirection.sqrMagnitude <= 0.01f)
        {
            horizontalDirection =
                Vector3.ProjectOnPlane(
                    transform.forward,
                    Vector3.up
                ).normalized;
        }

        if (horizontalDirection.sqrMagnitude <= 0.01f)
        {
            horizontalDirection = Vector3.forward;
        }

        /*
         * Al aterrizar aplica directamente la rotación vertical.
         * Esto evita que permanezca acostado.
         */
        Quaternion uprightRotation =
            Quaternion.LookRotation(
                horizontalDirection,
                Vector3.up
            );

        enemyrb.MoveRotation(uprightRotation);
        EnableNavMesh();
    }

    public bool isPlayerNear()
    {
        float distance =
            Vector3.Distance(
                player.transform.position,
                enemyrb.position
            );

        return distance <= nearDistance;
    }

    private void OnCollisionStay(Collision collision)
    {
        /*
         * Si continúa tocando el suelo después del tiempo mínimo
         * y ya está cayendo, termina el aterrizaje.
         */
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;

            if (isJumping &&
                airTimeRemaining <= 0f &&
                enemyrb.linearVelocity.y <= 0.1f)
            {
                FinishLanding();
            }

            return;
        }

        // Evita que vuelva a pegarse a la pared durante el salto
        if (isJumping)
            return;

        bool isClimbableSurface =
            collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Roof");

        if (gameObject.CompareTag("Follower"))
            return;

        if (!isClimbableSurface)
            return;

        ContactPoint contact =
            collision.GetContact(0);

        surfaceNormal =
            contact.normal;

        isOnWall = true;

        // En una pared cancela cualquier salto aleatorio
        CancelRandomJump();

        Debug.DrawRay(
            contact.point,
            surfaceNormal * 3f,
            Color.green
        );
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;

            // Al abandonar el suelo cancela la espera aleatoria
            CancelRandomJump();
        }

        bool leftClimblableSurface =
            collision.gameObject.CompareTag("Wall") ||
            collision.gameObject.CompareTag("Roof");

        if (leftClimblableSurface)
        {
            isOnWall = false;
        }
    }
}