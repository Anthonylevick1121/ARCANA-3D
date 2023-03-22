using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using UnityEngine;

[RequireComponent(typeof(LibraryState))]
public class LibraryMazeState : MonoBehaviourPunCallbacks
{
    // each of these are expected to have children ordered by the MazeSectionPos enum.
    // [SerializeField] private Transform mazeSymbols;
    // [SerializeField] private Transform mapCover;
    // [SerializeField] private Transform mapLit;
    // [SerializeField] private Transform mapUnlit;
    [SerializeField] private GameObject[] mapCovers;
    [SerializeField] private Renderer[] mapSections;
    [SerializeField] private Color standardMapColor; // lever not flipped
    [SerializeField] private Color completedMapColor; // lever flipped
    [SerializeField] private Color playerSectionColor; // player in section
    
    private Renderer[] mapSymbols;
    [SerializeField] private Material mapSymbolMat; // player in section
    [SerializeField] private Material mapSymbolEnemyMat; // player in section
    // enemy section is emissive
    
    // private GameObject[] defaultOpenSymbolBacks;
    // private GameObject[] defaultClosedSymbolBacks;
    // [SerializeField] private Material offSymbolMat, onSymbolMat;
    
    private LibraryState library;
    private int enemySection;
    private int playerSection = -1;
    private bool[] levers;
    
    // Start is called before the first frame update
    private void Start()
    {
        library = GetComponent<LibraryState>();
        levers = new bool[Enum.GetValues(typeof(MazeSectionPos)).Length];
        // int symbolCount = mazeSymbols.childCount;
        // defaultOpenSymbolBacks = new GameObject[symbolCount];
        // defaultClosedSymbolBacks = new GameObject[symbolCount];
        // for (int i = 0; i < symbolCount; i++)
        // {
            // defaultOpenSymbolBacks[i] = mazeSymbols.GetChild(i).Find("Circle").gameObject;
            // defaultClosedSymbolBacks[i] = mazeSymbols.GetChild(i).Find("Triangle").gameObject;
        // }
        mapSymbols = new Renderer[mapSections.Length];
        for(int i = 0; i < mapSections.Length; i++)
        {
            // color the cover symbol
            mapSymbols[i] = mapCovers[i].transform.GetChild(0).GetComponentInChildren<Renderer>();
            mapSymbols[i].material = mapSymbolMat;
            // color the map sections
            mapSections[i].material.color = standardMapColor;
            mapSections[i].material.DisableKeyword("_EMISSIVE");
        }
        
        // debug?
        if (!PhotonNetwork.IsConnected)
        {
            OnRoomPropertiesUpdate(PhotonPacket.MAZE_ENEMY.Mock(4));
            OnRoomPropertiesUpdate(PhotonPacket.MAZE_PLAYER.Mock(9));
        }
    }
    
    private void Update()
    {
        if (!library.debug) return;
        
        int colorPlayer = -1;
        int colorLever = -1;
        // debug press-any-lever
        for (KeyCode i = KeyCode.Alpha0; i <= KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                colorPlayer = i == KeyCode.Alpha0 ? 9 : i - KeyCode.Alpha1;
                break;
            }
        }
        for (KeyCode i = KeyCode.Keypad0; i <= KeyCode.Keypad9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                colorLever = i == KeyCode.Keypad0 ? 9 : i - KeyCode.Keypad1;
                break;
            }
        }
        
        if (colorPlayer >= 0)
            OnRoomPropertiesUpdate(new Hashtable { { PhotonPacket.MAZE_PLAYER.key, colorPlayer } });
        if (colorLever >= 0)
            OnRoomPropertiesUpdate(new Hashtable { { PhotonPacket.MAZE_LEVER.key, colorLever } });
    }
    
    private void UpdateSectionColor(int section)
    {
        Color color = section == playerSection ? playerSectionColor : standardMapColor;
        if (levers[section])
            color *= completedMapColor;
        mapSections[section].material.color = color;
    }
    
    // private void OnLeverFlip(MazeSectionPos pos, bool flipped)
    // {
    //     int idx = (int) pos;
    //     levers[idx] = true;
    //     mapSections[idx].material.color = completedMapColor;
    //     library.statusText.SetStatus("Lever Flipped!\nMore magic flows to the center of the maze...");
    //     
    //     // set the glow state of the matching symbol
    //     // mazeSymbols.GetChild(idx).GetComponentInChildren<Renderer>().material = flipped ? onSymbolMat : offSymbolMat;
    //     // defaultOpenSymbolBacks[idx].SetActive(!flipped);
    //     // defaultClosedSymbolBacks[idx].SetActive(flipped);
    //     // ensure map section is unhidden
    //     // mapCover.GetChild(idx).gameObject.SetActive(false);
    // }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        // print("room property updates for "+deltaProps.ToString());
        
        if (PhotonPacket.MAZE_LEVER.WasChanged(deltaProps))
        {
            // OnLeverFlip((MazeSectionPos) PhotonPacket.MAZE_LEVER.Value, PhotonPacket.MAZE_LEVER_FLIP.Value);
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
            // uncover
            mapCovers[idx].SetActive(false);
            // save
            int prevPlayer = playerSection;
            playerSection = idx;
            // update tracking
            if(prevPlayer >= 0) UpdateSectionColor(prevPlayer);
            UpdateSectionColor(idx);
        }
        
        if (PhotonPacket.MAZE_ENEMY.WasChanged(deltaProps))
        {
            int idx = PhotonPacket.MAZE_ENEMY.Get(deltaProps);
            print("library received enemy pos "+idx);
            
            // emission tracking on map and cover
            if (enemySection >= 0)
            {
                mapSections[enemySection].material.DisableKeyword("_EMISSION");
                mapSymbols[enemySection].material = mapSymbolMat;
            }
            
            mapSections[idx].material.EnableKeyword("_EMISSION");
            mapSymbols[idx].material = mapSymbolEnemyMat;
            
            // also do the cover in case it's an unknown section
            
            // light the torch in old section
            // mapLit.GetChild(enemyPos).gameObject.SetActive(true);
            // mapUnlit.GetChild(enemyPos).gameObject.SetActive(false);
            // unlight the torch in new section
            // mapUnlit.GetChild(idx).gameObject.SetActive(true);
            // mapLit.GetChild(idx).gameObject.SetActive(false);
            // store section id
            enemySection = idx;
        }
        
        if (PhotonPacket.MAZE_WIN.GetOr(deltaProps, false))
        {
            library.statusText.SetStatus("Ritual Circle activated!\nYou saved the arch mage.");
        }
    }
}
