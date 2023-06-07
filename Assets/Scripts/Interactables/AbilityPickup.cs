using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour, IInteractable
{
    public AbilitiesManager.Ability abilityFound;
    
    public void Interact()
    {
        AbilitiesManager.instance.AbilityFound(abilityFound);
    }
}
