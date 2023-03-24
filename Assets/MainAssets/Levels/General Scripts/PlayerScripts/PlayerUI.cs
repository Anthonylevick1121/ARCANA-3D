using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the game UI and any on-screen popups that the player may see.
/// </summary>
[RequireComponent(typeof(PlayerCore))]
public class PlayerUI : MonoBehaviour
{
    // I considered removing this / moving it into another class, however the idea of having all UI-related code here
    // is appealing enough to keep this for now.
    
    [SerializeField] public TextMeshProUGUI promptText;
    [SerializeField] public StatusTextListener status;
    [SerializeField] public TextMeshProUGUI debugText;
    [SerializeField] public Canvas hudCanvas;
    [SerializeField] public PauseMenuLogic pauseMenu;
    
    // Update is called once per frame
    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
    
}
