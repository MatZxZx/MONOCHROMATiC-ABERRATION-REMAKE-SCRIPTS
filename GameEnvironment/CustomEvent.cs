using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEvent : MonoBehaviour
{
    public UnityEvent OnEvent;

    void OnTriggerEnter(Collider other)
    {
        OnEvent.Invoke();
    }
}
