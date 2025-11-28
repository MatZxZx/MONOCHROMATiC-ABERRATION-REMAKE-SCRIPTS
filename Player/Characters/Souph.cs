using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Souph : Player
{
    [Header("SOUPH States")]
    [SerializeField] private bool bikeEnabled = false;
    [SerializeField] private bool canCrush = false;
    [SerializeField] private bool canJet = false;

    //[Header("SOUPH Floats")]

    [Header("SOUPH Events")]
    public UnityEvent OnVehicle;
    public UnityEvent OnExplode;

    //[Header("SOUPH References")]

    // Update is called once per frame
    void Update()
    {
        SetMotion();
        CharacterMoveset();

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
    private void CharacterMoveset()
    {
        AdaptToSurface(IsGrounded(), 10);
    }

    public void JetPack(InputAction.CallbackContext cc)
    {

        if (cc.started && !IsGrounded())
        {
            canJet = true;
            OnVehicle.Invoke();
            Debug.Log("jettt");
        }
        else if (cc.performed || isCrashing)
        {
            OnExplode.Invoke();
        }
        else if (cc.canceled)
        {
            canJet = false;
        }


    }

    public void Crusher(InputAction.CallbackContext cc)
    {

        if (cc.performed)
        {
            canCrush = true;
            OnVehicle.Invoke();
            Debug.Log("CRUSHHH");
        }
        else if (cc.canceled)
        {
            canCrush = false;
        }


    }
    public void BikeSpawn(InputAction.CallbackContext cc)
    {

        if (cc.performed)
        {
            bikeEnabled = !bikeEnabled;
            OnVehicle.Invoke();
            Debug.Log("moto en " + bikeEnabled);
        }

    }

    public void BikeAccelerate(InputAction.CallbackContext cc)
    {

        if (cc.performed)
        {
            Debug.Log("aselero wei");
        }

    }

    public override void Jump(InputAction.CallbackContext cc)
    {
        base.Jump(cc);
        if (cc.started && IsGrounded()) { SFXManager.PlayOneShot("Audio/SFX/Player/SOUPH/jump"); }
    }
    
}
