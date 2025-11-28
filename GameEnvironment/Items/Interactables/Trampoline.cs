using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : Item
{
    [SerializeField] float power = 500;
    public override void ItemCollisionBehaviour(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Player player = collision.transform.GetComponent<Player>();
            if(Mathf.Abs(player.currentVertical) > 8)
            {
                 player.transform.position = Vector3.Lerp(player.transform.position, transform.position, .25f);
                player.transform.rotation = transform.localRotation;
                float verticalLimit = Mathf.Lerp(10, 60, Mathf.Abs(player.currentVertical) / 100);
                player.rb.AddForce(transform.up * (power + verticalLimit), ForceMode.Impulse);
                player.Motion.SetTrigger("Spin");
                SFXManager.PlayOneShot("Audio/SFX/Game/bump1");

            }

        }
    }
}
