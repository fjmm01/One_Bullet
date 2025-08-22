using System.Collections;
using UnityEngine;

public class BufferedInput
{
    public string inputName;
    public float timestamp;
    public bool consumed;
    public ICommand command;

    public BufferedInput(string name, ICommand cmd)
    {
        inputName = name;
        command = cmd;
        timestamp = Time.time;
        consumed = false;
    }

    public bool IsExpired(float bufferwindow)
    {
        return Time.time - timestamp > bufferwindow;
    }
}