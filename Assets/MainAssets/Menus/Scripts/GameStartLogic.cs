using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartLogic : BaseMenuLogic
{
    [SerializeField] private Button startButton;
    
    // private static readonly string hostWaitMsg = "Waiting for teammate...";
    // private static readonly string clientWaitMsg = "Waiting for start...";
    private static readonly string hostWaitMsg = "Waiting...";
    private static readonly string clientWaitMsg = "Wait For Host";
    
    // Start is called before the first frame update
    private void Start()
    {
        startButton.interactable = false;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text =
            PhotonNetwork.IsMasterClient ? hostWaitMsg : clientWaitMsg;
    }
    
    public void StartGameButton()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            startButton.interactable = false;
            PhotonPacket.START.Value = true;
            ScreenFade.instance.LoadSceneWithFade("Maze", true);
        }
    }
    
    public void CancelGame()
    {
        // return to room select
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("3_RoomSelect");
    }
    
    private void Update()
    {
        if (!startButton.interactable &&
            PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Game";
            startButton.interactable = true;
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if(!PhotonNetwork.IsMasterClient && PhotonPacket.START.WasChanged(deltaProps))
            ScreenFade.instance.LoadSceneWithFade("Library", true);
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        startButton.interactable = false;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = hostWaitMsg;
    }
}
