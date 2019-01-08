using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    Animator anim;
    CharacterController controller;

    [SerializeField]
    private float rotateSpeed = 300;

    Camera mainCam;
    // Use this for initialization

    Vector3 moveVector;

    MyInputManager input;

    private float dampTime = 0.2f;
    private float turnDampTime = 0.2f;

    private float lastJogTime;

    private float stoppingWindow = 0.5f;

    bool canTransitionToStop = false;
    bool isRunning = false;
    bool isGrounded = true;

    float gravity;

    [SerializeField]
    private float groundCheckHeight;

    [SerializeField]
    private float groundCheckDistance = 0.1f;

    [SerializeField]
    private LayerMask groundLayers;

    private float lastGroundedTime;
    private float groundedLossTime = 0.1f;
    private float airMoveSpeed = 6;




	void Start () {
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
        input = GetComponent<MyInputManager>();
        lastGroundedTime = 1;
	}
	
	// Update is called once per frame
	void Update () {

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        Vector3 camForward = mainCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector2 inputVector = input.inputVector;

        moveVector = camForward * inputVector.y + mainCam.transform.right * inputVector.x;

        float turnAngle = Mathf.Atan2(transform.InverseTransformDirection(moveVector).x, transform.InverseTransformDirection(moveVector).z) * Mathf.Rad2Deg;

        anim.SetFloat("turnAngle", turnAngle, turnDampTime, Time.deltaTime);

        if (moveVector.magnitude > 1)
            moveVector.Normalize();

        isRunning = input.RunButtonHold;

        if(moveVector.magnitude > 0.1f && !info.IsTag("rootmotion"))
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotateSpeed);

        if(isRunning == false)
        anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z*100, dampTime, Time.deltaTime);
        else
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 150, dampTime, Time.deltaTime);

        if (anim.GetFloat("Forward") >= 50)
        {
            canTransitionToStop = true;
            lastJogTime = Time.time;
        }
            

        if (!info.IsTag("rootmotion") && anim.GetFloat("Forward") < 70f && inputVector.magnitude <= 0.1f && Time.time < lastJogTime + stoppingWindow && !info.IsName("jogStop") && anim.IsInTransition(0) == false && canTransitionToStop && isGrounded)
        {
            canTransitionToStop = false;
            anim.CrossFadeInFixedTime("jogStop", 0.05f);
        }

        if (isGrounded)
            gravity = 1;
        else
            gravity = Mathf.MoveTowards(gravity, 20, Time.deltaTime * 30);

        checkGrounded();

    
	}

  
   
    void checkGrounded()
    {
   
        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0f, controller.radius + 0.05f, 0f), Vector3.down);
        Ray sphereRay = new Ray(transform.TransformPoint(0f, controller.radius + 0.05f, 0f), Vector3.down);
        Ray wallRay = new Ray(transform.TransformPoint(0, 1, 0), transform.forward);



       
            if (Physics.SphereCast(sphereRay, controller.radius, out hit, groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.normal.y > 0.7f)
                {
                if (Time.time > lastGroundedTime + 0.7f)
                    anim.CrossFadeInFixedTime("LandingHard", 0.05f);

                    isGrounded = true;
                    lastGroundedTime = Time.time;
                    anim.SetBool("Grounded", isGrounded);
                    return;
                }
                else if (Time.time > lastGroundedTime + groundedLossTime && controller.velocity.y < -1)
                {
                    isGrounded = false;
                    anim.SetBool("Grounded", isGrounded);
                    return;
                }
            }
           

           
 
        

        if (Time.time > lastGroundedTime + groundedLossTime && controller.velocity.y <= -1)
        {
            if (!Physics.SphereCast(sphereRay, controller.radius, out hit, 0.8f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                isGrounded = false;
                anim.SetBool("Grounded", isGrounded);
            }
             
        }

     
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(transform.TransformPoint(0,contro))
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y > 0.7f)
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);
        }
    }

    private void OnAnimatorMove()
    {
        Vector3 deltaMovement = anim.deltaPosition;

        if (!isGrounded)
        {
            deltaMovement = moveVector * Time.deltaTime*airMoveSpeed;
        }

        deltaMovement.y = gravity * Time.deltaTime*-1;


        controller.Move(deltaMovement);

    }
}
