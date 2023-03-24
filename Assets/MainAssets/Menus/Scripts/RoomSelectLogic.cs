using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomSelectLogic : BaseMenuLogic
{
    public TMP_InputField join;
    public TMP_InputField create;
    public void CreateRoom()
    {
        if(create.text.Length > 0) PhotonNetwork.CreateRoom(create.text, new RoomOptions { MaxPlayers = 2 });
    }
    
    public void JoinRoom()
    {
        if(join.text.Length > 0) PhotonNetwork.JoinRoom(join.text);
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("4_RoomLobby");
        // ScreenFade.instance.LoadSceneWithFade("4_RoomLobby", Color.white);
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
