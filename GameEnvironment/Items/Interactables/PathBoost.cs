using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PathBoost : Item
{
    [SerializeField] private SplineAnimate splineAnimate;
    [SerializeField] private Transform splinePosition;
    private Player player;
    [SerializeField] private bool isOn;
    [SerializeField] private float speed = 50;
    [SerializeField] private float followSpeed  = 25;
    

    void Awake()
    {
        
    }
    void Update()
    {
        if (isOn)
        {
            player.rb.velocity = player.transform.forward * speed;
            Vector3 followPosition = new Vector3(splinePosition.position.x, player.transform.position.y, splinePosition.position.z);
            player.transform.position = Vector3.Lerp(player.transform.position, followPosition, followSpeed * Time.deltaTime);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, splinePosition.rotation, followSpeed * Time.deltaTime);
            player.Motion.SetBool("isGrounded", true);

            if (splineAnimate.ElapsedTime >= splineAnimate.Duration)
            {
                player.rb.AddForce(splinePosition.forward * 500);
                isOn = false;
                splineAnimate.Restart(false);
            }
        }
    }

    public override void ItemBehaviour(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            isOn = true;
            splineAnimate.Play();
            splineAnimate.MaxSpeed = speed;
            SFXManager.PlayOneShot("Audio/SFX/Game/path");

        }
    }

}
