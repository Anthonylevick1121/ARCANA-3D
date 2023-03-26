using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorParent : MonoBehaviour
{
    // the lever present in each section of the maze
    private MazeSectionLever lever;
    // the symbol next to the lever
    private Renderer leverSymbol;
    // we have reference to the moving walls in this section:
    // the walls that are up when the librarian lever is up
    private Transform wallsMatchingLever;
    // the walls that are down when the librarian lever is up
    private Transform wallsDiffLever;
    
    // there are also green floor sections of the maze which could be here,
    // but so far we shouldn't need them.
    
    // wall movement animation
    // y position of the walls when fully up
    private static readonly float wallUpY = 0f;
    // y pos when fully down (will also be disabled)
    private static readonly float wallDownY = -8.2f;
    // time in seconds to animate from up to down or vice versa
    private static readonly float wallTransitionTime = 1f;
    
    // how far along are we in the wall transition? 0-1.
    private float wallTransitionAlpha;
    // which direction are we going / have we arrived in? matches librarian lever where true means up, false means down
    private bool wallLeverUp;
    // a sorta insurance bool to avoid issues with floating point calcs not denoting state correctly
    private bool inRestingState;
    
    // has area lever been flipped?
    private bool areaLeverDown;
    
    // Start is called before the first frame update
    private void Start()
    {
        Transform t = transform;
        lever = t.GetChild(0).GetComponent<MazeSectionLever>();
        leverSymbol = t.GetChild(1).GetComponentInChildren<Renderer>();
        wallsMatchingLever = t.GetChild(2);
        wallsDiffLever = t.GetChild(3);
        
        // set symbol material
        leverSymbol.material = MazePuzzle.instance.dormantSymbolMat;
        
        // set up initial states for the walls
        SetWallPosition(wallUpY);
        // we are not animating / are in a finished state
        wallTransitionAlpha = 1;
        // starting state is with the lever up
        wallLeverUp = true;
        inRestingState = true;
        wallsMatchingLever.gameObject.SetActive(wallLeverUp);
        wallsDiffLever.gameObject.SetActive(!wallLeverUp);
        // all areas start up
        areaLeverDown = false;
    }
    
    public void SetWallState(bool flipped)
    {
        // all states are ignored if area lever has been pulled.
        if (areaLeverDown) return;
        
        // flipped refers to the position of the librarian lever.
        // however, in this class, it's easier to use true = up, false = down, so we switch
        bool leverUp = !flipped;
        
        // technically, we shouldn't need to check if we're already in the state being asked for, but we probably should
        // if we're already at / going toward the target state, ignore the call
        if (wallLeverUp == leverUp)
            return;
        
        // toggle direction
        wallLeverUp = leverUp;
        // invert the progress
        wallTransitionAlpha = Mathf.Clamp01(1 - wallTransitionAlpha);
        // check if we're starting a new trans, and if so, enable the lower walls
        if(inRestingState)
            (wallLeverUp ? wallsMatchingLever : wallsDiffLever).gameObject.SetActive(true);
        // denote that work needs to be done
        inRestingState = false;
    }
    
    public void PullAreaLever()
    {
        areaLeverDown = true;
        // set symbol material
        leverSymbol.material = MazePuzzle.instance.activeSymbolMat;
        // move everything dooown
        inRestingState = false;
    }
    
    // FixedUpdate is called as many times as needed to maintain a set call amount / time ratio.
    private void FixedUpdate()
    {
        // no update needed if no transition
        if (inRestingState) return;
        
        // a down area lever means to ignore all existing transition stuff
        if (areaLeverDown)
        {
            // we're done if both are finished moving down
            if (TryMoveOut(wallsMatchingLever) && TryMoveOut(wallsDiffLever))
                inRestingState = true;
            
            return;
        }
        
        // calculate the elapsed transition (no 0-1 bounds check)
        wallTransitionAlpha += Time.fixedDeltaTime / wallTransitionTime;
        // set wall position, accounting for direction
        float posAlpha = wallLeverUp ? wallTransitionAlpha : 1 - wallTransitionAlpha;
        SetWallPosition(Mathf.Lerp(wallDownY, wallUpY, posAlpha)); // lerp does bounds check
        
        // check if transition is finished
        if (wallTransitionAlpha >= 1)
        {
            // disable the walls that are down
            (wallLeverUp ? wallsDiffLever : wallsMatchingLever).gameObject.SetActive(false);
            // denote we shouldn't move until another update
            inRestingState = true;
        }
    }
    
    // literally just sets the positions of the walls to match the given position.
    // walls that default to down are reversed.
    private void SetWallPosition(float wallY)
    {
        Vector3 wallPos = wallsMatchingLever.localPosition;
        wallPos.y = wallY;
        wallsMatchingLever.localPosition = wallPos;
        
        wallPos = wallsDiffLever.localPosition;
        wallPos.y = wallDownY + (wallUpY - wallY); // invert
        wallsDiffLever.localPosition = wallPos;
    }
    
    // used to move the wall parents down to their final state after area lever is pulled
    // returns true if it is in the full down position.
    private bool TryMoveOut(Transform wallParent)
    {
        if (!wallParent.gameObject.activeSelf) return true;
        
        Vector3 pos = wallParent.localPosition;
        // check if we're finished
        if (pos.y <= wallDownY)
        {
            wallParent.gameObject.SetActive(false);
            return true;
        }
        
        // move down, possibly past wallDownY, doesn't matter much
        pos.y -= (Time.fixedDeltaTime / wallTransitionTime) * (wallUpY - wallDownY);
        wallParent.localPosition = pos;
        return false;
    }
    
    // debug method to flip a lever
    public void DebugFlipAreaLever(PlayerCore player, HoldableItem item) => lever.BaseInteract(player, item);
    public bool DebugGetLibLever() => !wallLeverUp;
}
