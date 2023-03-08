using UnityEngine;

[RequireComponent(typeof(PlayerUI))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCamera))]
[RequireComponent(typeof(PlayerInteraction))]
public class PlayerCore : MonoBehaviour
{
    // might add more to this later, for now it holds the input map and requires all the other player components
    
    [HideInInspector] public PlayerUI ui;
    [HideInInspector] public PlayerMovement movement;
    [HideInInspector] public PlayerCamera view;
    [HideInInspector] public PlayerInteraction interaction;
    
    public PlayerActionMap.PlayerActionsActions InputActions { get; private set; }
    
    private void Awake()
    {
        PlayerActionMap input = new ();
        InputActions = input.playerActions;
    }
    
    private void Start()
    {
        ui = GetComponent<PlayerUI>();
        movement = GetComponent<PlayerMovement>();
        view = GetComponent<PlayerCamera>();
        interaction = GetComponent<PlayerInteraction>();
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
