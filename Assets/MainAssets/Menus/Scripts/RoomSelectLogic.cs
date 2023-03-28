using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelectLogic : BaseMenuLogic
{
    [SerializeField] private TMP_InputField join;
    [SerializeField] private TMP_InputField create;
    [SerializeField] private Button joinBtn;
    [SerializeField] private Button createBtn;
    
    public void CreateRoom()
    {
        if (create.text.Length == 0) return;
        PhotonNetwork.CreateRoom(create.text, new RoomOptions { MaxPlayers = 2 });
        createBtn.interactable = false;
    }
    
    public void JoinRoom()
    {
        if (join.text.Length == 0) return;
        PhotonNetwork.JoinRoom(join.text);
        joinBtn.interactable = false;
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("4_RoomLobby");
        // ScreenFade.instance.LoadSceneWithFade("4_RoomLobby", Color.white);
    }
    
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        create.text = "Room already exists, try again!";
        createBtn.interactable = true;
    }
    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        join.text = "Room doesn't exist, create it instead!";
        joinBtn.interactable = true;
    }
}
