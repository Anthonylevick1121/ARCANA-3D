using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerCore))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private PlayerCore player;
    //private PhotonView view; 
    
    private bool isGrounded;
    private bool crouching;
    private bool sprinting;
    private bool lerpCrouch;
    private float crouchTimer = 0f;
    private float gravity = -9.8f;
    private float currentFallSpeed = 0;
    
    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float jumpHeight = 0.6f;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        player = GetComponent<PlayerCore>();
        //added this component for the if statement
        //view = GetComponent<PhotonView>();
        
        player.InputActions.Jump.performed += ctx => Jump();
        player.InputActions.Crouch.performed += ctx => Crouch();
        player.InputActions.Sprint.performed += ctx => Sprint(ctx.ReadValueAsButton());
    }
    
    private void FixedUpdate()
    {
        // handle continuous movement
        
        //3-2-23 added an ifStatement to wrap the function in 
        //if (view.IsMine)
        {
            Vector2 input = player.InputActions.Movement.ReadValue<Vector2>();
            Vector3 movement = moveSpeed * transform.TransformDirection(input.x, 0, input.y);
        
            // consistently add downward acceleration
            currentFallSpeed += gravity * Time.fixedDeltaTime;
        
            if (isGrounded && currentFallSpeed < 0)
            {
                currentFallSpeed = -2f; // the -2 helps ensure the character collider stays touching the ground.
            }
        
            movement.y = currentFallSpeed;
            movement *= Time.fixedDeltaTime;
            Vector3 pos = transform.position;
            /*CollisionFlags flags = */controller.Move(movement);
            Vector3 delta = transform.position - pos;
            if (currentFallSpeed > 0 && delta.y < movement.y / 2/*(flags & CollisionFlags.CollidedAbove) > 0*/)
            {
                // we moved less than we should: airborne collision, reflect upward momentum but a little slower.
                // The reason I'm doing the above condition instead of checking the flags is because sometimes an upward
                // collision still allows the character to shove around it and move up anyways. This code will allow for
                // that flexibility while still ensuring you don't stick to the roof.
                currentFallSpeed = -currentFallSpeed / 2;
            }
        
            // cached grounded var
            isGrounded = controller.isGrounded;
        
            // handle crouching
            if (lerpCrouch)
            {
                crouchTimer += Time.fixedDeltaTime;
            
                float p = crouchTimer * crouchTimer;
                controller.height = Mathf.Lerp(controller.height, crouching ? 1 : 2, p);
            
                if (p >= 1)
                {
                    lerpCrouch = false;
                    crouchTimer = 0f;
                }
            }
        }
        
    }
    
    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint(bool sprint)
    {
        sprinting = sprint;
        moveSpeed = sprinting ? 8 : 5;
    }
    
    public void Jump()
    {
        if (isGrounded)
        {
            currentFallSpeed = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}

