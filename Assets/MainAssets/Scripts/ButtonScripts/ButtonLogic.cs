using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ButtonLogic : MonoBehaviour
{
    public LevelLoader level;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void StartButtonClick()
    {
        level.LoadNextLevel(1);
    }

    public void LeaveButtonClick()
    {
        Debug.Log("Check");
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
