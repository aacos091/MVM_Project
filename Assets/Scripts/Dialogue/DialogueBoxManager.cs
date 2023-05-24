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

    public bool isActive;

    public bool stopGame;
    
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        
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
        
        dialogueText.text = textLines[currentLine];

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentLine += 1;
        }

        if (currentLine > endAtLine)
        {
            DisableTextBox();
        }
    }

    public void EnableTextBox()
    {
        isActive = true;
        textBox.SetActive(true);

        if (stopGame)
        {
            Time.timeScale = 0;
        }
    }

    public void DisableTextBox()
    {
        isActive = false;
        textBox.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReloadScript(TextAsset theText)
    {
        if (theText != null)
        {
            textLines = new string[1];
            textLines = (theText.text.Split('\n'));
        }
    }
}
