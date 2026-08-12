using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float deleteTime=5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deleteTime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        Countdown();
    }

    private void Countdown()
    {
        if(deleteTime > 0)
        {
            deleteTime -= Time.deltaTime;
        }
        else
        {
            DeleteBullet();
        }
    }

    private void DeleteBullet()
    {
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
