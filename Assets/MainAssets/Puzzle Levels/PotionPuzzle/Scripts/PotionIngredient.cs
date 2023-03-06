using UnityEngine;

public class PotionIngredient : PotionPuzzleObject
{
	public override string GetPrompt() => "Pick up " + PotionPuzzle.INGREDIENT_NAMES[Id];
}
