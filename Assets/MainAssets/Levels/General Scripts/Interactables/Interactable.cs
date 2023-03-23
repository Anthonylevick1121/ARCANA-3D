using JetBrains.Annotations;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // [SerializeField] private string promptMessage;
    [HideInInspector] protected new AudioSource audio;
    [SerializeField] private AudioClip interactionSound;
    
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        audio = GetComponent<AudioSource>();
    }
    
    public void BaseInteract(PlayerCore player, [CanBeNull] HoldableItem heldItem)
    {
        if(Interact(player, heldItem) && interactionSound != null)
            audio.PlayOneShot(interactionSound);
    }
    
    protected abstract bool Interact( PlayerCore player, [CanBeNull] HoldableItem heldItem);
    
    public abstract string GetPrompt([CanBeNull] HoldableItem heldItem);
}
