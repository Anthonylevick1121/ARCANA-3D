using UnityEngine;

public class MazeSectionLever : Interactable
{
    public MazeSectionPos mazeSection;
    
    // private new MeshRenderer renderer;
    // [SerializeField] private Material startMat, flipMat;
    
    private Transform parent;
    private bool flipped;
    
    public override string GetPrompt(HoldableItem item) => "Flip Lever";
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        parent = transform.parent;
        // renderer = GetComponent<MeshRenderer>();
        flipped = false;
        // renderer.material = startMat;
    }
    
    protected override bool Interact(PlayerCore player, HoldableItem heldItem)
    {
        // toggle torches and walls
        for (int i = 0; i <= 4; i++)
        {
            GameObject obj = parent.GetChild(i+2).gameObject;
            obj.SetActive(!obj.activeSelf);
        }
        
        // tell the main puzzle, so it knows when all have been flipped later
        if(mazeSection != MazeSectionPos.Tutorial)
            MazePuzzle.instance.TouchLever(mazeSection);
        
        // rendering update
        flipped = !flipped;
        transform.localRotation *= Quaternion.Euler(Vector3.forward * 180);
        // renderer.material = flipped ? flipMat : startMat;
        
        // temp notification
        string mainStatus = "Sounds echo through the maze...\nThe passages have changed.";
        
        string name = System.Enum.GetName(typeof(MazeSectionPos), mazeSection);
        mainStatus += $"\n(lever {name} turned to state {(flipped ? "on" : "off")})";
        player.ui.status.SetStatus(mainStatus);
        
        // send to librarian (should only send on first but it'll just ignore if already sent)
        PhotonPacket.MAZE_LEVER.Value = (mazeSection, flipped);
        
        return true;
    }
}