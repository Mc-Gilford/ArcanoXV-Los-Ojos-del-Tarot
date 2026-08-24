using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using DigitalRuby.RainMaker;

public class Jugador : Character
{

    private PlayerInput inputJugador;

    private InputAction accionMovimiento;

    private InputAction accionSalto;

    private InputAction sprintAction;

    private InputAction forwardDashAction;

    private InputAction backwardDashAction;

    private InputAction leftDashAction;

    private InputAction rightDashAction;

    private Rigidbody rb;
    
    private Vector2 direccion; 

    private bool puedeSaltar = true;

    private float lastTapW = 0f;
    private float lastTapS = 0f;
    private float lastTapA = 0f;
    private float lastTapD = 0f;
    private int drunkEffect = 10;

    public AudioClip[] stepClips;

    [SerializeField] private bool isRunning= false;
    [SerializeField] private bool isAbleToRun= true;
    [SerializeField] private bool isTired= false;
    [SerializeField] private bool canDash = true;
    public bool isInsane { get; private set; }
    [SerializeField] private bool isInsideSafeRoom = false;
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float salto = 10f;
    [SerializeField] private float stamina = 10f;
    [SerializeField] private float cordura = 600f;
    [SerializeField] private float sanityTimer=0f;
    [SerializeField] private int sanityPoints=10;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float timeDoubleTap = 0.2f;
    [SerializeField] private float invertedControllers = 1f;
    [SerializeField] private bool isDrunk = false;
    [SerializeField] private int drunkShield = 1;

    private GameObject rainPrefab;
    private RainScript rain;

    //Variables para la reproduccion de audio

    [Tooltip("Distancia recorrida (m) entre cada paso.")]
    public float stepDistance = 0.75f;
    [Tooltip("Volumen de los pasos.")]
    [Range(0f, 1f)] public float volume = 0.7f;
    [Tooltip("Variación de tono entre pasos para que no parezca grabado.")]
    public float pitchJitter = 0.12f;
    private Vector3 _lastPos;
    private float _accumulated;
    private float _lastMovedTime = -1f;      // cuándo caminó por última vez
    private const float _stopGrace = 0.3f;   // gracia para cortar el crujido al detenerse


    
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rainPrefab = GameObject.Find("RainPrefab");
        if(rainPrefab!=null)
        {
            Debug.Log("Si exixte rain");
            rain = rainPrefab.GetComponent<RainScript>();
        }
        if(audioSource != null)
        {
            Debug.Log("Si hay audio jsjs");
        }
        _lastPos = transform.position;
        DefinePlayerData();
    }

    public void DefinePlayerData()
    {
        dashForce = 8f;
        salto = 10f;
        cordura = 600f;
        puedeSaltar = true;
        isInsane = false;
        health = 4;
        inputJugador = GetComponent<PlayerInput>();
        accionMovimiento = inputJugador.actions.FindAction("Movimiento");
        accionSalto = inputJugador.actions.FindAction("Saltar");
        sprintAction = inputJugador.actions.FindAction("Run");
        forwardDashAction = inputJugador.actions.FindAction("WDash");
        forwardDashAction.started += ctx => ForwardDash();
        backwardDashAction = inputJugador.actions.FindAction("SDash");
        backwardDashAction.started += ctx => BackwardDash();
        leftDashAction = inputJugador.actions.FindAction("ADash");
        leftDashAction.started += ctx => LeftDash();
        rightDashAction = inputJugador.actions.FindAction("DDash");
        rightDashAction.started += ctx => RightDash();

        sprintAction.started += ctx => Run();
        sprintAction.canceled += ctx => StopRunning();
        rb = GetComponent<Rigidbody>();
        //StartCoroutine(GetDrunk());
    }

    public float getCordura()
    {
        return cordura;
    }

    // Update is called once per frame
    void Update()
    {
        MoverJugador();
        ValidateStamina();
        ValidateSanity();
    }

    private void ValidateSanity()
    {
        if(cordura >= 0 && !isInsideSafeRoom)
        {
            cordura -= Time.deltaTime;
            sanityTimer += Time.deltaTime;
            DecreaseSanity();
            if(!isInsane && sanityPoints <=3)
            {
                Debug.Log("Me voy a volver loco aqui");
                isInsane = true;
            }
        }
    }

    private void DecreaseSanity()
    {
        if(sanityTimer >= 60)
        {
            sanityPoints--;
            sanityTimer=0;
        }
    }

    private void ValidateStamina()
    {
        if(accionSalto.triggered && puedeSaltar)
        {
            puedeSaltar = false;
            Saltar();
        }

        if(stamina > 0 && isRunning && isAbleToRun)
        {
            stamina -= Time.deltaTime; 
        }
        else if(!isRunning && stamina <10 && !isTired)
        {
            stamina += Time.deltaTime;
        }
        else if(stamina <=0 && isAbleToRun)
        {
            isAbleToRun = false;
            isRunning = false;
            isTired = true;
            canDash = false;
            velocidad /= 3f;
            StartCoroutine(FullRechargeStamina());
        }
    } 

    void FixedUpdate()
    {
        
    }

    private void SavedProgress()
    {
        StartCoroutine(RecoverSanity());
    }

    IEnumerator RecoverSanity()
    {
        Debug.Log("Recobrando la cordura");
        yield return new WaitForSeconds(1);
        cordura = 540f;
        isInsane = false;
    }

    IEnumerator FullRechargeStamina()
    {
        Debug.Log("Recargando stamina");

        yield return new WaitForSeconds(10);
        stamina = 10;
        canDash = true;
        isTired = false;
        isAbleToRun = true;
    }

    IEnumerator GetDrunk()
    {
        invertedControllers = -1f;
        isDrunk=true;
        drunkShield = 0;
        yield return new WaitForSeconds(drunkEffect);
        invertedControllers = 1f;
        isDrunk=false;
        drunkShield = 1;
        //GoToSafeRoom();
    }

    private void Saltar () 
    {
        rb.AddForce(Vector3.up * salto, ForceMode.Impulse);
    }

    private void Shoot()
    {
        Debug.Log("Pow");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hola");
        if(collision.collider.CompareTag("Ground"))
        {
            puedeSaltar = true;
        }

        if(collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Follower"))
        {
            TakeDamage(1*drunkShield);
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("COLISIONANDO CON: " + collider.name);
        Debug.Log("TAG: " + collider.gameObject.tag);

        if (collider.CompareTag("Home"))
        {
            Debug.Log("The tags is right");

            if (rain != null)
            {
                Debug.Log("Aqui entra");
                rain.FollowCamera = false;
                rain.RainIntensity = 0.2f;
                rain.RainMistThreshold = 0.1f;
            }
            else
            {
                Debug.Log("RAIN ES NULL");
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Home"))
        {
            if (rain != null)
            {
                rain.FollowCamera = true;
                rain.RainIntensity = 1;
                rain.RainMistThreshold = 0.7f;
            }
        }
    }

    private void MoverJugador()
    {
        direccion = accionMovimiento.ReadValue<Vector2>();   

        Vector3 forward  = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        //Evitar que el personaje se incline
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = (forward * (direccion.y*invertedControllers) + right * (direccion.x*invertedControllers)) * velocidad * Time.deltaTime;

        transform.position += movement;
        IsMoving();
    }

    private void Run()
    {
        if(isAbleToRun)
        {
            isRunning = true;
            velocidad *= 3f;
        }
    }

    private void StopRunning()
    {
        if(!isTired && isRunning)
        {
            isRunning = false;
            velocidad /= 3f;
        }
        
    }

    private void ForwardDash()
    {
        Debug.Log("W Tap Detected");
        float currenTime = Time.time;

        if(currenTime - lastTapW <= timeDoubleTap)
        {
            switch(invertedControllers)
            {
                case 1:
                    ManageDash("forward");
                    break;
                case -1:
                    ManageDash("backward");
                    break;
            }
            lastTapW = 0f;

        }else
        {
            lastTapW = currenTime;
        }

        
    }

    private void LeftDash()
    {
        Debug.Log("A Tap Detected");
        float currenTime = Time.time;

        if(currenTime - lastTapA <= timeDoubleTap)
        {
            switch(invertedControllers)
            {
                case 1:
                    ManageDash("left");
                    break;
                case -1:
                    ManageDash("right");
                    break;
            }
            
            lastTapA = 0f;

        }else
        {
            lastTapA = currenTime;
        }

        
    }

    private void BackwardDash()
    {
        Debug.Log("S Tap Detected");
        float currenTime = Time.time;

        if(currenTime - lastTapS <= timeDoubleTap)
        {
            switch(invertedControllers)
            {
                case 1:
                    ManageDash("backward");
                    break;
                case -1:
                    ManageDash("forward");
                    break;
            }
            lastTapS = 0f;

        }else
        {
            lastTapS = currenTime;
        }

        
    }

    private void RightDash()
    {
        Debug.Log("D Tap Detected");
        float currenTime = Time.time;

        if(currenTime - lastTapD <= timeDoubleTap)
        {
            switch(invertedControllers)
            {
                case 1:
                    ManageDash("right");
                    break;
                case -1:
                    ManageDash("left");
                    break;
            }
            lastTapD = 0f;

        }else
        {
            lastTapD = currenTime;
        }

        
    }

    private void IsMoving()
    {
        if (stepClips == null || stepClips.Length == 0) return;

        Vector3 pos = transform.position;
        float walkDist = Vector3.Distance(
            new Vector3(pos.x, 0f, pos.z),
            new Vector3(_lastPos.x, 0f, _lastPos.z));
        _lastPos = pos;

        if (walkDist <= 0.0001f)
        {
            // Dejó de caminar: corta el crujido en curso tras una pequeña gracia,
            // para que no siga sonando como si aún estuviera andando.
            if (audioSource.isPlaying && Time.time - _lastMovedTime > _stopGrace)
                audioSource.Stop();
            return;
        }

        _lastMovedTime = Time.time;
        _accumulated += walkDist;
        if (_accumulated < stepDistance) return;

        if (audioSource.isPlaying)
        {
            // Un crujido a la vez: espera a que termine el paso actual antes de
            // que suene el siguiente (nada de pasos encimados). Se mantiene el
            // paso "pendiente" para que no se pierda al acabar el anterior.
            _accumulated = stepDistance;
            return;
        }

        _accumulated = 0f;
        PlayStep();
    }

    private void PlayStep()
    {
        AudioClip clip = stepClips[Random.Range(0, stepClips.Length)];
        audioSource.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        audioSource.PlayOneShot(clip, volume);
    }

    private void ManageDash(string typeDash)
    {
        Vector3 dashDirection = new Vector3(0,0,0);
        if(canDash && stamina > 6)
        {
            switch (typeDash)
            {
            
                case "forward":
                    dashDirection = Camera.main.transform.forward;
                    dashDirection.y = 0;
                    DoDash(dashDirection);
                    break;
                case "backward":
                    dashDirection = -Camera.main.transform.forward;
                    dashDirection.y = 0;
                    DoDash(dashDirection);
                    break;
                case "left":
                    dashDirection = -Camera.main.transform.right;
                    dashDirection.y = 0;
                    DoDash(dashDirection);
                    break;
                case "right":
                    dashDirection = Camera.main.transform.right;
                    dashDirection.y = 0;
                    DoDash(dashDirection);
                    break;
            }
        }

    }

    private void DoDash(Vector3 direction)
    {
        Debug.Log("Double dash made");
        rb.AddForce(direction.normalized * dashForce, ForceMode.Impulse);
        stamina -= 6f;
    }

    public void HealHP()
    {
        Debug.Log("Salud curada");
        this.health += 1;
    }

    public void LetsDrink()
    {
        StartCoroutine(GetDrunk());
    }

    public void GoToSafeRoom()
    {
        //Definir un punto de teletransporte dentro de la habitacion segura
        transform.position = new Vector3(-4.01225f,1.05f,-0.66555f);
        isInsideSafeRoom=true;
    }

    public void LoseHalfHP()
    {
        float waste = health % 2;
        float half=0f;
        if(waste == 0)
        {
            health /= 2;
        }
        else
        {
            half = health/2;
            health = Mathf.FloorToInt(half);
        }
    }

    /*private void DashW()
    {
        Debug.Log("Double dash made");
        Vector3 dashDirection = Camera.main.transform.forward;
        dashDirection.y = 0;
        rb.AddForce(dashDirection.normalized * dashForce, ForceMode.Impulse);
    }

    private void DashS()
    {
        Debug.Log("Double dash made");
        Vector3 dashDirection = -Camera.main.transform.forward;
        dashDirection.y = 0;
        rb.AddForce(dashDirection.normalized * dashForce, ForceMode.Impulse);
    }*/
}
