using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PotionPuzzle : MonoBehaviour
{
    public static PotionPuzzle instance;
    
    [SerializeField] private PotionPuzzleCategory bottles;
    [SerializeField] private PotionPuzzleCategory ingredients;
    [SerializeField] private PotionPuzzleCategory symbols;
    
    // STATIC DATA
    // these don't actually affect the game ASIDE from length checks which error if a different number of prefabs are given.
    private static readonly string[] BOTTLE_NAMES = {"Cone", "Cube", "Cylinder", "Sphere", "Inverted"};
    private static readonly string[] COLOR_NAMES = {"Red", "Blue", "Green", "Yellow", "Purple"};
    // (no symbols because 2 symbols associated per color, in order based on the prefab list)
    private static readonly string[] INGREDIENT_NAMES = {"Ivy Leaf", "Blue", "Green", "Yellow", "Purple"};
    // does affect the game actually
    private static readonly int INGREDIENT_COUNT = 4;
    
    private static readonly int[,] SOLUTION_TABLE = { { } };
    
    // PUZZLE INSTANCE DATA
    private int correctBottle; // get from solution
    private int[] correctIngredients; // parse from solution
    // these just create an effect, they aren't technically needed for the solution
    private int correctSymbol; // there are two symbols per color; this is just the full symbol tho
    private int correctToxic; // get from solution
    
    private void Awake() => instance = this;
    
    // Start is called before the first frame update
    private void Start()
    {
        // if (true) return;
        
        // validate prefab lists
        bottles.ValidatePrefabs(BOTTLE_NAMES.Length, "bottle");
        // ingredients.ValidatePrefabs(INGREDIENT_NAMES.Length, "ingredient");
        // symbols.ValidatePrefabs(COLOR_NAMES.Length * 2, "symbol");
        
        // randomize the locations of each
        bottles.InitializePlacement();
        ingredients.InitializePlacement(); // todo special effect idx
        // symbols.InitializePlacement(); // todo symbols and special effect idx

        if (true) return;
        
        // create a solution
        int solIndex = Random.Range(0, SOLUTION_TABLE.Length);
        correctBottle = SOLUTION_TABLE[solIndex, 0];
        correctSymbol = SOLUTION_TABLE[solIndex, 1] * 2 + Random.Range(0, 2);
        correctToxic = SOLUTION_TABLE[solIndex, 2];
        correctIngredients = new int[INGREDIENT_COUNT];
        for (int i = 0; i < INGREDIENT_COUNT; i++)
            correctIngredients[i] = SOLUTION_TABLE[solIndex, 3 + i];
    }
    
    public void AddIngredient(PotionIngredient ing)
    {
        Debug.Log("added ingredient! "+ing.name);
        ingredients.ReplaceConsumable(ing);
    }
    
    public void AddBottle(PotionBottle bottle)
    {
        Debug.Log("added bottle! "+bottle.name);
        bottles.ReplaceConsumable(bottle);
    }
}
