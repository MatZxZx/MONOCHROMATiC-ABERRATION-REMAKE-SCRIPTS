using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Player : MonoBehaviour
{
    /// <summary>
    ///  Este es el script principal del jugador, está diseñado para ser instanciado multiples veces sin tener conflictos.
    ///  Importante aclarar, este script está diseñado para un platformer 3D/2.5D basado en velocidad y fisicas.
    ///  La gran parte de propiedades son Protected. Esto es para poder usarse como una clase padre y poder hacer personajes con un Moveset especifico en base a este.
    ///  Las propiedades de a continuación todavía no se van a documentar en caso de agregar más. Por ahora solo voy a documentar los métodos.
    ///  También se considera que se usa el InputSystem de Unity.
    ///  Dicho eso, acá ta el script.
    /// </summary>
    public string selectedCharacter = "";
    [field: Header("Input")]
    [field: SerializeField] protected Vector2 input;
    [field: SerializeField] protected Vector2 smoothedInput;
    protected Vector3 totalVelocity;
    private Vector2 inputVelocity;
    [field: SerializeField] public PlayerInput playerInput { get; protected set; }
    [field: SerializeField] protected float inputSmoothTime;

    [field: Header("States")]
    [field: SerializeField] public bool isIdle { get; protected set; }
    [field: SerializeField] public bool isFalling { get; protected set; }
    [field: SerializeField] public bool isJumping { get; protected set; }
    [field: SerializeField] public bool isRunning { get; protected set; }
    [field: SerializeField] public bool isBraking { get; protected set; }
    [field: SerializeField] public bool isDrifting { get; protected set; }
    [field: SerializeField] public bool isCrouching { get; protected set; }
    [field: SerializeField] public bool isTaunting { get; protected set; }
    [field: SerializeField] public bool isStomping { get; protected set; }
    [field: SerializeField] public bool isCrashing { get; protected set; }

    [Header("Other States")]
    [field: SerializeField] protected bool freezeTraction = false;
    [field: SerializeField] protected bool limitVelocity = false;
    protected bool jumpTrigger = false;
    public bool ChangeDimension;

    [Header("FloatValues")]
    [field: SerializeField] protected Dictionary<Stat, float> baseStats = new Dictionary<Stat, float>();
    public float currentSpeed;
    public float currentVertical;
    [field: SerializeField] protected float moveSpeed = 20;
    [field: SerializeField] public float maxSpeed { get; protected set; }
    [field: SerializeField] protected float walkDrag = 3.5f;
    [field: SerializeField] protected float runMultiplier;
    [field: SerializeField] public float Desacceleration { get; protected set; }
    [field: SerializeField] protected float jumpForce = 8f;
    [field: SerializeField] protected float holdJumpForce = 4f;
    [field: SerializeField] protected float maxHoldTime = 0.2f;
    private float holdTimer = 0f;
    [field: SerializeField] private float longJumpForce = 800;
    [field: SerializeField] private float timeOnAir = 0;
    [field: SerializeField] protected float crouchMultiplier;
    [field: SerializeField] private float rayOffset = 0.8f, rayHeight = 0.8f;
    [field: SerializeField] private float jumpCounter;
    [field: SerializeField] protected int jumpCount = 4;
    [field: SerializeField] protected float gravityStrength = 10f;


    [Header("References")]
    public static Player Instance;
    public PlayerScore playerScore;
    public Rigidbody rb { get; protected set; }
    Quaternion baseRotation;
    public CapsuleCollider coll;
    public SplineContainer spline2D;
    public GameObject inputObj;
    public enum Stat { moveSpeed, maxSpeed, jumpForce, crouchMultiplier, height, drag, Desacceleration, jumpCount }
    [field: SerializeField] private LayerMask gndLayer;
    public CinemachineVirtualCameraBase cam;
    [field: SerializeField] protected Transform mainCam;
    [field: SerializeField]  public Animator Motion { get; protected set; }

    ////////////////////////////////////////////////////////////// UNITY FUNC. //////////////////////////////////////////////////////////////

    //Inicio del Script
    private void Awake()
    {
        Init();
    }

    //Bucle con el tiempo ajustado. 
    void FixedUpdate()
    {
        InputEnable();
    }

    ////////////////////////////////////////////////////////////// SETS //////////////////////////////////////////////////////////////

    //Define y busca las referencias de lo que haya dentro del objeto, en sus hijos o fuera de este.
    //TODO: Todo debería ser parte del mismo objeto
    protected virtual void Init()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name.ToLower() == "input")
            {
                inputObj = transform.GetChild(i).gameObject;
            }
            if(transform.GetChild(i).gameObject.name.ToLower() == "player_mc")
            {
                mainCam = transform.GetChild(i).gameObject.transform;
            }
            if(transform.GetChild(i).gameObject.name.ToLower() == "player_flc")
            {
                cam = transform.GetChild(i).GetComponent<CinemachineVirtualCameraBase>();
            }
        }

        Instance = this;
        playerScore = GetComponent<PlayerScore>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();
        Motion = transform.GetChild(0).GetComponentInChildren<Animator>();
        baseRotation = Quaternion.identity;

        InitStats();

    }

    //Estas son estadisticas que se guardan al principio para luego llamarlas.
    private void InitStats()
    {
        baseStats.Add(Stat.moveSpeed, moveSpeed);
        baseStats.Add(Stat.maxSpeed, maxSpeed);
        baseStats.Add(Stat.jumpForce, jumpForce);
        baseStats.Add(Stat.crouchMultiplier, crouchMultiplier);
        baseStats.Add(Stat.height, coll.height);
        baseStats.Add(Stat.drag, rb.drag);
        baseStats.Add(Stat.Desacceleration, Desacceleration);
        baseStats.Add(Stat.jumpCount, jumpCount);
    }

    //Esto activa o desactiva los inputs y movilidad del jugador dependiendo si un objeto está activo o no. 
    //Muy útil para casos donde se requiera que el jugador esté quieto. Ej: Una Cinematica.
    //Es importante aclarar que esto se ejecuta constantemente en el FixedUpdate, y en consecuencia, los métodos que hay dentro de él y los métodos que se llaman en esos métodos. (lindo trabalenguas eh)
    private void InputEnable()
    {
        if (inputObj.activeSelf)
        {
            playerInput.enabled = true;
            SetInput();
            SetLocomotion();
        }
        else
        {
            playerInput.enabled = false;
        }
    }

    //Esto consigue los inputs crudos desde el ActionMap del InputSystem para luego suavizarlos, todo esto dado por un valor para la cantidad de suavidad.
    private void SetInput()
    {
        input = playerInput.actions[playerInput.currentActionMap.actions[0].name].ReadValue<Vector2>();
        smoothedInput = Vector2.SmoothDamp(smoothedInput, input.normalized, ref inputVelocity, inputSmoothTime);
    }
    //Esto guarda el último input del jugador. Solo lo hace cuando cualquiera de los inputs no valen 0.
    private Vector2 LastInput()
    {
        Vector2 last = new Vector2();
        if (input.y != 0)
        {
            last.y = input.y;
        }
        if (input.x != 0)
        {
            last.x = input.x;
        }
        return last;
    }

    //Esto consigue el ActionMap donde están las asignaciones de teclas. En este caso solo busca las del ActionMap Base y lo define como el ActionMap actual.
    private void SetLocomotion()
    {
        //Una importante aclaración, esta condición solo se ejecuta en un frame (una vez).
        if (playerInput.currentActionMap.name != "Base")
        {
            SetPhysics();
            playerInput.SwitchCurrentActionMap("Base");
        }

        MoveSet();
        ReturnToBaseStats();
        SetStates();

    }

    //Cambia los valores del componente Rigidbody. Tiene valores por defecto, pero también se pueden cambiar.
    public void SetPhysics(float mass = 1, float drag = 1, float angularDrag = 0.05f, float forceSpeed = 20, float maxForceSpeed = 20)
    {
        rb.mass = mass;
        rb.angularDrag = angularDrag;
        rb.drag = drag;
        moveSpeed = forceSpeed;
        maxSpeed = maxForceSpeed;
    }

    //Esto cambia los valores de los atributos importantes que tengan que ver con la movilidad.
    //Nota: Este método hasta ahora no se está usando.
    public void SetStats(float _moveSpeed = 20, float _maxSpeed = 5, float _fallMultiplier = 60, float _lowJumpMultiplier = 40, float _crouchMultiplier = 2, float _height = 2, float _drag = 1)
    {
        moveSpeed = _moveSpeed;
        maxSpeed = _maxSpeed;
        jumpForce = _lowJumpMultiplier;
        crouchMultiplier = _crouchMultiplier;
        coll.height = _height;
        rb.drag = _drag;
    }

    //Esto define los estados dada una condición.
    //Nota: Es probable que esto se quite o se cambie. Razón: No veo viable llenar todo de ifs.
    private void SetStates()
    {
        if (currentSpeed < 1)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }
    }

    /////////////////////////////////////////////////////////////// STATES ///////////////////////////////////////////////////////////////
    
    //Esto define un estado que le hace saber al juego si el jugador está tocando el suelo. 
    //Hay bastantes formas de hacerlo. Este método usa el enfoque de usar multiples Raycasts para una detección más refinada.
    //En este caso usa 4 Raycasts que se posicionan en las 4 esquinas del jugador mirando hacia abajo.
    //Si hay algún raycast detecta un objeto con la capa de "Ground", devuelve true. De lo contrario, devuelve false.
    protected bool IsGrounded()
    {

        float rayLength = baseStats[Stat.height] / 2 + rayHeight;
        bool grounded = false;
        jumpCounter += 1 * Time.deltaTime;

        Vector3[] raycastOrigins = new Vector3[]
        {
            transform.position + transform.forward * rayOffset,
            transform.position - transform.forward * rayOffset,
            transform.position + transform.right * rayOffset,
            transform.position - transform.right * rayOffset
        };

        foreach (Vector3 origin in raycastOrigins)
        {
            //RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.down, rayLength, gndLayer);
            if (Physics.Raycast(origin, Vector3.down, rayLength, gndLayer))
            {
                grounded = true;
                jumpCounter = 0;
                jumpCount = (int)baseStats[Stat.jumpCount];
                Debug.DrawRay(origin, Vector3.down * rayLength, Color.green);
            }

        }

        return grounded;
    }

    //Esto le hace saber al juego si el jugador está pegado a una pared o si chocó con una.
    protected bool OnWall()
    {
        bool wall;

        if (WallDirection() != Vector3.zero)
        {
            wall = true;

        }
        else
        {
            wall = false;
        }
        return wall;
    }
    
    ////////////////////////////////////////////////////////////// MOVEMENT //////////////////////////////////////////////////////////////

    //Acá van los movimientos básicos del jugador que requieran actualizarse constantemente. 
    private void MoveSet()
    {
        Movement(ChangeDimension);
        AerealMovement();
        //Jumping(IsGrounded() && isJumping);
        Drift();
        Crash();
        //Brake();
    }

    //Acá van los distintos tipos de movimientos que tiene el jugador. En este caso, el 3D y el 2D.
    //Dado un booleano, se puede cambiar dinámicamente el modo en que se traducen los inputs a fuerzas fisicas.
    private void Movement(bool is2d)
    {
        Draging();
        if (!is2d)
        {
            Movement3D();
        }
        else
        {
            Movement2D();
        }

        if(rb.velocity.y < 0)
        {
            rb.AddForce(0,-gravityStrength,0, ForceMode.Acceleration);
        }
        coll.material.bounciness = Mathf.Lerp(0,1, totalVelocity.y / maxSpeed);

    }
    //Esto se encarga de crear y traducir los inputs del jugador a fuerzas fisicas de manera tridimensional (X, Y, Z). 
    private void Movement3D()
    {
        Vector3 move = new Vector3(input.x, 0, input.y) * moveSpeed;

        move = SetCamera3D(move);
        SetRotation();

        totalVelocity = new Vector3(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y), Mathf.Abs(rb.velocity.z));

        SpeedStatus();
        LimitVelocity();

        rb.AddForce(move);
    }

    //Esto se encarga de crear y traducir los inputs del jugador a fuerzas fisicas de manera bidimensional (X, Y). 
    //Esta basado en el método Movement3D.
    private void Movement2D()
    {
        Vector3 move = new Vector3(input.x, 0, input.y) * moveSpeed;

        move = SetCamera2D(move);
        SetRotation(1.75f);

        totalVelocity = new Vector3(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y), Mathf.Abs(rb.velocity.z));

        SpeedStatus();
        LimitVelocity();

        SplineHandler(move);

        //rb.AddForce(move);
    }

    private void SplineHandler(Vector3 pos)
    {
        // 1) Encontrar t más cercano (usa la variante que prefieras)
        float nearestT = FindNearestT_Refined(coarseSamples: 48, refineSteps: 8, refineRange: 1f / 40f);

        // 2) Posición y tangente
        Vector3 closestPoint = spline2D.EvaluatePosition(nearestT);
        Vector3 tangent = spline2D.EvaluateTangent(nearestT);

        Vector3 dir = new Vector3(tangent.x, 0f, tangent.z).normalized;

        // 3) Rotación suave en Y
        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
        //transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 20).eulerAngles.y, transform.rotation.eulerAngles.z)); 

        // 4) Fuerza correctiva lateral (solo en XZ)
        Vector3 desiredPos = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        Vector3 offset = desiredPos - transform.position;
        rb.AddForce(offset * 20, ForceMode.Acceleration);
        rb.AddForce(pos);

        // Opcional: Si querés avanzar el 't' por input en vez de depender solo de posición,
        // podés combinar nearestT con la velocidad deseada para estimar t += deltaT...

    }

    private float FindNearestT_Refined(int coarseSamples = 40, int refineSteps = 8, float refineRange = 1f/40f)
    {
        // Coarse sample
        float bestT = 0f;
        float bestSq = float.MaxValue;
        for (int i = 0; i <= coarseSamples; i++)
        {
            float tt = i / (float)coarseSamples;
            Vector3 p = spline2D.EvaluatePosition(tt);
            float sq = (p - transform.position).sqrMagnitude;
            if (sq < bestSq) { bestSq = sq; bestT = tt; }
        }

        // Refinement: do a few local ternary searches around bestT
        float left = Mathf.Max(0f, bestT - refineRange);
        float right = Mathf.Min(1f, bestT + refineRange);

        for (int step = 0; step < refineSteps; step++)
        {
            float t1 = left + (right - left) / 3f;
            float t2 = right - (right - left) / 3f;
            float s1 = ((Vector3)spline2D.EvaluatePosition(t1) - transform.position).sqrMagnitude;
            float s2 = ((Vector3)spline2D.EvaluatePosition(t2) - transform.position).sqrMagnitude;
            if (s1 > s2) left = t1;
            else right = t2;
        }

        return (left + right) * 0.5f;
    }

    //Esto devuelve la dirección combinando el input del jugador con la dirección hacia donde mira la cámara.
    //Con este enfoque el jugador puede moverse libremente en el mundo teniendo a la camara por detrás.
    public Vector3 SetCamera3D(Vector3 pos)
    {
        Transform mainCam = Camera.main.transform;
        Vector3 diff = mainCam.transform.position - transform.position;
        Vector3 direction = diff.normalized;
        direction.y = 0;
        pos = -direction * pos.z + mainCam.right * pos.x;
        return pos;
    }

    //Esto devuelve la dirección combinando el input del jugador con la dirección hacia donde mira la cámara.
    //En este caso hace exactamente lo mismo que el SetCamera3D, pero anulando el eje Z para poder moverse solo a la izquierda y derecha.
    private Vector3 SetCamera2D(Vector3 pos)
    {
        //pos.z = 0;
        pos = SetCamera3D(pos);

        return pos;
    }


    //Esto define las propiedades que tengan que ver con velocidad, tanto horizontal; como vertical.
    //Los valores son absolutos.
    private void SpeedStatus()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        float speed = horizontalVelocity.magnitude;

        currentVertical = rb.velocity.y;
        currentSpeed = speed;
    }

    //Acá van acciones que ocurren cuando el jugador está en el aire.
    private void AerealMovement()
    {
        ResetRotation();
        if (!IsGrounded() && rb.velocity.y < -2)
        {
            isFalling = true;
            timeOnAir += 1 * Time.deltaTime;
            //rb.drag = Mathf.Lerp(baseStats[Stat.drag],0, rb.velocity.y / -10);
        }
        else
        {
            timeOnAir = 0;
            isFalling = false;
        }
    }

    //Esto se encarga del salto que hace el jugador. Mientras más tiempo se presione el botón, más alto saltará. 
    //Solo hace esto cuando se le pasa un booleano que le dé la primera señal para poder saltar. En este caso, IsGrounded.
    //La segunda señal se la da el evento del InputSystem.
    private void Jumping(bool canJump)
    {
        if (canJump && isJumping)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    //Similar al Jumping, solo que esta vez la señal es cuando el jugador está derrapando y está en el suelo.
    private void LongJump()
    {
        if (isDrifting && IsGrounded())
        {
            Vector3 direction = transform.forward + transform.up;
            rb.AddForce(direction * longJumpForce);
        }
    }

    //Esto se encarga de KMNBASHNJBDAKJLDSALKJHSABKLJSDKBLJSABKLJHSLKHBSASDABKHDSABOYIDSAYOBIDSAIOY YA ME CANSÉ DE DOCUMENTAR DESPUÉS LO HAGO XDDDDD
    private void Drift()
    {
        if (IsGrounded() && isRunning && isCrouching)
        {
            isDrifting = true;
            //IncreaseDrag(4);
        }
    }

    private void Crash()
    {
        if(OnWall() && currentSpeed > 40)
        {    
            isCrashing = true;
        }
        else
        {
            isCrashing = false;
        }
    }

    public void Lean(Vector2 i)
    {
        StartCoroutine(Pirouette(transform.forward * i.normalized.x, 10, (int)i.normalized.x, 0.1f));
    }

    public IEnumerator Pirouette(Vector3 direction, float force, int orientation = -1, float waitingTime = 1)
    {
        
        rb.freezeRotation = false;
        rb.AddTorque(orientation * direction * force);
        yield return new WaitForSeconds(waitingTime);
        rb.freezeRotation = true;
    }
    private void Draging()
    {
        if (!isRunning && IsGrounded())
        {
            rb.drag = Mathf.Lerp(baseStats[Stat.drag] * walkDrag, baseStats[Stat.drag], currentSpeed / (maxSpeed / 2));
        }
        else
        {
            rb.drag = baseStats[Stat.drag];
        }
    }

    private void Brake()
    {
        bool doingNothing = input == Vector2.zero && LastInput() == Vector2.zero;
        if (-input == LastInput() && totalVelocity.magnitude > 20)
        {
            StartCoroutine(Braking());
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }

    }
    
    protected virtual IEnumerator Braking()
    {
        freezeTraction = true;
        IncreaseDrag(10);
        yield return new WaitUntil(() => !isBraking);
        freezeTraction = false;
        rb.drag = baseStats[Stat.drag];
    }

    private float Traction(bool freeze = false)
    {
        if (!freeze)
        {
            return Mathf.Lerp(10, 1, currentSpeed / maxSpeed);
        }
        else
        {
            return 0;
        }
    }
    private void IncreaseDrag(float factor)
    {
        rb.drag += factor * Time.deltaTime;
    }

    private void LimitVelocity()
    {
        Desacceleration = Mathf.Lerp(0, 1, currentSpeed / maxSpeed * runMultiplier);
        float desaccel = Desacceleration * 10 * Time.deltaTime;
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
    }

    private void ReturnToBaseStats()
    {

        if (!isCrouching)
        {
            coll.height = baseStats[Stat.height];
            coll.center = Vector3.zero;
        }
        //if (!isDrifting)
        //{
        //rb.drag = baseStats[Stat.drag];
        //}
    }

    ////////////////////////////////////////////////////////////// INPUT EVENTS //////////////////////////////////////////////////////////////
    
    //Todos estos se tienen que referenciar en el componente PlayerInput en el inspector.
    //Se les pueden hacer override en caso de que algún personaje quiera hacer lo mismo pero con alguna distinción.
    virtual public void Move(InputAction.CallbackContext cc)
    {
        if (cc.performed)
        {
            Lean(input);
        }
    }
    virtual public void CameraMove(InputAction.CallbackContext cc){}
    virtual public void CameraZoom(InputAction.CallbackContext cc)
    {
        if (cc.performed)
        {
            Vector2 callbackInput = cc.ReadValue<Vector2>().normalized;
            if (callbackInput == Vector2.up)
            {
                cam.GetComponent<CameraManager>().ZoomIn();
            }
            else if (callbackInput == Vector2.down)
            {
                cam.GetComponent<CameraManager>().ZoomOut();
            }
        }
    }

    virtual public void Jump(InputAction.CallbackContext cc)
    {
        if (cc.started && IsGrounded() && !isJumping && jumpCount > 0)
        {
            jumpCount--;
            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            LongJump();
            isJumping = true;
        }
        else if (cc.canceled || cc.performed)
        {
            isJumping = false;
        }
    }

    virtual public void Run(InputAction.CallbackContext cc)
    {
        if (cc.performed && !isRunning && !isIdle)
        {
            isRunning = true;
            maxSpeed = maxSpeed * runMultiplier;
            moveSpeed = moveSpeed * runMultiplier;
        }
        else if (cc.canceled)
        {
            isRunning = false;
            maxSpeed = baseStats[Stat.maxSpeed];
            moveSpeed = baseStats[Stat.moveSpeed];
        }
    }
    virtual public void Crouch(InputAction.CallbackContext cc)
    {
        if (cc.performed && !isCrouching)
        {
            isCrouching = true;
            coll.height = coll.height / crouchMultiplier;
            coll.center = new Vector3(0, -0.5f, 0);
            maxSpeed = maxSpeed / runMultiplier;
            moveSpeed = moveSpeed / runMultiplier;
            if (isRunning)
            {
                rb.AddForce(transform.up * 150);
            }
            if (!IsGrounded())
            {
                isStomping = true;
            }
        }
        else if (cc.canceled)
        {
            isCrouching = false;
            isDrifting = false;
            isStomping = false;
            //rb.drag = baseStats[Stat.drag];
            maxSpeed = baseStats[Stat.maxSpeed];
            moveSpeed = baseStats[Stat.moveSpeed];
        }
    }

    //Actualmente sin uso.
    virtual public void Taunt(InputAction.CallbackContext cc)
    {
        if (cc.performed && !isTaunting)
        {
            isTaunting = true;
        }
        else if (cc.canceled)
        {
            isTaunting = false;
        }
    }

    ////////////////////////////////////////////////////////////// MISC //////////////////////////////////////////////////////////////

    //Esto define una rotación hacia donde debe mirar el objeto del jugador.
    //Créditos del código original: samyam
    
    public void SwitchOnCamera()
    {
        
    }
    protected virtual void SetRotation(float tractionMultiplier = 1)
    {
        float totalTraction;
        if(IsGrounded())
        { totalTraction = Traction(freezeTraction); }
        else{ totalTraction = Traction(freezeTraction) / 2;}

        if (input != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + mainCam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * totalTraction * tractionMultiplier);
        }
    }

    //Esto reinicia la rotación del jugador a 0 de manera suave. Dependiendo de un factor, rotará más lento o más rápido.
    private void ResetRotation()
    {

        if (isFalling)
        {
            float factor = .05f;
            float x = Mathf.LerpAngle(transform.eulerAngles.x, baseRotation.eulerAngles.x, factor);
            float z = Mathf.LerpAngle(transform.eulerAngles.z, baseRotation.eulerAngles.z, factor);

            transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, z);
            //transform.rotation.eulerAngles = Quaternion.Lerp(transform.rotation, baseRotation, .25f);
        }

    }

    //Esto devuelve la dirección hacia donde mira la pared que el jugador chocó.
    //Usa un enfoque parecido al IsGrounded, solo que esta vez usa un Raycast y está ubicado en la cabeza del jugador mirando hacia adelante.
    //Si el raycast detecta un objeto con la capa de "Ground", devuelve la dirección hacia donde mira la pared.
    
    protected Vector3 WallDirection()
    {
        float rayLength = 1.5f;

        Vector3 origin = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(origin, transform.forward, out hit, rayLength, gndLayer))
        {
            Debug.DrawRay(origin, transform.forward * rayLength, Color.green);

            return hit.normal;
        }
        else
        {
            return Vector3.zero;
        }
    }

    protected void AdaptToSurface(bool grounded, float adaptivity = 8)
    {
        float rayLength = 1.5f;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayLength) && grounded)
        {
            // Dirección actual de avance (mantiene la rotación Y)
            Vector3 forward = transform.forward;

            // Proyectar el forward sobre el plano del suelo para evitar resets de rotación
            Vector3 projectedForward = Vector3.ProjectOnPlane(forward, hit.normal).normalized;

            // Construir la nueva rotación:
            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, hit.normal);

            // Interpolación suave
            Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, adaptivity * Time.deltaTime);

            rb.MoveRotation(newRotation);
        }
    }

    // protected void AdaptToSurface(bool grounded, float speedLimit = 10, float adaptivity = 15)
    // {
    //     float rayLength = 1.5f;

    //     if (Physics.Raycast(transform.position, -currentGravity.normalized, out RaycastHit hit, rayLength) && grounded && currentSpeed > speedLimit)
    //     {
    //         Vector3 surfaceNormal = hit.normal;

    //         // 1. Calcular forward basado en la dirección de movimiento
    //         Vector3 velocity = rb.velocity;

    //         // Si estás casi frenado, usá el forward actual
    //         Vector3 forward = velocity.magnitude > 0.1f ?
    //                         velocity.normalized :
    //                         transform.forward;

    //         // 2. Proyectar el forward sobre el plano de la superficie
    //         forward = Vector3.ProjectOnPlane(forward, surfaceNormal).normalized;

    //         if (forward.sqrMagnitude < 0.01f)
    //             forward = Vector3.Cross(transform.right, surfaceNormal).normalized;

    //         // 3. Construir la nueva rotación basada en forward + normal
    //         Quaternion targetRot = Quaternion.LookRotation(forward, surfaceNormal);

    //         // 4. Interpolar suavemente
    //         Quaternion newRot = Quaternion.Slerp(rb.rotation, targetRot, rotationAlignSpeed * Time.deltaTime);

    //         rb.MoveRotation(newRot);

    //         // 5. Actualizar la gravedad para alinearla a la superficie
    //         currentGravity = -surfaceNormal * gravityStrength;
    //     }
    //     else
    //     {
    //         // Si no hay suelo, volvés a gravedad normal
    //         currentGravity = Vector3.down * gravityStrength;
    //     }

    //     // Aplicar la gravedad dinámica
    //     rb.AddForce(currentGravity, ForceMode.Acceleration);
    // }

}

