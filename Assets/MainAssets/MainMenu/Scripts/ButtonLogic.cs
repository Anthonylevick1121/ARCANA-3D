using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonLogic : MonoBehaviourPunCallbacks
{
    public TransitionSceneLoader sceneLoader;
    
    private Image image; // for room lobby start game text
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "4_RoomLobby")
        {
            image = GetComponent<Image>();
            image.color = Color.gray;
        }
    }
    
    public void StartButtonClick()
    {
        sceneLoader.LoadSceneWithFade("2_ServerConnect_Loading");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void JoinButtonClick()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonPacket.START.Value = true;
            // sceneLoader.LoadSceneWithFade("Maze");
            GenericLevelLoader.instance.UseLoadingScreen("Maze");
        }
    }
    
    private void Update()
    {
        if (image && PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
            image.color = Color.white;
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // if(SceneManager.GetActiveScene().name == "4_RoomLobby" && !PhotonNetwork.IsMasterClient && PhotonPacket.POTION_SYMBOL.WasChanged(propertiesThatChanged))
        if(SceneManager.GetActiveScene().name == "4_RoomLobby" && !PhotonNetwork.IsMasterClient && PhotonPacket.START.WasChanged(propertiesThatChanged))
            GenericLevelLoader.instance.UseLoadingScreen("Library");
    }
}
