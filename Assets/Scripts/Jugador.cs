using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Jugador : MonoBehaviour
{

    private PlayerInput inputJugador;

    private InputAction accionMovimiento;

    private InputAction accionSalto;

    private Rigidbody rb;
    
    private Vector2 direccion; 

    private bool puedeSaltar = true;

    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float salto = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        puedeSaltar = true;
        inputJugador = GetComponent<PlayerInput>();
        accionMovimiento = inputJugador.actions.FindAction("Movimiento");
        accionSalto = inputJugador.actions.FindAction("Saltar");
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
    }

    void FixedUpdate()
    {
        
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
}
