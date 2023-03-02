using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Interactable : MonoBehaviour
{
    [SerializeField]
    private string promptMessage;
    [SerializeField]
    private UnityEvent onInteract;
    
    public void BaseInteract()
    {
        Interact();
    }
    protected virtual void Interact()
    {
        if(onInteract != null)
            onInteract.Invoke();
    }

    public string GetPrompt() => promptMessage;
}
