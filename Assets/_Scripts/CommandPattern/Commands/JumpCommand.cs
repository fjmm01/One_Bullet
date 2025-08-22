using System.Collections;
using UnityEngine;

public class JumpCommand : ActionCommand
{
    public override string CommandName => "Jump";
    public JumpCommand(PlayerController controller) : base(controller)
    {

    }

    public override void Execute()
    {
        if(CanExecute())
        {
            var movement = playerController.GetComponent<PlayerMovement>();
            if(movement && movement.IsGrounded)
            {
                movement.TryJump();
                wasExecuted = true;
            }
        }
    }

    public override void Undo()
    {
        //Cant undo a jump, but reset the flag
        wasExecuted = false;
    }

    public override bool CanExecute()
    {
        var movement = playerController?.GetComponent<PlayerMovement>();
        return base.CanExecute() && movement != null && movement.ReadyToJump && movement.IsGrounded;
    }
    
}