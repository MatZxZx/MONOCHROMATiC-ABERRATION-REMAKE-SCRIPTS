using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : Breakable
{
    [SerializeField] private float breakLimit = 20;
    [SerializeField] private float breakImpulse = 800;
    public override void BreakBehaviour(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            Player player = other.collider.GetComponent<Player>();
            //float des = Mathf.Abs(player.Desacceleration - .25f);
            player.rb.AddForce(player.transform.forward + (player.transform.up * 3) * breakImpulse, ForceMode.VelocityChange);
            SFXManager.PlayOneShot("Audio/SFX/Game/balloon");
            gameObject.SetActive(false);
        
        }
    }
}
