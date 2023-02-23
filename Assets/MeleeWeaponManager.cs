using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponManager : MonoBehaviour
{
    public enum MeleeWeapons
    {
        Knife,
        BaseballBat,
        Crowbar
    };

    public MeleeWeapons currentMeleeWeapon;
    
    // Start is called before the first frame update
    void Start()
    {
        currentMeleeWeapon = MeleeWeapons.Knife;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentMeleeWeapon)
        {
            case MeleeWeapons.Knife:
                break;
            case MeleeWeapons.BaseballBat:
                break;
            case MeleeWeapons.Crowbar:
                break;
            default:
                Debug.Log("Invalid Selection");
                break;
        }
    }
}
