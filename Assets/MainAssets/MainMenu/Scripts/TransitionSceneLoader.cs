using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneLoader : MonoBehaviour
{
    public Animator transition;
    
    public float transitionTime = 1f;
    
    public void LoadSceneWithFade(string name)
    {
        StartCoroutine(LoadScene(name));
    }
    
    IEnumerator LoadScene(string name)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(name);
    }
}
