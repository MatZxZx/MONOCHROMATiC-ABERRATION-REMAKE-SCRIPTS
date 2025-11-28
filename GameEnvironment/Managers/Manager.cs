using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField] protected int counter = 0;
    [SerializeField] protected int points = 0;
    //[SerializeField] protected AudioSource audioSrc;

    private void Awake()
    {
        //audioSrc = GetComponent<AudioSource>();
    }

    public void Add(int _counter = 0, int _points = 0)
    {

        counter += _counter;
        points += _points;

    }
    public void AddCounter(int _counter = 0)
    {
        counter += _counter;
    }
    public void AddPoints(int _points = 0)
    {
        counter += _points;
    }
    public int GetCounter()
    {
        return counter;
    }
    public int GetPoints()
    {   
        return points;
    }
}
