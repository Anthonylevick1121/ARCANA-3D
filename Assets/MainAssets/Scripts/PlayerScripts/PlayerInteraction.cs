using UnityEngine;

[RequireComponent(typeof(PlayerCore))]
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private float distance = 3f;
    // We'll add this back in a bit if needed
    [SerializeField]
    private LayerMask mask;
    
    private PlayerUI ui;
    private new Camera camera;

    private Interactable currentInteractable;
    
    // Start is called before the first frame update
    private void Start()
    {
        camera = Camera.main;
        ui = GetComponent<PlayerUI>();
        PlayerCore player = GetComponent<PlayerCore>();
        player.InputActions.Interact.performed += ctx => TryInteract();
        
        // default behavior should be to cast against everything except ignore raycast (hence the ~)
        // mask = ~LayerMask.GetMask("Ignore Raycast");
    }

    // Update is called once per frame
    private void Update() {
        ui.UpdateText(string.Empty);
        currentInteractable = null;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, distance)) return;
        
        Interactable interactable = hitInfo.collider.GetComponentInParent<Interactable>();
        if (!interactable) return;
        
        currentInteractable = interactable;
        ui.UpdateText(interactable.GetPrompt());
    }
    
    private void TryInteract()
    {
        Debug.Log("Interact with "+currentInteractable);
        if (!currentInteractable) return;
        currentInteractable.BaseInteract();
    }
}
