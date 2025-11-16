using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int interactions = 0;
    [SerializeField] protected Collider coll;
    void Awake()
    {
        coll = GetComponent<Collider>();
    }
    void OnCollisionEnter(Collision collision)
    {
        BreakBehaviour(collision);
        interactions++;
    }

    virtual public void BreakBehaviour(Collision collision){}
}
