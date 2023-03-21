using TMPro;
using UnityEngine;

public class LibraryState : MonoBehaviour
{
    [SerializeField] public StatusTextListener statusText;
    [SerializeField] public TextMeshProUGUI debugText;
    
    // Start is called before the first frame update
    private void Start()
    {
        MusicManager.DestroyInstance();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            debugText.gameObject.SetActive(!debugText.gameObject.activeSelf);
    }
}
