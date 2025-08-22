using UnityEngine;

public class MoveCommand: MovementCommand
{
    public override string CommandName => "Move";
    public MoveCommand(PlayerController controller, Vector2 moveInput): base(controller)
    {
        inputValue = moveInput;
    }

    public override void Execute()
    {
        if(playerController?.GetComponent<PlayerMovement>())
        {
            playerController.GetComponent<PlayerMovement>().MoveInput = inputValue;

        }
    }
}
