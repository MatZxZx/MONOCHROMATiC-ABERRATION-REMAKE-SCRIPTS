using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arc : Item
{
    [SerializeField] int quantity = 1;
    [SerializeField] int points = 100;
    [SerializeField] SpriteRenderer[] extraArcs;

    void Awake()
    {
        motion = GetComponent<Animator>();
    }
    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = Color.green;
            PassColor();
            ArcManager.Instance.Add(quantity, points);
            Collider coll = GetComponent<Collider>();
            coll.enabled = false;
            motion.SetTrigger("Pass");
            SFXManager.PlayOneShot("Audio/SFX/Game/arc");
        }
    }

    private void PassColor()
    {
        foreach (SpriteRenderer sr in extraArcs)
        {
            sr.color = Color.green;
        }
    }
}
