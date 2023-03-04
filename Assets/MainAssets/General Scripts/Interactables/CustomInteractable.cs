using UnityEngine;
using UnityEngine.Events;

public class CustomInteractable : Interactable
{
    [SerializeField]
    private UnityEvent<PlayerCore, HoldableItem> onInteract;
    
    protected override void Interact(PlayerCore player, HoldableItem heldItem)
    {
        if(onInteract != null)
            onInteract.Invoke(player, heldItem);
    }
}
