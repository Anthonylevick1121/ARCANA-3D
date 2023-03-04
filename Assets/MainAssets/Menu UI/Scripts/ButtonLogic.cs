using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ButtonLogic : MonoBehaviour
{
    public LevelLoader level;
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    
    public void StartButtonClick()
    {
        level.LoadNextLevel(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void JoinButtonClick()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }
}
