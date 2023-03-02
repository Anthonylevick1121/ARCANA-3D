using UnityEngine;

public class PickUps : Interactable
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    
    // Update is called once per frame
    private void Update()
    {
        
    }
    
    protected override void Interact()
    {
        Vector3 mousePosition = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }
}