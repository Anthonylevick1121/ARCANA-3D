using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenericLevelLoader : MonoBehaviour
{
    public static GenericLevelLoader instance;
    
    private void Awake()
    {
        if (instance != null)
            return;
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        load = false;
    }
    
    // accessed by level loader in the loading screen scene
    private string nextScene;
    
    // false for the static loader, true for loading screen one
    private bool load = true;
    private bool updated = false;
    
    private void Update()
    {
        if (load)
        {
            if (!updated)
            {
                updated = true;
                return;
            }
            load = false;
            PhotonNetwork.SendAllOutgoingCommands();
            PhotonNetwork.LoadLevel(instance.nextScene);
        }
    }
    
    public void UseLoadingScreen(string sceneToLoad)
    {
        nextScene = sceneToLoad;
        SceneManager.LoadScene("5_LevelLoad");
    }
}
