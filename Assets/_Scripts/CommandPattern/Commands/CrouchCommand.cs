using System.Collections;
using UnityEngine;

public class CrouchCommand : ActionCommand
{
    private bool crouchState;

    public override string CommandName => "Crouch";

    public CrouchCommand(PlayerController controller, bool isPressed) : base(controller)
    {
        crouchState = isPressed;
    }

    public override void Execute()
    {
        if (playerController)
        {
            playerController.SetCrouchInput(crouchState);
            wasExecuted = true;
        }
    }

    public override void Undo()
    {
        if (playerController)
        {
            playerController.SetCrouchInput(!crouchState);
            wasExecuted = false;
        }
    }


    public override bool CanExecute()
    {
        return playerController != null;
    }
}