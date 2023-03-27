using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HiddenDoorKey : Interactable
{
    private Animator keyAnimator;
    [SerializeField] private Animator doorAnimator;
    
    private static readonly int keyPushTrigger = Animator.StringToHash("Push");
    private static readonly int doorOpenTrigger = Animator.StringToHash("Open Doors");
    
    private bool pushed;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        keyAnimator = GetComponent<Animator>();
    }
    
    protected override bool Interact(PlayerCore player, HoldableItem heldItem)
    {
        if (pushed) return false;
        pushed = true;
        // start the anim
        keyAnimator.SetTrigger(keyPushTrigger);
        return true;
    }
    
    public void OnPushFinish()
    {
        // open the doors
        doorAnimator.SetTrigger(doorOpenTrigger);
    }
    
    public override string GetPrompt(HoldableItem heldItem) => pushed ? "" : "Push the rock inward";
}
