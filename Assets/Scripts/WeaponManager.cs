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

    [Header("Pistol Ammo")] 
    public int pistolBullets;
    public int currentPistolMagCount;
    public int maxPistolMagCount; 
    public int pistolMagID = 0;
    public List<int> pistolMags = new List<int>();

    [Header("Uzi Ammo")] public int uziBullets;
    public int currentUziMagCount;
    public int maxUziMagCount;
    public int uziMagID = 0;
    public List<int> uziMags = new List<int>();

    [Header("Shotgun Ammo")] 
    public int shotgunShells;
    public int currentShellCount;
    public int maxShellCount;

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
    
    public void findNewPistolMag(int amountOfBullets)
    {
        pistolMags.Add(amountOfBullets);
    }
}
