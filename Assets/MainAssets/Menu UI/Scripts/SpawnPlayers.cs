using Photon.Pun;
using TMPro;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    
    //TODO: change coords to explorer/librarian rooms respectively
    public float spawnX;
    public float spawnY;

//removed this field since it wasn't needed

    private void Start()
    {
        Vector2 spawnPosition = new Vector2(spawnX, spawnY);
        GameObject localPlayer = (GameObject)PhotonNetwork.Instantiate(playerPrefab.name,
            spawnPosition, Quaternion.identity);
    
        localPlayer.transform.Find("Main Camera").gameObject.SetActive(true);
    }
}
