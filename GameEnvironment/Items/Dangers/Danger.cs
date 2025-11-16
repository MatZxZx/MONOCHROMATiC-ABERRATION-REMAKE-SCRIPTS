using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Danger : MonoBehaviour
{
    public int interactions = 0;
    [SerializeField] protected int damageRange;
    void OnTriggerEnter(Collider other)
    {
        DangerBehaviour(other);
        interactions++;
    }
    public virtual void DangerBehaviour(Collider other) { }
}
