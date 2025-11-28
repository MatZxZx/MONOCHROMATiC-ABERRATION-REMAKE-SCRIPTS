using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jig : Item
{
    [SerializeField] int quantity = 1;
    [SerializeField] int points = 100;

    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            sr.enabled = false;
            JigManager.Instance.Add(quantity, points);
            player.playerScore.Add(quantity, points);
            //UIEffects.Instance.Bump("jig_icon");
            Collider coll = GetComponent<Collider>();
            coll.enabled = false;
            SFXManager.PlayOneShot("Audio/SFX/Game/jig");
            DisableItem(gameObject, 1);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        ItemBehaviour(collision.collider);
    }
}
