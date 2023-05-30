using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueBoxManager : MonoBehaviour
{
    public GameObject textBox;

    public TMP_Text dialogueText;

    public TextAsset textFile;
    public string[] textLines;

    public int currentLine;
    public int endAtLine;

    public PlayerController player;
    public ActivateAtLine Actline;
    public CutsceneManager cutscene;
    //private Animator _playerAnimator;

    public bool isActive;

    public bool stopGame;
    public bool stopPlayerMovement;

    private bool isTyping = false;
    private bool cancelTyping = false;

    public float typeSpeed;
    
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        Actline = FindObjectOfType<ActivateAtLine>();
        cutscene = FindObjectOfType<CutsceneManager>();
        //_playerAnimator = GetComponent<Animator>();

        if (textFile != null)
        {
            textLines = (textFile.text.Split('\n'));
        }

        if (endAtLine == 0)
        {
            endAtLine = textLines.Length - 1;
        }

        if (isActive)
        {
            EnableTextBox();
        }
        else
        {
            DisableTextBox();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        
        // dialogueText.text = textLines[currentLine];

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTyping)
            {
                currentLine += 1;
                
                if (currentLine > endAtLine)
                {
                    DisableTextBox();
                    //Activate.FalsePress();
                    //Actline.ObjectDestroyed();
                }
                else //if (cutscene.inCutscene == false)
                {
                    StartCoroutine(TextScroll(textLines[currentLine]));
                }
            }
            else if (isTyping && !cancelTyping)
            {
                cancelTyping = true;
            }
        }
    }

    private IEnumerator TextScroll(string lineOfText)
    {
        int letter = 0;
        dialogueText.text = "";
        isTyping = true;
        cancelTyping = false;
        while (isTyping && !cancelTyping && (letter < lineOfText.Length - 1))
        {
            dialogueText.text += lineOfText[letter];
            letter += 1;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
        dialogueText.text = lineOfText;
        isTyping = false;
        cancelTyping = false;
    }

    public void EnableTextBox()
    {
        isActive = true;
        textBox.SetActive(true);


        if (stopPlayerMovement)
        {
            player.CanMove = false;
        }

        //if (stopGame)
        //{
        //    Time.timeScale = 0;
        //}

        StartCoroutine(TextScroll(textLines[currentLine]));
        //Actline.FalsePress();
    }

    public void DisableTextBox()
    {
        if(isActive)
        {
            
            isActive = false;
            textBox.SetActive(false);
                  
            //player.CanMove = true;  //Probably need to lock this behind if statement to know if a cutscene is happening

            if (cutscene.inCutscene == false)
            {
                player.CanMove = true;
            }
            
            //Time.timeScale = 1;
            //Activate.ObjectDestroyed();

            //else if (Activate.destroyWhenActivated == false)
            //{
            //    Activate.TruePress();
            //}
        }
    }

    //public void ScriptEnabled()
    //{
    //    if (enabled == true)
    //    {
    //        enabled = false;
    //    }
    //    else
    //    {
    //        enabled = true;
    //    }
    //}

    public void ReloadScript(TextAsset theText)
    {
        if (theText != null)
        {
            textLines = new string[1];
            textLines = (theText.text.Split('\n'));
        }
    }
}
