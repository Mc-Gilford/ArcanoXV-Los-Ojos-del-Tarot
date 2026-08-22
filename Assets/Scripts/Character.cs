using UnityEngine;

public class Character : MonoBehaviour
{

    public int health { get; set; }
    public float speed { get; set; }
    private bool isAlive;
    private float jumpforce;
    private GameManager gameManager;

    [SerializeField] protected AudioSource audioSource;

    
    void Awake()
    {
        if(audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void  TakeDamage(int damageTaken)
    {
        this.health -= damageTaken;
        if(this.health <=0)
        {
            isAlive = false;
            if (!gameObject.CompareTag("Follower"))
            {
                Die();
            }
        }
    }

    public virtual void Die()
    {
        if(gameObject.CompareTag("Player"))
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            Debug.Log("mueriendo...");
            gameManager.GameOver();
            //Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Muerte "+gameObject.name);
        }
        
    }
}
