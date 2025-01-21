using System;
using TMPro;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public TextMeshPro levelText;

    private void Start()
    {
        if (levelText != null)
        {
            levelText.text = GetCurrentLevel();
        }
    }

    public void CloseGame()
    {
        Application.Quit();
    }
    
    public void LoadNextScene()
    {
        // Get the current scene index
        int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        
        // Load the next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneIndex + 1);
    }
    
    public void LoadMenu()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private String GetCurrentLevel()
    {
        //Gives the string "Level " + the current scene index
        return "Level " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }
}
