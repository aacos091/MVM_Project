using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesManager : MonoBehaviour
{
    public static AbilitiesManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public enum Ability
    {
        Armor,
        Flashlight,
        UVLight,
        GrapplingHook,
        RunningShoes
    };

    // Booleans for the different upgrades
    // If true, player has that upgrade
    public bool armorFound;
    public bool flashlightFound;
    public bool uvLightFound;
    public bool grapplingHookFound;
    public bool runningShoesFound;

    public void AbilityFound(Ability foundAbility)
    {
        switch (foundAbility)
        {
            case Ability.Armor:
                armorFound = true;
                break;
            case Ability.Flashlight:
                flashlightFound = true;
                break;
            case Ability.UVLight:
                uvLightFound = true;
                break;
            case Ability.GrapplingHook:
                grapplingHookFound = true;
                break;
            case Ability.RunningShoes:
                runningShoesFound = true;
                break;
            default:
                Console.WriteLine("Invalid Selection");
                break;

        }
    }
}
