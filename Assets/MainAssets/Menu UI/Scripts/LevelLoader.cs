using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    
    public float transitionTime = 1f;
    
    public void LoadNextLevel(int index)
    {
        StartCoroutine(LoadLevel(index));
    }
    
    IEnumerator LoadLevel(int index)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(index);
    }
}
