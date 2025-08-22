using System.Collections;
using UnityEngine;


public class LookCommand : MovementCommand
{
    public override string CommandName => "Look";
    public LookCommand(PlayerController controller,Vector2 lookInput): base(controller)
    {
        inputValue = lookInput;
    }

    public override void Execute()
    {
        var cameraController = playerController?.GetComponentInChildren<FPSCameraController>();
        if (cameraController)
        {
            //Store look input for camera controller to use
            cameraController.SetLookInput(inputValue);
        }
    }
}