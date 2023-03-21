using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Serialization;

public class ConnectToSever : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [FormerlySerializedAs("level")] public TransitionSceneLoader sceneLoader;
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    
    public override void OnJoinedLobby()
    {
        sceneLoader.LoadSceneWithFade("3_RoomSelect"); // CreateAndJoin
    }
}
