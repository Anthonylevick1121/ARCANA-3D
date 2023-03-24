using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

[RequireComponent(typeof(LibraryState))]
public class LibraryMazeState : MonoBehaviourPunCallbacks
{
    // each of these are expected to have children ordered by the MazeSectionPos enum.
    [SerializeField] private Renderer[] mapSections;
    [SerializeField] private Material[] mapStateStart;
    [SerializeField] private Material[] mapStateAlt;
    [SerializeField] private Material[] mapStateDone;
    // [SerializeField] private Color completedMapColor;
    [SerializeField] private Color playerMapColor; // player in section
    [SerializeField] private Color inactiveMapColor;
    
    [SerializeField] private GameObject[] mapCovers;
    private Renderer[] mapSymbols; // sourced from the covers
    [SerializeField] private Material mapSymbolBaseMat; // player in section
    [SerializeField] private Material mapSymbolEnemyMat; // player in section
    // enemy section is emissive
    
    private LibraryState library;
    private int enemySection = -1;
    private int playerSection = -1;
    private bool[] levers;
    private bool librarianLever;
    
    private enum DebugType
    {
        PlayerPos, EnemyPos, Lever
    }
    private static readonly int DebugTypeCount = Enum.GetValues(typeof(DebugType)).Length;
    
    private DebugType debugType = DebugType.PlayerPos;
    
    // Start is called before the first frame update
    private void Start()
    {
        library = GetComponent<LibraryState>();
        levers = new bool[Enum.GetValues(typeof(MazeSectionPos)).Length];
        mapSymbols = new Renderer[mapSections.Length];
        Assert.AreEqual(levers.Length, mapSections.Length, "A map section is required for every maze position.");
        for(int i = 0; i < mapSections.Length; i++)
        {
            // color the cover symbols, those will be seen first
            mapSymbols[i] = mapCovers[i].transform.GetChild(0).GetComponentInChildren<Renderer>();
            mapSymbols[i].material = mapSymbolBaseMat;
            // ensure each section is displaying proper initial state
            UpdateSectionColor(i);
        }
        library.debugText.text = "Debug mode: " + Enum.GetName(typeof(DebugType), debugType);
    }
    
    private void Update()
    {
        if (!library.debug) return;
        
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            // cycle debug type
            debugType = (DebugType) (((int) debugType + 1) % DebugTypeCount);
            library.debugText.text = "Debug mode: " + Enum.GetName(typeof(DebugType), debugType);
        }
        
        int section = -1;
        // debug-send packets relating to diff sections
        
        for (KeyCode i = KeyCode.Alpha0; i <= KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                section = i == KeyCode.Alpha0 ? 9 : i - KeyCode.Alpha1;
                break;
            }
        }
        for (KeyCode i = KeyCode.Keypad0; i <= KeyCode.Keypad9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                section = i == KeyCode.Keypad0 ? 9 : i - KeyCode.Keypad1;
                break;
            }
        }
        
        if (section < 0) return;
        
        PhotonPacketType<int> packet = debugType switch
        {
            DebugType.PlayerPos => PhotonPacket.MAZE_PLAYER,
            DebugType.EnemyPos => PhotonPacket.MAZE_ENEMY,
            DebugType.Lever => PhotonPacket.MAZE_LEVER,
            _ => throw new ArgumentException(debugType.ToString()) // this should never occur
        };
        OnRoomPropertiesUpdate(packet.Mock(section));
    }
    
    private void UpdateSectionColor(int section)
    {
        if (section < 0) return;
        
        // could be:
        // - inactive, or player - color
        // - enemy or not - emission
        // - librarian + player lever state - picks material
        
        // since it changes the material, first pick material
        Material targetMat = mapStateStart.Length == 0 ? mapSections[section].material : // small debug addition
            (levers[section] ? mapStateDone : librarianLever ? mapStateAlt : mapStateStart)[section];
        
        // set emission based on enemy
        if(enemySection == section) targetMat.EnableKeyword("_EMISSION");
        else targetMat.DisableKeyword("_EMISSION");
        
        // set color based on player
        targetMat.color = playerSection == section ? playerMapColor : inactiveMapColor;
        
        // assign material
        mapSections[section].material = targetMat;
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        // print("room property updates for "+deltaProps.ToString());
        
        if (PhotonPacket.MAZE_LEVER.WasChanged(deltaProps))
        {
            int idx = PhotonPacket.MAZE_LEVER.Get(deltaProps);
            print("maze lever flip: "+idx);
            levers[idx] = true;
            if(idx == (int) MazeSectionPos.Tutorial)
                library.statusText.SetStatus("Lever Flipped!\nA door opens... guide well.");
            else
                library.statusText.SetStatus("Lever Flipped!\nMore magic flows to the ritual circle...");
            UpdateSectionColor(idx);
        }
        
        if (PhotonPacket.MAZE_PLAYER.WasChanged(deltaProps))
        {
            int idx = PhotonPacket.MAZE_PLAYER.Get(deltaProps);
            print("player in section "+idx);
            // set material color
            if (playerSection >= 0) mapSections[playerSection].material.color = inactiveMapColor;
            mapSections[idx].material.color = playerMapColor;
            // uncover
            mapCovers[idx].SetActive(false);
            // save
            playerSection = idx;
        }
        
        if (PhotonPacket.MAZE_ENEMY.WasChanged(deltaProps))
        {
            int idx = PhotonPacket.MAZE_ENEMY.Get(deltaProps);
            print("library received enemy pos "+idx);
            
            // emission tracking on map and cover
            if (enemySection >= 0)
            {
                mapSections[enemySection].material.DisableKeyword("_EMISSION");
                mapSymbols[enemySection].material = mapSymbolBaseMat;
            }
            
            mapSections[idx].material.EnableKeyword("_EMISSION");
            mapSymbols[idx].material = mapSymbolEnemyMat;
            
            enemySection = idx;
        }
    }
    
    public void OnFlipLibrarianLever(bool flipped)
    {
        print("librarian flip! "+flipped);
        librarianLever = flipped;
        // trigger an update of all maps
        for(int i = 0; i < levers.Length; i++)
            UpdateSectionColor(i);
    }
}
