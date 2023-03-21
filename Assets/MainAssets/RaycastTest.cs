using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    [SerializeField] private bool doRaycast = false;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private Transform raycastTarget;
    
    private void Update()
    {
        if (doRaycast)
        {
            // doRaycast = false;
            DoRaycast();
        }
    }
    
    private void DoRaycast()
    {
        RaycastHit hit;
        Vector3 delta = raycastTarget.position - transform.position;
        if (Physics.Raycast(transform.position, delta, out hit, delta.magnitude, raycastMask))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);
            print("hit: "+hit.transform.name);
        }
        else
        {
            Debug.DrawLine(transform.position, raycastTarget.position, Color.red);
        }
    }
}