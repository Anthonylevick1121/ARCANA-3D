using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class HoldableItem : Interactable
{
    private Rigidbody rbody;
    
    [SerializeField]
    private AudioClip dropSound;
    
    // we only want to play the drop sound when it hits something immediately after being dropped
    // too many collision sounds is really messy and loud lmao
    private bool playDropSound;
    
    protected override void Start()
    {
        base.Start();
        rbody = GetComponent<Rigidbody>();
        playDropSound = false;
    }
    
    protected override bool Interact( PlayerCore player, HoldableItem heldItem)
    {
        player.interaction.HoldItem(this);
        return true;
    }
    
    public void SetIsHeld(bool isHeld)
    {
        rbody.isKinematic = isHeld;
        if (!isHeld) playDropSound = true;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (!playDropSound || collision.collider.isTrigger) return;
        playDropSound = false;
        audio.PlayOneShot(dropSound);
    }
}