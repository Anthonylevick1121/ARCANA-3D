using UnityEngine;

[RequireComponent(typeof(PlayerCore))]
public class PlayerInteraction : MonoBehaviour
{
    private PlayerCore player;
    private HoldableItem heldItem;
    
    // this is where held items will be held, at this position
    [SerializeField] private Transform heldItemAnchor;
    
    [SerializeField] private float distance = 3f;
    [SerializeField] private LayerMask interactMask;
    
    [SerializeField] private AudioClip pickupAudio, dropAudio;
    
    private Interactable hoveredInteractable;
    
    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<PlayerCore>();
        player.InputActions.Interact.performed += ctx => TryInteract();
        player.InputActions.DropItem.performed += ctx => DropItem();
        
        // default behavior should be to cast against everything except ignore raycast (hence the ~)
        // mask = ~LayerMask.GetMask("Ignore Raycast");
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (!player.InputActions.enabled)
            return; // disable interaction updating while input is disabled
        
        player.ui.promptText.text = string.Empty;
        hoveredInteractable = null;
        Ray ray = new Ray(player.view.camera.transform.position, player.view.camera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, distance, interactMask)) return;
        
        Interactable interactable = hitInfo.collider.GetComponentInParent<Interactable>();
        if (!interactable) return;
        
        hoveredInteractable = interactable;
        if (hoveredInteractable)
        {
            if (interactable.GetPrompt(heldItem) != null)
            {
                player.ui.promptText.text = interactable.GetPrompt(heldItem);
            }
            
        }
       
    }
    
    private void TryInteract()
    {
        Debug.Log("Interact with "+(hoveredInteractable==null?"null":hoveredInteractable.name));
        if (!hoveredInteractable) return;
        hoveredInteractable.BaseInteract(player, heldItem);
    }
    
    public void DropItem()
    {
        if (!heldItem) return;
        // remove the constraints on the held item
        heldItem.SetIsHeld(false);
        heldItem.transform.SetParent(null);
        Debug.Log("dropped "+heldItem.name);
        heldItem.gameObject.layer = LayerMask.NameToLayer("Interactable");
        heldItem = null;
    }
    
    public void HoldItem(HoldableItem item)
    {
        DropItem(); // drop currently-held item if any
        // disable gravity physics
        item.SetIsHeld(true);
        // parent the item
        item.transform.SetParent(heldItemAnchor);
        // align position
        item.transform.localPosition = Vector3.zero;
        // disallow further interaction
        item.gameObject.layer = LayerMask.NameToLayer("Default");
        // save
        heldItem = item;
        Debug.Log("Picked up "+item.name);
    }
}
