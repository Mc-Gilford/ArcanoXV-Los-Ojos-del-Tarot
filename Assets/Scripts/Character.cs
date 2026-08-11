using UnityEngine;

public class Character : MonoBehaviour
{

    private int health;
    private float speed;
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
            Die();
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
