using UnityEngine;

public class CreditsLogic : BaseMenuLogic
{
    [SerializeField] private Canvas canvas;
    
    private void Start()
    {
        canvas.sortingOrder = (int) CanvasLayer.CreditsScreen;
    }
}
