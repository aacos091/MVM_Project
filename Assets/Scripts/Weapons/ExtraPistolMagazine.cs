using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPistolMagazine : MonoBehaviour
{
    private AmmoManager ammo;
    public int bullets;

    private void OnTriggerEnter2D(Collider2D col)
    {
        ammo = col.GetComponentInChildren<AmmoManager>();
        ammo.findNewPistolMag(bullets);
        
        Destroy(gameObject);
    }
}
