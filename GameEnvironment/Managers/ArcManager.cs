using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcManager : Manager
{
    public bool isOn;
    public static ArcManager Instance;

    void Awake()
    {
        Instance = this;
    }
}
