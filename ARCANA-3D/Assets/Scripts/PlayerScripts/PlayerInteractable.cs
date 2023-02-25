using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractable : MonoBehaviour
{

    private Camera cam;
    
    [SerializeField]
    private float distance = 3f;

    [SerializeField] private LayerMask mask;

    private PlayerUI playerUI;

    private InputManager input;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        input = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update(){
        playerUI.UpdateText(string.Empty);
    Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>())
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (input.players.Interact.triggered)
                {
                    Debug.Log("check");
                    interactable.BaseInteract();
                }
            }

        }
    }
}
