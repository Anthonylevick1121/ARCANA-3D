using UnityEngine;

public class LibraryMazeLever : Interactable
{
    [SerializeField] private LibraryMazeState maze;
    
    private bool flipped;
    
    public override string GetPrompt(HoldableItem item) => "Flip Lever";
    
    protected override bool Interact(PlayerCore player, HoldableItem heldItem)
    {
        flipped = !flipped;
        
        print($"interact: lib lever flip to {flipped}");
        
        PhotonPacket.MAZE_LIB_LEVER.Value = flipped;
        // animate
        transform.localRotation *= Quaternion.Euler(Vector3.forward * 180);
        
        // toggle allll the fun maze stuff
        maze.OnFlipLibrarianLever(flipped);
        
        return true;
    }
}
