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
    protected PatrolPath currentPath;


    //the current states represented as a function to be executed every frame
    private Action currentState;



    //values used for animator.SetFloat damp value
    protected float animatorDampValue = 0.2f;

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
    protected float lookAtDistance = 10;

    //how much space there needs to be horizontally for the lookat to activate
    protected float lookAtWidth = 5;

    protected Vector3 lookAtPoint;

    // Use this for initialization
    void Awake()
    {
        //get components
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        head = anim.GetBoneTransform(HumanBodyBones.Head);
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
    }


    protected virtual void Idle()
    {
        //stay still and only do awareness
        animatorForwardTarget = 0;
        bool playerSpotted = Awareness();
    }

    protected virtual void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentWaypointIndex++;
            agent.SetDestination(NextWayPoint());
        }

        animatorForwardTarget = 25;
        RotateTowardsSteeringTarget();

        SetAnimatorValues();

        if (lookingAt == false)
        {
            SetLookAtDirection();
        }
		else
		{
			if(Time.time > lastLookAtTime+lookAtDuration)
			{
				lookingAt = false;
				lookAtWeightTarget = 0;
			}
		}

    }

    protected virtual bool Awareness()
    {
        if (Time.time < lastAwarenessCheck + awarenessInterval)
            return false;

        lastAwarenessCheck = Time.time;
        Vector3 playerCenterPoint = Player.instance.transform.TransformPoint(0, 1.2f, 0);
        return !Physics.Linecast(head.position, playerCenterPoint, visionBlockingLayers, QueryTriggerInteraction.Ignore);



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

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(rotationDirection), rotationSpeed * Time.deltaTime);
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

            if (currentPathIndex > patrolAreas.Count-1)
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
        if (Time.time < lastLookAtTime + lookAtInterval+lookAtDuration)
        {
            return;
        }

        Vector3 point = head.position + transform.forward * lookAtDistance;
        Ray ray = new Ray(point, transform.right);


		//hit something on the forward direction
		if(Physics.Linecast(point, head.position, visionBlockingLayers, QueryTriggerInteraction.Ignore))
		return;

        if (!Physics.Raycast(ray, lookAtWidth, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        {
            lookAtPoint = point + transform.right * lookAtWidth;
            lastLookAtTime = Time.time;
            lookingAt = true;
			lookAtWeightTarget = 1;
            return;
        }

        ray = new Ray(point, transform.right * -1);
        if (!Physics.Raycast(ray, lookAtWidth, visionBlockingLayers, QueryTriggerInteraction.Ignore))
        {
            lookAtPoint = point + transform.right * -1 * lookAtWidth;
            lastLookAtTime = Time.time;
            lookingAt = true;
			lookAtWeightTarget = 1;
            return;
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
}
