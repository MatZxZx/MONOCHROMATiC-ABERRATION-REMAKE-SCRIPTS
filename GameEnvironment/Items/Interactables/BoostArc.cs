using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostArc : SpeedPanel
{

    [SerializeField] bool withDesacceleration = true;
    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.transform.rotation = transform.localRotation;
            player.rb.velocity = Vector3.zero + player.transform.forward * player.currentSpeed;
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position, .75f);

            if (withDesacceleration)
            {
                float des = Mathf.Abs(player.Desacceleration - .25f);
                player.rb.AddForce(transform.forward * force * des);
            }
            else
            {
                player.rb.AddForce(transform.forward * force);
            }
            motion.SetTrigger("Pass");
            player.Motion.SetTrigger("Dash");
            SFXManager.PlayOneShot("Audio/SFX/Game/arcBooster");
        }
    }
}
