using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> enemies;//Add Empties objects
    public List<GameObject> positions; 
    //private float startDelay = 1.0f;
    private float randomInterval = 1.0f;
    private Transform spawnPosition;
    public int limitEnemies=30;
    private int timerInterval = 0;
    private float timerCounter = 0f;
    public int enemyCount;

    void Start()
    {
        spawnPosition = null;        
    }

    // Update is called once per frame
    void Update()
    {
        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length-1;
    }

    private void createEnemy()
    {
        Debug.Log("Creando Enemigo");
        spawnPosition = selectEmptyPostion();
        GameObject enemy = selectEnemy();
        Instantiate(enemy, spawnPosition.position, spawnPosition.rotation);
        randomInterval = getRandomInterval();
        timerInterval = 0;
        timerCounter = 0;
    }

    private Transform selectEmptyPostion()
    {
        int randomIndex = UnityEngine.Random.Range(0, positions.Count);
        //Debug.Log("Random enemies " + randomIndex);
        GameObject emptyPosition = positions[randomIndex];
        //Debug.Log("Obteniendo position "+emptyPosition.name);
        return emptyPosition.transform;
    }
    private GameObject selectEnemy()
    {
        int randomIndex = UnityEngine.Random.Range(0, enemies.Count);
        //Debug.Log("Random enemies " + randomIndex);
        GameObject enemy = enemies[randomIndex];
        Debug.Log("Obteniendo enemy "+enemy.name);
        return enemy;
    }
    private int getRandomInterval()
    {
        return UnityEngine.Random.Range(1, 5);
    }

    /**/
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Active Spawn");
            activateSpawn();
        }
    }

    private void activateSpawn()
    {
        timerCounter += Time.deltaTime;
        timerInterval = (int)timerCounter;

        Debug.Log("Timer : " + timerInterval + " Random Interval " + randomInterval + " Enemies: " + enemyCount);

        // 1. PRIMERO: Comprobamos si ya pasó el tiempo de espera
        if (timerInterval >= randomInterval)
        {
            // 2. SEGUNDO: Si el tiempo ya se cumplió, revisamos si hay espacio
            if (enemyCount < limitEnemies)
            {
                // Hay espacio (ej. hay 3 enemigos y el límite es 4): Creamos uno.
                // (Esta función ya resetea timerCounter y timerInterval a 0)
                createEnemy();
            }
            else
            {
                // No hay espacio (ya hay 4 enemigos): No creamos nada,
                // pero SI O SÍ reiniciamos el tiempo para que vuelva a contar desde cero.
                timerCounter = 0;
                timerInterval = 0;
                randomInterval = getRandomInterval();
                Debug.Log("Límite alcanzado (" + enemyCount + "/" + limitEnemies + "). Reiniciando temporizador de espera.");
            }
        }
    }

}
