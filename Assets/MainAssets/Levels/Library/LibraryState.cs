using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class LibraryState : MonoBehaviourPunCallbacks
{
    [SerializeField] public PlayerCore player;
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
    
    private void Update()
    {
        if (!player.debug) return;
        if(Input.GetKeyDown(KeyCode.K))
            OnRoomPropertiesUpdate(PhotonPacket.GAME_WIN.Mock(true));
        if(Input.GetKeyDown(KeyCode.L))
            OnRoomPropertiesUpdate(PhotonPacket.GAME_WIN.Mock(false));
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
            
            if (win) VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeCompleteA);
        }
    }
}
