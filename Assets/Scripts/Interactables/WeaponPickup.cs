using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public WeaponManager.Weapons weaponToPickup;

    public void Interact()
    {
        WeaponManager.instance.WeaponFound(weaponToPickup);
    }


}

