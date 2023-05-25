using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    // 5-21, the Code still can't close the box when near the entrances, and I can't get it to pause the game during dialogue or have the shotgun disappear ONLY AFTER the dialogue is done
    public static DialogueController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;

    private int index;

    public GameObject shotgunText;
    public GameObject BarredEntranceText;
    public GameObject RealEntranceText;
    public bool inDialogue = false;

    void Start()
    {
        textComponent.text = "";
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[index];
        }
    }

    public void StartDialogue()
    {
        index = 0;
        gameObject.SetActive(true);
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    // With this version of code, take StartDialogue out of Start.  Doesn't fully work how I want, though, was just trying to find a way to properly freeze time
    //public void StartDialogue()
    //{
    //    if (DialogueBox.active == false)
    //    {
    //        index = 0;
    //        DialogueBox.SetActive(true);
    //        StartCoroutine(TypeLine());
    //    }
    //    else if (textComponent.text == lines[index])
    //    {
    //        NextLine();
    //    }
    //    else
    //    {
    //        StopAllCoroutines();
    //        textComponent.text = lines[index];
    //    }
    //}

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            //Time.timeScale = 1;
            gameObject.SetActive(false);
        }
    }

    public void getBarredEntranceText()
    {
        Time.timeScale = 0;
        BarredEntranceText.SetActive(true);

    }

    public void getRealEntranceText()
    {
        Time.timeScale = 0;
        RealEntranceText.SetActive(true);

    }

    public void NextLine2()
    {
        Time.timeScale = 1;
        BarredEntranceText.SetActive(false);
    }

    public void NextLine3()
    {
        Time.timeScale = 1;
        RealEntranceText.SetActive(false);
    }
}