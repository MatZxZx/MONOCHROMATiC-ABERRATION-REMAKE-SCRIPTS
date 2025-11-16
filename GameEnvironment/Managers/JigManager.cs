using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigManager : Manager
{

    public bool isOn;
    public static JigManager Instance;

    void Awake()
    {
        Instance = this;
    }

}
