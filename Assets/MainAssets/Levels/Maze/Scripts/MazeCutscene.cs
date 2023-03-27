using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MazeCutscene : MonoBehaviourPunCallbacks
{
    private PlayerCore player;
    
    private Animator animator;
    private static readonly int playTriggerId = Animator.StringToHash("Play");
    
    [SerializeField] private Transform cameraCutsceneParent;
    
    // cache to return to at the end the anim
    private Vector3 cameraPos;
    private Quaternion cameraRot;
    
    private bool autoPlayAnim;
    
    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
        
        // this is initialized after MazePuzzle, so the instance is available.
        player = MazePuzzle.instance.player;
        
        // tell librarian we're in the maze
        PhotonPacket.MAZE_PLAYER_ENTER.Value = true;
        // check if lib player made it to their room already
        autoPlayAnim = PhotonPacket.MAZE_LIB_ENTER.Value; // unset values default to false
        // fun voice line if the player is first
        if(!autoPlayAnim)
            VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeFirstP);
    }
    
    private void Update()
    {
        if (autoPlayAnim)
        {
            autoPlayAnim = false;
            StartCutscene();
        }
        if(player.debug && Input.GetKeyDown(KeyCode.C))
            StartCutscene();
    }
    
    private void StartCutscene()
    {
        VoicePlayer.instance.PlayVoiceLine(VoiceLineId.MazeIntroP);
        // lock input
        // note that pause state is synced, so it will be *very hard* to trigger cutscene with a menu open
        player.InputActions.Disable();
        // cache transform
        Transform camera = player.view.camera.transform;
        cameraPos = camera.localPosition;
        cameraRot = camera.localRotation;
        
        camera.SetParent(cameraCutsceneParent);
        camera.localPosition = Vector3.zero;
        camera.localRotation = Quaternion.identity;
        animator.SetTrigger(playTriggerId);
        
        // set interaction text to instructions (temp impl rn)
        player.ui.promptText.text = "Activate all the Sigils...";
    }
    
    public void OnEndCutscene()
    {
        // restore camera
        Transform camera = player.view.camera.transform;
        camera.SetParent(player.transform);
        // restore transform
        camera.localPosition = cameraPos;
        camera.localRotation = cameraRot;
        player.InputActions.Enable();
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.MAZE_LIB_ENTER.GetOr(deltaProps, false))
            StartCutscene();
    }
}
