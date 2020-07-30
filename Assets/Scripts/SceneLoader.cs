using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    GameObject StartButton;
    GameObject ExitButton;
    GameObject TryAgainButton;
    GameObject MenuButton;

    Animator start;
    Animator exit;
    Animator tryAgain;
    Animator menu;

    public void LoadNextScene()
    {
        StartButton = GameObject.Find("Start Button");
        start = StartButton.GetComponent<Animator>();
        start.SetBool("ButtonDown", true);
        StartCoroutine(StartDelay());
    }

    public void ExitGame()
    {
        ExitButton = GameObject.Find("Exit Button");
        exit = ExitButton.GetComponent<Animator>();
        exit.SetBool("ButtonDown", true);
        StartCoroutine(ExitDelay());
    }

    public void TryAgain()
    {
        TryAgainButton = GameObject.Find("Try Again Button");
        tryAgain = TryAgainButton.GetComponent<Animator>();
        tryAgain.SetBool("ButtonDown", true);
        StartCoroutine(TryAgainDelay());
    }

    public void GoToMenu()
    {
        MenuButton = GameObject.Find("Menu Button");
        menu = MenuButton.GetComponent<Animator>();
        menu.SetBool("ButtonDown", true);
        StartCoroutine(GoToMenuDelay());
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    IEnumerator ExitDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }

    IEnumerator TryAgainDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator GoToMenuDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }
}
