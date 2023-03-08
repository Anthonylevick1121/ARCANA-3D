public class PotionGoal : Interactable
{
    public override string GetPrompt() => "Place Ingredient / Bottle";
    
    protected override void Interact(PlayerCore player, HoldableItem heldItem)
    {
        if(TryAdd(heldItem))
            player.interaction.DropItem();
    }
    
    private bool TryAdd(HoldableItem item)
    {
        return item switch
        {
            PotionBottle bottle => PotionPuzzle.instance.AddBottle(bottle),
            PotionIngredient ing => PotionPuzzle.instance.AddIngredient(ing),
            _ => false
        };
    }
}
