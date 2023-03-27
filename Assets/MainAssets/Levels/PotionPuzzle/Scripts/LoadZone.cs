using System;
using UnityEngine;

public class LoadZone : MonoBehaviour
{
    private bool loaded;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!loaded && other.CompareTag("Player"))
        {
            loaded = true;
            // other.gameObject.GetComponent<PlayerCore>().InputActions.Disable();
            ScreenFade.instance.LoadSceneWithFade("Maze", true);
        }
    }
}
