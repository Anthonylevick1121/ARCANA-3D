public class PotionGoal : Interactable
{
    public override string GetPrompt(HoldableItem heldItem) =>
        heldItem is PotionBottle ? "Drink Potion" : "Add Ingredient to Brew";
    
    protected override void Interact(PlayerCore player, HoldableItem heldItem)
    {
        if(TryAdd(heldItem))
            player.interaction.DropItem();
        else
            player.ui.status.SetStatus("Pick up an ingredient to add first.");
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
