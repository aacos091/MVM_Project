using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraMagazine : MonoBehaviour
{
    public int bullets;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        WeaponController weaponCon = col.GetComponentInChildren<WeaponController>();

        if (weaponCon == null)
        {
            return;
        }
        else
        {
            weaponCon.findNewMag(bullets);
        }
        
        Destroy(gameObject);
    }
}
