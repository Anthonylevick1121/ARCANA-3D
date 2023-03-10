public class PotionIngredient : PotionPuzzleObject
{
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "Ingredient";
    }
    
    // public override string GetPrompt(HoldableItem heldItem) => "Pick up " + PotionPuzzle.INGREDIENT_NAMES[Id];
    public override string GetPrompt(HoldableItem heldItem) => "Pick up Ingredient";
}
