using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update

   
    
    public TMP_InputField join;
    public TMP_InputField create;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(create.text);

    }
    

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(join.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        create.text = "Room already exists, try again!";
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        join.text = "Room doesn't exist, create it instead!";
    }
}
