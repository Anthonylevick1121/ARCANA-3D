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
    
    public void EndCredits() => ScreenFade.instance.LoadSceneWithFade("CreditsPage", false);
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.GAME_WIN.WasChanged(deltaProps))
        {
            bool win = PhotonPacket.GAME_WIN.Get(deltaProps);
            // statusText.SetStatus("Ritual Circle activated!\nYou saved the arch mage.");
            // disable input and other text
            player.ToggleGameInput(false);
            
            if(PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
            (win ? winScreen : loseScreen).SetActive(true);
        }
    }
}
