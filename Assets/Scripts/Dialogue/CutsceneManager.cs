using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public PlayerController player;
    public DialogueBoxManager DialogueBox;
    //private Animator _playerAnimator;

    public TextAsset theText;

    public int startLine;
    public int endLine;
    public int cutsceneLine;

    public float waitTime;

    public bool inCutscene = false;
    public bool destroyWhenActivated;
    //public bool conCutscene = false;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        DialogueBox = FindObjectOfType<DialogueBoxManager>();
        //_playerAnimator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        cutsceneLine = DialogueBox.currentLine;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Player")
        {
            Debug.Log("TEST CUTSCENE");
            //_playerAnimator.SetBool("IsMoving", false);
            player.CanMove = false;
            //Add as many needed canMove lines for enemies, or code them in each cutscene script
            DialogueBox.ReloadScript(theText);
            DialogueBox.currentLine = startLine;
            DialogueBox.endAtLine = endLine;
            StartCutscene();
            
        }


    }

    public void StartCutscene()
    {
        Debug.Log("Cutscene Started");
        inCutscene = true;


        //if(gameObject.CompareTag("AlleyScene1"))
        //{
        //    //Police Sound
        //    //WaitForSecondsRealtime(waitTime); //waitTime might actually be irrelevant as I'll probably want the wait time to be unique to each instance, but oh well

        //    player.transform.localScale = new Vector3(-1f, 1f, 1f);
        //    new WaitForSeconds(waitTime);
        //    DialogueBox.EnableTextBox();
        //}
        //Police Sound
        //WaitForSecondsRealtime(waitTime); //waitTime might actually be irrelevant as I'll probably want the wait time to be unique to each instance, but oh well

        player.transform.localScale = new Vector3(-1f, 1f, 1f);
        DialogueBox.Invoke("EnableTextBox", 2f);

        //if (DialogueBox.currentLine > 2 && DialogueBox.currentLine < endLine)
        if (DialogueBox.currentLine == 3)
        {
            Debug.Log("Next Step");
            //DialogueBox.currentLine = 3;
            //DialogueBox.endAtLine = 5;
            DialogueBox.DisableTextBox();
            DialogueBox.Invoke("EnableTextBox", 2f);
            //DialogueBox.endAtLine = 2;
            //DialogueBox.DisableTextBox();
            //player.transform.localScale = new Vector3(-1f, 1f, 1f);
            //new WaitForSeconds(waitTime);
            //DialogueBox.EnableTextBox();
        }




        //if (Collider2D.CompareTag("AlleyScene1"))
        //{

        //}

        //inCutscene = false;
        //ObjectDestroyed();

    }

    public void ObjectDestroyed()
    {
        if (destroyWhenActivated)
        {
            Destroy(gameObject);
        }

    }
}
