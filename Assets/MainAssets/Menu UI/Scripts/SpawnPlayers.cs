using Photon.Pun;
using TMPro;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    
    public float spawnX;
    public float spawnY;
    public float spawnZ;

//removed this field since it wasn't needed

    private void Start()
    {
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);
        GameObject localPlayer = (GameObject)PhotonNetwork.Instantiate(playerPrefab.name,
            spawnPosition, Quaternion.identity);
    
        localPlayer.transform.Find("Main Camera").gameObject.SetActive(true);
    }
}
