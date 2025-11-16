using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Souph : Player
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetMotion();

    }

     private void SetMotion()
    {
        Motion.SetFloat("horizontalVelocity", currentSpeed);
        Motion.SetFloat("verticalVelocity", currentVertical);
        Motion.SetBool("isGrounded", IsGrounded());
        Motion.SetBool("isIdle", isIdle);
        Motion.SetBool("isRunning", isRunning);
        Motion.SetBool("isJumping", isJumping);
        Motion.SetBool("isCrouching", isCrouching);
    }
}
