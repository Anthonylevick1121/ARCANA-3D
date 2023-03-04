public class PotionGoal : Interactable
{
	protected override void Interact(PlayerCore player, HoldableItem heldItem)
	{
		if(heldItem is PotionBottle bottle) PotionPuzzle.instance.AddBottle(bottle);
		else if (heldItem is PotionIngredient ing) PotionPuzzle.instance.AddIngredient(ing);
		else return;
		player.interaction.DropItem();
	}
}
