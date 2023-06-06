using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public void Interact()
    {

        AbilitiesManager.instance.AbilityFound(AbilitiesManager.Ability.Flashlight);


    }
}
