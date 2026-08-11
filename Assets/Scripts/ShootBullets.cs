using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class ShootBullets : MonoBehaviour
{
    public GameObject bulletModel;

    public float shootForce, upwardForce, spread;

    [SerializeField] public float shootCooldown=1f;
    
    [SerializeField] private bool canShoot=true;

    public int magazine, bulletsLeft;

    private Vector3 directionWithoutSpread, directionWithSpread;
    

    public Camera playerCamera;
    public Transform attackPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShowPointer();
    }

    private void ShowPointer()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;


        //Checa si el ray pegara a algo
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        directionWithoutSpread = targetPoint - attackPoint.position;


        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        directionWithSpread = directionWithoutSpread + new Vector3(x,y,0);

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

    public void Shoot(InputAction.CallbackContext context)
    {
        if(context.started && canShoot)
        {
            bulletsLeft -= 1;
            StartCoroutine(GunKnockback()); 

            GameObject currentBullet = Instantiate(bulletModel, attackPoint.position, Quaternion.identity);

            currentBullet.transform.forward = directionWithSpread.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);


        }
    }

    IEnumerator GunKnockback()
    {
        Debug.Log("Enfriando arma");
        canShoot = false;
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
