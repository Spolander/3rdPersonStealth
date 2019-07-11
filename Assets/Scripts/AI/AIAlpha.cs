using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class AIAlpha : MonoBehaviour
{

    public static AIAlpha instance;

    private List<AIAgent> agents;

    public enum SituationState { Normal, Alert };

    [SerializeField]
    private SituationState situation;
    public SituationState Situation { get { return situation; } }

    private bool escortInProgress =  false;
    public bool EscortInProgress{get{return escortInProgress;}}

    public static float groundLevel = 4;

    public static float groundLevelThreshold = 2;

    private Transform exitPoint;
    public Vector3 ExitPoint{get{return exitPoint.position;}}

    void Awake()
    {
        instance = this;

        exitPoint = transform.Find("ExitPoint");
    }

    void Start()
    {
        //Find all agents and assign them to the list
        agents = (FindObjectsOfType(typeof(AIAgent)) as AIAgent[]).ToList();
    }
    //agent calls this when a player is spotted by it
    public void ReportPlayerSpotted(AIAgent agent)
    {

    }

    //agent calls this when the agent's behavior changes to chase
    public void ReportChase(AIAgent agent)
    {
        situation = SituationState.Alert;
    }

    //agent calls this when it starts to escort the player out of the premises
    public void ReportEscort(AIAgent agent)
    {
        escortInProgress = true;
    }
    public void ReportEscortOver(AIAgent agent)
    {
        escortInProgress = false;
    }

    //agent calls this when the player exited the area
    public void ReportPlayerOutsideArea(AIAgent agent)
    {
        escortInProgress = false;
        
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i] != agent)
                agents[i].ReturnToPositions();
        }

        //send the caller to guard
        agent.ChangeState(AIAgent.AIState.Guard);
    }

    public void ReportPlayerLost()
    {
        //if no one sees the player, escalate situation
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].PlayerSpotted)
                return;
        }

        situation = SituationState.Alert;
    }

    private void ReturnToPositions(AIAgent agent)
    {
        agent.ReturnToPositions();
    }
}
