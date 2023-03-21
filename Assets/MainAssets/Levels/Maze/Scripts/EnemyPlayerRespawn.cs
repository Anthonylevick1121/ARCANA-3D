using UnityEngine;

public class EnemyPlayerRespawn : MonoBehaviour
{
    private EnemyController controller;
    
    private void Start()
    {
        controller = GetComponentInParent<EnemyController>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            controller.RespawnPlayer();
    }
}
