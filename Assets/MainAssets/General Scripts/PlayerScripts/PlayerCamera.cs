using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerCore))]
public class PlayerCamera : MonoBehaviour
{
    [FormerlySerializedAs("cam")] [SerializeField] public new Camera camera;
    [SerializeField] private float xSensitivity = 10.0f;
    [SerializeField] private float ySensitivity = 10.0f;
    private float xRotation;
    private PhotonView view;

    private InputAction lookAction;
    private bool delayInput;
    
    private void Start()
    {
        //added a component for photonView
        view = GetComponent<PhotonView>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        xRotation = camera.transform.localRotation.x;
        
        PlayerCore player = GetComponent<PlayerCore>();
        // player.InputActions.Look.performed += ProcessLook;
        lookAction = player.InputActions.Look;
        delayInput = true;
    }
    
    private void Update()
    {
        //3-2-23 added an if statement for view.IsMine
        if (view.IsMine)
        {
            Vector2 input = lookAction.ReadValue<Vector2>();
            if (input == Vector2.zero) return;
            // I noticed a lot of instances where the game shoves the camera either into the ground or elsewhere on
            // frame one. idk if this is just an editor thing but this is an idea that seems to remove said shoving.
            // The user shouldn't notice a few frames of camera input.
            if (delayInput && input.magnitude > 75) return;
            if (delayInput && (Time.deltaTime < 0.02f || Time.timeSinceLevelLoad > 1f)) delayInput = false;
        
            // x rotation, rotate camera
            xRotation -= input.y * ySensitivity * Time.deltaTime;
            xRotation = Mathf.Clamp(xRotation, -80.0f, 80.0f);
            camera.transform.localRotation = Quaternion.Euler(xRotation,0,0);
        
            // y rotation, rotate character
            transform.Rotate(Vector3.up * (input.x * xSensitivity * Time.deltaTime));
        }
       
    }
}
