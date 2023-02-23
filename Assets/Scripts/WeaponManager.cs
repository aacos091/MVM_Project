using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum Weapons
    {
        Melee,
        Pistol,
        Uzi,
        Shotgun
    };

    public Weapons currentWeapon;
    
    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = Weapons.Pistol;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentWeapon)
        {
            case Weapons.Melee:
                break;
            case Weapons.Pistol:
                break;
            case Weapons.Uzi:
                break;
            case Weapons.Shotgun:
                break;
            default:
                Debug.Log("Invalid Selection");
                break;
        }
    }
}
