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
    
    // Start is called before the first frame update
    private void Start()
    {
        MusicManager.DestroyInstance();
        debugText.text = "Glowing symbol is symbol #" + (PhotonNetwork.IsConnected ? PhotonPacket.POTION_SYMBOL.Value : "(no connection)");
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(PhotonPacket.POTION_WIN.GetOr(propertiesThatChanged, false))
            statusText.SetStatus("Potion Puzzle Solved!");
    }
}
