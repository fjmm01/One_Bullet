using System.Collections;
using UnityEngine;

public class SprintCommand : ActionCommand
{
    private bool sprintState;

    public override string CommandName => "Sprint";

    public SprintCommand(PlayerController controller, bool isPressed) : base(controller)
    {
        sprintState = isPressed;
    }

    public override void Execute()
    {
        if (playerController)
        {
            playerController.SetSprintInput(sprintState);
            wasExecuted = true;
        }
    }

    public override void Undo()
    {
        if (playerController)
        {
            playerController.SetSprintInput(!sprintState);
            wasExecuted = false;
        }
    }

    public override bool CanExecute()
    {
        return playerController != null;
    }
}