using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(LibraryState))]
public class LibraryMazeState : MonoBehaviourPunCallbacks
{
    // each of these are expected to have children ordered by the MazeSectionPos enum.
    [SerializeField] private Transform mazeSymbols;
    [SerializeField] private Transform mapCover;
    [SerializeField] private Transform mapLit;
    [SerializeField] private Transform mapUnlit;
    
    private GameObject[] defaultOpenSymbolBacks;
    private GameObject[] defaultClosedSymbolBacks;
    // [SerializeField] private Material offSymbolMat, onSymbolMat;
    
    private LibraryState library;
    private int enemyPos;
    
    // Start is called before the first frame update
    private void Start()
    {
        library = GetComponent<LibraryState>();
        int symbolCount = mazeSymbols.childCount;
        defaultOpenSymbolBacks = new GameObject[symbolCount];
        defaultClosedSymbolBacks = new GameObject[symbolCount];
        for (int i = 0; i < symbolCount; i++)
        {
            defaultOpenSymbolBacks[i] = mazeSymbols.GetChild(i).Find("Circle").gameObject;
            defaultClosedSymbolBacks[i] = mazeSymbols.GetChild(i).Find("Triangle").gameObject;
        }
    }
    
    private void Update()
    {
        if (!library.debug) return;
        
        int color = -1;
        // debug press-any-lever
        for (KeyCode i = KeyCode.Alpha0; i <= KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                color = i == KeyCode.Alpha0 ? 9 : i - KeyCode.Alpha1;
                break;
            }
        }
        for (KeyCode i = KeyCode.Keypad0; i <= KeyCode.Keypad9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                color = i == KeyCode.Keypad0 ? 9 : i - KeyCode.Keypad1;
                break;
            }
        }
        
        if (color >= 0)
        {
            OnLeverFlip((MazeSectionPos) color, defaultOpenSymbolBacks[color].activeSelf);
        }
    }
    
    private void OnLeverFlip(MazeSectionPos pos, bool flipped)
    {
        int idx = (int) pos;
        // set the glow state of the matching symbol
        // mazeSymbols.GetChild(idx).GetComponentInChildren<Renderer>().material = flipped ? onSymbolMat : offSymbolMat;
        defaultOpenSymbolBacks[idx].SetActive(!flipped);
        defaultClosedSymbolBacks[idx].SetActive(flipped);
        // ensure map section is unhidden
        mapCover.GetChild(idx).gameObject.SetActive(false);
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.MAZE_LEVER_ACTION.GetOr(deltaProps, false))
        {
            print("maze lever flip: "+deltaProps);
            OnLeverFlip((MazeSectionPos) PhotonPacket.MAZE_LEVER.Value, PhotonPacket.MAZE_LEVER_FLIP.Value);
        }
        
        // just disable this for now
        if (false && PhotonPacket.MAZE_ENEMY.WasChanged(deltaProps))
        {
            int idx = PhotonPacket.MAZE_ENEMY.Get(deltaProps);
            // light the torch in old section
            mapLit.GetChild(enemyPos).gameObject.SetActive(true);
            mapUnlit.GetChild(enemyPos).gameObject.SetActive(false);
            // unlight the torch in new section
            mapUnlit.GetChild(idx).gameObject.SetActive(true);
            mapLit.GetChild(idx).gameObject.SetActive(false);
            // store section id
            enemyPos = idx;
        }
        
        if (PhotonPacket.MAZE_WIN.GetOr(deltaProps, false))
            library.statusText.SetStatus("Ritual Circle activated!\nYou saved the arch mage.");
    }
}
