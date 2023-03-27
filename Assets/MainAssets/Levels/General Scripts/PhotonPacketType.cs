using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;

public sealed class PhotonPacketType<T>
{
    public readonly string key;
    
    // for SOME GOD-FORSAKEN REASON, this does not increment atomically when packet type instances are created.
    // so I'm passing the number manually.
    // private static int counter = 0;
    
    public PhotonPacketType(string name)
    {
        // this.key = id.ToString();
        this.key = name;
        // Debug.Log($"created new packet type with key {key}");
    }
    
    public T Value
    {
        get => (T) (PhotonNetwork.CurrentRoom?.CustomProperties[key] ?? default(T));
        set => PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable { { key, value } });
    }
    
    public Hashtable Mock(T value) => new() { { key, value } };
    
    public bool WasChanged(Hashtable changedProps) => changedProps.ContainsKey(key);
    
    public T Get(Hashtable props) => (T) props[key];
    
    public T GetOr(Hashtable props, T defaultValue) => (T) props.GetValueOrDefault(key, defaultValue);
}

public static class PhotonPacket
{
    // this isn't used for every voice line, just ones that both need to hear, but is triggered by the other.
    public static readonly PhotonPacketType<int> VOICE = new ("voice");
    
    public static readonly PhotonPacketType<bool> PAUSE = new ("pause");
    
    public static readonly PhotonPacketType<int> POTION_SYMBOL = new ("potion symbol");
    public static readonly PhotonPacketType<bool> POTION_WIN = new ("potion win");
    
    // denotes when the other has entered their area, used to sync the voice lines / intro
    public static readonly PhotonPacketType<bool> MAZE_PLAYER_ENTER = new ("maze enter player");
    public static readonly PhotonPacketType<bool> MAZE_LIB_ENTER = new ("maze enter lib");
    
    // only one needed now because each will only flip once
    public static readonly PhotonPacketType<int> MAZE_LEVER = new ("maze lever");
    // librarian lever; down or not?
    public static readonly PhotonPacketType<bool> MAZE_LIB_LEVER = new ("maze lib lever");
    
    public static readonly PhotonPacketType<int> MAZE_PLAYER = new ("maze player");
    public static readonly PhotonPacketType<int> MAZE_ENEMY = new ("maze enemy");
    
    // final win/loss conditions
    // public static readonly PhotonPacketType<bool> GAME_END = new ("game end");
    // bool is important, denotes win or loss
    public static readonly PhotonPacketType<bool> GAME_WIN = new ("game win");
    
    public static Hashtable WithPacket<T>(this Hashtable table, PhotonPacketType<T> packet, T value)
    {
        table.Add(packet.key, value);
        return table;
    }
}