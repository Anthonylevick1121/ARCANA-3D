using JetBrains.Annotations;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    private string promptMessage;
    
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }
    
    public void BaseInteract(PlayerCore player, [CanBeNull] HoldableItem heldItem) => Interact(player, heldItem);
    
    protected abstract void Interact(PlayerCore player, [CanBeNull] HoldableItem heldItem);
    
    public virtual string GetPrompt() => promptMessage;
}
