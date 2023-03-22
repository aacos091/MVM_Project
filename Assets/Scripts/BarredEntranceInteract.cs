using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarredEntranceInteract : MonoBehaviour
{
    public void EnterBarredEntrance()
    {
        if (DialogueController.instance.inDialogue == false)
        {
            DialogueController.instance.getBarredEntranceText();
            DialogueController.instance.inDialogue = true;
        }
        // Probably write something to the ui here
        else
        {
            DialogueController.instance.NextLine2();
            DialogueController.instance.inDialogue = false;
        }

    }
}
