using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Rendering.PostProcessing;
public class AIAgent : MonoBehaviour
{

    protected NavMeshAgent agent;
    protected Animator anim;

    [Header("Default situation settings")]
    [SerializeField]
    protected float defaultSightDistance = 20;

    [SerializeField]
    protected float defaultFieldOfView = 70;

    [Header("Alert situation settings")]
    [SerializeField]
    protected float alertedSightDistance = 40;

    [SerializeField]
    protected float alertedFieldOfView = 90;

    //how often to check awareness
    protected float awarenessInterval = 0.2f;

    protected float lastAwarenessCheck;

    protected bool lastPlayerSpotted = false;

    [SerializeField]
    protected LayerMask visionBlockingLayers;

    protected Transform head;


    public enum AIState { Idle, Patrol, Investigate, Escort, Guard, Chase }

    [SerializeField]
    protected AIState state;

    [SerializeField]
    protected List<string> patrolAreas;

    //index that represents the current path from patrolAreas list
    protected int currentPathIndex = 0;

    protected int currentWaypointIndex = 0;

    protected int lastWaypoint = 0;
    protected PatrolPath currentPath;


    //the current states represented as a function to be executed every frame
    private Action currentState;



    //values used for animator.SetFloat damp value
    protected float animatorDampValue = 0.15f;

    //Current target for the forward value
    protected float animatorForwardTarget = 0;


    //current target for the turning value
    protected float animatorTurningTarget = 0;

    //character rotation speed
    protected float rotationSpeed = 150;


    //Random looking variables
    protected float lookAtInterval = 4;

    protected float lookAtDuration = 3;

    protected bool lookingAt = false;

    protected float lookAtWeight = 0;
    protected float lookAtWeightTarget = 0;

    protected float lastLookAtTime = 0;

    //how far ahead we check to look
    protected float lookAtDistance = 15;

    //how much space there needs to be horizontally for the lookat to activate
    protected float lookAtWidth = 5;


    protected float lookAtDamp = 1.6f;

    //maximum angle that the look at position can be from relative to the agent's direction
    protected float maxLookAtAngle = 80;

    protected Vector3 lookAtPoint;


    //RANDOM IDLE VARIABLES
    //minimum and maximum times between random idle animations
    protected float randomIdleMin = 30;
    protected float randomIdleMax = 120;
    protected float randomIdleTime;


    //INVESTIGATE STATE VARIABLES
    //the center point of interest

    protected bool playerSpotted = false;
    public bool PlayerSpotted { get { return playerSpotted; } }
    protected Vector3 investigateLocation;

    //how long to investigate the area
    protected float investigateDuration = 10;

    protected float investigateTimer = 0;

    protected float investigateLookatAngle = 70f;

    protected bool investigateLocationReached = false;

    //how many times has this agent investigated an area
    protected int investigationCount = 0;

    //Counter for how many footsteps have been heard in a rapid succession
    protected int footStepsHeard = 0;

    protected int footStepLimit = 3;

    //how long the footstep counter stays before being reset
    protected float footStepMemory = 20;

    //last time when a footstep was heard
    protected float lastFootStepTime;

    bool calculatingPath = false;



    //ESCORT VARIABLES
    protected Vector3 playerLastSeenPosition;

    protected float pathFindingInterval = 0.3f;

    protected float lastPathfindingTime;

    protected float escortStoppingDistance = 3;
    protected float defaultStoppingDistance = 0.4f;

    //how long from last player sighting
    protected float timeSinceLastSighting = 0;

    //how long till player is "lost"
    protected float playerLoseDuration = 5;

    //how long till to escalate to chase
    protected float chaseThreshold = 30;


    //if we have lost player and currently searching for him
    protected bool searchingForPlayer = false;

    protected Vector3 lastPlayerPosition;

    protected float maximumEscortDuration = 160;

    //multiplier when player is going the wrong way
    protected float wrongDirectionPenalty = 3.5f;

    protected float escortTimer = 0;
    protected bool makingEscortProgress;

    //GUARD VARIABLES
    //how long will the agent stay guarding the position
    protected float guardDuration = 20;
    protected float guardTimer = 0;

    [Header("Debug variables")]
    public bool cameraEnabled = false;




    // Use this for initialization
    void Awake()
    {
        //get components
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        head = anim.GetBoneTransform(HumanBodyBones.Head);

        randomIdleTime = UnityEngine.Random.Range(randomIdleMin, randomIdleMax);
    }

    void Start()
    {
        //initialize the currentPath
        currentPath = PatrolPathManager.instance.GetPathWithTag(patrolAreas[currentPathIndex]);

        ChangeState(AIState.Patrol);

        GetComponentInChildren<Camera>().enabled = cameraEnabled;
        GetComponentInChildren<PostProcessVolume>().enabled = cameraEnabled;
        GetComponentInChildren<AudioListener>().enabled = cameraEnabled;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentState != null)
        {
            currentState();
        }


        //DEVELOPER CHEATS
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            anim.speed += 1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            anim.speed -= 1;
        }
    }


    protected virtual void Idle()
    {
        //stay still and only do awareness
        animatorForwardTarget = 0;
        lookingAt = false;
        lookAtWeightTarget = 0;
        playerSpotted = Awareness();

        if (playerSpotted)
        {
            //if the situation has not been escalated yet, change state to escort
            if (AIAlpha.instance.Situation == AIAlpha.SituationState.Normal)
            {
                ChangeState(AIState.Escort);
            }
            else if (AIAlpha.instance.Situation == AIAlpha.SituationState.Alert)
            {
                ChangeState(AIState.Chase);
            }

        }
    }

    protected virtual void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance && agent.pathPending == false)
        {
            currentWaypointIndex++;
            agent.SetDestination(NextWayPoint());
        }

        animatorForwardTarget = 25;
        RotateTowardsSteeringTarget();

        SetAnimatorValues();

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        //look around
        LookAround();

        //check random idle time
        if (Time.time > randomIdleTime)
        {
            anim.CrossFadeInFixedTime("RandomIdle" + UnityEngine.Random.Range(0, 2).ToString(), 0.2f);
            randomIdleTime = Time.time + UnityEngine.Random.Range(randomIdleMin, randomIdleMax);
        }

        //execute awareness = look for player
        playerSpotted = Awareness();

        if (playerSpotted)
        {
            if (Player.instance.InsideRestrictedArea)
            {
                searchingForPlayer = false;
                //if the situation has not been escalated yet, change state to escort
                if (AIAlpha.instance.Situation == AIAlpha.SituationState.Normal)
                {
                    if (AIAlpha.instance.EscortInProgress == false)
                    {
                        AIAlpha.instance.ReportEscort(this);
                        ChangeState(AIState.Escort);
                    }
                    else
                    {
                        lookingAt = true;
                        lookAtPoint = Player.instance.transform.TransformPoint(0, 1.5f, 0);
                        lookAtWeightTarget = 1;
                    }

                }
                else if (AIAlpha.instance.Situation == AIAlpha.SituationState.Alert)
                {
                    ChangeState(AIState.Chase);
                }

            }

        }



        if (searchingForPlayer)
        {
            animatorForwardTarget = 50;
            timeSinceLastSighting += Time.deltaTime;

            if (timeSinceLastSighting > chaseThreshold && AIAlpha.instance.Situation == AIAlpha.SituationState.Normal)
            {
                AIAlpha.instance.ReportPlayerLost();
            }
        }

        //keep track of heard footsteps
        if (Time.time > lastFootStepTime + footStepMemory)
        {
            footStepsHeard = 0;
        }

    }
    protected virtual void LookAround()
    {
        //if we are not already looking at something and we are not in a idle random animation
        if (lookingAt == false && !anim.GetCurrentAnimatorStateInfo(0).IsTag("random"))
        {
            SetLookAtDirection();
        }
        else
        {
            Vector3 localLookAtDirection = transform.InverseTransformDirection(lookAtPoint - transform.position);

            float angle = Mathf.Acos(1 / localLookAtDirection.x) * Mathf.Rad2Deg;

            if ((Time.time > lastLookAtTime + lookAtDuration) || angle > maxLookAtAngle)
            {
                lookingAt = false;
                lookAtWeightTarget = 0;
            }
        }
    }

    protected virtual void LookLeftAndRight()
    {
        Vector3 lookAtDirection = Quaternion.LookRotation(transform.forward) * Quaternion.AngleAxis(Mathf.Sin(Time.time) * investigateLookatAngle, Vector3.up) * Vector3.forward;

        lookingAt = true;
        lookAtWeightTarget = 1;
        lookAtPoint = head.position + lookAtDirection.normalized * lookAtDistance;


        // //Rotate away from a wall
        // RaycastHit hit;
        // Ray ray = new Ray(head.position,transform.forward);
        // float maxWallDistance = 2;

        // if(Physics.Raycast(ray, out hit, maxWallDistance, visionBlockingLayers,QueryTriggerInteraction.Ignore))
        // {
        //     //if left is unblocked
        //     if(!Physics.Raycast(head.position, transform.right*-1, maxWallDistance, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        //     {
        //         transform.Rotate(Vector3.up*Time.deltaTime*rotationSpeed*-1, Space.World);
        //     }
        //     else if(!Physics.Raycast(head.position, transform.right, maxWallDistance, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        //     {
        //         transform.Rotate(Vector3.up*Time.deltaTime*rotationSpeed, Space.World);
        //     }
        // }
    }
    protected virtual bool Awareness()
    {
        //return the latest updated value if it isn't time yet
        if (Time.time < lastAwarenessCheck + awarenessInterval)
            return lastPlayerSpotted;

        lastAwarenessCheck = Time.time;
        Vector3 playerCenterPoint = Player.instance.transform.TransformPoint(0, 1.2f, 0);


        if (!Physics.Linecast(head.position, playerCenterPoint, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(head.forward, playerCenterPoint - head.position);
            float distance = Vector3.Distance(transform.position, playerCenterPoint);

            float fov = AIAlpha.instance.Situation == AIAlpha.SituationState.Normal ? defaultFieldOfView : alertedFieldOfView;
            float sightDistance = AIAlpha.instance.Situation == AIAlpha.SituationState.Normal ? defaultSightDistance : alertedSightDistance;

            if (angle <= fov && distance <= sightDistance)
            {
                lastPlayerSpotted = true;
                playerLastSeenPosition = Player.instance.transform.position;
                timeSinceLastSighting = 0;
                return true;
            }
        }

        lastPlayerSpotted = false;
        return false;



    }

    protected virtual void Investigate()
    {
        //if we have reached the source of the sound
        if ((agent.remainingDistance <= agent.stoppingDistance && agent.pathPending == false) || investigateLocationReached
        || (agent.pathPending == false && agent.pathStatus != NavMeshPathStatus.PathComplete))
        {
            //look around and search a bit
            investigateTimer += Time.deltaTime;

            if (investigateTimer > investigateDuration)
            {
                investigationCount++;
                anim.SetBool("LookingAround", false);
                lookingAt = false;
                lookAtWeightTarget = 0;
                investigateTimer = 0;
                investigateLocationReached = false;
                ChangeState(AIState.Patrol);
                return;
            }

            investigateLocationReached = true;
            animatorForwardTarget = 0;
            agent.velocity = Vector3.zero;
            //look left and right and not towards a wall
            LookLeftAndRight();

            anim.SetBool("LookingAround", true);
        }
        else if (!investigateLocationReached)
        {
            //choose moving speed based on the current situation
            animatorForwardTarget = AIAlpha.instance.Situation == AIAlpha.SituationState.Normal ? 25 : 50;
            RotateTowardsSteeringTarget();
            LookAround();
        }


        playerSpotted = Awareness();

        if (playerSpotted)
        {
            //stop animator from looking around
            anim.SetBool("LookingAround", false);


            //if the situation has not been escalated yet, change state to escort
            if (AIAlpha.instance.Situation == AIAlpha.SituationState.Normal)
            {
                //if no one is escorting the player, start the escort process
                if (AIAlpha.instance.EscortInProgress == false)
                {
                    AIAlpha.instance.ReportEscort(this);
                    ChangeState(AIState.Escort);
                }

            }
            else if (AIAlpha.instance.Situation == AIAlpha.SituationState.Alert)
            {
                ChangeState(AIState.Chase);
            }

        }

        SetAnimatorValues();

    }

    protected virtual void Escort()
    {
        //follow player

        playerSpotted = Awareness();



        if (Time.time > lastPathfindingTime + pathFindingInterval)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(playerLastSeenPosition, out hit, 5, NavMesh.AllAreas);
            if (hit.hit)
            {
                lastPathfindingTime = Time.time;
                agent.SetDestination(hit.position);
            }

        }

        if (agent.remainingDistance >= agent.stoppingDistance)
        {
            //match own speed with player's speed
            //normalize player's speed 0-150 to 0-100
            float playerSpeed = Player.instance.CurrentSpeed;
            playerSpeed = Mathf.Lerp(0, 100, playerSpeed / 150);
            animatorForwardTarget = Mathf.MoveTowards(animatorForwardTarget, Mathf.Clamp(playerSpeed, 25, 100), Time.deltaTime * 35);

            if (animatorForwardTarget < 25)
                animatorForwardTarget = 25;

            RotateTowardsSteeringTarget();
        }
        else
        {
            animatorForwardTarget = 0;
            agent.velocity = Vector3.zero;

            //Rotate towards player

        }

        //if the player is in a unreachable location, stand still if we still see him
        if (agent.pathPending == false && agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            animatorForwardTarget = 0;
            agent.ResetPath();
        }


        if (playerSpotted)
        {
            //if we see that the player has exited the area
            if (Player.instance.InsideRestrictedArea == false)
            {
                AIAlpha.instance.ReportPlayerOutsideArea(this);
            }
            lookingAt = true;
            lookAtWeightTarget = 1;
            lookAtPoint = Player.instance.transform.TransformPoint(0, 1.5f, 0);

            //Check if player is making progress towards the exit
            if (Player.instance.transform.position.y > AIAlpha.groundLevel + AIAlpha.groundLevelThreshold)
            {
                Vector3 delta = Player.instance.transform.position - lastPlayerPosition;
                if (delta.y < 0)
                {
                    //making progress
                    makingEscortProgress = true;
                }
                else
                {
                    //not making progress
                    makingEscortProgress = false;
                }

            }
            else
            {
                float oldDistance = Vector3.Distance(lastPlayerPosition, AIAlpha.instance.ExitPoint);
                float newDistance = Vector3.Distance(Player.instance.transform.position, AIAlpha.instance.ExitPoint);

                if (newDistance < oldDistance)
                {
                    //making progress
                    makingEscortProgress = true;
                }
                else
                {
                    //not making progress
                    makingEscortProgress = false;
                }
            }


            lastPlayerPosition = Player.instance.transform.position;


            //increase escort timer
            if (makingEscortProgress)
            {
                escortTimer += Time.deltaTime;
            }
            else
            {
                escortTimer += Time.deltaTime * wrongDirectionPenalty;
            }

            if (escortTimer > maximumEscortDuration)
            {
                AIAlpha.instance.ReportChase(this);
                ChangeState(AIState.Chase);
            }
        }
        else
        {
            //increase time since last 
            timeSinceLastSighting += Time.deltaTime;

            //change to patrol current or closest floor if player hasn't been seen
            if (timeSinceLastSighting > playerLoseDuration)
            {
                //report to AIalpha to allow other agents to escort the player
                AIAlpha.instance.ReportEscortOver(this);
                ChangeState(AIState.Patrol);

                //get closest floor 
                currentPath = PatrolPathManager.instance.GetClosestFloorPath(playerLastSeenPosition.y);
                currentPathIndex++;
                if (currentPathIndex > patrolAreas.Count - 1)
                    currentPathIndex = 0;

                currentWaypointIndex = currentPath.GetClosestWaypointIndex(transform.position);

                searchingForPlayer = true;
                agent.SetDestination(currentPath.transform.TransformPoint(currentPath.Waypoints[currentWaypointIndex]));
            }
            LookAround();
        }

        SetAnimatorValues();

    }

    protected void Chase()
    {
        //follow player

        playerSpotted = Awareness();



        if (Time.time > lastPathfindingTime + pathFindingInterval)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(playerLastSeenPosition, out hit, 5, NavMesh.AllAreas);
            if (hit.hit)
            {
                lastPathfindingTime = Time.time;
                agent.SetDestination(hit.position);
            }

        }

        if (agent.remainingDistance >= agent.stoppingDistance)
        {
            animatorForwardTarget = 100;

            RotateTowardsSteeringTarget();
        }
        else
        {
            animatorForwardTarget = 0;
            agent.velocity = Vector3.zero;

            //Initiate takedown

        }

        //if the player is in a unreachable location, stand still if we still see him
        if (agent.pathPending == false && agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            animatorForwardTarget = 0;
            agent.ResetPath();
        }


        if (playerSpotted)
        {
            //if we see that the player has exited the area
            if (Player.instance.InsideRestrictedArea == false)
            {
                AIAlpha.instance.ReportPlayerOutsideArea(this);
            }
            lookingAt = true;
            lookAtWeightTarget = 1;
            lookAtPoint = Player.instance.transform.TransformPoint(0, 1.5f, 0);
        }
        else
        {
            //increase time since last 
            timeSinceLastSighting += Time.deltaTime;

            //change to patrol current or closest floor if player hasn't been seen
            if (timeSinceLastSighting > playerLoseDuration)
            {
                //report to AIalpha to allow other agents to escort the player
                ChangeState(AIState.Patrol);

                //get closest floor 
                currentPath = PatrolPathManager.instance.GetClosestFloorPath(playerLastSeenPosition.y);
                currentPathIndex++;
                if (currentPathIndex > patrolAreas.Count - 1)
                    currentPathIndex = 0;

                currentWaypointIndex = currentPath.GetClosestWaypointIndex(transform.position);

                searchingForPlayer = true;
                agent.SetDestination(currentPath.transform.TransformPoint(currentPath.Waypoints[currentWaypointIndex]));
            }
            LookAround();
        }

        SetAnimatorValues();
    }
    protected virtual void Guard()
    {
        animatorForwardTarget = 0;
        LookLeftAndRight();

        Awareness();

        //increase guard progress
        guardTimer += Time.deltaTime;


        //if we have guarded long enough return to patrol and reset guard progress
        if (guardTimer >= guardDuration)
        {
            guardTimer = 0;
            ChangeState(AIState.Patrol);
            return;
        }
        timeSinceLastSighting = 0;

        if (playerSpotted)
        {
            RotateTowardsPlayer();
            if (Player.instance.InsideRestrictedArea)
            {
                //Reset guarding progress
                guardTimer = 0;
                searchingForPlayer = false;
                //if the situation has not been escalated yet, change state to escort
                if (AIAlpha.instance.Situation == AIAlpha.SituationState.Normal)
                {
                    ChangeState(AIState.Escort);
                }
                else if (AIAlpha.instance.Situation == AIAlpha.SituationState.Alert)
                {
                    ChangeState(AIState.Chase);
                }

            }
        }

        SetAnimatorValues();
    }

    public void ChangeState(AIState state)
    {
        this.state = state;

        //reset all timers
        escortTimer = 0;
        guardTimer = 0;
        investigateTimer = 0;
        investigateLocationReached = false;

        if (state == AIState.Patrol)
        {
            //set the action
            currentState = Patrol;
            //start the path from the beginning based on index
            currentPath = PatrolPathManager.instance.GetPathWithTag(patrolAreas[currentPathIndex]);
            agent.stoppingDistance = defaultStoppingDistance;
            agent.SetDestination(NextWayPoint());
        }
        else if (state == AIState.Investigate)
        {
            //Set the action
            agent.stoppingDistance = defaultStoppingDistance;
            currentState = Investigate;
            agent.SetDestination(investigateLocation);
        }
        else if (state == AIState.Escort)
        {
            agent.stoppingDistance = escortStoppingDistance;
            currentState = Escort;
            agent.SetDestination(playerLastSeenPosition);
        }
        else if (state == AIState.Guard)
        {
            agent.stoppingDistance = defaultStoppingDistance;
            currentState = Guard;
            agent.destination = transform.position;
        }
        else if (state == AIState.Chase)
        {
            agent.stoppingDistance = defaultStoppingDistance;
            currentState = Chase;
            agent.destination = playerLastSeenPosition;
        }
    }

    protected IEnumerator CheckReachableLocation(Vector3 location)
    {
        calculatingPath = true;
        NavMeshPath path = new NavMeshPath();

        yield return NavMesh.CalculatePath(transform.position, location, NavMesh.AllAreas, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            agent.destination = location;
        }
        else
        {
            agent.destination = playerLastSeenPosition;
        }
        calculatingPath = false;

    }
    //audio trigger that will be called by the player's footstep script
    public enum AudioTriggerType { Footstep, Elevator, AirDuct };
    public void AudioTrigger(AudioTriggerType type, Vector3 position)
    {
        bool playerSpotted = Awareness();

        //if we hear a player's footstep
        if (type == AudioTriggerType.Footstep)
        {
            //if the player is not seen currently and we are trying to keep up with the player
            //we can have a new destination for the nav mesh agent



            footStepsHeard++;
            lastFootStepTime = Time.time;

            if (footStepsHeard >= footStepLimit)
            {
                investigateLocation = position;
                //We have a clue where the player is
                NavMeshHit hit;
                NavMesh.SamplePosition(position, out hit, 10, NavMesh.AllAreas);

                if (hit.hit)
                    investigateLocation = hit.position;


                //if we don't see player
                if (playerSpotted == false)
                {
                    if (state == AIState.Chase || state == AIState.Escort)
                    {
                        playerLastSeenPosition = position;
                        if (calculatingPath == false)
                        {
                            StartCoroutine(CheckReachableLocation(position));
                        }

                    }
                    else
                    {
                        //check if the sound is coming from the restricted area or NOT!!!!
                        if (Player.instance.InsideRestrictedArea && AIAlpha.instance.EscortInProgress == false)
                        {
                            if (calculatingPath == false)
                            {
                                agent.ResetPath();
                                StartCoroutine(CheckReachableLocation(position));
                            }

                            ChangeState(AIState.Investigate);
                        }

                    }
                }
            }

        }


    }

    //rotates the character towards the nav mesh target
    //ALSO sets the animator turning target
    protected void RotateTowardsSteeringTarget()
    {

        Vector3 rotationDirection = agent.steeringTarget - transform.position;
        rotationDirection.y = 0;

        Vector3 localDirection = transform.InverseTransformDirection(rotationDirection);

        float angle = 0;
        // if (!Mathf.Approximately(localDirection.x, 0))
        //     angle = Mathf.Acos(1 / localDirection.x) * Mathf.Rad2Deg;


        // if(float.IsNaN(angle))
        // angle = 0;
        // animatorTurningTarget = angle;

        float multiplier = Mathf.Lerp(1, 10, 1 - (agent.remainingDistance / 5));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rotationDirection), rotationSpeed * multiplier * Time.deltaTime);
    }

    protected void RotateTowardsPlayer()
    {
        Vector3 dir = Player.instance.transform.position - transform.position;
        dir.y = 0;


    }
    void SetAnimatorValues()
    {
        anim.SetFloat("Forward", animatorForwardTarget, animatorDampValue, Time.deltaTime);
        anim.SetFloat("TurnAngle", animatorTurningTarget, animatorDampValue, Time.deltaTime);
        lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtWeightTarget, Time.deltaTime * lookAtDamp);

    }
    Vector3 NextWayPoint()
    {
        if (currentWaypointIndex > currentPath.Waypoints.Length - 1)
        {
            currentWaypointIndex = 0;

            currentPathIndex++;

            if (currentPathIndex > patrolAreas.Count - 1)
            {
                currentPathIndex = 0;
            }
            currentPath = PatrolPathManager.instance.GetPathWithTag(patrolAreas[currentPathIndex]);
            return currentPath.transform.TransformPoint(currentPath.Waypoints[currentWaypointIndex]);
        }
        else
        {
            return currentPath.transform.TransformPoint(currentPath.Waypoints[currentWaypointIndex]);

        }





    }

    void SetLookAtDirection()
    {
        if (Time.time < lastLookAtTime + lookAtInterval + lookAtDuration)
        {
            return;
        }

        Vector3 point = head.position + transform.forward * lookAtDistance;
        Ray ray = new Ray(point, transform.right);


        //hit something on the forward direction
        if (Physics.Linecast(point, head.position, visionBlockingLayers, QueryTriggerInteraction.Ignore))
            return;

        bool canLookRight = false;
        bool canLookLeft = false;
        if (!Physics.Raycast(ray, lookAtWidth, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        {
            lastLookAtTime = Time.time;
            lookingAt = true;
            lookAtWeightTarget = 1;
            canLookRight = true;
        }

        ray = new Ray(point, transform.right * -1);
        if (!Physics.Raycast(ray, lookAtWidth, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        {
            lastLookAtTime = Time.time;
            lookingAt = true;
            lookAtWeightTarget = 1;
            canLookLeft = true;
        }

        //if both directions are OK
        if (canLookLeft && canLookRight)
        {
            if (UnityEngine.Random.value <= 0.5f)
            {
                lookAtPoint = point + transform.right * -1 * lookAtWidth;
            }
            else
            {
                lookAtPoint = point + transform.right * lookAtWidth;
            }

        }
        else if (canLookLeft)
        {
            lookAtPoint = point + transform.right * -1 * lookAtWidth;
        }
        else if (canLookRight)
        {
            lookAtPoint = point + transform.right * lookAtWidth;
        }

    }
    void OnAnimatorMove()
    {
        Vector3 velocity = anim.deltaPosition / Time.deltaTime;
        agent.velocity = velocity;

        if (agent.remainingDistance <= agent.stoppingDistance)
            agent.velocity = Vector3.zero;
    }

    void OnAnimatorIK()
    {
        Vector3 lookDirection = Quaternion.LookRotation(transform.forward) * Quaternion.AngleAxis(45f * (float)Math.Sin(Time.time), Vector3.up) * Vector3.forward;

        //Set the look at position and keep the y position always at the head's position
        anim.SetLookAtPosition(new Vector3(lookAtPoint.x, head.position.y, lookAtPoint.z));
        anim.SetLookAtWeight(lookAtWeight, 0.2f, 1f, 0, 0.5f);
    }

    //basically return everyone to their patrols, except for the one guy guarding the border
    public virtual void ReturnToPositions()
    {
        timeSinceLastSighting = 0;
        if (state != AIState.Guard)
        {
            ChangeState(AIState.Patrol);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        MovingDoor m = col.GetComponent<MovingDoor>();

        if (m != null)
        {
            m.OpenQueue = true;
        }
    }
    void OnTriggerExit(Collider col)
    {
        MovingDoor m = col.GetComponent<MovingDoor>();

        if (m != null)
        {
            m.CloseQueue = true;
        }
    }
}
