using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPickup : MonoBehaviour, IInteractable
{
    public void Interact()
    {

        WeaponManager.instance.WeaponFound(WeaponManager.Weapons.Shotgun);
        
        
    }
}

