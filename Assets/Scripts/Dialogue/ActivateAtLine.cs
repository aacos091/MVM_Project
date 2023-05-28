using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateAtLine : MonoBehaviour
{
    public TextAsset theText;

    public int startLine;
    public int endLine;

    //public string dialogueTag;

    public DialogueBoxManager DialogueBox;

    public bool requiredButtonPress;
    public bool waitForPress;

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

            //if (destroyWhenActivated)
            //{
            //waitForPress = false;
            //    Destroy(gameObject);
            //}

            ObjectDestroyed();

            waitForPress = false;

            //if(destroyWhenActivated)
            //{
            //    waitForPress = false;
            //}          
        }
        //else if (DialogueBox.isActive == true && Input.GetKeyDown(KeyCode.E))
        //{
        //    waitForPress = true;
        //}
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

            //DialogueBox.Actline = ;
            //tag = FindObjectOfType(Collider2D);
            
            //dialogueTag = 
            
            //DialogueBox.ReloadScript(theText);
            //DialogueBox.currentLine = startLine;
            //DialogueBox.endAtLine = endLine;
            //DialogueBox.EnableTextBox();

            //if(destroyWhenActivated)
            //{
            //    Destroy(gameObject);
            //}
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if(other.name == "Player")
        {
            waitForPress = false;
        }
    }

    public void ObjectDestroyed()
    {
        if(destroyWhenActivated)
        {
            Destroy(gameObject);
        }
        
    }

    public void TruePress()
    {
        waitForPress = true;
    }

    public void FalsePress()
    {
        waitForPress = false;
    }

    //public void StartText()
    //{
    //    if(Input.GetKeyDown(KeyCode.E))
    //    {
    //        waitForPress = true;
    //        return;
    //    }
    //}
}
