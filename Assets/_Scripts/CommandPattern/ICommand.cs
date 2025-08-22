using System;
using UnityEngine;

public interface ICommand
{
    void Execute();
    void Undo();
    bool CanExecute();
    string CommandName { get; }
    float Timestamp { get; }
}
