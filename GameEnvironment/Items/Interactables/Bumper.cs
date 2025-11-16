using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bumper : Item
{
    [SerializeField] float power = 500;

    void Awake()
    {
        motion = GetComponentInChildren<Animator>();
    }

    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position, .5f);
            player.rb.velocity = Vector3.zero;
            player.transform.rotation = transform.localRotation;
            player.rb.AddForce(transform.up * power);
            motion.SetTrigger("Bump");
            player.Motion.SetTrigger("Spin");
            SFXManager.PlayOneShot("Audio/SFX/Game/bumper");

        }
        // else if (other.CompareTag("Item"))
        // {
        //     Rigidbody rb = other.AddComponent<Rigidbody>();
        //     rb.AddForce(transform.up * power);
        // }
    }

    [ContextMenu("Draw Ray")]
    public void OnDrawGizmos() {
        Gizmos.DrawRay(transform.position, transform.up * 50);   
    }

}
