using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

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


    [SerializeField] private bool isRunning= false;
    [SerializeField] private bool isAbleToRun= true;
    [SerializeField] private bool isTired= false;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float salto = 20f;
    [SerializeField] private float stamina = 10f;
    [SerializeField] private float cordura = 10f;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float timeDoubleTap = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dashForce = 8f;
        puedeSaltar = true;
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

    IEnumerator FullRechargeStamina()
    {
        Debug.Log("Recargando stamina");

        yield return new WaitForSeconds(10);
        stamina = 10;
        canDash = true;
        isTired = false;
        isAbleToRun = true;
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
        if(collision.collider.CompareTag("suelo"))
        {
            puedeSaltar = true;
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

        Vector3 movement = (forward * direccion.y + right * direccion.x) * velocidad * Time.deltaTime;

        transform.position += movement;
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
            ManageDash("forward");
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
            ManageDash("left");
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
            ManageDash("backward");
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
            ManageDash("right");
            lastTapD = 0f;

        }else
        {
            lastTapD = currenTime;
        }

        
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
