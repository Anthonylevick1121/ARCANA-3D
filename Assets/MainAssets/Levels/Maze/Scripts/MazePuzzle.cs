using UnityEngine;

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
    [SerializeField] private Material onSymbolMat;//, offSymbolMat;
    // internal state, tracking which levers have been touched at least once
    private bool[] touchedLevers = new bool[9];
    private int leverTouchCount = 0;
    
    // ritual circle checks for game end cond
    public bool CheckWinnable() => leverTouchCount == touchedLevers.Length;
    
    public void TouchLever(MazeSectionPos pos)
    {
        int idx = (int) pos;
        if (touchedLevers[idx]) return; // no need to recompute if same value
        touchedLevers[idx] = true;
        ritualSymbols[idx].GetComponentInChildren<Renderer>().material = onSymbolMat;
        // we've touched one more lever
        leverTouchCount++;
        enemy.OnLeverPulled();
    }
    
    private void Start() => MusicManager.DestroyInstance();
    
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
