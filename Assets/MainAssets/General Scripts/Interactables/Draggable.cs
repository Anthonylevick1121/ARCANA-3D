using UnityEngine;
using UnityEngine.UIElements;

public class Draggable : MonoBehaviour
{
    // Start is called before the first frame update
     private Vector3 mousePosition;
     private Camera mainCamera;
    
     private void Start()
     {
         mainCamera = Camera.main;
         
     }
    
     private Vector3 GetMousePos()
     {
         return mainCamera.WorldToScreenPoint(transform.position);
     }
    
     // collider event function
     private void OnMouseDown()
     {
         mousePosition = Input.mousePosition - GetMousePos();
     }
    
     // collider event function
     private void OnMouseDrag()
     {
         transform.position = mainCamera.ScreenToWorldPoint(Input.mousePosition - mousePosition);
         
     }
}
