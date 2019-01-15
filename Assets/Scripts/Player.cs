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
    private float groundLossHeight;
    Vector3 slopeNormal;


    //Animator matching targets
    Vector3 matchingLocation;
    Transform matchingObject;
    Quaternion matchingRotation;
    Quaternion originalRotation;


    //interaction variables
    float vaultDistance = 0.8f;
    float vaultRayHeight = 1;

    //vault obstacle variables
    float obstacleDistance = 1.8f;
    float obstacleHeight = 1.6f;


    //close up variables
    private bool closeUpEnabled = false;

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
        if (matchingObject == null)
            return;


        Vector3 location = matchingObject.TransformPoint(matchingLocation);

     

        if (info.IsName("VaultMedium"))
        {
            anim.MatchTarget(location, matchingRotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), 0, 0.2f);

            transform.rotation = Quaternion.Lerp(originalRotation, matchingRotation, info.normalizedTime / 0.3f);
        }
        else if (info.IsName("ClimbUpMedium"))
        {
            lastGroundedTime = Time.time;
            anim.MatchTarget(location, matchingRotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), 0.0f, 0.3f);

            transform.rotation = Quaternion.Lerp(originalRotation, matchingRotation, info.normalizedTime / 0.3f);
        }
        else if (info.IsName("ClimbUpLow"))
        {
            lastGroundedTime = Time.time;
            anim.MatchTarget(location, matchingRotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), 0f, 0.15f);

            transform.rotation = Quaternion.Lerp(originalRotation, matchingRotation, info.normalizedTime / 0.3f);
        }
        else if (info.IsName("ClimbUpHigh"))
        {
            lastGroundedTime = Time.time;
            anim.MatchTarget(location, matchingRotation, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 1), 0f, 0.185f);

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
        {
            if(anim.GetBool("Grounded"))
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotateSpeed);
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotateSpeed*0.5f);
        }
           

        if (isRunning == false)
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 100, dampTime, Time.deltaTime);
        else
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 150, dampTime, Time.deltaTime);

        if (anim.GetFloat("Forward") >= 120)
        {
            canTransitionToStop = true;
            lastJogTime = Time.time;
        }
        else if (anim.GetFloat("Forward") >= 30)
        {
            canTransitionToStop = true;
            lastWalkTime = Time.time;
        }


        if (!anim.GetBool("crouching") && !info.IsTag("rootmotion") && anim.GetFloat("Forward") < 70f && inputVector.magnitude <= 0.1f && !info.IsName("jogStop") && !info.IsName("walkStop") && anim.IsInTransition(0) == false && canTransitionToStop && isGrounded)
        {
            if (Time.time < lastJogTime + stoppingWindow)
            {
                anim.CrossFadeInFixedTime("jogStop", 0.05f);
                canTransitionToStop = false;
            }
            else if (Time.time < lastWalkTime + stoppingWindow)
            {
                anim.CrossFadeInFixedTime("walkStop", 0.25f);
                canTransitionToStop = false;
            }
           
           
        }

        if (input.CrouchButtonDown)
            anim.SetBool("crouching", !anim.GetBool("crouching"));


        if (isGrounded)
            gravity = 1;
        else
            gravity = Mathf.MoveTowards(gravity, 20, Time.deltaTime * 30);

    }
	void Update () {


        if (closeUpEnabled == false)
        {
            Locomotion();
            checkGrounded();
            CheckInteractions();
            MatchAnimator();
        }
        else
        {
            FirstPersonInteraction();
        }
     
    
	}
    void FirstPersonInteraction()
    {
        if (input.JumpButtonDown || Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = mainCam.ScreenPointToRay(VirtualCursor.instance.ScreenPosition);
            RaycastHit hit;

            //if (Physics.SphereCast(ray, 0.02f,out hit, 2, 1 << LayerMask.NameToLayer("FPSInteract"), QueryTriggerInteraction.Ignore))
            if (Physics.Raycast(ray, out hit, 2, 1 << LayerMask.NameToLayer("FPSInteract"), QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.GetComponent<Interactable>())
                    hit.collider.GetComponent<Interactable>().Interact();
            }
        }
        else if (input.InteractButtonDown)
        {
            CameraFollow.playerCam.ActivateCloseUp(null, Vector3.zero, Vector3.zero, false);
            ShowMeshes(true);
            controller.enabled = true;
            closeUpEnabled = false;
            lastGroundedTime = Time.time;
            isGrounded = true;
            anim.SetBool("Grounded", true);
            VirtualCursor.instance.Activate(false);
        }
    }
    void CheckInteractions()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        Debug.DrawRay(transform.TransformPoint(0, vaultRayHeight, 0), transform.forward*vaultDistance,Color.cyan);
        Debug.DrawRay(transform.TransformPoint(0, obstacleHeight, 0), transform.forward * obstacleDistance, Color.red);

        Vector3 wallNormal = Vector3.zero;
        Vector3 wallPoint = Vector3.zero;
        if (isGrounded && !info.IsTag("rootmotion"))
        {
            if (input.JumpButtonDown && !anim.IsInTransition(0))
            {
                RaycastHit hit;
                Ray ray = new Ray(transform.TransformPoint(0, vaultRayHeight, 0), transform.forward);
                if (Physics.Raycast(ray, out hit, vaultDistance, interactLayers, QueryTriggerInteraction.Ignore))
                {
                    wallNormal = hit.normal;
                    wallPoint = hit.point;
                    if (hit.collider.tag == "mediumVault")
                    {
                        ray = new Ray(transform.TransformPoint(0, obstacleHeight, 0), -hit.normal);

                        if (Physics.Raycast(ray, obstacleDistance, groundLayers, QueryTriggerInteraction.Ignore))
                            return;

                        matchingLocation = hit.point + hit.normal * 0.1f + Vector3.up * 0.085f;
                        matchingRotation = Quaternion.LookRotation(-hit.normal);

                        ray = new Ray(hit.point - hit.normal * 0.01f + Vector3.up, Vector3.down);
                        if (Physics.Raycast(ray, out hit, 1, interactLayers, QueryTriggerInteraction.Ignore))
                        {
                            matchingLocation.y = hit.point.y;
                        }

                        matchingObject = hit.collider.transform;
                        matchingLocation = matchingObject.transform.InverseTransformPoint(matchingLocation);
                        originalRotation = transform.rotation;
                        transform.SetParent(null);
                        anim.CrossFadeInFixedTime("VaultMedium", 0.1f);
                    }
                    else
                    {
                        //check downwards to determine distance and collision
                        Ray downRay = new Ray(hit.point + Vector3.up * 2.5f - hit.normal * 0.5f, Vector3.down);
                        Debug.DrawRay(downRay.origin, downRay.direction, Color.red);
                        if (Physics.Raycast(downRay, out hit, 2.5f, interactLayers, QueryTriggerInteraction.Ignore))
                        {
                            if (hit.normal.y < 0.7f)
                                return;

                            Vector3 directionX = Vector3.Cross(Vector3.up, wallNormal);
                            Vector3 originPos = wallPoint + wallNormal * 0.2f;
                            originPos.y = hit.point.y + 0.2f;

                            wallNormal.y = 0;
                            //horizontal checks to determine if there's something blocking the player from getting up
                            for (int i = -1; i < 2; i++)
                            {
                                if (Physics.Linecast(originPos - directionX * (i * 0.3f), originPos - directionX * (i * 0.3f) - wallNormal * 1.5f, groundLayers, QueryTriggerInteraction.Ignore))
                                    return;


                            }

                            //no obstacles horizontally
                            matchingLocation = wallPoint + transform.right * 0.2f;
                            matchingLocation.y = hit.point.y + 0.05f;
                            matchingRotation = Quaternion.LookRotation(-wallNormal);
                            originalRotation = transform.rotation;

                            matchingObject = hit.collider.transform;
                            matchingLocation = matchingObject.transform.InverseTransformPoint(matchingLocation);

                            float heightDistance = hit.point.y - transform.position.y;

                            if (heightDistance < 1.9f)
                            {
                                anim.CrossFadeInFixedTime("ClimbUpLow", 0.1f);
                            }
                            else if (heightDistance > 1.9f && heightDistance < 2.9f)
                            {
                                anim.CrossFadeInFixedTime("ClimbUpMedium", 0.1f);
                            }
                            else if (heightDistance > 2.9f)
                            {
                                anim.CrossFadeInFixedTime("ClimbUpHigh", 0.1f);
                            }

                        }
                    }
                }
            }
            else if (input.InteractButtonDown)
            {
                float x = -0.5f;

                for (int i = -1; i < 2; i++)
                {
                    Ray ray = new Ray(transform.TransformPoint(x*i, 1, 0), transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, vaultDistance, interactLayers, QueryTriggerInteraction.Collide))
                    {
                        if (hit.collider.tag == "closeUpObject")
                        {
                            
                            CloseUpObject cu = hit.collider.GetComponent<CloseUpObject>();

                            if (Vector3.Angle(-transform.forward, Vector3.Scale(cu.CloseUpDirection, new Vector3(1, 0, 1))) > cu.ActivationAngle)
                                return;

                            CameraFollow.playerCam.ActivateCloseUp(cu.transform,cu.CloseUpPoint, cu.CloseUpDirection, true);
                            VirtualCursor.instance.Activate(true);
                            closeUpEnabled = true;
                            transform.position = cu.PlayerPoint;
                            transform.rotation = Quaternion.LookRotation(Vector3.Scale(-cu.CloseUpDirection, new Vector3(1, 0, 1)));
                            anim.Play("Move");
                            ShowMeshes(false);
                            controller.enabled = false;
                            break;

                        }
                    }
                }
            }
        }
    }
    void ShowMeshes(bool show)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
            rends[i].enabled = show;
    }
   
    void checkGrounded()
    {
        
        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0f, 0.1f, 0f), Vector3.down);
        Ray sphereRay = new Ray(transform.TransformPoint(0f, controller.radius + 0.1f, 0f), Vector3.down);
        Ray wallRay = new Ray(transform.TransformPoint(0, 1, 0), transform.forward);


        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (Physics.SphereCast(sphereRay, controller.radius, out hit, groundCheckDistance, groundLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.normal.y > 0.7f)
            {
                if (Time.time > lastGroundedTime + 0.7f)
                    anim.CrossFadeInFixedTime("LandingHard", 0.05f);

                slopeNormal = Vector3.up;
                isGrounded = true;
                lastGroundedTime = Time.time;
                anim.SetBool("Grounded", isGrounded);
                if (hit.collider.tag == "platform")
                    transform.SetParent(hit.collider.transform);
                else if(!info.IsTag("rootmotion"))
                    transform.SetParent(null);

                return;

             
            }
            else if (Time.time > lastGroundedTime + groundedLossTime && (controller.velocity.y < -1 ||info.IsTag("rootmotion")))
            {
                if (isGrounded)
                    groundLossHeight = transform.position.y;
                isGrounded = false;
                slopeNormal = hit.normal;
                anim.SetBool("Grounded", isGrounded);
                 if (!info.IsTag("rootmotion"))

                    transform.SetParent(null);
                return;
            }
            else
                slopeNormal = hit.normal;
        }
        else if (Physics.Raycast(ray, out hit, groundCheckDistance*2, groundLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.normal.y > 0.7f)
            {
                if (Time.time > lastGroundedTime + 0.7f && transform.position.y < groundLossHeight - 4)
                    anim.CrossFadeInFixedTime("LandingHard", 0.05f);
                slopeNormal = Vector3.up;
                isGrounded = true;
                lastGroundedTime = Time.time;
                anim.SetBool("Grounded", isGrounded);
                if (hit.collider.tag == "platform")
                    transform.SetParent(hit.collider.transform);
                else if (!info.IsTag("rootmotion"))
                    transform.SetParent(null);

                return;
            }
            else if (Time.time > lastGroundedTime + groundedLossTime && (controller.velocity.y < -1 || info.IsTag("rootmotion")))
            {
                if (isGrounded)
                    groundLossHeight = transform.position.y;
                slopeNormal = hit.normal;
                isGrounded = false;
                anim.SetBool("Grounded", isGrounded);

                 if (!info.IsTag("rootmotion"))
                    transform.SetParent(null);
                return;
            }
            else
            {
                  if (!info.IsTag("rootmotion"))
                    transform.SetParent(null);
                slopeNormal = hit.normal;
            }
        }







        if (Time.time > lastGroundedTime + groundedLossTime)
        {
            if (!Physics.SphereCast(sphereRay, controller.radius, out hit, 0.8f, groundLayers, QueryTriggerInteraction.Ignore))
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("rootmotion"))
                {
                    gravity = 3;
                    if (Physics.Raycast(transform.TransformPoint(0, 0.1f, 0), Vector3.down, 1.5f, groundLayers, QueryTriggerInteraction.Ignore))
                        return;
                    
                    if (isGrounded)
                        groundLossHeight = transform.position.y;
                    isGrounded = false;
                    slopeNormal = Vector3.up;
                    transform.SetParent(null);
                    anim.SetBool("Grounded", isGrounded);
                }
                else
                {
                if(controller.velocity.y <= -1 || info.IsTag("rootmotion"))
                    {
                        if (isGrounded)
                            groundLossHeight = transform.position.y;
                        isGrounded = false;
                        slopeNormal = hit.normal;
                        transform.SetParent(null);
                        anim.SetBool("Grounded", isGrounded);
                    }
                    else
                        slopeNormal = hit.normal;
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
            deltaMovement = Vector3.MoveTowards(deltaMovement, moveVector * Time.deltaTime*airMoveSpeed*anim.GetFloat("Forward")/100,Time.deltaTime*5);
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

        if (Physics.Raycast(ray, out hit, checkDistance, groundLayers, QueryTriggerInteraction.Ignore) && isGrounded)
        {
            if (hit.point.y < transform.position.y)
                deltaMovement.y = -hit.distance;
        }

        if (!isGrounded)
            deltaMovement = Vector3.ProjectOnPlane(deltaMovement, slopeNormal);

        Debug.DrawRay(transform.position, deltaMovement, Color.green);

        //controller.SimpleMove(deltaMovement / Time.deltaTime);
        if(controller.enabled)
        controller.Move(deltaMovement);

    }
}
