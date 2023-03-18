using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private PlayerCore player;
    
    // todo the lighting... I think the right call for making the lighting work is going to be grid-ifying the maze
    // todo  and using x depth neighbor checks every time we cross a tile boundary. Honestly not that expensive just
    // todo  takes a bit of processing upfront.
    
    // just going to refresh every frame for now, doesn't seem to be that bad lol
    
    // we don't really want to be setting the destination every frame
    // that would be really costly on a large map
    // instead, refresh it more often depending on how close we are to the player.
    private readonly float distanceUpdateRatio = 0.9f;
    private readonly float maxRefreshInterval = 5f; // seconds
    private float nextRefreshInterval;
    private float timeSinceRefresh;
    // private bool valid, firstValid;
    
    private float distCache;
    
    // private readonly float gameOverDistance = 5f;
    
    // Start is called before the first frame update
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // firstValid = false;
        // RefreshTarget();
    }
    
    private void RefreshTarget()
    {
        navAgent.SetDestination(player.transform.position);
        // nextRefreshDistance = navAgent.remainingDistance * distanceUpdateRatio;
        timeSinceRefresh = 0;
        nextRefreshInterval = -1;
        // valid = false;
    }
    
    private void Update()
    {
        /*timeSinceRefresh += Time.deltaTime;
        
        if (navAgent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            // valid = true;
            return;
        }
        
        if (nextRefreshInterval < 0)
        {
            // calc
            distCache = navAgent.remainingDistance;
            
            // if (!firstValid && distCache > 0)
                // firstValid = true;
            
            if (navAgent.speed == 0)
                nextRefreshInterval = maxRefreshInterval;
            else
                nextRefreshInterval = distCache / distanceUpdateRatio / navAgent.speed;
            
            nextRefreshInterval = Mathf.Clamp(nextRefreshInterval, 0, maxRefreshInterval);
            
            print("dist remain: "+distCache);
            print("time till next check: "+nextRefreshInterval);
        }
        
        debugText.text = "Distance: " + navAgent.remainingDistance + "\ntime until refresh: "+(nextRefreshInterval - timeSinceRefresh);
        
        if(timeSinceRefresh >= nextRefreshInterval)
            RefreshTarget();*/
        navAgent.SetDestination(player.transform.position);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            player.ui.status.SetStatus("You are caught and ded!\naaahhgghhhh...");
    }
}
