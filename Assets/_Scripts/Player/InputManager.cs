using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Flags")]
    [SerializeField] private Vector2 _moveInput;
    [SerializeField] private Vector2 _lookInput;


    #region Getters and Setters
    public Vector2 MoveInput {get => _moveInput; set => _moveInput = value;}
    public Vector2 LookInput {get => _lookInput; set => _lookInput = value;}
    #endregion

    PlayerControls _playerControls;

    private void Awake()
    { 
        _playerControls = new PlayerControls();

        //Set Bindings
        _playerControls.Player.Movement.started += OnMove;
        _playerControls.Player.Movement.performed += OnMove;
        _playerControls.Player.Movement.canceled += OnMove;

        _playerControls.Player.Look.started += OnLook;
        _playerControls.Player.Look.performed += OnLook;
        _playerControls.Player.Look.canceled += OnLook;

        
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
}