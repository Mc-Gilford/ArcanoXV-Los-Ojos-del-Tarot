using UnityEngine;

public class Character : MonoBehaviour
{

    public int health { get; set; }
    public float speed { get; set; }
    private bool isAlive;
    private float jumpforce;

    [SerializeField] protected AudioSource audioSource;

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
        Destroy(gameObject);
        Debug.Log("Muerte "+gameObject.name);
    }
}
