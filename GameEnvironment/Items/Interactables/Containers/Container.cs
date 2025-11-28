using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Item
{
    [SerializeField] private ContainerData data;

    public void ContainerBehaviour()
    {
        string command = data.typeCommand.Replace("()", "(" + data.amount + ", " + data.amount * 100 + ")");
        Debug.Log(command);
        Invoke(command, 0.1f);
    }
}
