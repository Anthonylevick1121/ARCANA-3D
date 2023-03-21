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
    
    private void Update()
    {
        if (load)
        {
            load = false;
            PhotonNetwork.LoadLevel(instance.nextScene);
        }
    }
    
    public void UseLoadingScreen(string sceneToLoad)
    {
        nextScene = sceneToLoad;
        SceneManager.LoadScene("5_LevelLoad");
    }
}
