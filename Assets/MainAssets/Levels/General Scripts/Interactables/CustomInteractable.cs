using UnityEngine;
using UnityEngine.Events;

public class CustomInteractable : Interactable
{
    [SerializeField]
    private string promptMessage;
    
    [SerializeField]
    private UnityEvent<PlayerCore, HoldableItem> onInteract;
    
    protected override bool Interact( PlayerCore player, HoldableItem heldItem)
    {
        if(onInteract != null)
            onInteract.Invoke(player, heldItem);
        return onInteract != null;
    }
    
    public override string GetPrompt(HoldableItem heldItem) => promptMessage;
}
