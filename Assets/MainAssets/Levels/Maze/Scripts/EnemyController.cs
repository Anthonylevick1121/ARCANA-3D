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
    
    private static readonly string[] RESPAWN_NOTIFS =
    {
        "You were caught! Seems you're safe... this time...",
        "Caught again... Something feels off.\nYou should avoid another encounter..."
    };
    private int respawnCount = 0;
    // just to ensure there are no "double caught" bugs
    private bool caught = false;
    
    private bool playCloseVoice;
    
    private MazeSectionPos mazeSection = MazeSectionPos.Tutorial; // not the case, but not tracked, and it'll cause an update
    
    // Start is called before the first frame update
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = baseSpeed;
        player.ui.pauseMenu.pauseToggled.AddListener(paused => navAgent.isStopped = paused);
        playCloseVoice = true;
    }
    
    public void OnLeverPulled() => navAgent.speed += speedGainPerLever;
    
    private void Update()
    {
        if(player.debug && Input.GetKeyDown(KeyCode.K))
            RespawnPlayer();
        
        // calc current maze section
        MazeSectionPos section = MazePuzzle.GetMazeSection(transform.position);
        if (section != mazeSection)
        {
            mazeSection = section;
            PhotonPacket.MAZE_ENEMY.Value = (int) section;
            print("Enemy in section "+Enum.GetName(typeof(MazeSectionPos), section));
        }
        
        Vector3 dest = debugNavigation && debugTarget ? debugTarget.position : player.transform.position;
        // round to tile
        dest.x = Mathf.Round(dest.x / 8) * 8;
        dest.y = 2;
        dest.z = Mathf.Round(dest.z / 8) * 8;
        
        (NavMeshPath path, float dist) = GetDistance(dest, false);
        navAgent.SetPath(path);
        
        // check dist for voice line
        if (playCloseVoice && dist is >= 0 and < 75)
        {
            playCloseVoice = false;
            VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeEnemyCloseP);
        }
        
        if (!debugNavigation) return;
        // NavMeshPath path = new ();
        // navAgent.CalculatePath(debugTarget ? debugTarget.position : player.transform.position, path);
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
        if (caught) return;
        caught = true;
        
        VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeFailure1P + respawnCount);
        
        if (respawnCount < RESPAWN_NOTIFS.Length)
        {
            ScreenFade.instance.FadeScreen(CanvasLayer.MoveTransition, () =>
            {
                player.movement.SetPosition(playerRespawnPos.position);
                navAgent.Warp(enemyRespawnPos.position);
                player.ui.status.SetStatus(RESPAWN_NOTIFS[respawnCount++]);
                caught = false;
            });
        }
        else
        {
            // lose condition
            PhotonPacket.GAME_WIN.Value = false;
            PhotonPacket.VOICE.Value = (int) VoiceLineId.MazeFailure3A;
            // PhotonPacket.GAME_END.Value = true;
            ScreenFade.instance.LoadSceneWithFade("EndScene", false, CanvasLayer.EndTransition);
        }
    }
    
    private (NavMeshPath, float) GetDistance(Vector3 pos, bool acceptPartial)
    {
        // use the nav mesh to our advantage
        NavMeshPath path = new ();
        navAgent.CalculatePath(pos, path);
        if (path.status == NavMeshPathStatus.PathInvalid || !acceptPartial && path.status == NavMeshPathStatus.PathPartial)
            return (path, -1); // only reachable tiles should be affected
        float distance = 0;
        for (int i = 1; i < path.corners.Length; i++)
            distance += Vector3.Distance(path.corners[i], path.corners[i - 1]);
        
        // add remaining dist
        distance += Vector3.Distance(path.corners[^1], pos);
        
        return (path, distance);
    }
    
    public bool CheckTorchEffectReachable(Vector3 pos)
    {
        (_, float dist) = GetDistance(pos, true);
        return dist >= 0 && dist <= torchEffectDistMultiplier * Mathf.Max(minTorchEffectSpeedFactor, navAgent.speed);
    }
}
