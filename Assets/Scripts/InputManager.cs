using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;

    public PlayerInput.PlayerActionsActions players;

    private playerMotor motor;

    private PlayerLook look;

    private PhotonView view;
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        players = playerInput.playerActions;
        motor = GetComponent<playerMotor>();
        look = GetComponent<PlayerLook>();
        players.Jump.performed += ctx => motor.Jump();
        
        players.Crouch.performed += ctx => motor.Crouch();
        players.Sprint.performed += ctx => motor.Sprint();
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            motor.ProcessMove(players.Movement.ReadValue<Vector2>());
        }
    }

    private void LateUpdate()
    {
        if (view.IsMine)
        {
            look.ProcessLook(players.Look.ReadValue<Vector2>());
        }
    }

    private void OnEnable()
    {
        players.Enable();
    }

    private void OnDisable()
    {
        players.Disable();
    }
}
