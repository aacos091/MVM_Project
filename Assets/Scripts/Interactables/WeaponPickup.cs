using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public string weapPickup;

    public void Interact()
    {
        WeaponManager.instance.WeaponFound(WeaponManager.Weapons.Shotgun);
    }


}

