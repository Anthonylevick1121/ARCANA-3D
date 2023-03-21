using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(LibraryState))]
public class LibraryPotionState : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject[] symbols;
    [SerializeField] private GameObject solutionEffect;
    [SerializeField] private Material solutionSymbolMat;
    [SerializeField] private Material notSolutionSymbolMat;
    
    private LibraryState library;
    
    // Start is called before the first frame update
    private void Start()
    {
        library = GetComponent<LibraryState>();
        
        // pick/fetch solution
        int solutionSymbol = PhotonNetwork.IsConnected
            ? PhotonPacket.POTION_SYMBOL.Value
            : Random.Range(0, symbols.Length);
        
        library.debugText.text = "(debug text)\nGlowing symbol is symbol #" + solutionSymbol +
            (PhotonNetwork.IsConnected ? "" : " (no connection)");
        
        // assign materials
        for (int i = 0; i < symbols.Length; i++)
        {
            symbols[i].GetComponentInChildren<MeshRenderer>().material =
                i == solutionSymbol ? solutionSymbolMat : notSolutionSymbolMat;
            if (i == solutionSymbol)
                Instantiate(solutionEffect, symbols[i].transform);
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(PhotonPacket.POTION_WIN.GetOr(propertiesThatChanged, false))
            library.statusText.SetStatus("Potion Puzzle Solved!");
    }
}
