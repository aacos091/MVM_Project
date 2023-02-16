using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class WeaponController : MonoBehaviour
{   
    // Weapon Type & Ammo
    public int pistolBullets;
    public int currentPistolMagCount;
    public int maxPistolMagCount;
    public int numOfMags;
    
    public Transform gunBarrel;
    public LayerMask targetLayer;

    private RaycastHit hit;

    private void Start()
    {
        Debug.Log("Amount of bullets: " + pistolBullets);
        Debug.Log("Amount in mag: " + currentPistolMagCount);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void FixedUpdate()
    {
        
    }

    void Shoot()
    {
        if (currentPistolMagCount > 0)
        {
            Debug.DrawRay(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right) * 15f, Color.yellow, 1f);
            Debug.Log("shot the gun");
            
            if (Physics2D.Raycast(gunBarrel.position, gunBarrel.TransformDirection(Vector3.right), 15f, targetLayer))
            {
                Debug.Log("you hit something");
            }

            --currentPistolMagCount;
        }
        else
        {
            Debug.Log("No Ammo");
        }
    }

    void Reload()
    {
        if (currentPistolMagCount == maxPistolMagCount)
        {
            Debug.Log("Full mag");
        }
        else
        {
            if (pistolBullets > 0)
            {
                while (currentPistolMagCount < maxPistolMagCount && pistolBullets != 0)
                {
                    pistolBullets--;
                    currentPistolMagCount++;
                }
            }
            else
            {
                Debug.Log("no more ammo");
            }
                
        } 
    }
}
