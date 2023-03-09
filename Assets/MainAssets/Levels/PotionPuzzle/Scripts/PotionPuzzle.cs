using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class PotionPuzzle : MonoBehaviour
{
    public static PotionPuzzle instance;

    [SerializeField] public StatusTextListener statusText;
    [SerializeField] private TextMeshProUGUI debugText;
    private string solutionText;
    
    [SerializeField] private GameObject toxicEffect;
    
    [SerializeField] private PotionPuzzleCategory bottles;
    [SerializeField] private PotionPuzzleCategory ingredients;
    [SerializeField] private PotionPuzzleCategory symbols;
    
    [SerializeField] private Material[] symbolColors;
    
    // STATIC DATA
    // these don't actually affect the game ASIDE from length checks which error if a different number of prefabs are given.
    public static readonly string[] BOTTLE_NAMES = { "Cone", "Cube", "Cylinder", "Sphere", "Inverted" };
    
    public static readonly string[] COLOR_NAMES = { "Red", "Blue", "Green", "Yellow", "Purple" };
    
    // (no symbols because 2 symbols associated per color, in order based on the prefab list)
    public static readonly string[] INGREDIENT_NAMES =
    {
        "Ivy Leaf", "Mead", "Meat", "Carrot", "Water", "Branch", "Skull", "Acid",
        "Plant Root", "Beaker 1", "Beaker 2", "Bread", "Potato", "Quill"
    };
    
    // does affect the game actually
    private static readonly int INGREDIENT_COUNT = 4;
    
    private static readonly int[,] SOLUTION_TABLE =
    {
        { 0, 0, 0, 0, 1, 2, 3 }, { 0, 1, 0, 4, 0, 5, 6 }, { 0, 2, 0, 7, 6, 8, 0 }, { 0, 3, 0, 9, 11, 0, 12 },
        { 0, 4, 0, 0, 10, 3, 13 }, { 0, 0, 9, 3, 9, 6, 5 }, { 0, 1, 9, 9, 11, 8, 4 }, { 0, 2, 9, 1, 13, 9, 0 },
        { 0, 3, 9, 2, 10, 7, 9 }, { 0, 4, 9, 8, 9, 6, 12 }, { 0, 0, 1, 4, 7, 9, 1 }, { 0, 1, 1, 11, 3, 1, 7 },
        { 0, 2, 1, 6, 0, 1, 13 }, { 0, 3, 1, 0, 12, 1, 5 }, { 0, 4, 1, 1, 2, 3, 8 }, { 1, 0, 3, 8, 3, 13, 11 },
        { 1, 1, 3, 12, 4, 7, 3 }, { 1, 2, 3, 10, 2, 3, 9 }, { 1, 3, 3, 3, 5, 6, 13 }, { 1, 4, 3, 3, 7, 1, 2 },
        { 1, 0, 5, 12, 13, 5, 0 }, { 1, 1, 5, 5, 7, 9, 6 }, { 1, 2, 5, 3, 10, 8, 5 }, { 1, 3, 5, 11, 5, 0, 12 },
        { 1, 4, 5, 8, 5, 4, 0 }, { 1, 0, 6, 6, 13, 7, 2 }, { 1, 1, 6, 13, 1, 9, 6 }, { 1, 2, 6, 2, 6, 4, 1 },
        { 1, 3, 6, 12, 3, 5, 6 }, { 1, 4, 6, 0, 10, 11, 6 }, { 2, 0, 11, 9, 11, 13, 2 }, { 2, 1, 11, 11, 5, 1, 6 },
        { 2, 2, 11, 8, 2, 11, 7 }, { 2, 3, 11, 0, 11, 2, 8 }, { 2, 4, 11, 3, 11, 13, 6 }, { 2, 0, 4, 4, 10, 9, 7 },
        { 2, 1, 4, 7, 3, 4, 2 }, { 2, 2, 4, 10, 4, 1, 8 }, { 2, 3, 4, 6, 13, 2, 4 }, { 2, 4, 4, 6, 12, 3, 4 },
        { 2, 0, 10, 10, 9, 8, 6 }, { 2, 1, 10, 13, 3, 10, 7 }, { 2, 2, 10, 4, 0, 10, 12 }, { 2, 3, 10, 2, 3, 10, 13 },
        { 2, 4, 10, 6, 10, 8, 1 }, { 3, 0, 13, 10, 6, 1, 13 }, { 3, 1, 13, 13, 9, 4, 11 }, { 3, 2, 13, 13, 10, 0, 4 },
        { 3, 3, 13, 11, 1, 6, 13 }, { 3, 4, 13, 5, 13, 9, 10 }, { 3, 0, 2, 0, 11, 4, 2 }, { 3, 1, 2, 10, 4, 2, 3 },
        { 3, 2, 2, 13, 2, 6, 11 }, { 3, 3, 2, 2, 4, 10, 1 }, { 3, 4, 2, 2, 10, 5, 0 }, { 3, 0, 7, 11, 6, 13, 7 },
        { 3, 1, 7, 9, 1, 7, 10 }, { 3, 2, 7, 5, 7, 11, 6 }, { 3, 3, 7, 0, 7, 4, 9 }, { 3, 4, 7, 11, 5, 7, 1 },
        { 4, 0, 12, 10, 13, 11, 12 }, { 4, 1, 12, 6, 12, 1, 11 }, { 4, 2, 12, 9, 0, 12, 10 },
        { 4, 3, 12, 11, 12, 10, 6 }, { 4, 4, 12, 0, 12, 11, 5 }, { 4, 0, 8, 8, 10, 1, 13 }, { 4, 1, 8, 6, 13, 0, 8 },
        { 4, 2, 8, 1, 9, 11, 8 }, { 4, 3, 8, 4, 5, 8, 6 }, { 4, 4, 8, 10, 8, 1, 11 }
    };
    
    // PUZZLE INSTANCE DATA
    private int correctBottle; // get from solution
    private int[] correctIngredients; // parse from solution
    // these just create an effect, they aren't technically needed for the solution
    private int correctColor; // get from solution
    private int correctSymbol; // there are two symbols per color; this is just the full symbol tho
    private int correctToxic; // get from solution
    private int[] symbolColorIds; // symbol -> color idx, there will be 2 symbols per color
    
    // runtime, player-driven action data
    private readonly List<int> placedIngredients = new ();
    
    private void Awake() => instance = this;
    
    // Start is called before the first frame update
    private void Start()
    {
        MusicManager.DestroyInstance();
        
        // validate prefab lists
        bottles.ValidatePrefabs(BOTTLE_NAMES.Length, "bottle");
        ingredients.ValidatePrefabs(INGREDIENT_NAMES.Length, "ingredient");
        symbols.ValidatePrefabs(COLOR_NAMES.Length * 2, "symbol");
        Assert.AreEqual(COLOR_NAMES.Length, symbolColors.Length, "need "+COLOR_NAMES.Length+" color materials.");
        
        // create a solution
        int solIndex = Random.Range(0, SOLUTION_TABLE.GetLength(0));
        Debug.Log("solution index: "+solIndex);
        correctBottle = SOLUTION_TABLE[solIndex, 0];
        correctColor = SOLUTION_TABLE[solIndex, 1];
        correctToxic = SOLUTION_TABLE[solIndex, 2];
        correctIngredients = new int[INGREDIENT_COUNT];
        for (int i = 0; i < INGREDIENT_COUNT; i++)
            correctIngredients[i] = SOLUTION_TABLE[solIndex, 3 + i];
        
        // obtain doubled color list
        List<int> colorList = new ();
        for(int i = 0; i < COLOR_NAMES.Length * 2; i++)
            colorList.Add(i/2);
        // randomize the color to symbol matching
        correctSymbol = -1;
        symbolColorIds = new int[COLOR_NAMES.Length * 2];
        for (int i = 0; i < symbolColorIds.Length; i++)
        {
            int idx = Random.Range(0, colorList.Count);
            int colorId = colorList[idx];
            colorList.RemoveAt(idx);
            symbolColorIds[i] = colorId;
            if (colorId == correctColor && correctSymbol < 0)
                correctSymbol = i;
        }
        
        // net sync the relevant solution parameters
        PhotonPacket.POTION_SYMBOL.Value = correctSymbol;
        
        string text = "solution #"+solIndex;
        text += "\nbottle #"+correctBottle+": "+BOTTLE_NAMES[correctBottle];
        text += "\nsymbol #"+correctSymbol+", color "+COLOR_NAMES[correctColor];
        text += "\ntoxic ingredient #"+correctToxic+": "+INGREDIENT_NAMES[correctToxic];
        text += "\ningredient list: ";
        for (int i = 0; i < correctIngredients.Length; i++)
            text += INGREDIENT_NAMES[correctIngredients[i]] + (i < INGREDIENT_COUNT - 1 ? ", " : "");
        solutionText = text;
        debugText.text = "(debug only text)\npress P to reveal the solution.";
        
        // randomize the locations of each
        bottles.InitializePlacement();
        ingredients.InitializePlacement((id, obj) =>
        {
            if (id == correctToxic) Instantiate(toxicEffect, obj.transform);
        });
        symbols.InitializePlacement((id, obj) =>
        {
            obj.GetComponentInChildren<MeshRenderer>().material = symbolColors[symbolColorIds[id]];
        });
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            debugText.text = solutionText;
        if (Input.GetKeyDown(KeyCode.L))
            debugText.gameObject.SetActive(!debugText.gameObject.activeSelf);
    }
    
    public bool AddIngredient(PotionIngredient ing)
    {
        Debug.Log("added ingredient! "+ing.name);
        placedIngredients.Add(ing.Id);
        statusText.SetStatus(INGREDIENT_NAMES[ing.Id]+" was added to the brew.");
        ingredients.ReplaceConsumable(ing);
        return true;
    }
    
    public bool AddBottle(PotionBottle bottle)
    {
        Debug.Log("filled bottle! "+bottle.name);
        int bottleType = bottle.Id;
        bottles.ReplaceConsumable(bottle);
        
        // check what potion we made
        bool correct = bottleType == correctBottle && placedIngredients.Count == INGREDIENT_COUNT;
        for (int i = 0; correct && i < INGREDIENT_COUNT; i++)
            correct = correctIngredients[i] == placedIngredients[i];

        if (correct)
        {
            statusText.SetStatus("You got the right potion! Yay!");
            PhotonPacket.POTION_WIN.Value = true;
        }
        else
            statusText.SetStatus("That was the wrong potion... you feel sick...");
        
        placedIngredients.Clear();
        
        return true;
    }
}
