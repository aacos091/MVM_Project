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
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
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

    void StartDialogue()
    {
        index = 0;
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
            gameObject.SetActive(false);
        }
    }

    public void getShotgunText()
    {
        Time.timeScale = 0;
        shotgunText.SetActive(true);

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

    public void NextLine1()
    {
        Time.timeScale = 1;
        shotgunText.SetActive(false);
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
