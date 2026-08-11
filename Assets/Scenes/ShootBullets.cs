using UnityEngine;

public class ShootBullets : MonoBehaviour
{
    public float shootForce, upwardForce;

    [SerializeField] public float shootCooldown=1f;

    public int magazine, bulletsLeft;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reload(int ammo)
    {
        int recoverBullets = bulletsLeft+ammo;
        if(recoverBullets >= magazine)
        {
            bulletsLeft=magazine;
        }
        else
        {
            bulletsLeft = recoverBullets;
        }
    }
}
