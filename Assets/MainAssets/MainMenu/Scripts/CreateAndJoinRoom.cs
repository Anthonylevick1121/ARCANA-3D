using Photon.Pun;
using TMPro;

public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
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
        PhotonNetwork.LoadLevel("4_RoomLobby");
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
