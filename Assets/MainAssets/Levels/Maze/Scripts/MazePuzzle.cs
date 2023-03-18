using System;
using UnityEngine;

public class MazePuzzle : MonoBehaviour
{
    public static MazePuzzle instance;
    private void Awake() => instance = this;
    
    [SerializeField] private PlayerCore player;
    
    // ORDERED list of the corridor parent objects, in order of the MazeSectionPos enum
    [SerializeField] private GameObject[] corridorColors;
    
    /*
     * get a lever
     * every lever toggles its associated set of walls AND a librarian action for their map
     */
    
    // Start is called before the first frame update
    private void Start()
    {
        
    }
    
    private void Update()
    {
        int color = -1;
        // debug press-any-lever
        for (KeyCode i = KeyCode.Alpha1; i <= KeyCode.Alpha9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                color = i - KeyCode.Alpha1;
                break;
            }
        }
        for (KeyCode i = KeyCode.Keypad1; i <= KeyCode.Keypad9; i++)
        {
            if (Input.GetKeyDown(i))
            {
                color = i - KeyCode.Keypad1;
                break;
            }
        }
        
        if (color >= 0)
        {
            corridorColors[color].transform.GetChild(0).GetComponent<MazeSectionLever>()
                .BaseInteract(player, null);
        }
    }
}
