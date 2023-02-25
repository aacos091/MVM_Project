using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public enum Weapons
    {
        Melee,
        Pistol,
        Uzi,
        Shotgun
    };

    public GameObject melee, pistol, uzi, shotgun;
    
    public bool uziFound, shotgunFound;

    public void ChangeWeapon(Weapons newWeapon)
    {
        switch (newWeapon)
        {
            case Weapons.Melee:
                melee.SetActive(true);
                pistol.SetActive(false);
                uzi.SetActive(false);
                shotgun.SetActive(false);
                break;
            case Weapons.Pistol:
                melee.SetActive(false);
                pistol.SetActive(true);
                uzi.SetActive(false);
                shotgun.SetActive(false);
                break;
            case Weapons.Uzi:
                if (uziFound)
                {
                    melee.SetActive(false);
                    pistol.SetActive(false);
                    uzi.SetActive(true);
                    shotgun.SetActive(false);
                }
                else
                {
                    Debug.Log("You don't have the uzi");
                }
                break;
            case Weapons.Shotgun:
                if (shotgunFound)
                {
                    melee.SetActive(false);
                    pistol.SetActive(false);
                    uzi.SetActive(false);
                    shotgun.SetActive(true);
                }
                else
                {
                    Debug.Log("You don't have the shotgun");
                }
                break;
            default:
                Debug.Log("Invalid Selection");
                break;
        }
    }
}
