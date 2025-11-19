 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Zero : Player
{
    [Header("ZERO States")]
    [SerializeField] private bool canDeltaIncrement = false;
    [SerializeField] private bool canAnticipate = false;
    [SerializeField] private bool canDash = false;
    [SerializeField] private bool canStep = false;
    [SerializeField] private bool isWallJumping = false;

    [Header("ZERO Floats")]
    private float wallJumpForce;
    [SerializeField] private float wallJumpDegradation = 1.2f;
    [SerializeField] private float wallJumpTotalForce = 2000;
    [SerializeField] private float dashRange = 1000;
    [SerializeField] private float anticipationForce;
    [SerializeField] private float anticipationLimit = 80;
    [SerializeField] private int dashCount = 1;
    [SerializeField] private float stepDistance = 4;
    [SerializeField] private float stepCooldown = .25f;
    [SerializeField] private float dynamicSpeed;

    [Header("ZERO Events")]
    public UnityEvent OnDash;

    [Header("ZERO References")]
    [SerializeField] private Vector2 stepDirection;
    private Timer anticipationTimer = new Timer(1.75f);
    private Timer stepTimer = new Timer(1);
    private Timer dashTimer = new Timer(.4f);

    protected override void Init()
    {
        base.Init();
        wallJumpForce = wallJumpTotalForce;
    }

    void Update()
    {
        SetMotion();
        CharacterMoveset();

        if (IsGrounded()){ wallJumpForce = wallJumpTotalForce; }

        if (rb.velocity.y < -20 && rb.velocity.y > -40 && IsGrounded())
        {
            SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/landing");
        }
        else if (rb.velocity.y < -40 && IsGrounded())
        {
            SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/heavylanding");
        }

    }

    private void CharacterMoveset()
    {
        AdaptToSurface(IsGrounded());
        Anticipation();
        stepTimer.Run();
        dashTimer.Run();

        if (isCrashing)
        {
            StartCoroutine(WallKick());
        }

        if (IsGrounded())
        {
            dashCount = 1;
        }
    }

    protected override IEnumerator Braking()
    {
        SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/brake");
        return base.Braking();
    }

    protected override void SetRotation(float tractionMultiplier = 1)
    {
        base.SetRotation(tractionMultiplier);
    }   

    private void DoubleJump()
    {
        rb.AddForce(transform.up * lowJumpMultiplier, ForceMode.VelocityChange);
        SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/doubleJump");
    }

    private void Anticipation()
    {
        if (canAnticipate)
        {
            if (isIdle)
            {
                rb.velocity = Vector3.zero;
            }
            float counter = anticipationTimer.Run();
            Debug.Log(counter);
            anticipationForce = Mathf.Lerp(0, anticipationLimit, counter / 2);
        }
        else
        {
            anticipationTimer.Reset();
        }
    }
    private IEnumerator WallKick(float cooldown = 1)
    {
        rb.freezeRotation = false;
        Pirouette(transform.right, 100);
        rb.AddForce(((-transform.forward * 2) + transform.up) * 30, ForceMode.VelocityChange);
        SFXManager.PlayOneShot("Audio/SFX/Player/hit");
        Debug.Log("crashie jiji");
        yield return new WaitForSeconds(1);
        rb.freezeRotation = true;
    }
    private IEnumerator FastStepping()
    {
        // if (canStep)
        // {
        //     Vector3 direction = transform.position + (transform.right * stepDistance) * stepDirection.x;
        //     transform.position = Vector3.Lerp(transform.position, direction, 0.25f);
        //     if (transform.position == direction) {
        //         canStep = false;
        //     }
        // }
        Vector3 direction = transform.position + (mainCam.right * stepDistance) * stepDirection.x;
        Vector3 diff = Vector3.one * 0.1f;
        while (transform.position - diff != direction.normalized - diff)
        {
            transform.position = Vector3.Lerp(transform.position, direction, .5f);
            yield return null;
        }
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
    public void DeltaIncrement(InputAction.CallbackContext cc)
    {

        if (cc.performed && !isIdle)
        {
            Debug.Log("deltaincrement");
            canDeltaIncrement = true;
        }
        else if (cc.canceled)
        {
            canDeltaIncrement = false;
        }

    }
    public void Anticipation(InputAction.CallbackContext cc)
    {

        if (cc.performed)
        {
            Debug.Log("anticipation");
            canAnticipate = true;
        }
        else if (cc.canceled || canAnticipate)
        {
            canAnticipate = false;
            if(anticipationTimer.counter > 0.2f)
            {
                SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/anticipation");
                rb.velocity = transform.forward * anticipationForce;
            }
            //rb.AddForce(transform.forward * dashRange * 2);
        }

    }
    public void Dash(InputAction.CallbackContext cc)
    {

        if (cc.performed && !isFalling && dashCount > 0 && dashTimer.counter > 0.3) //Antes estaba !IsGrounded()
        {
            canDash = true;
            OnDash.Invoke();
            dashCount--;
            dashTimer.Reset();
            rb.AddForce(transform.forward * dashRange, ForceMode.VelocityChange);
            Motion.SetTrigger("Dash");
            SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/dash");
        }
        else if (cc.canceled)
        {
            canDash = false;
        }

    }
    public void FastStep(InputAction.CallbackContext cc)
    {

        if (cc.performed && stepTimer.counter >= stepCooldown)
        {
            stepDirection = cc.ReadValue<Vector2>().normalized;
            canStep = true;
            Pirouette(transform.forward, 100, (int)stepDirection.x);
            rb.AddForce((mainCam.right * dashRange) * stepDirection.x, ForceMode.Impulse);
            //transform.Translate(transform.position + mainCam.right);
            stepTimer.Reset();
            SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/fastStep");

        }
        else if (cc.canceled)
        {
            canStep = false;
        }

    }

    public void WallJump(InputAction.CallbackContext cc)
    {
        if (cc.started && OnWall() && !IsGrounded())
        {
            isWallJumping = true;
            rb.AddForce((WallDirection() + transform.up) * wallJumpForce);
            SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/doubleJump");
            rb.MoveRotation(Quaternion.LookRotation(WallDirection()));
            wallJumpForce /= wallJumpDegradation;
        }
        else
        {
            isWallJumping = false;
        }
    }

    public override void Jump(InputAction.CallbackContext cc)
    {
        base.Jump(cc);
        if (cc.started && IsGrounded()) { SFXManager.PlayOneShot("Audio/SFX/Player/ZERO/jump"); }
        if(cc.started && !IsGrounded() && jumpCount > 0)
        {
            DoubleJump();
        }
    }
    
}
