using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PlayerUI))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
[RequireComponent(typeof(PlayerInteraction))]
public class PlayerCore : MonoBehaviour
{
    // might add more to this later, for now it holds the input map and requires all the other player components
    
    public PlayerActionMap.PlayerActionsActions InputActions { get; private set; }
    
    private void Awake()
    {
        PlayerActionMap input = new ();
        InputActions = input.playerActions;
    }
    
    private void OnEnable()
    {
        InputActions.Enable();
    }
    
    private void OnDisable()
    {
        InputActions.Disable();
    }
}
