using UnityEngine;
using UnityEngine.AI;

public class TorchLighter : MonoBehaviour
{
    [SerializeField] private new Light light;

    private Vector3 tilePos;
    
    private static readonly float lightCheckInterval = 0.5f;
    private float curStayTime;
    
    private void Start()
    {
        Transform transform = this.transform.parent;
        Vector3 pos = transform.localPosition;
        pos.y = 1; // right above ground
        // move to center of tile
        switch (transform.rotation.eulerAngles.y)
        {
            case 0: pos.z += 4;
                break;
            case 90: pos.x += 4;
                break;
            case 180: pos.z -= 4;
                break;
            case 270: pos.x -= 4;
                break;
            default:
                Debug.LogWarning("Torch found with non-quad rotation: "+gameObject.name+" at "+transform.position);
                break;
        }
        
        tilePos = pos;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyTorch")) return;
        
        if (MazePuzzle.instance.enemy.CheckTorchEffectReachable(tilePos))
            light.enabled = false;
        else
            curStayTime = 0;
    }
    
    private void OnTriggerStay(Collider other)
    {
        // no stay behavior for doused lights.
        if (!other.CompareTag("EnemyTorch") || !light.enabled) return;
        
        // every half second, try again
        curStayTime += Time.fixedDeltaTime;
        if (curStayTime >= lightCheckInterval)
        {
            if(MazePuzzle.instance.enemy.CheckTorchEffectReachable(tilePos))
                light.enabled = false;
            else
                curStayTime -= lightCheckInterval;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnemyTorch"))
            light.enabled = true;
    }
    
    private bool CheckReachableRaycast(Vector3 startPos)
    {
        // calc delta on x and z axis
        // algo starts at the target pos and tries to get to tile pos
        // float deltaX = tilePos.x - startPos.x;
        // float deltaZ = tilePos.z - startPos.z;
        Vector3 deltaX = new (tilePos.x - startPos.x, 0, 0);
        Vector3 deltaZ = new (0, 0, tilePos.z - startPos.z);
        
        float xDist = Mathf.Abs(deltaX.x);
        float zDist = Mathf.Abs(deltaZ.z);
        
        // get the middle position between the two raycasts that will happen; 2 sets of raycasts, 2 middle positions
        Vector3 xFirstMid = startPos + deltaX;
        Vector3 zFirstMid = startPos + deltaZ;
        
        LayerMask mask = LayerMask.NameToLayer("Walls");
        
        // try x first
        if (Physics.Raycast(startPos, deltaX, xDist, mask) && Physics.Raycast(xFirstMid, deltaZ, zDist, mask))
        {
            Debug.DrawLine(startPos, xFirstMid, Color.blue, 2f);
            Debug.DrawLine(xFirstMid, xFirstMid+deltaZ, Color.cyan, 2f);
            return true;
        }
        // try z first now
        if (Physics.Raycast(startPos, deltaZ, zDist, mask) && Physics.Raycast(zFirstMid, deltaX, xDist, mask))
        {
            Debug.DrawLine(startPos, zFirstMid, Color.blue, 2f);
            Debug.DrawLine(zFirstMid, zFirstMid+deltaX, Color.cyan, 2f);
            return true;
        }
        
        return false;
    }
}
