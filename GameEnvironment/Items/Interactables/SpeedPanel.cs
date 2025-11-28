using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPanel : Item
{

    [SerializeField] protected float force;
    [SerializeField] protected bool passThrough = false;
    [SerializeField] protected Quaternion orientation;

    void Awake()
    {
        motion = GetComponentInChildren<Animator>();
    
        if (passThrough)
        {
            orientation = Quaternion.Euler(transform.eulerAngles - transform.forward);
        }
        else
        {
            orientation = transform.rotation;
        }
    }
    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.transform.rotation = orientation;
            player.rb.velocity = Vector3.zero + player.transform.forward * player.currentSpeed;
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position, .2f);
            //float des = Mathf.Abs(player.Desacceleration - .5f);
            player.rb.AddForce(transform.forward * force); // * des
            SFXManager.PlayOneShot("Audio/SFX/Game/booster");
            motion.SetTrigger("Boost");
        }
    }

}
