using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using Assets.Scripts.Cam.Effects;
public class Player : MonoBehaviour
{
    public static Player instance;
    Animator anim;
    CharacterController controller;

    [SerializeField]
    private float rotateSpeed = 300;

    Vector2 inputVector;


    [SerializeField]
    private float crawlSpaceSpeed = 2.5f;

    private bool inCrawlSpace = false;
    private bool inCrawlSpaceTransition = false;
    [SerializeField]
    private float crawlSpaceRotationSpeed = 150;

    private float crawlSpaceStepDistance = 0.7f;

    private Vector3 lastCrawlPosition;

    Camera mainCam;
    // Use this for initialization

    Vector3 moveVector;

    Vector3 startingPoint;
    Quaternion startingRotation;


    //Checkpoint position and rotation
    Vector3 checkPointPosition;
    Quaternion checkPointRotation;

    MyInputManager input;

    [SerializeField]
    private float dampTime = 0.2f;
    private float turnDampTime = 0.2f;

    private float lastJogTime;
    private float lastWalkTime;

    private float stoppingWindow = 0.5f;

    bool canTransitionToStop = false;
    bool isRunning = false;
    bool isWalking = false;
    bool isGrounded = true;

    float maxFallingHeight = 7.5f;
    float lastGroundedHeight;

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
    Vector3 groundNormal;


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
    public bool CloseUpEnabled { get { return closeUpEnabled; } }

    [SerializeField]
    private LayerMask interactLayers;

    [SerializeField]
    private LayerMask fpsInteractLayers;

    private float interactRadius = 1;
    public float InteractRadius { get { return interactRadius; } }


    //item that was last inspected in first person
    private Item inspectedItem;

    private bool dead = false;
    public bool Dead { get { return dead; } }

    public delegate void Death();
    public static event Death OnDeath;

    public delegate void Restart();
    public static event Restart OnRestart;


    private float defaultControllerOffset = 1.08f;
    private float defaultControllerHeight = 2;

    private float crouchingControllerOffset = 0.9f;
    private float crouchingControllerHeight = 1.66f;


    private float airDamping = 0.5f;


    //VARIABLES FOR AI
    private bool insideRestrictedArea = true;
    public bool InsideRestrictedArea{get{return insideRestrictedArea;}}


    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCam = Camera.main;
        anim = GetComponent<Animator>();
        input = GetComponent<MyInputManager>();
        lastGroundedTime = 1;
        lastGroundedHeight = transform.position.y;

        startingPoint = new Vector3(-50.693f, -16.69835f, -32);
        startingRotation = Quaternion.Euler(0, -262, 0);

        checkPointPosition = transform.position;
        checkPointRotation = transform.rotation;


    }

    private void Awake()
    {
        instance = this;
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

        if (isGrounded)
        {
            inputVector = input.inputVector;
        }
        else
        {
            inputVector = Vector2.MoveTowards(inputVector, input.inputVector, airDamping * Time.deltaTime);
        }


        moveVector = camForward * inputVector.y + mainCam.transform.right * inputVector.x;

        float turnAngle = Mathf.Atan2(transform.InverseTransformDirection(moveVector).x, transform.InverseTransformDirection(moveVector).z) * Mathf.Rad2Deg;

        anim.SetFloat("turnAngle", turnAngle, turnDampTime, Time.deltaTime);

        if (moveVector.magnitude > 1)
            moveVector.Normalize();

        isRunning = input.RunButtonHold;

        if (input.WalkButtonDown && !anim.GetBool("crouching"))
            isWalking = !isWalking;

        if (moveVector.magnitude > 0.1f && !info.IsTag("rootmotion") && !info.IsName("LandingHard") && !inCrawlSpace)
        {
            if (anim.GetBool("Grounded"))
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotateSpeed);
            else
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveVector), Time.deltaTime * rotateSpeed * 0.5f);
        }
        else if (inCrawlSpace)
        {
            transform.Rotate(Vector3.up * crawlSpaceRotationSpeed * Time.deltaTime * Input.GetAxisRaw("Mouse X"));
        }


        if (isRunning == false)
        {
            if (isWalking == false)
                anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 100, dampTime, Time.deltaTime);
            else
                anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 25, dampTime, Time.deltaTime);

        }

        else
        {
            anim.SetFloat("Forward", transform.InverseTransformDirection(moveVector).z * 150, dampTime, Time.deltaTime);
        }


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


        if (!anim.GetBool("crouching") && !info.IsTag("rootmotion") && anim.GetFloat("Forward") < 80f && inputVector.magnitude <= 0.1f && !info.IsName("jogStop") && !info.IsName("walkStop") && anim.IsInTransition(0) == false && canTransitionToStop && isGrounded)
        {
            if (Time.time < lastJogTime + stoppingWindow)
            {
                anim.CrossFadeInFixedTime("jogStop", 0.3f);
                canTransitionToStop = false;
            }
            else if (Time.time < lastWalkTime + stoppingWindow)
            {
                anim.CrossFadeInFixedTime("walkStop", 0.25f);
                canTransitionToStop = false;
            }


        }

        if (input.CrouchButtonDown && !inCrawlSpace && !inCrawlSpaceTransition)
        {
            isWalking = false;
            anim.SetBool("crouching", !anim.GetBool("crouching"));

            if (anim.GetFloat("Forward") > 100 && anim.IsInTransition(0) == false && info.IsTag("move") && isRunning)
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "playerSlide_" + Random.Range(1, 4).ToString(), transform.position, transform, 0, 0);
                anim.CrossFadeInFixedTime("runningSlide", 0.2f);

            }

        }



        if (isGrounded)
            gravity = 1;
        else
            gravity = Mathf.MoveTowards(gravity, 20, Time.deltaTime * 30);

        if (inCrawlSpace && controller.enabled)
        {
            anim.SetFloat("Forward", 0);
            moveVector.y = -gravity;
            controller.Move(moveVector * Time.deltaTime * crawlSpaceSpeed);

            if (Vector3.Distance(transform.position, lastCrawlPosition) > crawlSpaceStepDistance)
            {
                lastCrawlPosition = transform.position;
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "airduct_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
            }
        }




    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
            OnRestart();

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
            if (Physics.Raycast(ray, out hit, 2, fpsInteractLayers, QueryTriggerInteraction.Ignore))
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
            GameplayCanvas.instance.HideUI(false);
            //if (inspectedItem)
            //{
            //    Inventory.instance.AddItem(inspectedItem);
            //    inspectedItem = null;
            //}

        }
    }
    void CheckInteractions()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        Debug.DrawRay(transform.TransformPoint(0, vaultRayHeight, 0), transform.forward * vaultDistance, Color.cyan);
        Debug.DrawRay(transform.TransformPoint(0, obstacleHeight, 0), transform.forward * obstacleDistance, Color.red);

        Vector3 wallNormal = Vector3.zero;
        Vector3 wallPoint = Vector3.zero;
        if (isGrounded && !info.IsTag("rootmotion"))
        {
            if (input.JumpButtonDown && !anim.IsInTransition(0))
            {
                RaycastHit hit;
                for (int ii = 2; ii > 0; ii--)
                {
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
                            break;
                        }
                        else
                        {
                            //check downwards to determine distance and collision
                            Ray downRay = new Ray(hit.point + Vector3.up * 2.5f - hit.normal * 0.5f, Vector3.down);
                            Debug.DrawRay(downRay.origin, downRay.direction, Color.red);
                            if (Physics.Raycast(downRay, out hit, 2.5f / ii, interactLayers, QueryTriggerInteraction.Ignore))
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

                                if (Physics.Raycast(matchingObject.TransformPoint(matchingLocation), Vector3.up, 2, groundLayers, QueryTriggerInteraction.Ignore))
                                    return;
                                
                                if(Physics.Raycast(transform.TransformPoint(0,0.5f,0), Vector3.up,3,groundLayers, QueryTriggerInteraction.Ignore))
                                return;
                         

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
                                break;
                            }
                        }
                    }
                }
            }
            else if (input.InteractButtonDown)
            {

                Collider[] cols = Physics.OverlapSphere(transform.position, interactRadius, interactLayers, QueryTriggerInteraction.Collide);

                if (cols != null)
                    for (int i = 0; i < cols.Length; i++)
                    {
                        if (cols[i].tag == "closeUpObject")
                        {
                            CloseUpObject cu = cols[i].GetComponent<CloseUpObject>();

                            if (Vector3.Angle(-transform.forward, Vector3.Scale(cu.CloseUpDirection, new Vector3(1, 0, 1))) > cu.ActivationAngle)
                                return;

                            if (cu.GetComponent<Item>())
                                inspectedItem = cu.GetComponent<Item>();

                            cu.OnInteract();
                            Transform t = cu.transform;
                            if (cu.ParentCamera == false)
                                t = null;

                            if (t)
                                CameraFollow.playerCam.ActivateCloseUp(t, cu.CloseUpPoint, cu.CloseUpDirection, true);
                            else
                                CameraFollow.playerCam.ActivateCloseUp(t, cu.transform.TransformPoint(cu.CloseUpPoint), cu.CloseUpDirection, true);

                            GameplayCanvas.instance.HideUI(true);
                            anim.SetFloat("Forward", 0);
                            VirtualCursor.instance.Activate(true);
                            closeUpEnabled = true;
                            transform.position = cu.PlayerPoint;
                            transform.rotation = Quaternion.LookRotation(Vector3.Scale(-cu.CloseUpDirection, new Vector3(1, 0, 1)));
                            anim.Play("Move");
                            ShowMeshes(false);
                            controller.enabled = false;
                            return;
                        }
                        else if (cols[i].tag == "crawlEntrance")
                        {
                            CrawlSpaceEntrance cse = cols[i].GetComponent<CrawlSpaceEntrance>();
                            if (inCrawlSpace)
                                StartCoroutine(crawlEnterAnimation(false, cse.EntrancePoint, cse.ExitPoint));
                            else
                                StartCoroutine(crawlEnterAnimation(true, cse.ExitPoint, cse.EntrancePoint));
                        }
                    }


            }
        }

        //if (inCrawlSpace && !inCrawlSpaceTransition)
        //{
        //    if (input.InteractButtonDown)
        //    {
        //        Collider[] c = new Collider[1];
        //        Physics.OverlapSphereNonAlloc(transform.position, 1f, c, interactLayers, QueryTriggerInteraction.Collide);

        //        if (c[0] != null)
        //        {
        //            if (c[0].tag == "crawlEntrance")
        //            {
        //                CrawlSpaceEntrance cse = c[0].GetComponent<CrawlSpaceEntrance>();
        //                StartCoroutine(crawlEnterAnimation(false, cse.EntrancePoint, cse.ExitPoint));
        //            }
        //        }
        //    }
        //}
    }

    IEnumerator crawlEnterAnimation(bool enter, Vector3 start, Vector3 end)
    {
        anim.SetFloat("Forward", 0);
        float lerp = 0;

        inCrawlSpaceTransition = true;
        controller.enabled = false;

        if (enter)
        {

            CameraFollow.playerCam.ActivateCrawlSpaceMode(enter);
            ShowMeshes(false);
        }


        while (lerp < 1)
        {
            lerp += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, lerp);

            yield return null;
        }
        controller.height = 0.8f;
        controller.center = new Vector3(0, 0.4f, 0);
        lastGroundedTime = Time.time;
        isGrounded = true;
        inCrawlSpaceTransition = false;
        controller.enabled = true;

        inCrawlSpace = enter;

        if (!enter)
        {
            CameraFollow.playerCam.ActivateCrawlSpaceMode(enter);
            ShowMeshes(true);

            CrouchModeChange(anim.GetBool("crouching"));
        }

        gravity = 0;

    }
    void ShowMeshes(bool show)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
            rends[i].enabled = show;

        if (show)
        {
            Cloth c = GetComponentInChildren<Cloth>();
            if (c != null)
                c.ClearTransformMotion();
        }

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
                {
                    if (transform.position.y < groundLossHeight - 4)
                    {
                        if (anim.GetBool("landRoll") == false)
                            anim.CrossFadeInFixedTime("LandingHard", 0.05f);
                        else
                        {
                            anim.SetBool("landRoll", false);
                            anim.CrossFadeInFixedTime("landRoll", 0.1f);
                        }
                    }

                }


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
        else if (Physics.Raycast(ray, out hit, groundCheckDistance * 2, groundLayers, QueryTriggerInteraction.Ignore))
        {
            if (hit.normal.y > 0.7f)
            {
                if (Time.time > lastGroundedTime + 0.7f)
                {
                    if (transform.position.y < groundLossHeight - 4)
                    {
                        if (anim.GetBool("landRoll") == false)
                            anim.CrossFadeInFixedTime("LandingHard", 0.05f);
                        else
                        {
                            anim.SetBool("landRoll", false);
                            anim.CrossFadeInFixedTime("landRoll", 0.1f);
                        }
                    }

                }
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
                    if (controller.velocity.y <= -1 || info.IsTag("rootmotion"))
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
        groundNormal = hit.normal;
        if (hit.normal.y > 0.7f)
        {
            isGrounded = true;
            anim.SetBool("Grounded", isGrounded);

            if (lastGroundedHeight - transform.position.y > maxFallingHeight && Time.time > 1)
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "deathLand", transform.position, transform, 0, 0);
                OnDeath();
            }

            lastGroundedHeight = transform.position.y;
        }
    }

    private void OnAnimatorMove()
    {
        if (inCrawlSpace)
            return;
        Vector3 deltaMovement = anim.deltaPosition;

        if (!isGrounded)
        {
            deltaMovement = Vector3.MoveTowards(deltaMovement, moveVector * Time.deltaTime * airMoveSpeed * anim.GetFloat("Forward") / 100, Time.deltaTime * 5);
        }

        deltaMovement.y = gravity * Time.deltaTime * -1;

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("rootmotion") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
        {
            transform.position = anim.rootPosition;
            return;
        }


        //check for stairs when going downwards
        RaycastHit hit;
        Ray ray = new Ray(transform.TransformPoint(0, 0.1f, 0.5f), Vector3.down);
        float checkDistance = 1f;
        Debug.DrawRay(ray.origin, ray.direction.normalized * checkDistance, Color.black);

        if (Physics.Raycast(ray, out hit, checkDistance, groundLayers, QueryTriggerInteraction.Ignore) && isGrounded)
        {
            if (hit.point.y < transform.position.y)
                deltaMovement.y = -hit.distance;
        }

        if (!isGrounded)
            deltaMovement = Vector3.ProjectOnPlane(deltaMovement, slopeNormal);


        //controller.SimpleMove(deltaMovement / Time.deltaTime);
        if (controller.enabled)
            controller.Move(deltaMovement);

    }

    private void OnEnable()
    {
        Player.OnDeath += PlayerDeath;
        Player.OnRestart += PlayerRestart;
    }

    private void OnDisable()
    {
        Player.OnDeath -= PlayerDeath;
        Player.OnRestart -= PlayerRestart;
    }

    private void PlayerDeath()
    {
        dead = true;
        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "gameOver", transform.position, null, 0, 0.3f);
        DarkAmbient.darkAmbientActivated = false;

        if (SecretAreaTrigger.instance)
            SecretAreaTrigger.instance.ResetSecretArea();
        SecretMusic.secretMusicActivated = false;
        LastNotesMusic.noteMusicActivated = false;
        BreathActivator.breathActivated = false;

        StartCoroutine(RestartDelay());

    }
    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(5);
        OnRestart();
    }
    public void CompleteLoop()
    {
        DarkAmbient.darkAmbientActivated = false;
        SecretMusic.secretMusicActivated = false;
        LastNotesMusic.noteMusicActivated = false;
        BreathActivator.breathActivated = false;
        transform.position = startingPoint;
        transform.rotation = startingRotation;

        checkPointPosition = startingPoint;
        checkPointRotation = startingRotation;
        lastGroundedHeight = transform.position.y;

        anim.Play("Move");

        Cloth c = GetComponentInChildren<Cloth>();
        if (c != null)
            c.ClearTransformMotion();

        RetroSize.instance.Pixelate(false);
        RetroSize.instance.enabled = false;

        CameraFollow.playerCam.ActivateCloseUp(null, Vector3.zero, Vector3.zero, false);
        ShowMeshes(true);
        controller.enabled = true;
        closeUpEnabled = false;
        lastGroundedTime = Time.time;
        isGrounded = true;
        anim.SetBool("Grounded", true);
        VirtualCursor.instance.Activate(false);
        GameplayCanvas.instance.HideUI(false);


        //reset checkpoints
        Checkpoint[] cps = FindObjectsOfType(typeof(Checkpoint)) as Checkpoint[];

        for (int i = 0; i < cps.Length; i++)
        {
            cps[i].ResetCheckpoint();
        }


        Elevator.instance.ResetElevators();
    }
    public void PlayerRestart()
    {
        dead = false;
        transform.position = checkPointPosition;
        transform.rotation = checkPointRotation;
        lastGroundedHeight = transform.position.y;

        anim.Play("Move");

        Cloth c = GetComponentInChildren<Cloth>();
        if (c != null)
            c.ClearTransformMotion();

        if (WindowCleanerElevator.windowCleanerPowerEnabled)
            WindowCleanerElevator.LowerAllWindowCleanerElevators();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "checkpoint")
        {
            other.enabled = false;
            checkPointPosition = transform.position;
            checkPointRotation = transform.rotation;
            Checkpoint cp = other.GetComponent<Checkpoint>();

            if (cp.Tag == "vision")
            {
                VisionAnimator.visionReached = true;
            }
        }
        else if (other.tag == "sea" && !dead)
        {
            OnDeath();
        }
    }
    public void CrouchModeChange(bool crouching)
    {
        if (crouching)
        {
            controller.center = new Vector3(0, crouchingControllerOffset, 0);
            controller.height = crouchingControllerHeight;
        }
        else
        {
            controller.center = new Vector3(0, defaultControllerOffset, 0);
            controller.height = defaultControllerHeight;
        }
    }
}
