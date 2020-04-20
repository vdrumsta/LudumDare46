using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHelper : MonoBehaviour
{
    public void NextScene()
    {
        Debug.Log("loading next scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        Debug.Log("Using LoadScene with scene = " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    public void RestartScene()
    {
        Debug.Log("Restarting scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
