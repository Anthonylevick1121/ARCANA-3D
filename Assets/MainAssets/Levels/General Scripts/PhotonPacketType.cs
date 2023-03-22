using Photon.Pun;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        get => (T) PhotonNetwork.CurrentRoom?.CustomProperties[key];
        set
        {
            // Debug.Log($"setting packet type {key} named {}");
            PhotonNetwork.CurrentRoom?.SetCustomProperties(new Hashtable { { key, value } });
        }
    }
    
    public Hashtable Mock(T value) => new() { { key, value } };
    
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
    public static readonly PhotonPacketType<bool> START = new ("start");
    
    public static readonly PhotonPacketType<int> POTION_SYMBOL = new ("potion symbol");
    public static readonly PhotonPacketType<bool> POTION_WIN = new ("potion win");
    
    // separated because fricc photon
    // only one needed now because only one flip
    public static readonly PhotonPacketType<int> MAZE_LEVER = new ("maze lever");
    // public static readonly PhotonPacketType<bool> MAZE_LEVER_FLIP = new ("maze flip");
    // public static readonly PhotonPacketType<bool> MAZE_LEVER_ACTION = new ("maze flip action");
    
    // player entered this section of maze
    public static readonly PhotonPacketType<int> MAZE_PLAYER = new ("maze player");
    
    public static readonly PhotonPacketType<int> MAZE_ENEMY = new ("maze enemy");
    public static readonly PhotonPacketType<bool> MAZE_WIN = new ("maze win");
    
    public static Hashtable WithPacket<T>(this Hashtable table, PhotonPacketType<T> packet, T value)
    {
        table.Add(packet.key, value);
        return table;
    }
}