using Photon.Pun;
using UnityEngine;

public class DisconnectPhoton : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start() => PhotonNetwork.LeaveRoom();
}
