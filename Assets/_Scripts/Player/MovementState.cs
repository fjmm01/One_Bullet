/// <summary>
/// Enhanced movement states for the player
/// Supports traditional FPS movement plus advanced mechanics like sliding
/// </summary>
public enum MovementState
{
    /// <summary>
    /// Default ground movement state
    /// </summary>
    Walking,

    /// <summary>
    /// Fast ground movement state
    /// </summary>
    Sprinting,

    /// <summary>
    /// Slow, low-profile movement state
    /// </summary>
    Crouching,

    /// <summary>
    /// Player is airborne (jumping, falling)
    /// </summary>
    Air,

    /// <summary>
    /// Momentum-based sliding movement
    /// Typically triggered by crouch while sprinting
    /// </summary>
    Sliding,

    /// <summary>
    /// Wall-running state (for future implementation)
    /// </summary>
    WallRunning,

    /// <summary>
    /// Grappling hook state (for future implementation)
    /// </summary>
    Grappling,

    /// <summary>
    /// Climbing/mantling state (for future implementation)
    /// </summary>
    Climbing
}