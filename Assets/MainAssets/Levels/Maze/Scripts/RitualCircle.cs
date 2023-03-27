using UnityEngine;

public class RitualCircle : Interactable
{
    public override string GetPrompt(HoldableItem heldItem) => "Activate Ritual Circle";
    
    protected override bool Interact(PlayerCore player, HoldableItem heldItem)
    {
        if (MazePuzzle.instance.CheckWinnable())
        {
            player.ui.status.SetStatus("Ritual Circle Activated!\nThe arch mage has been saved.");
            PhotonPacket.GAME_WIN.Value = true;
            VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeCompleteA);
            // PhotonPacket.GAME_END.Value = true;
            ScreenFade.instance.LoadSceneWithFade("EndScene", Color.white, false);
        }
        else
        {
            player.ui.status.SetStatus("The circle hums, but the\nflow of magic is too weak...");
        }
        
        return true;
    }
}
