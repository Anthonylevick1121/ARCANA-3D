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
    // public bool TryGet(Hashtable props, out object value) => props.TryGetValue(key, out value);
    /*public void IfPacket(Hashtable props, System.Action<T> action)
    {
        T val = default;
        if(props.TryGetValue())
    }*/
}

public static class PhotonPacket
{
    public static readonly PhotonPacketType<bool> START = new ();
    
    public static readonly PhotonPacketType<int> POTION_SYMBOL = new ();
    public static readonly PhotonPacketType<bool> POTION_WIN = new ();
    
    public static readonly PhotonPacketType<(MazeSectionPos, bool)> MAZE_LEVER = new ();
    public static readonly PhotonPacketType<MazeSectionPos> MAZE_ENEMY = new ();
    public static readonly PhotonPacketType<bool> MAZE_WIN = new ();
}