using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAtLine : MonoBehaviour
{
    public TextAsset theText;

    public int startLine;
    public int endLine;

    public DialogueBoxManager DialogueBox;

    public bool requiredButtonPress;
    private bool waitForPress;

    public bool destroyWhenActivated;
    
    // Start is called before the first frame update
    void Start()
    {
        DialogueBox = FindObjectOfType<DialogueBoxManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(waitForPress && Input.GetKeyDown(KeyCode.E))
        {
            DialogueBox.ReloadScript(theText);
            DialogueBox.currentLine = startLine;
            DialogueBox.endAtLine = endLine;
            DialogueBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Player")
        {
            if (requiredButtonPress)
            {
                waitForPress = true;
                return;
            }
            
            DialogueBox.ReloadScript(theText);
            DialogueBox.currentLine = startLine;
            DialogueBox.endAtLine = endLine;
            DialogueBox.EnableTextBox();

            if(destroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.name == "Player")
        {
            waitForPress = false;
        }
    }

    public void StartText()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            waitForPress = true;
            return;
        }
    }
}
