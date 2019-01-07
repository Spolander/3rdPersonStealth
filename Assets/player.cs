using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour {


    protected Animator anim;
    private Animator handsAnimator;
    // Use this for initialization

    protected Vector3 animVelocity;




    protected float airTimer = 0;

    protected CharacterController controller;

    [SerializeField]
    protected float moveSpeedMultiplier = 5f;

    protected Vector3 moveVector;
    protected Vector3 limitVector;

    [SerializeField]
    protected LayerMask groundLayers;

    public LayerMask GroundLayers { get { return groundLayers; }set { groundLayers = value; } }

    [SerializeField]
    protected float gravityAcceleration = 25;


    protected Vector3 lookPoint;

    protected float lookAtWeight;

    protected bool running = false;
    protected bool canMove = true;
    private float runSpeedMultiplier = 1f;
    private float maxRunSpeedMultiplier = 2;

    protected bool grounded = false;
    protected float lastGroundedTime;

    protected bool isJumping = false;
    protected float gravity;

    [SerializeField]
    protected float jumpingForce = 5;

    [SerializeField]
    protected float jumpingDuration = 0.2f;

    protected Vector3 groundNormal;
    protected Vector3 slopeNormal;

    protected float lastJumpTime;

    protected float groundedLossTime = 0.1f;

    protected MyInputManager input;

    protected Camera myCamera;

    public int team = 1;

    public virtual Vector3 Velocity { get { return controller.velocity; } }
    Vector2 inputVector;

    firstPersonCamera headCamera;

    protected virtual void Awake () {
        headCamera = GetComponentsInChildren<firstPersonCamera>()[1];
        myCamera = GetComponentInChildren<Camera>();
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        handsAnimator = GetComponentsInChildren<Animator>()[1];
        input = GetComponent<MyInputManager>();
	}
    
    // Update is called once per frame
    protected virtual void Update () {


        inputVector = input.inputVector;
        checkGrounded();
        anim.SetBool("grounded", grounded);

        airTimer -= Time.deltaTime;

        print(transform.InverseTransformDirection(controller.velocity).z / (moveSpeedMultiplier * maxRunSpeedMultiplier));


        anim.SetFloat("Forward", transform.InverseTransformDirection(controller.velocity).z/(moveSpeedMultiplier*maxRunSpeedMultiplier), 0.1f, Time.deltaTime);
        anim.SetFloat("Horizontal", inputVector.x, 0.1f,Time.deltaTime);
        handsAnimator.SetFloat("Forward", anim.GetFloat("Forward"));
        


        if(canMove)
        Move();

        if (running)
            runSpeedMultiplier = Mathf.MoveTowards(runSpeedMultiplier, maxRunSpeedMultiplier, Time.deltaTime*3);
        else
            runSpeedMultiplier = Mathf.MoveTowards(runSpeedMultiplier, 1f, Time.deltaTime*3);

        running = Input.GetKey(KeyCode.LeftShift);

        if (headCamera)
            headCamera.DoHeadBob(controller.velocity.magnitude / (moveSpeedMultiplier * maxRunSpeedMultiplier));

    }
  
 
  
    
    protected virtual void Move()
    {

        Vector2 inputVector = input.inputVector;

        if (inputVector.magnitude > 1)
            inputVector.Normalize();

        moveVector.x = inputVector.x * moveSpeedMultiplier;
        moveVector.z = inputVector.y * moveSpeedMultiplier * runSpeedMultiplier;
        moveVector.y = 0;
        
        
        moveVector = transform.TransformDirection(moveVector);


        if (isJumping)
        {
            gravity = -jumpingForce;

        }
        else
        {

            //WE ARE ON GROUND
            if (grounded)
            {
                gravity = 1;

                //CHECK FOR JUMPING
                if (isJumping == false && input.JumpButtonDown)
                    StartCoroutine(jumpingAnimation(false));
            }
            //We are off the ground and not jumping, so add gravity downwards
            else if (isJumping == false)
            {
                gravity = Mathf.MoveTowards(gravity, gravityAcceleration, Time.deltaTime * gravityAcceleration);
            }
        }




        float magnitude = Vector3.Scale(new Vector3(1, 0, 1), moveVector).magnitude;
            moveVector = Vector3.ProjectOnPlane(moveVector, groundNormal);

        moveVector = moveVector.normalized * magnitude;
            moveVector.y -= gravity;
        


        if (controller.enabled)
            controller.Move(moveVector * Time.deltaTime);

    
     
    }
    protected virtual IEnumerator jumpingAnimation(bool ledgeJump)
    {
        anim.CrossFadeInFixedTime("Jump", 0.1f);
        isJumping = true;
        //Reset ground normal to be normal
        groundNormal = Vector3.up;
        grounded = false;
        lastJumpTime = Time.time;
        anim.SetBool("grounded", false);

       

        //wait for jumping duration and then disable jumping
        yield return new WaitForSeconds(jumpingDuration);

        isJumping = false;
    }
    protected void ActivateJump()
    {
        anim.CrossFadeInFixedTime("Jump", 0.1f);
        anim.SetBool("grounded", false);
    }
    public void setCanMove(bool can)
    {
        canMove = can;
        moveVector = Vector3.zero;
        moveVector.y = 0;

    }

  

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
       
    }

    protected virtual void checkGrounded()
    {

        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0f, controller.radius + 0.05f, 0f), Vector3.down);
        Ray sphereRay = new Ray(transform.TransformPoint(0f, 1.5f, 0f), Vector3.down);


        if (Physics.SphereCast(sphereRay, controller.radius, out hit, 1f, groundLayers, QueryTriggerInteraction.Ignore) && !grounded)
        {
            slopeNormal = hit.normal;

        }
        else if (Physics.Raycast(ray, out hit, 1f, groundLayers, QueryTriggerInteraction.Ignore) && !grounded)
        {
            slopeNormal = hit.normal;
        }
        else
            slopeNormal = Vector3.up;

        if (Time.time > lastJumpTime + 0.2f)
        {
            if (Physics.SphereCast(sphereRay, controller.radius, out hit, 1f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.normal.y > 0.7f)
                {
                    groundNormal = hit.normal;
                    grounded = true;
                    lastGroundedTime = Time.time;
                    return;
                }
                else if (Time.time > lastGroundedTime + groundedLossTime)
                {
                    lastGroundedTime = Time.time;
                    grounded = false;
                    return;
                }
            }
        }
        if (Time.time > lastGroundedTime + groundedLossTime)
        {
            grounded = false;
        }

    }

    public void lockCameraToHead(bool locked)
    {

        if (myCamera == null)
            return;

        if (!myCamera.GetComponent<firstPersonCamera>())
            return;

        if (locked)
        {
            myCamera.GetComponent<firstPersonCamera>().enabled = false;
            myCamera.transform.parent = anim.GetBoneTransform(HumanBodyBones.Head);
            myCamera.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
        else
        {

            myCamera.transform.parent = transform;
            myCamera.transform.localEulerAngles = Vector3.zero;
            myCamera.GetComponent<firstPersonCamera>().enabled = true;
            myCamera.GetComponent<firstPersonCamera>().resetAngles();
        }
     
    }

 
}
