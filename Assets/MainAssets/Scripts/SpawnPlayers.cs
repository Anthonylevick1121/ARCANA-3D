using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    
    //TODO: change coords to explorer/librarian rooms respectively
    public float spawnX;
    public float spawnY;

    private void Start()
    {
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }
}
