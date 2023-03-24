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
    
    // Start is called before the first frame update
    private void Start()
    {
        startButton.interactable = false;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting...";
    }
    
    public void StartGameButton()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonPacket.START.Value = true;
            startButton.interactable = false;
            ScreenFade.instance.LoadSceneWithFade("Maze");
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
            ScreenFade.instance.LoadSceneWithFade("Library");
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        startButton.interactable = false;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Waiting...";
    }
}
