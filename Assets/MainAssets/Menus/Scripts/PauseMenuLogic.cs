using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenuLogic : BaseMenuLogic
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private PlayerCore player;
    
    [HideInInspector]
    public UnityEvent<bool> pauseToggled = new UnityEvent<bool>();
    
    private void Start()
    {
        pauseMenu.SetActive(false);
        player.InputActions.Pause.performed += ctx => SetPaused(!pauseMenu.activeSelf);
    }
    
    private void SetPaused(bool pause, bool netUpdate = true)
    {
        if(pause) Pause(netUpdate);
        else Resume(netUpdate);
    }
    
    public void QuitToMain()
    {
        if(PhotonNetwork.IsConnected) PhotonNetwork.LeaveRoom();
        ScreenFade.instance.LoadSceneWithFade("1_MainMenu", true);
    }
    
    public void Pause() => Pause(true);
    private void Pause(bool netUpdate)
    {
        pauseToggled.Invoke(true);
        if(netUpdate) PhotonPacket.PAUSE.Value = true;
        player.ToggleGameInput(false);
        player.InputActions.Pause.Enable(); // keep this enabled
        pauseMenu.SetActive(true);
    }
    
    public void Resume() => Resume(true);
    private void Resume(bool netUpdate)
    {
        if(netUpdate) PhotonPacket.PAUSE.Value = false;
        pauseMenu.SetActive(false);
        player.ToggleGameInput(true);
        pauseToggled.Invoke(false);
    }
    
    public override void OnRoomPropertiesUpdate(Hashtable deltaProps)
    {
        if (PhotonPacket.PAUSE.WasChanged(deltaProps))
            SetPaused(PhotonPacket.PAUSE.Get(deltaProps), false);
    }
    
    /*public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // abandonment!
        PhotonNetwork.LeaveRoom();
        // load error scene
    }*/
}
