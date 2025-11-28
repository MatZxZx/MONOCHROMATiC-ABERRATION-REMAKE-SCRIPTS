using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Item : MonoBehaviour
{
    public int interactions = 0;
    public UnityEvent OnInteract;
    [SerializeField] protected Animator motion;
    
    void OnTriggerEnter(Collider other)
    {
        ItemBehaviour(other);
        interactions++;
        if (OnInteract.GetPersistentEventCount() != 0)
        {
            OnInteract.Invoke();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        ItemCollisionBehaviour(collision);
        interactions++;
        if (OnInteract.GetPersistentEventCount() != 0)
        {
            OnInteract.Invoke();
        }
    }

    virtual public void ItemBehaviour(Collider other) { }
    virtual public void ItemCollisionBehaviour(Collision collision) { }
    virtual public void DisableItem(GameObject obj, float delay = 0)
    {
        StartCoroutine(Disable(obj, delay));
    }
    IEnumerator Disable(GameObject _obj, float time)
    {
        yield return new WaitForSeconds(time);
        _obj.SetActive(false);
    }
}

