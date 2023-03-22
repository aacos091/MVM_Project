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

    private Animator _playerAnimator;

    public PistolController pistolCon;
    public UziController uziCon;
    public ShotgunController shotgunCon;
    
    public bool uziFound, shotgunFound = false;

    private void Start()
    {
        _playerAnimator = GetComponentInParent<Animator>();
        
        _playerAnimator.SetBool("PistolEquipped", true);
        _playerAnimator.SetBool("UziEquipped", false);
        _playerAnimator.SetBool("ShotgunEquipped", false);
    }

    public void ChangeWeapon(Weapons newWeapon)
    {
        switch (newWeapon)
        {
            // case Weapons.Melee:
            //     melee.SetActive(true);
            //     pistol.SetActive(false);
            //     uzi.SetActive(false);
            //     shotgun.SetActive(false);
            //     break;
            case Weapons.Pistol:
                melee.SetActive(false);
                //pistol.SetActive(true);
                pistolCon.WhenActivated();
                //uzi.SetActive(false);
                uziCon.WhenDeactivated();
                //shotgun.SetActive(false);
                shotgunCon.WhenDeactivated();
                _playerAnimator.SetBool("PistolEquipped", true);
                _playerAnimator.SetBool("UziEquipped", false);
                _playerAnimator.SetBool("ShotgunEquipped", false);
                break;
            case Weapons.Uzi:
                if (uziFound)
                {
                    melee.SetActive(false);
                    //pistol.SetActive(true);
                    pistolCon.WhenDeactivated();
                    //uzi.SetActive(false);
                    uziCon.WhenActivated();
                    //shotgun.SetActive(false);
                    shotgunCon.WhenDeactivated();
                    _playerAnimator.SetBool("PistolEquipped", false);
                    _playerAnimator.SetBool("UziEquipped", true);
                    _playerAnimator.SetBool("ShotgunEquipped", false);
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
                    //pistol.SetActive(true);
                    pistolCon.WhenDeactivated();
                    //uzi.SetActive(false);
                    uziCon.WhenDeactivated();
                    //shotgun.SetActive(false);
                    shotgunCon.WhenActivated();
                    _playerAnimator.SetBool("PistolEquipped", false);
                    _playerAnimator.SetBool("UziEquipped", false);
                    _playerAnimator.SetBool("ShotgunEquipped", true);
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
