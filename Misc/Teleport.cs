using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{

    public Transform spawn;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.transform.position = spawn.position;
            other.attachedRigidbody.velocity = Vector3.zero;
        }
    }
}
