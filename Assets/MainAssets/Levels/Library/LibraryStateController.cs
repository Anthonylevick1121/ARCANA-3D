using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;

/// <summary>
/// Initialize any dynamic stuff that needs to change run to run in the library
/// </summary>
public class LibraryStateController : MonoBehaviourPunCallbacks
{
    // [SerializeField] private 
    
    [SerializeField] private StatusTextListener statusText;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private GameObject[] symbols;
    [SerializeField] private GameObject solutionEffect;
    [SerializeField] private Material solutionSymbolMat;
    [SerializeField] private Material notSolutionSymbolMat;
    
    // Start is called before the first frame update
    private void Start()
    {
        MusicManager.DestroyInstance();
        
        // pick/fetch solution
        int solutionSymbol = PhotonNetwork.IsConnected
            ? PhotonPacket.POTION_SYMBOL.Value
            : Random.Range(0, symbols.Length);
        
        debugText.text = "(debug text)\nGlowing symbol is symbol #" + solutionSymbol + (PhotonNetwork.IsConnected ? "" : " (no connection)");
        
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
            statusText.SetStatus("Potion Puzzle Solved!");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            debugText.gameObject.SetActive(!debugText.gameObject.activeSelf);
    }
}
