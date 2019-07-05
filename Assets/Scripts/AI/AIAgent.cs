using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
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
    protected float animatorDampValue = 0.1f;

    //Current target for the forward value
    protected float animatorForwardTarget = 0;


    //current target for the turning value
    protected float animatorTurningTarget = 0;

    //character rotation speed
    protected float rotationSpeed = 120;


    //Random looking variables
    protected float lookAtInterval = 4;

    protected float lookAtDuration = 3;

    protected bool lookingAt = false;

    protected float lookAtWeight = 0;
    protected float lookAtWeightTarget = 0;

    protected float lastLookAtTime = 0;

    //how far ahead we check to look
    protected float lookAtDistance = 10;

    //how much space there needs to be horizontally for the lookat to activate
    protected float lookAtWidth = 5;

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



    //ESCORT VARIABLES
    protected Vector3 playerLastSeenPosition;

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
        bool playerSpotted = Awareness();
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
        bool playerSpotted = Awareness();


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
                return true;
            }
        }

        lastPlayerSpotted = false;
        return false;



    }

    protected virtual void Investigate()
    {
        //if we have reached the source of the sound
        if ((agent.remainingDistance <= agent.stoppingDistance && agent.pathPending == false) || investigateLocationReached)
        {
            //look around and search a bit
            investigateTimer += Time.deltaTime;

            if (investigateTimer > investigateDuration)
            {
                investigationCount++;
                anim.SetBool("LookingAround",false);
                lookingAt = false;
                lookAtWeightTarget = 0;
                investigateTimer = 0;
                investigateLocationReached = false;
                ChangeState(AIState.Patrol);
                return;
            }

            investigateLocationReached = true;
            animatorForwardTarget = 0;

            //look left and right and not towards a wall
            LookLeftAndRight();

            anim.SetBool("LookingAround",true);
        }
        else if (!investigateLocationReached)
        {
            //choose moving speed based on the current situation
            animatorForwardTarget = AIAlpha.instance.Situation == AIAlpha.SituationState.Normal ? 25 : 50;
            RotateTowardsSteeringTarget();
            LookAround();
        }




        SetAnimatorValues();

    }

    protected virtual void Escort()
    {
        //follow player
        //match own speed with player's speed
        bool playerSpotted = Awareness();

        if(playerSpotted)
        {
            playerLastSeenPosition = Player.instance.transform.position;
        }
    }
    void ChangeState(AIState state)
    {
        this.state = state;

        if (state == AIState.Patrol)
        {
            //set the action
            currentState = Patrol;
            //start the path from the beginning based on index
            currentPath = PatrolPathManager.instance.GetPathWithTag(patrolAreas[currentPathIndex]);

            agent.SetDestination(NextWayPoint());
        }
        else if (state == AIState.Investigate)
        {
            //Set the action
            currentState = Investigate;
            investigateLocationReached = false;
            agent.SetDestination(investigateLocation);
        }
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
                NavMesh.SamplePosition(position, out hit, 4, NavMesh.AllAreas);

                if (hit.hit)
                    investigateLocation = hit.position;

                if (playerSpotted == false)
                {
                    if (state == AIState.Chase || state == AIState.Escort)
                    {
                        agent.SetDestination(position);
                    }
                    else
                    {
                        //check if the sound is coming from the restricted area or NOT!!!!
                        if (Player.instance.InsideRestrictedArea)
                        {

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

    void SetAnimatorValues()
    {
        anim.SetFloat("Forward", animatorForwardTarget, animatorDampValue, Time.deltaTime);
        anim.SetFloat("TurnAngle", animatorTurningTarget, animatorDampValue, Time.deltaTime);
        lookAtWeight = Mathf.MoveTowards(lookAtWeight, lookAtWeightTarget, Time.deltaTime * animatorDampValue);

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
    }

    void OnAnimatorIK()
    {
        Vector3 lookDirection = Quaternion.LookRotation(transform.forward) * Quaternion.AngleAxis(45f * (float)Math.Sin(Time.time), Vector3.up) * Vector3.forward;

        //Set the look at position and keep the y position always at the head's position
        anim.SetLookAtPosition(new Vector3(lookAtPoint.x, head.position.y, lookAtPoint.z));
        anim.SetLookAtWeight(lookAtWeight, 0.2f, 1f, 0, 0.5f);
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
