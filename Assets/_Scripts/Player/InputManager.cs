using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CommandManager commandManager;

    [Header("Settings")]
    [SerializeField] private bool enableInputBuffering = true;
    [SerializeField] private float mouseSensitivity = 1.0f;

    private PlayerControls playerControls;
    [SerializeField]private Vector2 currentMoveInput;
    private Vector2 currentLookInput;

    private void Awake()
    {
        // Get references
        if (!playerController) playerController = GetComponent<PlayerController>();
        if (!commandManager) commandManager = GetComponent<CommandManager>();

        // Initialize input system
        playerControls = new PlayerControls();
        BindInputActions();
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    private void Update()
    {
        // Process continuous inputs (movement and look)
        ProcessContinuousInputs();
    }

    private void BindInputActions()
    {
        // Movement bindings
        playerControls.Player.Movement.started += OnMovement;
        playerControls.Player.Movement.performed += OnMovement;
        playerControls.Player.Movement.canceled += OnMovement;

        // Look bindings
        playerControls.Player.Look.started += OnLook;
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += OnLook;

        // Action bindings
        playerControls.Player.Jump.started += OnJump;
        playerControls.Player.Sprint.started += OnSprintStarted;
        playerControls.Player.Sprint.canceled += OnSprintCanceled;
        playerControls.Player.Crouch.started += OnCrouchStarted;
        playerControls.Player.Crouch.canceled += OnCrouchCanceled;
    }

    private void ProcessContinuousInputs()
    {
        // Handle movement input
        if (currentMoveInput != Vector2.zero)
        {
            var moveCommand = new MoveCommand(playerController, currentMoveInput);
            commandManager.ExecuteCommand(moveCommand);
        }

        // Handle look input
        if (currentLookInput != Vector2.zero)
        {
            var lookCommand = new LookCommand(playerController, currentLookInput * mouseSensitivity);
            commandManager.ExecuteCommand(lookCommand);
        }
    }

    // Input callback methods
    private void OnMovement(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        currentLookInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            var jumpCommand = new JumpCommand(playerController);

            if (enableInputBuffering)
            {
                commandManager.BufferCommand(jumpCommand);
            }
            else
            {
                commandManager.ExecuteCommand(jumpCommand);
            }
        }
    }

    private void OnSprintStarted(InputAction.CallbackContext context)
    {
        var sprintCommand = new SprintCommand(playerController, true);
        commandManager.ExecuteCommand(sprintCommand);
    }

    private void OnSprintCanceled(InputAction.CallbackContext context)
    {
        var sprintCommand = new SprintCommand(playerController, false);
        commandManager.ExecuteCommand(sprintCommand);
    }

    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        var crouchCommand = new CrouchCommand(playerController, true);
        commandManager.ExecuteCommand(crouchCommand);
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        var crouchCommand = new CrouchCommand(playerController, false);
        commandManager.ExecuteCommand(crouchCommand);
    }

    /// <summary>
    /// Public method to execute custom commands
    /// </summary>
    public void ExecuteCustomCommand(ICommand command)
    {
        commandManager.ExecuteCommand(command);
    }

    /// <summary>
    /// Public method to buffer custom commands
    /// </summary>
    public void BufferCustomCommand(ICommand command)
    {
        commandManager.BufferCommand(command);
    }
}