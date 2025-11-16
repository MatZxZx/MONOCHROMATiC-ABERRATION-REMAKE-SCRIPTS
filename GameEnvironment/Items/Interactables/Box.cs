using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Breakable
{
    [SerializeField] private float breakLimit = 8;
    [SerializeField] private float breakImpulse = 800;
    public override void BreakBehaviour(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Player player = other.collider.GetComponent<Player>();
            if (player.currentSpeed >= breakLimit)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.mass = 0;
                float des = Mathf.Abs(player.Desacceleration - .25f);
                player.rb.AddForce(player.transform.forward * breakImpulse * des);
                gameObject.SetActive(false);
                SFXManager.PlayOneShot("Audio/SFX/Game/boxBreak");
                Debug.Log("LOL");
            }
        }
    }

}
