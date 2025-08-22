using System;
using UnityEngine;

/// <summary>
/// Updated Player Controller that works with Command Pattern input system
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private CommandManager commandManager;
    public Transform orientation;

    public static MovementState CurrentState;

    #region Input Flags
    private bool jumpFlag;
    private bool crouchFlag;
    private bool sprintFlag;
    #endregion

    #region Properties
    public PlayerMovement PlayerMovement => playerMovement;
    public CommandManager CommandManager => commandManager;
    public bool IsJumping => jumpFlag;
    public bool IsCrouching => crouchFlag;
    public bool IsSprinting => sprintFlag;
    #endregion

    private void Awake()
    {
        InitializeComponents();
        CurrentState = MovementState.Walking;
    }

    private void Update()
    {
        HandleMovementInputs();
        HandleMovementState();

        // Try to consume buffered jump input if we can jump
        TryConsumeBufferedJump();
    }

    private void InitializeComponents()
    {
        // Get component references
        if (!playerMovement) playerMovement = GetComponent<PlayerMovement>();
        if (!inputManager) inputManager = GetComponent<InputManager>();
        if (!commandManager) commandManager = GetComponent<CommandManager>();

        // Validate critical components
        if (!playerMovement)
        {
            Debug.LogError("PlayerController: PlayerMovement component not found!");
        }

        if (!commandManager)
        {
            Debug.LogError("PlayerController: CommandManager component not found!");
        }
    }

    private void HandleMovementInputs()
    {
        // Handle slide logic
        if (CurrentState == MovementState.Sprinting && crouchFlag && !playerMovement.IsSliding)
        {
            Debug.Log("Starting slide from sprint + crouch");
            playerMovement.StartSlide();
        }

        // Stop sliding when crouch is released
        if (!crouchFlag && playerMovement.IsSliding)
        {
            playerMovement.StopSlide();
        }

        // Handle jump
        if (jumpFlag)
        {
            playerMovement.TryJump();
            jumpFlag = false; // Reset jump flag after attempting
        }

        // Handle crouching (but not while sliding)
        if (crouchFlag && !sprintFlag && !playerMovement.IsCrouching && !playerMovement.IsSliding)
        {
            playerMovement.Crouch();
        }
        else if (!crouchFlag && !playerMovement.IsSliding)
        {
            playerMovement.UnCrouch();
        }
    }

    private void HandleMovementState()
    {
        // State priority: Sliding > Sprinting > Crouching > Air > Walking
        if (playerMovement.IsSliding)
        {
            CurrentState = MovementState.Sliding; // Need to add this to enum
        }
        else if (sprintFlag && playerMovement.IsGrounded)
        {
            CurrentState = MovementState.Sprinting;
        }
        else if (crouchFlag && playerMovement.IsGrounded)
        {
            CurrentState = MovementState.Crouching;
        }
        else if (!playerMovement.IsGrounded)
        {
            CurrentState = MovementState.Air;
        }
        else
        {
            CurrentState = MovementState.Walking;
        }
    }

    private void TryConsumeBufferedJump()
    {
        // Try to consume buffered jump input if we can jump now
        if (playerMovement.IsGrounded && playerMovement.ReadyToJump && commandManager)
        {
            if (commandManager.ConsumeBufferedInput("Jump", 0.2f))
            {
                jumpFlag = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // Handle movement in FixedUpdate for physics consistency
        if (playerMovement.IsSliding)
        {
            playerMovement.SlidingMovement(playerMovement.MoveInput);
        }
        else
        {
            playerMovement.Move(playerMovement.MoveInput);
        }
    }

    #region Public Input Methods (Called by Commands)

    /// <summary>
    /// Sets sprint input state - called by SprintCommand
    /// </summary>
    public void SetSprintInput(bool isPressed)
    {
        sprintFlag = isPressed;

        // Log state change for debugging
        Debug.Log($"Sprint input: {(isPressed ? "Started" : "Stopped")}");
    }

    /// <summary>
    /// Sets crouch input state - called by CrouchCommand
    /// </summary>
    public void SetCrouchInput(bool isPressed)
    {
        crouchFlag = isPressed;

        // Log state change for debugging
        Debug.Log($"Crouch input: {(isPressed ? "Started" : "Stopped")}");
    }

    /// <summary>
    /// Triggers jump - called by JumpCommand
    /// </summary>
    public void TriggerJump()
    {
        if (playerMovement.IsGrounded && playerMovement.ReadyToJump)
        {
            jumpFlag = true;
            Debug.Log("Jump triggered");
        }
    }

    #endregion

    #region Public Utility Methods

    /// <summary>
    /// Execute a custom command through the player
    /// </summary>
    public void ExecuteCommand(ICommand command)
    {
        commandManager?.ExecuteCommand(command);
    }

    /// <summary>
    /// Buffer a command for later execution
    /// </summary>
    public void BufferCommand(ICommand command)
    {
        commandManager?.BufferCommand(command);
    }

    /// <summary>
    /// Get current movement speed for external systems
    /// </summary>
    public float GetCurrentSpeed()
    {
        if (playerMovement)
        {
            return playerMovement.GetComponent<Rigidbody>().linearVelocity.magnitude;
        }
        return 0f;
    }

    /// <summary>
    /// Check if player can perform certain actions
    /// </summary>
    public bool CanSprint()
    {
        return playerMovement.IsGrounded && !playerMovement.IsCrouching && !playerMovement.IsSliding;
    }

    public bool CanCrouch()
    {
        return playerMovement.IsGrounded;
    }

    public bool CanSlide()
    {
        return sprintFlag && playerMovement.IsGrounded && !playerMovement.IsSliding;
    }

    #endregion

    #region Event Handlers (Optional)

    private void OnEnable()
    {
        // Subscribe to command manager events if needed
        if (commandManager)
        {
            commandManager.OnCommandExecuted += OnCommandExecuted;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (commandManager)
        {
            commandManager.OnCommandExecuted -= OnCommandExecuted;
        }
    }

    private void OnCommandExecuted(ICommand command)
    {
        // React to specific commands if needed
        switch (command.CommandName)
        {
            case "Jump":
                // Could add screen shake, sound effects, etc.
                break;
            case "Sprint":
                // Could add speed lines effect, etc.
                break;
        }
    }

    #endregion
}