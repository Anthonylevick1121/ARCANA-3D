using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionBottle : PotionPuzzleObject
{
    public override string GetPrompt() => "Pick up " + PotionPuzzle.BOTTLE_NAMES[Id] + " Bottle";
}
