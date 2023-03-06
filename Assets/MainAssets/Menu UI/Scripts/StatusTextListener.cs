using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatusTextListener : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField] private float statusDuration = 5f; // seconds display

    private float displayTimeLeft = 0;
    
    // Start is called before the first frame update
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
    }
    
    // Update is called once per frame
    private void Update()
    {
        if (displayTimeLeft > 0)
        {
            displayTimeLeft -= Time.deltaTime;
            if (displayTimeLeft <= 0)
                text.text = "";
        }
    }
    
    public void SetStatus(string msg)
    {
        text.text = msg;
        displayTimeLeft = statusDuration;
    }
}
