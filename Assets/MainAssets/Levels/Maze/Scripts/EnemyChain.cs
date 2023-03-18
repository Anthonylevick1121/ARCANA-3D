using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChain : MonoBehaviour
{
    public NavMeshAgent ahead;
    public float followDistance;
    
    private NavMeshAgent navAgent;
    
    // Start is called before the first frame update
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
