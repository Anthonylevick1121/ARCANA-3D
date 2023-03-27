using Photon.Pun;
using UnityEngine;

public class EndScreenLogic : BaseMenuLogic
{
    [SerializeField] private GameObject winScreen, loseScreen;
    
    // Start is called before the first frame update
    private void Start()
    {
        if (!PhotonNetwork.IsConnected) return; // is debug
        bool win = PhotonPacket.GAME_WIN.Value;
        if(PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();
        winScreen.GetComponentInParent<Canvas>().sortingOrder = (int) CanvasLayer.EndScreen;
        winScreen.SetActive(win);
        loseScreen.SetActive(!win);
    }
    
    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void Credits() => ScreenFade.instance.LoadSceneWithFade("CreditsPage", false);
}
