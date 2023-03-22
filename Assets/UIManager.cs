using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //public static UIManager instance;

    //public GameObject gameOverScreen;

    public GameObject levelEndScreen;

    public GameObject pauseMenu;
    public string mainMenuName = "1_Main Menu";
    public Selectable selectOnOpen;

    private void Awake()
    {
        //instance = this;
    }

    public void QuitToMain()
    {
        SceneManager.LoadScene(mainMenuName);
        Time.timeScale = 1f;
    }

    public void Resume()
    {
        // Unpauses the game
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            // Allow player to move
        }

        // Pauses the game
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            // Stop player from moving
        }
    }
}
