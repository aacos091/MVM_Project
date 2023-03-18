using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    
    public GameObject blackOutSquare;

    private void Start()
    {
        StartCoroutine(FadeBlackOutSquare(false, 1));
    }

    public void StartGame()
    {
        StartCoroutine(StartProcess(1, 1));
        //StartCoroutine(FadeBlackOutSquare(false, 5));
        //SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LeavePrologue()
    {
        StartCoroutine(StartProcess(1, 2));
    }

    public IEnumerator StartProcess(int fadeSpeed = 5, int sceneNumber = 1)
    {
        // StartCoroutine(FadeBlackOutSquare(false, 5));
        // yield return new WaitForSeconds(5);
        
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;
        
        while (blackOutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }
        
        SceneManager.LoadScene(sceneNumber);
    }

    IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, int fadeSpeed = 5)
    {
        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        if (fadeToBlack)
        {
            while (blackOutSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        else
        {
            while (blackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                blackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
    }
}
