using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class LibraryState : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerCore player;
    [SerializeField] private GameObject winScreen, loseScreen;
    
    public bool debug => player.debug;
    public StatusTextListener statusText => player.ui.status;
    public TextMeshProUGUI debugText => player.ui.debugText;
    
    // Start is called before the first frame update
    private void Start()
    {
        MusicManager.DestroyInstance();
        debugText.gameObject.SetActive(debug);
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.GAME_END.GetOr(deltaProps, false))
        {
            bool win = PhotonPacket.GAME_WIN.Get(deltaProps);
            // statusText.SetStatus("Ritual Circle activated!\nYou saved the arch mage.");
            // disable input and other text
            player.InputActions.Disable();
            // I'll make a func for this later
            player.ui.promptText.gameObject.SetActive(false);
            player.ui.status.gameObject.SetActive(false);
            player.ui.debugText.gameObject.SetActive(false);
            
            PhotonNetwork.LeaveRoom();
            (win ? winScreen : loseScreen).SetActive(true);
        }
    }
}
