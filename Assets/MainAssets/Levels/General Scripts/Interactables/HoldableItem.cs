using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HoldableItem : Interactable
{
    [HideInInspector]
    public Rigidbody rbody;

    protected override void Start()
    {
        base.Start();
        rbody = GetComponent<Rigidbody>();
    }

    protected override void Interact(PlayerCore player, HoldableItem heldItem)
    {
        player.interaction.HoldItem(this);
    }
}