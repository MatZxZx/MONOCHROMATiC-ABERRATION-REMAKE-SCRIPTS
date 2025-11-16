using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cord : Item
{

    [SerializeField] private Transform[] points;
    [SerializeField] private Transform grip;
    private Player player;
    [SerializeField] private bool sling = false;
    [SerializeField] private float speed;

    void FixedUpdate()
    {
        CordSling(player);
    }

    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            SFXManager.PlayOneShot("Audio/SFX/Game/cord");
            sling = true;
        }
    }

    private void CordSling(Player player)
    {

        if (sling)
        {
            grip.position = Vector3.Lerp(grip.position, points[1].position, speed);
            player.transform.position = grip.position;

            if (Mathf.RoundToInt(grip.position.y) == Mathf.RoundToInt(points[1].position.y))
            {
                sling = false;
                grip.position = points[0].position;
                player.rb.AddForce(transform.up * 1000);
            }

        }

    }

}
