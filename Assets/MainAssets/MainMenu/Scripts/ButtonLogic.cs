using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviourPunCallbacks
{
    public LevelLoader level;
    
    private void Start()
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient &&
            SceneManager.GetActiveScene().name == "4_RoomLobby")
            GetComponent<Image>().color = Color.gray;
    }
    
    public void StartButtonClick()
    {
        level.LoadNextLevel("2_ServerConnect_Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void JoinButtonClick()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            level.LoadNextLevel("PotionPuzzleScene");
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(SceneManager.GetActiveScene().name == "4_RoomLobby" && !PhotonNetwork.IsMasterClient && PhotonPacket.POTION_SYMBOL.WasChanged(propertiesThatChanged))
            PhotonNetwork.LoadLevel("Library");
    }
}
