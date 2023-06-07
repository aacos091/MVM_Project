using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public enum Items
    {
        health,
        pistolAmmo,
        pistolMag,
        uziAmmo,
        uziMag,
        shotgunAmmo
    }

    public Items itemToPickup;

    // Use this to dictate how many bullets you are getting picking up for any of the weapons, if this set to that
    public int ammoPickup;
    
    public void Interact()
    {
        switch (itemToPickup)
        {
            case Items.health:
                // Health PickUp
                break;
            case Items.pistolAmmo:
                // Additional pistol bullets
                break;
            case Items.pistolMag:
                // Additional pistol magazine
                break;
            case Items.uziAmmo:
                // Additional uzi bullets
                break;
            case Items.uziMag:
                // Additional uzi magazine
                break;
            case Items.shotgunAmmo:
                // Additional shotgun shells
                break;
            default:
                Debug.Log("You picked something up");
                break;
        }
    }
}
