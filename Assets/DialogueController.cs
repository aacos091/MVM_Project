using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public static DialogueController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public TMP_Text dialogue;
    private bool _inDialogue = false;
    public int i = 0;

    public void DisplayText(string[] stringsToDisplay)
    {
        

        // while (i <= stringsToDisplay.Length)
        // {
        //     _inDialogue = true;
        //     Time.timeScale = 0;
        //     dialogue.text = stringsToDisplay[i];
        //     
        //     if (Input.GetKeyDown(KeyCode.E))
        //     {
        //         i++;
        //     }
        // }
        _inDialogue = true;
        Time.timeScale = 0;
        dialogue.text = stringsToDisplay[i];
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            i++;
            dialogue.text = stringsToDisplay[i];
        }

        if (i == stringsToDisplay.Length)
        {
            _inDialogue = false;
            Time.timeScale = 1;
            dialogue.text = "";
        }

            // if (i < stringsToDisplay.Length)
        // {
        //     _inDialogue = true;
        //     Time.timeScale = 0;
        //     dialogue.text = stringsToDisplay[i];
        // }
        // else 
        // {
        //     _inDialogue = false;
        //     Time.timeScale = 1;
        //     dialogue.text = "";
        //     i = 0;
        // }

        

            // if (i < stringsToDisplay.Length)
        // {
        //     _inDialogue = true;
        //     Time.timeScale = 0;
        //     dialogue.text = stringsToDisplay[i];
        //     if (Input.GetKeyDown(KeyCode.E)) i++;
        // }
        
        // if (i == stringsToDisplay.Length)
        // {
        //     _inDialogue = false;
        //     Time.timeScale = 1;
        //     dialogue.text = "";
        //     i = 0;
        // }
        

        // if (i <= stringsToDisplay.Length)
        // {
        //     _inDialogue = true;
        //     Time.timeScale = 0;
        //     dialogue.text = stringsToDisplay[i];
        // }
    }

    public IEnumerator DisplayStrings(string[] stringsToDisplay)
    {
        while (i < stringsToDisplay.Length)
        {
            dialogue.text = stringsToDisplay[i];
            yield return waitForKeyPress(KeyCode.E);
            i++;
        }

        i = 0;

        // wait for player to press space
        // wait for this function to return

        // do other stuff after key press
    }
    
    private IEnumerator waitForKeyPress(KeyCode key)
    {
        bool done = false;
        while(!done) // essentially a "while true", but with a bool to break out naturally
        {
            if(Input.GetKeyDown(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }
 
        // now this function returns
    }
}
