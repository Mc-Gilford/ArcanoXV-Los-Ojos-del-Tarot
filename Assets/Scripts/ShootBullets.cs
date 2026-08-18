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

    [SerializeField] private bool hasBulletsToShoot;

    public int magazine, bulletsLeft, bulletsOnGun;

    private Vector3 directionWithoutSpread, directionWithSpread;
    

    public Camera playerCamera;
    public Transform attackPoint;


    [SerializeField] private int ammoClip;
    //Use of a textMeshPro for a shootpoint guide
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootForce = 10f;
        magazine = 24;
        ammoClip = 5;
        CheckBullets();
    }

    // Update is called once per frame
    void Update()
    {
        ShowPointer();
    }

    private void LateUpdate()
    {
        
    }

    private void CheckBullets()
    {
        if(bulletsOnGun <= 0)
        {
            hasBulletsToShoot = false;
        } 
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

    public void Reload(InputAction.CallbackContext context)
    {

        int bulletsToReload = ammoClip - bulletsOnGun;
        if(bulletsLeft >= bulletsToReload)
        {
            bulletsLeft-=bulletsToReload;
            bulletsOnGun += bulletsToReload;
            hasBulletsToShoot = true;
        }
        else if(bulletsLeft > 0)
        {
            bulletsOnGun += bulletsLeft;
            bulletsLeft = 0;
            hasBulletsToShoot = true;
        }
    }

    public void GetAmmo(int ammo)
    {
        int currentBullets = bulletsLeft + ammo;
        if(currentBullets < magazine)
        {
            bulletsLeft += ammo;
        }else if(currentBullets >= ammo)
        {
            bulletsLeft = magazine;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if(context.started && canShoot && hasBulletsToShoot)
        {
            Debug.Log("Shoot sound");
            bulletsOnGun -= 1;
            StartCoroutine(GunKnockback()); 

            GameObject currentBullet = Instantiate(bulletModel, attackPoint.position, Quaternion.LookRotation(directionWithSpread.normalized) * Quaternion.Euler(90, 0, 0));

            currentBullet.transform.forward = directionWithSpread.normalized;

            currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

            CheckBullets();

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
