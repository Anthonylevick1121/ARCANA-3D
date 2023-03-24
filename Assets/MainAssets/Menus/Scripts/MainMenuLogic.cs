using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLogic : BaseMenuLogic
{
    public void PlayGame() => SceneManager.LoadScene(PhotonNetwork.IsConnected ? "3_RoomSelect" : "2_ServerConnect");
    
    public void Credits() => SceneManager.LoadScene("CreditsPage");
    
    public void QuitGame() => Application.Quit();
}
