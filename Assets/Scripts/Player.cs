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
    private float lastWalkTime;

    private float stoppingWindow = 0.5f;

    bool canTransitionToStop = false;
    bool isRunning = false;
    bool isGrounded = true;

    float gravity;

    [SerializeField]
    private float groundCheckHeight;

    [SerializeField]
    private float groundCheckDistance = 0.15f;

    [SerializeField]
    private LayerMask groundLayers;


    private float lastGroundedTime;
    private float groundedLossTime = 0.1f;
    private float airMoveSpeed = 6;


    //Animator matching targets
    Vector3 matchingLocation;
    Quaternion matchingRotation;
    Quaternion originalRotation;


    //interaction variables
    float vaultDistance = 0.8f;
    float vaultRayHeight = 1;

    //vault obstacle variables
    float obstacleDistance = 1.8f;
    float obstacleHeight = 1.6f;

    [SerializeField]
    private LayerMask interactLayers;

    void Start () {
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
        input = GetComponent<MyInputManager>();
        lastGroundedTime = 1;
	}

    // Update is called once per frame
    void MatchAnimator()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (anim.IsInTransition(0))
            return;

        if (info.IsName("VaultMedium"))
        {
            anim.MatchTarget(matchingLocation, matchingRotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), 0, 0.2f);

            transform.rotation = Quaternion.Lerp(originalRotation, matchingRotation, info.normalizedTime / 0.3f);
        }
    }
    void Locomotion()
    {
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

        if (moveVector.magnitude > 0.1f && !info.IsTag("rootmotion"))
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotateSpeed);

        if (isRunning == false)
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 100, dampTime, Time.deltaTime);
        else
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 150, dampTime, Time.deltaTime);

        if (anim.GetFloat("Forward") >= 80)
        {
            canTransitionToStop = true;
            lastJogTime = Time.time;
        }
        else if (anim.GetFloat("Forward") >= 30)
        {
            canTransitionToStop = true;
            lastWalkTime = Time.time;
        }


        if (!info.IsTag("rootmotion") && anim.GetFloat("Forward") < 70f && inputVector.magnitude <= 0.1f && !info.IsName("jogStop") && !info.IsName("walkStop") && anim.IsInTransition(0) == false && canTransitionToStop && isGrounded)
        {
            if (Time.time < lastJogTime + stoppingWindow)
            {
                anim.CrossFadeInFixedTime("jogStop", 0.05f);
                canTransitionToStop = false;
            }
            else if (Time.time < lastWalkTime + stoppingWindow)
            {
                anim.CrossFadeInFixedTime("walkStop", 0.1f);
                canTransitionToStop = false;
            }
           
           
        }


        if (isGrounded)
            gravity = 1;
        else
            gravity = Mathf.MoveTowards(gravity, 20, Time.deltaTime * 30);
    }
	void Update () {



        Locomotion();
        checkGrounded();
        CheckInteractions();
        MatchAnimator();
    
	}
    void CheckInteractions()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        Debug.DrawRay(transform.TransformPoint(0, vaultRayHeight, 0), transform.forward*vaultDistance,Color.cyan);
        Debug.DrawRay(transform.TransformPoint(0, obstacleHeight, 0), transform.forward * obstacleDistance, Color.red);

        if (isGrounded && !info.IsTag("rootmotion"))
        {
            if (input.JumpButtonDown)
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.TransformPoint(0, vaultRayHeight, 0), transform.forward);
                if (Physics.Raycast(ray, out hit, vaultDistance, interactLayers, QueryTriggerInteraction.Ignore))
                {
                    ray = new Ray(transform.TransformPoint(0, obstacleHeight, 0), -hit.normal);

                    if (Physics.Raycast(ray,obstacleDistance, groundLayers, QueryTriggerInteraction.Ignore))
                        return;

                    matchingLocation = hit.point+hit.normal*0.1f+Vector3.up*0.085f;
                    matchingRotation = Quaternion.LookRotation(-hit.normal);

                    ray = new Ray(hit.point - hit.normal * 0.01f + Vector3.up, Vector3.down);
                    if (Physics.Raycast(ray, out hit, 1, interactLayers, QueryTriggerInteraction.Ignore))
                    {
                        matchingLocation.y = hit.point.y;
                    }

                    originalRotation = transform.rotation;
                        anim.CrossFadeInFixedTime("VaultMedium", 0.1f);
                }
            }
        }
    }
  
   
    void checkGrounded()
    {
   
        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0f, 0.1f, 0f), Vector3.down);
        Ray sphereRay = new Ray(transform.TransformPoint(0f, controller.radius + 0.1f, 0f), Vector3.down);
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
        else if (Physics.Raycast(ray, out hit, groundCheckDistance*2, groundLayers, QueryTriggerInteraction.Ignore))
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







        if (Time.time > lastGroundedTime + groundedLossTime)
        {
            if (!Physics.SphereCast(sphereRay, controller.radius, out hit, 0.8f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("rootmotion"))
                {
                    gravity = 3;
                    isGrounded = false;
                    anim.SetBool("Grounded", isGrounded);
                }
                else
                {
                if(controller.velocity.y <= -1)
                    {
                        isGrounded = false;
                        anim.SetBool("Grounded", isGrounded);
                    }
                }
                

               
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

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("rootmotion"))
        {
            transform.position = anim.rootPosition;
            return;
        }


        //check for stairs when going downwards
        RaycastHit hit;
        Ray ray = new Ray(transform.TransformPoint(0,0.1f,0.5f), Vector3.down);
        float checkDistance = 1f;
        Debug.DrawRay(ray.origin, ray.direction.normalized * checkDistance, Color.black);
        if (Physics.Raycast(ray, out hit, checkDistance, groundLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.point.y < transform.position.y)
                deltaMovement.y = -hit.distance;
        }

        controller.Move(deltaMovement);

    }
}
