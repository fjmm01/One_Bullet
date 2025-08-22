using System.Collections;
using UnityEngine;

public abstract class MovementCommand : ICommand

{
    protected PlayerController playerController;
    protected Vector2 inputValue;

    public float Timestamp { get;  set; }
    public abstract string CommandName { get; }

    protected MovementCommand(PlayerController controller)
    {
        playerController = controller;
        Timestamp = Time.time;
    }

    public abstract void Execute();
    public virtual void Undo() { }//Most movement commands cant be undone
    public virtual bool CanExecute() => playerController != null;


}