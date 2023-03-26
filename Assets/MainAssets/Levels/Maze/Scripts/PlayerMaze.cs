using System;
using UnityEngine;

[RequireComponent(typeof(PlayerCore))]
public class PlayerMaze : MonoBehaviour
{
    private PlayerCore player;
    private int mazeSection = -1;
    
    // Start is called before the first frame update
    private void Start()
    {
        player = GetComponent<PlayerCore>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        int section = (int) MazePuzzle.GetMazeSection(transform.position);
        if (section != mazeSection)
        {
            print("player entered section " + Enum.GetName(typeof(MazeSectionPos), section));
            PhotonPacket.MAZE_PLAYER.Value = section;
            mazeSection = section;
        }
    }
}
