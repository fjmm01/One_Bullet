
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    [Header("Buffer Settings")]
    [SerializeField] private float bufferWindow = 0.2f;
    [SerializeField] private int maxHistorySize = 100;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private Queue<BufferedInput> inputBuffer = new Queue<BufferedInput>();
    private List<ICommand> commandHistory = new List<ICommand>();
    private Queue<ICommand> immediateCommands = new Queue<ICommand>();

    public event Action<ICommand> OnCommandExecuted;

    private void Update()
    {
        ProcessBufferedInputs();
        ExecuteImmediateCommands();
        CleanExpiredBufferedInputs();
    }

    public void ExecuteCommand(ICommand command)
    {
        if (command != null)
        {
            BufferedInput bufferedInput = new BufferedInput(command.CommandName, command);
            inputBuffer.Enqueue(bufferedInput);

            if (showDebugLogs)
                Debug.Log($"Buffered command: {command.CommandName} at {Time.time}");
        }
    }
    public void BufferCommand(ICommand command)
    {
        if (command != null)
        {
            BufferedInput bufferedInput = new BufferedInput(command.CommandName, command);
            inputBuffer.Enqueue(bufferedInput);

            if (showDebugLogs)
                Debug.Log($"Buffered command: {command.CommandName} at {Time.time}");
        }
    }

    public bool ConsumeBufferedInput(string inputName, float maxAge = -1f)
    {
        if(maxAge < 0f) maxAge = bufferWindow;
        foreach(var bufferedInput in inputBuffer)
        {
            if(bufferedInput.inputName == inputName && !bufferedInput.consumed)
            {
                if(Time.time -bufferedInput.timestamp <= maxAge)
                {
                    if(bufferedInput.command.CanExecute())
                    {
                        ExecuteCommandInternal(bufferedInput.command);
                        bufferedInput.consumed = true;
                        if (showDebugLogs)
                            Debug.Log($"Consumed buffered command: {inputName}");

                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool UndoLastCommand()
    {
        if (commandHistory.Count > 0)
        {
            var lastCommand = commandHistory.Last();
            try
            {
                lastCommand.Undo();
                commandHistory.RemoveAt(commandHistory.Count - 1);

                if (showDebugLogs)
                    Debug.Log($"Undid command: {lastCommand.CommandName}");

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to undo command {lastCommand.CommandName}: {e.Message}");
            }
        }
        return false;
    }

    public List<ICommand> GetCommandHistory()
    {
        return new List<ICommand>(commandHistory);
    }

    public void ClearAll()
    {
        inputBuffer.Clear();
        commandHistory.Clear();
        immediateCommands.Clear();
    }


    private void ProcessBufferedInputs()
    {
        var processableInputs = inputBuffer.Where(input =>
            !input.consumed &&
            !input.IsExpired(bufferWindow) &&
            input.command.CanExecute()).ToList();

        foreach (var input in processableInputs)
        {
            ExecuteCommandInternal(input.command);
            input.consumed = true;
        }
    }

    private void ExecuteImmediateCommands()
    {
        while (immediateCommands.Count > 0)
        {
            var command = immediateCommands.Dequeue();
            if (command.CanExecute())
            {
                ExecuteCommandInternal(command);
            }
        }
    }
    private void ExecuteCommandInternal(ICommand command)
    {
        try
        {
            command.Execute();

            // Add to history (limit size)
            commandHistory.Add(command);
            if (commandHistory.Count > maxHistorySize)
            {
                commandHistory.RemoveAt(0);
            }

            OnCommandExecuted?.Invoke(command);

            if (showDebugLogs)
                Debug.Log($"Executed command: {command.CommandName} at {Time.time}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to execute command {command.CommandName}: {e.Message}");
        }
    }

    private void CleanExpiredBufferedInputs()
    {
        var expiredInputs = inputBuffer.Where(input => input.IsExpired(bufferWindow)).ToList();

        foreach (var expired in expiredInputs)
        {
            // Remove expired inputs from queue
            var tempQueue = new Queue<BufferedInput>();
            while (inputBuffer.Count > 0)
            {
                var input = inputBuffer.Dequeue();
                if (!input.IsExpired(bufferWindow))
                {
                    tempQueue.Enqueue(input);
                }
            }
            inputBuffer = tempQueue;
        }
    }
}