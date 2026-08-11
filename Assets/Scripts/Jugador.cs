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

    private Rigidbody rb;
    
    private Vector2 direccion; 

    private bool puedeSaltar = true;


    [SerializeField] private bool isRunning= false;
    [SerializeField] private bool isAbleToRun= true;
    [SerializeField] private bool isTired= false;
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float salto = 20f;
    [SerializeField] private float stamina = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        puedeSaltar = true;
        inputJugador = GetComponent<PlayerInput>();
        accionMovimiento = inputJugador.actions.FindAction("Movimiento");
        accionSalto = inputJugador.actions.FindAction("Saltar");
        sprintAction = inputJugador.actions.FindAction("Run");

        sprintAction.started += ctx => Run();
        sprintAction.canceled += ctx => StopRunning();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoverJugador();
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
        isTired = false;
        isAbleToRun = true;
    }

    private void Saltar () 
    {
        rb.AddForce(Vector3.up * salto, ForceMode.Impulse);
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
}
