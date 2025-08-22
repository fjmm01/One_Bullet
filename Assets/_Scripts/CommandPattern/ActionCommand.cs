using System.Collections;
using UnityEngine;

public abstract class ActionCommand : ICommand
{
    protected PlayerController playerController;
    protected bool wasExecuted;

    public float Timestamp { get; set; }
    public abstract string CommandName { get; }

    protected ActionCommand(PlayerController controller)
    {
        playerController = controller;
        Timestamp = Time.time;
        wasExecuted = false;
    }

    public abstract void Execute();
    public abstract void Undo();
    public virtual bool CanExecute() => playerController != null && !wasExecuted;
}