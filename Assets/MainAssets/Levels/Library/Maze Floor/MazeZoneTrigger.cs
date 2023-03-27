using UnityEngine;

public class MazeZoneTrigger : MonoBehaviour
{
    private bool entered;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!entered && other.CompareTag("Player"))
        {
            entered = true;
            PhotonPacket.MAZE_LIB_ENTER.Value = true;
        }
    }
}
