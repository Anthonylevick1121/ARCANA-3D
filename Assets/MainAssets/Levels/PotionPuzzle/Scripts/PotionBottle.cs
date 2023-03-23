public class PotionBottle : PotionPuzzleObject
{
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "Bottle";
    }
    
    // public override string GetPrompt(HoldableItem heldItem) => "Pick up " + PotionPuzzle.BOTTLE_NAMES[Id] + " Bottle";
    public override string GetPrompt(HoldableItem heldItem) => "Pick up Bottle";
}
