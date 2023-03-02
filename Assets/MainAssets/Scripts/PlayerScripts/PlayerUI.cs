using TMPro;
using UnityEngine;

/// <summary>
/// Handles the game UI and any on-screen popups that the player may see.
/// </summary>
[RequireComponent(typeof(PlayerCore))]
public class PlayerUI : MonoBehaviour
{
    // I considered removing this / moving it into another class, however the idea of having all UI-related code here
    // is appealing enough to keep this for now.
    
    [SerializeField]
    public TextMeshProUGUI promptText;
    
    private void Start()
    {
        promptText.text = "";
    }
    
    // Update is called once per frame
    public void UpdateText(string promptMessage)
    {
        promptText.text = promptMessage;
    }
}
