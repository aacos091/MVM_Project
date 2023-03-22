using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealEntranceInteract : MonoBehaviour
{
    public void EnterRealEntrance()
    {
        if (DialogueController.instance.inDialogue == false)
        {
            DialogueController.instance.getRealEntranceText();
            DialogueController.instance.inDialogue = true;
        }
        // Probably write something to the ui here
        else
        {
            DialogueController.instance.NextLine3();
            DialogueController.instance.inDialogue = false;
        }

    }
}
