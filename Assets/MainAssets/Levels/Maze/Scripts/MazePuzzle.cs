using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class MazePuzzle : MonoBehaviourPunCallbacks
{
    public static MazePuzzle instance;
    private void Awake() => instance = this;
    
    public bool debug => player.debug;
    
    [SerializeField] public PlayerCore player;
    [SerializeField] public EnemyController enemy;
    
    // ORDERED list of the corridor parent objects, in order of the MazeSectionPos enum
    [SerializeField] private CorridorParent[] corridorParents;
    // same, but these are the symbols on the ritual circle
    [SerializeField] private GameObject[] ritualSymbolParents;
    private Renderer[] ritualSymbols; // cache the renderers
    [SerializeField] private GameObject tutorialDoor;
    [SerializeField] private GameObject tutorialDoorGroundSymbol;
    
    [SerializeField] public Material dormantSymbolMat;
    [SerializeField] public Material activeSymbolMat;
    
    // internal state, tracking which levers have been touched at least once
    // private bool[] touchedLevers = new bool[9];
    private int leverTouchCount = 0;
    
    private static readonly int MAZE_TILE_DIM = 8;
    // private static readonly int BIG_MAZE_MAX = 464;
    // private static readonly int BIG_MAZE_DIM = BIG_MAZE_MAX / MAZE_TILE_DIM;
    
    private void Start()
    {
        MusicManager.DestroyInstance();

        ritualSymbols = new Renderer[ritualSymbolParents.Length];
        for (int i = 0; i < ritualSymbols.Length; i++)
        {
            Renderer sym = ritualSymbols[i] = ritualSymbolParents[i].GetComponentInChildren<Renderer>();
            sym.material = dormantSymbolMat;
        }
        tutorialDoorGroundSymbol.SetActive(false);
    }
    
    public static MazeSectionPos GetMazeSection(Vector3 pos)
    {
        if (pos.z < -4)
            return MazeSectionPos.Tutorial;
        
        // tile positions are:
        // 0-19 = 0
        // 20-38 = 1
        // 39-58 = 2
        
        int xTile = Mathf.FloorToInt(pos.x / MAZE_TILE_DIM);
        int zTile = Mathf.FloorToInt(pos.z / MAZE_TILE_DIM);
        
        int xPos = xTile < 20 ? 0 : xTile < 39 ? 1 : 2;
        int zPos = zTile < 20 ? 0 : zTile < 39 ? 1 : 2;
        
        // int xPos = Mathf.Clamp(Mathf.FloorToInt(pos.x / MAZE_TILE_DIM / (BIG_MAZE_DIM / 3f)), 0, 2);
        // int zPos = Mathf.Clamp(Mathf.FloorToInt(pos.z / MAZE_TILE_DIM / (BIG_MAZE_DIM / 3f)), 0, 2);
        int sectionInt = (xPos * 3 + zPos);
        return (MazeSectionPos) sectionInt;
        // print($"current maze pos: {sectionInt}; xPos {xPos} zPos {zPos}");
    }
    
    // ritual circle checks for game end cond
    public bool CheckWinnable() => leverTouchCount == 9;
    
    public void TouchLever(MazeSectionPos pos)
    {
        int idx = (int) pos;
        
        // set symbol material
        ritualSymbols[idx].material = activeSymbolMat;
        
        // notify corridor parent for wall/symbol behavior
        corridorParents[idx].PullAreaLever();
        
        // don't count tutorial levers for the final check
        if (pos == MazeSectionPos.Tutorial)
        {
            // instead, open the door
            tutorialDoor.SetActive(false);
            tutorialDoorGroundSymbol.SetActive(true);
            return;
        }
        // we've touched one more lever
        leverTouchCount++;
        enemy.OnLeverPulled();
    }
    
    private void Update()
    {
        if (!debug) return;
        
        int section = -1;
        // debug press-any-lever
        for (KeyCode i = KeyCode.Alpha1; i <= KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                section = i - KeyCode.Alpha1;
                break;
            }
        }
        for (KeyCode i = KeyCode.Keypad1; i <= KeyCode.Keypad9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                section = i - KeyCode.Keypad1;
                break;
            }
        }
        
        if (section >= 0)
            corridorParents[section].DebugFlipAreaLever(player, null);
        
        if(Input.GetKeyDown(KeyCode.BackQuote))
            OnRoomPropertiesUpdate(PhotonPacket.MAZE_LIB_LEVER.Mock(!corridorParents[0].DebugGetLibLever()));
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.MAZE_LIB_LEVER.WasChanged(deltaProps))
        {
            bool flipped = PhotonPacket.MAZE_LIB_LEVER.Get(deltaProps);
            print("Librarian lever was flipped! "+flipped);
            
            foreach (CorridorParent corridor in corridorParents)
                corridor.SetWallState(flipped);
        }
    }
}
