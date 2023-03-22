using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPickup : MonoBehaviour
{
    public void FoundShotgun()
    {
        if (WeaponManager.instance.shotgunFound == false)
        {
            DialogueController.instance.getShotgunText();
            WeaponManager.instance.shotgunFound = true;
        }
        // Probably write something to the ui here
        else
        {
            DialogueController.instance.NextLine1();
            Destroy(gameObject); // Probably do a little fade out animation before this
        }
        
    }

}

