using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Flags")]
    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private Vector2 _lookInput;
    [SerializeField] private bool _jumpInput;
    [SerializeField] private bool _crouchInput;
    [SerializeField] private bool _sprintInput;

    #region Properties
    private bool needNewJumpInput;
    #endregion

    #region Getters and Setters
    public Vector2 MoveInput {get => _moveInput; set => _moveInput = value;}
    public Vector2 LookInput {get => _lookInput; set => _lookInput = value;}

    public bool JumpInput {get => _jumpInput; set => _jumpInput = value; }
    public bool CrouchInput {get => _crouchInput; set => _crouchInput = value; }
    public bool SprintInput {get => _sprintInput; set => _sprintInput = value; }
    #endregion

    PlayerControls _playerControls;

    private void Awake()
    { 
        _playerControls = new PlayerControls();

        //Movement Bindings
        _playerControls.Player.Movement.started += OnMove;
        _playerControls.Player.Movement.performed += OnMove;
        _playerControls.Player.Movement.canceled += OnMove;
        //Look Bindings
        _playerControls.Player.Look.started += OnLook;
        _playerControls.Player.Look.performed += OnLook;
        _playerControls.Player.Look.canceled += OnLook;
        //Jump Bindings
        _playerControls.Player.Jump.started += OnJump;
        _playerControls.Player.Jump.performed += OnJump;
        _playerControls.Player.Jump.canceled += OnJump;
        //Crouch Bindings
        _playerControls.Player.Crouch.started += OnCrouch;
        _playerControls.Player.Crouch.performed += OnCrouch;
        _playerControls.Player.Crouch.canceled += OnCrouch;
        //Sprint Bindings
        _playerControls.Player.Sprint.started += OnSprint;
        _playerControls.Player.Sprint.performed += OnSprint;
        _playerControls.Player.Sprint.canceled += OnSprint;

    }

    private void OnEnable()
    {
        _playerControls.Enable();
    }

    private void OnDisable()
    { 
        _playerControls.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    { 
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    { 
        _lookInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    { 
        _jumpInput = context.ReadValueAsButton();
        needNewJumpInput = false;
    }
    private void OnCrouch(InputAction.CallbackContext context)
    {
        _crouchInput = context.ReadValueAsButton();
    }
    private void OnSprint(InputAction.CallbackContext context)
    {
        _sprintInput = context.ReadValueAsButton();
    }
}