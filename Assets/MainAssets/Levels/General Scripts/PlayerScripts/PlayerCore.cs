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
    
    public bool debug = false;
    
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
        
        SetDebug(debug);
    }
    
    private void SetDebug(bool debug)
    {
        this.debug = debug;
        ui.debugText.gameObject.SetActive(debug);
        if (debug && ui.debugText.text.Length == 0)
            ui.debugText.text = "debug mode on.";
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) SetDebug(!debug);
    }
    
    public void ToggleGameInput(bool active)
    {
        if(active) InputActions.Enable();
        else InputActions.Disable();
        
        ui.hudCanvas.enabled = active;
        
        // cursor is on when input is not
        Cursor.visible = !active;
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
    }
    
    private void OnEnable() => InputActions.Enable();
    private void OnDisable() => InputActions.Disable();
}
