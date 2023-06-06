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
    
    // Booleans for the different upgrades
    // If true, player has that upgrade
    public bool armorFound;
    public bool flashlightFound;
    public bool uvLightFound;
    public bool grapplingHookFound;
    public bool runningShoesFound;

    public enum Abilities
    {
        Armor;
        Flashlight;
        UVLight;
        GrapplingHook;
        RunningShoes;
    }
}
