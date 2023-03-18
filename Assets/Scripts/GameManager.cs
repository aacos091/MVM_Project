using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UIController.instance.FadeBlackOutSquare(false, 1));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("User has quit");
            Application.Quit();
        }
    }

    // public void NewScene(int scene)
    // {
    //     UIController.instance.StartCoroutine(UIController.instance.FadeBlackOutSquare(true, 1));
    //     SceneManager.LoadScene(scene);
    //     UIController.instance.StartCoroutine(UIController.instance.FadeBlackOutSquare(false, 1));
    // }
    
    public IEnumerator NewScene(int scene)
    {
        PlayerController.instance.CanMove = false;
        StartCoroutine(UIController.instance.FadeBlackOutSquare(true, 1));
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(scene);
        StartCoroutine(UIController.instance.FadeBlackOutSquare(false, 1));
        PlayerController.instance.CanMove = true;
        //PlayerController.instance.transform.position = newPlayerPosition;
    }
}
