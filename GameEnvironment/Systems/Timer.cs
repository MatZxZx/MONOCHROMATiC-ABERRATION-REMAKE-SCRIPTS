using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public float totalDuration;
    public float counter;
    public float speed;
    public Timer(float _totalDuration = 60, float _counter = 0, float _speed = 1)
    {
        totalDuration = _totalDuration;
        counter = _counter;
        speed = _speed;
    }
    public float Run()
    {
        return counter += speed * Time.deltaTime;
    }
    public float Reset()
    {
        return counter = 0;
    }
}

//AL RATO VUELVO J