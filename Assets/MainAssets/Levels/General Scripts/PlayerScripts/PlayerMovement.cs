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
    
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private AudioSource footstepPlayer;
    [SerializeField] private AudioClip walkAudio, runAudio;
    private float startVolume;
    private bool isMoving;
    
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
        
        footstepPlayer.clip = walkAudio;
        startVolume = footstepPlayer.volume;
        isMoving = false;
    }
    
    private void FixedUpdate()
    {
        // handle continuous movement
        
        //3-2-23 added an ifStatement to wrap the function in 
        //if (!view.IsMine) return;
        
        Vector2 input = player.InputActions.Movement.ReadValue<Vector2>();
        
        Vector3 movement = moveSpeed * (sprinting ? sprintMultiplier : 1) * transform.TransformDirection(input.x, 0, input.y);
        
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
        
        // handle audio; stop footsteps if in air or not moving, and start audio if on ground and moving
        delta.y = 0; // audio doesn't care about vertical movement
        if (footstepPlayer.isPlaying && (!isGrounded || delta == Vector3.zero))
        {
            isMoving = false; // we aren't moving anymore, future movements should max the volume again
            // reduce volume linearly over the course of 0.25 seconds
            float newVolume = footstepPlayer.volume - (startVolume / 0.25f) * Time.fixedDeltaTime;
            if (newVolume <= 0)
                footstepPlayer.Stop();
            else
                footstepPlayer.volume = newVolume;
        }
        else if (!isMoving && (isGrounded && delta != Vector3.zero))
        {
            // moving now
            isMoving = true;
            footstepPlayer.volume = startVolume;
            if(!footstepPlayer.isPlaying)
                footstepPlayer.Play();
        }
    }
    
    private void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }
    
    private void Sprint(bool sprint)
    {
        sprinting = sprint;
        // moveSpeed = sprinting ? 8 : 5;
        footstepPlayer.clip = sprint ? runAudio : walkAudio;
        if(isMoving && !footstepPlayer.isPlaying)
            footstepPlayer.Play();
    }
    
    private void Jump()
    {
        if (isGrounded)
        {
            currentFallSpeed = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        controller.enabled = false;
        transform.position = pos;
        controller.enabled = true;
    }
}

