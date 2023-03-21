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
    
    [SerializeField] private Material offSymbolMat, onSymbolMat;
    
    private LibraryState library;
    private int enemyPos;
    
    // Start is called before the first frame update
    private void Start()
    {
        library = GetComponent<LibraryState>();
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.MAZE_LEVER.WasChanged(deltaProps))
        {
            (MazeSectionPos pos, bool flipped) = PhotonPacket.MAZE_LEVER.Get(deltaProps);
            int idx = (int) pos;
            // set the glow state of the matching symbol
            mazeSymbols.GetChild(idx).GetComponentInChildren<Renderer>().material = flipped ? onSymbolMat : offSymbolMat;
            // ensure map section is unhidden
            mapCover.GetChild(idx).gameObject.SetActive(false);
        }
        
        if (PhotonPacket.MAZE_ENEMY.WasChanged(deltaProps))
        {
            int idx = (int) PhotonPacket.MAZE_ENEMY.Get(deltaProps);
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
