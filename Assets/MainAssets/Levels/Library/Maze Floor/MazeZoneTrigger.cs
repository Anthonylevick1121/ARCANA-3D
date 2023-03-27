using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class MazeZoneTrigger : MonoBehaviourPunCallbacks
{
    private bool entered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!entered && other.CompareTag("Player"))
        {
            entered = true;
            PhotonPacket.MAZE_LIB_ENTER.Value = true;
            if(PhotonPacket.MAZE_PLAYER_ENTER.Value)
                VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeIntroL);
            else
                VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeFirstL);
        }
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if(entered && PhotonPacket.MAZE_PLAYER_ENTER.WasChanged(deltaProps))
            VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeIntroL);
    }
}
