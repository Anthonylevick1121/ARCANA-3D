using TMPro;
using UnityEngine;

public class LibraryState : MonoBehaviour
{
    [SerializeField] public StatusTextListener statusText;
    [SerializeField] public TextMeshProUGUI debugText;
    
    [SerializeField] private PlayerCore player;
    [SerializeField] private GameObject winScreen, loseScreen;
    
    public bool debug = false;
    
    // Start is called before the first frame update
    private void Start()
    {
        MusicManager.DestroyInstance();
        debugText.gameObject.SetActive(debug);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            debug = !debug;
            debugText.gameObject.SetActive(debug);
        }
    }
    
    // todo function to show win/loss screen
}
