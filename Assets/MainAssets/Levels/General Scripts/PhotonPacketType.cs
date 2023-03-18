using Photon.Pun;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public sealed class PhotonPacketType<T>
{
    public readonly string key;
    
    private static int counter = 0;
    
    public PhotonPacketType()
    {
        this.key = (counter++).ToString();
    }
    
    public T Value
    {
        get => (T) PhotonNetwork.CurrentRoom?.CustomProperties[key];
        set => PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable {{ key, value }});
    }
    
    public bool WasChanged(Hashtable changedProps) => changedProps.ContainsKey(key);
    
    public T Get(Hashtable props) => (T) props[key];
    
    public T GetOr(Hashtable props, T defaultValue) => (T) props.GetValueOrDefault(key, defaultValue);
}

public static class PhotonPacket
{
    public static readonly PhotonPacketType<int> POTION_SYMBOL = new ();
    public static readonly PhotonPacketType<bool> POTION_WIN = new ();
    public static readonly PhotonPacketType<MazeSectionPos> MAZE_LEVER = new ();
}