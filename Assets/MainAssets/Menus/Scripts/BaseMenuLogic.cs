using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseMenuLogic : MonoBehaviourPunCallbacks
{
    public void ChangeScene(string scene) => SceneManager.LoadScene(scene);
}
