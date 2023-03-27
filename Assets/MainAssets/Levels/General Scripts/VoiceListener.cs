using ExitGames.Client.Photon;
using Photon.Pun;

public class VoiceListener : MonoBehaviourPunCallbacks
{
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.VOICE.WasChanged(deltaProps))
        {
            VoiceLineId id = (VoiceLineId) PhotonPacket.VOICE.Get(deltaProps);
            VoicePlayer.instance.PlayVoiceLine(id);
        }
    }
}
