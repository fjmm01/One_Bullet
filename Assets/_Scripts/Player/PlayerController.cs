using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private PlayerMovement _playerMovement;
    public Transform orientation;

    public static MovementState CurrentState;

    #region Flags
    bool _jumpFlag;
    bool _crouchFlag;
    bool _sprintFlag;
    #endregion

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _inputManager = GetComponent<InputManager>();
        CurrentState = MovementState.Walking;
    }

    private void Update()
    {
        HandleInputs();
        HandleMovementInputs();
        HandleMovementState();
    }

    private void HandleMovementInputs()
    {
        if(CurrentState == MovementState.Sprinting && _crouchFlag && !_playerMovement.IsSliding)
        {
            Debug.Log("Entro en el If de StartSlide");
            _playerMovement.StartSlide();
        }
        if(!_crouchFlag && _playerMovement.IsSliding)
        {
            _playerMovement.StopSlide();
        }
        if (_jumpFlag)
        {
            _playerMovement.TryJump();
        }
        if(_crouchFlag && !_sprintFlag && !_playerMovement.IsCrouching)
        {
            _playerMovement.Crouch();
        }
        else
        {
            _playerMovement.UnCrouch();
        }
    }

    private void HandleMovementState()
    {
        if (_sprintFlag)
        {
            CurrentState = MovementState.Sprinting;
        }
        else if (_crouchFlag)
        {
            CurrentState = MovementState.Crouching;
        }
        else if (_playerMovement.IsGrounded)
        {
            CurrentState = MovementState.Walking;
        }
        else
        {
            CurrentState = MovementState.Air;
        }

    }



    private void HandleInputs()
    {
        _playerMovement.MoveInput = _inputManager.MoveInput;
        _jumpFlag = _inputManager.JumpInput;
        _sprintFlag = _inputManager.SprintInput;
        _crouchFlag = _inputManager.CrouchInput;
        
    }

    private void FixedUpdate()
    {
        
        if (_playerMovement.IsSliding)
        {
            _playerMovement.SlidingMovement(_inputManager.MoveInput);
        }
        else
        {
            _playerMovement.Move(_inputManager.MoveInput);
        }
        
        
    }

}
