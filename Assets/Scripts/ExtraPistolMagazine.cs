using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPistolMagazine : MonoBehaviour
{
    public int bullets;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        

        if (WeaponManager.instance == null)
        {
            return;
        }
        else
        {
            WeaponManager.instance.findNewPistolMag(bullets);
        }
        
        Destroy(gameObject);
    }
}
