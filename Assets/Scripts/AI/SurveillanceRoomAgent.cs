using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class SurveillanceRoomAgent : AIAgent
{

    [SerializeField]
    private Transform chair; //Target object for the agent destination


    [SerializeField]
    private Vector3 chairOffset; //offset for the destination

    private Vector3 chairMatchingPosition = new Vector3(-0.5f, -0.1f, 0f);

    protected override void Chase()
    {
        //chase normally unless the player is outside of the surveillance room, then return to patrol
        if (SurveillanceArea.instance.InsideSurveillanceArea(Player.instance.transform.position) == false)
        {
            ChangeState(AIState.Patrol);
            return;
        }
        base.Chase();
    }
    protected override void Patrol()
    {
        playerSpotted = Awareness();
        SetAnimatorValues();
        MatchAnimator();

        //if we are moving towards the chair
        if (agent.enabled)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && agent.pathPending == false && agent.hasPath)
            {
                animatorForwardTarget = 0;

                //if agent is enabled, disable agent and start sitting
                if (agent.enabled)
                {
                    matchingRotation = Quaternion.LookRotation(-chair.transform.right);
                    originalRotation = transform.rotation;
                    matchingPosition = chair.TransformPoint(chairMatchingPosition);
                    agent.enabled = false;
                    anim.SetBool("Sitting", true);
                }


            }
            else
            {
                //rotat towards target
                RotateTowardsSteeringTarget();
                animatorForwardTarget = 25;
            }
        }



        if (playerSpotted)
        {
            //look at player and don't sit
            lookingAt = true;
            lookAtPoint = Player.instance.transform.TransformPoint(0, 1.5f, 0);
            lookAtWeightTarget = 1;
            animatorForwardTarget = 0;
            anim.SetBool("Sitting", false);

            if (Player.instance.InsideRestrictedArea)
            {
                AIAlpha.instance.ReportPlayerSpotted(this, Player.instance.transform.position);

                //only during alert try to takedown the player
                if (SurveillanceArea.instance.InsideSurveillanceArea(Player.instance.transform.position))
                {
                    if (AIAlpha.instance.Situation == AIAlpha.SituationState.Alert)
                    {
                        ChangeState(AIState.Chase);
                        return;
                    }
                }
            }

        }
        else
        {
            lookingAt = false;

            if (agent.enabled)
            {
                if (agent.hasPath == false)
                {
                    ChangeState(AIState.Patrol);
                }
            }
        }


        //keep track of heard footsteps
        if (Time.time > lastFootStepTime + footStepMemory)
        {
            footStepsHeard = 0;
        }


    }
    public override void AudioTrigger(AudioTriggerType type, Vector3 position)
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

                AIAlpha.instance.ReportPlayerSpotted(this, investigateLocation);
            }

        }


    }

    public override void ChangeState(AIState state)
    {

        base.ChangeState(state);

        if (state == AIState.Patrol)
        {

            NavMeshHit hit;
            NavMesh.SamplePosition(chair.TransformPoint(chairOffset), out hit, 1, NavMesh.AllAreas);

            if (hit.hit)
            {
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            anim.SetBool("Sitting", false);
        }
    }
}
