public class PotionGoal : Interactable
{
    public override string GetPrompt(HoldableItem heldItem) =>
        heldItem is PotionBottle ? "Drink Potion" :
        heldItem is PotionIngredient ? "Add Ingredient to Brew" :
        PotionPuzzle.instance.HasPotionBrewing() ? "(interact to reset the brew)" :
        "Add ingredients then bottle it here";
    
    protected override bool Interact(PlayerCore player, HoldableItem heldItem)
    {
        if (!heldItem)
            return PotionPuzzle.instance.ClearPotion();
        
        bool success = TryAdd(heldItem);
        if(success)
            player.interaction.DropItem();
        else // don't think this branch will run now
            player.ui.status.SetStatus("Pick up an ingredient to add first.");
        return success;
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
