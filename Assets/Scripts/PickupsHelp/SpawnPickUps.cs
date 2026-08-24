using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPickups : MonoBehaviour
{
    // Pickups que pueden aparecer
    public List<GameObject> pickups;

    // Posiciones donde pueden aparecer
    public List<GameObject> positions;

    private Transform spawnPosition;

    private float randomInterval;
    private float timerCounter = 0f;

    void Start()
    {
        spawnPosition = null;

        // Seleccionamos el primer tiempo aleatorio
        randomInterval = getRandomInterval();

        //Debug.Log("Primer Pickup aparecera en: " + randomInterval + " segundos");
    }

    void Update()
    {
        activateSpawn();
    }

    private void createPickup()
    {
        //Debug.Log("Creando Pickup");

        spawnPosition = selectEmptyPosition();

        GameObject pickup = selectPickup();

        Instantiate(
            pickup,
            spawnPosition.position,
            spawnPosition.rotation
        );

        //Debug.Log("Pickup creado: " + pickup.name + " en: " + spawnPosition.name);

        // Reiniciamos el tiempo
        timerCounter = 0f;

        // Seleccionamos un nuevo tiempo entre 1 y 2 minutos120
        randomInterval = getRandomInterval();

        //Debug.Log("Siguiente Pickup aparecera en: " + randomInterval + " segundos");
    }

    private Transform selectEmptyPosition()
    {
        int randomIndex = UnityEngine.Random.Range(0, positions.Count);

        GameObject emptyPosition = positions[randomIndex];

        //Debug.Log("Posicion seleccionada: " + emptyPosition.name);

        return emptyPosition.transform;
    }

    private GameObject selectPickup()
    {
        int randomIndex = UnityEngine.Random.Range(0, pickups.Count);

        GameObject pickup = pickups[randomIndex];

        //Debug.Log("Pickup seleccionado: " + pickup.name);

        return pickup;
    }

    private float getRandomInterval()
    {
        return UnityEngine.Random.Range(20f, 40f);
    }

    private void activateSpawn()
    {
        timerCounter += Time.deltaTime;

        //Debug.Log("Timer: " + timerCounter + " | Siguiente Pickup: " + randomInterval);

        if (timerCounter >= randomInterval)
        {
            createPickup();
        }
    }
}