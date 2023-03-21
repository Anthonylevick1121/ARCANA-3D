public class RitualCircle : Interactable
{
    public override string GetPrompt(HoldableItem heldItem) => "Activate Ritual Circle";
    
    protected override bool Interact(PlayerCore player, HoldableItem heldItem)
    {
        if (MazePuzzle.instance.CheckWinnable())
        {
            player.ui.status.SetStatus("Ritual Circle Activated!\nThe arch mage has been saved.");
            PhotonPacket.MAZE_WIN.Value = true;
        }
        else
        {
            player.ui.status.SetStatus("The circle hums, but the\nflow of magic is too weak...");
        }
        
        return true;
    }
}
