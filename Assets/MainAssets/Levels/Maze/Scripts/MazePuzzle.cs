using UnityEngine;
using UnityEngine.Serialization;

public class MazePuzzle : MonoBehaviour
{
    public static MazePuzzle instance;
    private void Awake() => instance = this;
    
    [HideInInspector] public bool debug = false;
    
    [SerializeField] public PlayerCore player;
    [SerializeField] public EnemyController enemy;
    
    // ORDERED list of the corridor parent objects, in order of the MazeSectionPos enum
    [SerializeField] private GameObject[] corridorParents;
    [SerializeField] private GameObject[] ritualSymbols;
    [SerializeField] private GameObject tutorialDoor;
    
    [SerializeField] private Material dormantSymbolMat;
    [SerializeField] private Material activeSymbolMat;
    
    // internal state, tracking which levers have been touched at least once
    // private bool[] touchedLevers = new bool[9];
    private int leverTouchCount = 0;
    
    private static readonly int MAZE_TILE_DIM = 8;
    private static readonly int BIG_MAZE_MAX = 464;
    private static readonly int BIG_MAZE_DIM = BIG_MAZE_MAX / MAZE_TILE_DIM;
    
    private void Start()
    {
        MusicManager.DestroyInstance();
        
        foreach (GameObject parent in corridorParents)
            parent.transform.GetChild(2).GetComponentInChildren<Renderer>().material = dormantSymbolMat;
        foreach (GameObject sym in ritualSymbols)
            sym.GetComponentInChildren<Renderer>().material = dormantSymbolMat;
    }
    
    public MazeSectionPos GetMazeSection(Vector3 pos)
    {
        // TODO if -Z, clamp calc into tutorial maze pos and flip X
        if (pos.z < -4)
            return MazeSectionPos.Tutorial;
        
        int xPos = Mathf.Clamp(Mathf.FloorToInt(pos.x / MAZE_TILE_DIM / (BIG_MAZE_DIM / 3f)), 0, 2);
        int zPos = Mathf.Clamp(Mathf.FloorToInt(pos.z / MAZE_TILE_DIM / (BIG_MAZE_DIM / 3f)), 0, 2);
        int sectionInt = (xPos * 3 + zPos);
        return (MazeSectionPos) sectionInt;
        // print($"current maze pos: {sectionInt}; xPos {xPos} zPos {zPos}");
    }
    
    // ritual circle checks for game end cond
    public bool CheckWinnable() => leverTouchCount == 9;
    
    public void TouchLever(MazeSectionPos pos)
    {
        int idx = (int) pos;
        
        // set lever symbol material
        corridorParents[idx].transform.GetChild(2).GetComponentInChildren<Renderer>().material = activeSymbolMat;
        ritualSymbols[idx].GetComponentInChildren<Renderer>().material = activeSymbolMat;
        
        // don't count tutorial levers for the final check
        if (pos == MazeSectionPos.Tutorial)
        {
            // instead, open the door
            tutorialDoor.SetActive(false);
            return;
        }
        // we've touched one more lever
        leverTouchCount++;
        enemy.OnLeverPulled();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            debug = !debug;
        
        if (!debug) return;
        
        int color = -1;
        // debug press-any-lever
        for (KeyCode i = KeyCode.Alpha1; i <= KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                color = i - KeyCode.Alpha1;
                break;
            }
        }
        for (KeyCode i = KeyCode.Keypad1; i <= KeyCode.Keypad9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                color = i - KeyCode.Keypad1;
                break;
            }
        }
        
        if (color >= 0)
        {
            corridorParents[color].transform.GetChild(1).GetComponent<MazeSectionLever>()
                .BaseInteract(player, null);
        }
    }
}
