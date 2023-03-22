using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    
    [SerializeField] private PlayerCore player;
    [SerializeField] private Transform playerRespawnPos;
    [SerializeField] private Transform enemyRespawnPos;
    
    [SerializeField] private float baseSpeed;
    [SerializeField] private float speedGainPerLever;
    
    [SerializeField] private float torchEffectDistMultiplier; // 6.5
    [SerializeField] private float minTorchEffectSpeedFactor; // 6
    
    public bool debugNavigation;
    public Transform debugTarget;
    
    private MazeSectionPos mazeSection = MazeSectionPos.Tutorial; // not the case, but not tracked, and it'll cause an update
    
    // just going to refresh every frame for now, doesn't seem to be that bad lol
    
    // we don't really want to be setting the destination every frame
    // that would be really costly on a large map
    // instead, refresh it more often depending on how close we are to the player.
    // private readonly float distanceUpdateRatio = 0.9f;
    // private readonly float maxRefreshInterval = 5f; // seconds
    // private float nextRefreshInterval;
    // private float timeSinceRefresh;
    // private bool valid, firstValid;
    
    // private float distCache;
    
    // private readonly float gameOverDistance = 5f;
    
    // Start is called before the first frame update
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // firstValid = false;
        // RefreshTarget();
        navAgent.speed = baseSpeed;
    }
    
    public void OnLeverPulled() => navAgent.speed += speedGainPerLever;
    
    private void RefreshTarget()
    {
        navAgent.SetDestination(player.transform.position);
        // nextRefreshDistance = navAgent.remainingDistance * distanceUpdateRatio;
        // timeSinceRefresh = 0;
        // nextRefreshInterval = -1;
        // valid = false;
    }
    
    private void Update()
    {
        // calc current maze section
        MazeSectionPos section = MazePuzzle.instance.GetMazeSection(transform.position);
        if (section != mazeSection)
        {
            mazeSection = section;
            PhotonPacket.MAZE_ENEMY.Value = (int) section;
            print("Enemy in section "+Enum.GetName(typeof(MazeSectionPos), section));
        }
        
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
        navAgent.SetDestination(debugNavigation && debugTarget ? debugTarget.position : player.transform.position);
        
        if (!debugNavigation) return;
        NavMeshPath path = new ();
        navAgent.CalculatePath(debugTarget ? debugTarget.position : player.transform.position, path);
        Vector3 last = Vector3.zero;
        foreach (Vector3 pos in path.corners)
        {
            if (last == Vector3.zero)
            {
                last = pos;
                continue;
            }
            Debug.DrawLine(last, pos, Color.red);
            last = pos;
        }
    }
    
    public void RespawnPlayer()
    {
        player.ui.status.SetStatus("You are caught!\naaahhgghhhh...");
        player.movement.SetPosition(playerRespawnPos.position);
        navAgent.Warp(enemyRespawnPos.position);
    }
    
    public bool CheckTorchEffectReachable(Vector3 pos)
    {
        // use the nav mesh to our advantage
        NavMeshPath path = new ();
        navAgent.CalculatePath(pos, path);
        if (path.status != NavMeshPathStatus.PathComplete)
            return false; // only reachable tiles should be affected
        float distance = 0;
        for (int i = 1; i < path.corners.Length; i++)
            distance += Vector3.Distance(path.corners[i], path.corners[i - 1]);
        
        return distance <= torchEffectDistMultiplier * Mathf.Max(minTorchEffectSpeedFactor, navAgent.speed);
    }
}
