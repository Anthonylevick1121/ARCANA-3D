using UnityEngine;

public class TorchLighter : MonoBehaviour
{
    [SerializeField] private new GameObject light;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
            light.SetActive(false);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Enemy"))
            light.SetActive(true);
    }
}
