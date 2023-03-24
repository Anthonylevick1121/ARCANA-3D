using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject levelEndScreen;
    
    public GameObject pauseMenu;
    public string mainMenuName = "1_Main Menu";
    public Selectable selectOnOpen;
    
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
