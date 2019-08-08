using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class AIAlpha : MonoBehaviour
{

    public static AIAlpha instance;

    private List<AIAgent> agents;

    private List<AirductPlacement> airductPlacements;

    public enum SituationState { Normal, Alert };

    [SerializeField]
    private SituationState situation;
    public SituationState Situation { get { return situation; } }

    private bool escortInProgress = false;
    public bool EscortInProgress { get { return escortInProgress; } }

    public static float groundLevel = 4;

    public static float groundLevelThreshold = 2;

    private Transform exitPoint;
    public Vector3 ExitPoint { get { return exitPoint.position; } }

    private bool playerTakenDown = false;
    public bool PlayerTakenDown { get { return playerTakenDown; } }


    //keep track of last investigator agent
    AIAgent lastInvestigatedAgent;


    void Awake()
    {
        instance = this;

        exitPoint = transform.Find("ExitPoint");
    }

    void OnEnable()
    {
        Player.OnRestart += ResetAgents;
    }

    void OnDisable()
    {
        Player.OnRestart -= ResetAgents;
    }
    void Start()
    {
        //Find all agents and assign them to the list
        agents = (FindObjectsOfType(typeof(AIAgent)) as AIAgent[]).ToList();

        airductPlacements = (FindObjectsOfType(typeof(AirductPlacement)) as AirductPlacement[]).ToList();
    }

    //agent calls this when a player is spotted by it
    public void ReportPlayerSpotted(AIAgent agent, Vector3 position)
    {
        //in normal situation send the closest agent to investigate if no one is investigating right now


        if (situation == SituationState.Normal && escortInProgress == false)
        {
            AIAgent nearest = NearestAgent(position);
            nearest.SendToInvestigate(position);

            if (lastInvestigatedAgent == null)
                lastInvestigatedAgent = nearest;

            //Return the previoous investigator back to his patrol
            if (nearest != lastInvestigatedAgent)
            {
                lastInvestigatedAgent.ChangeState(AIAgent.AIState.Patrol);
            }

            lastInvestigatedAgent = nearest;
        }
        else if (situation == SituationState.Alert)
        {
            //if there's less than two chasing the player
            if (ChaserCount() < 2)
            {
                //In the alert mode, send nearest other agent to help the one that reported it first
                AIAgent nearest = NearestAgent(position, agent);

                if (nearest.State != AIAgent.AIState.Chase)
                {
                    nearest.SendToInvestigate(position);
                    lastInvestigatedAgent = nearest;
                }

            }

        }


    }

    private int ChaserCount()
    {
        int count = 0;
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].State == AIAgent.AIState.Chase)
                count++;
        }

        return count;
    }

    //how many agents are currently waiting
    private int WaiterCount()
    {
        int count = 0;
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].State == AIAgent.AIState.Wait)
                count++;
        }

        return count;
    }
    public void ReportPlayerTakeDown(AIAgent agent)
    {
        playerTakenDown = true;
    }

    public void ResetAgents()
    {
        escortInProgress = false;
        playerTakenDown = false;

        for (int i = 0; i < agents.Count; i++)
        {
            agents[i].Reset();
        }
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

        print(agent.gameObject.name);

        //return all possible waiters to their patrols
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].State == AIAgent.AIState.Wait)
            {
                agents[i].ChangeState(AIAgent.AIState.Patrol);
            }
        }
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

    public void ReportAirductEnter(AIAgent agent)
    {

        escortInProgress = false;
        //if there's already 2 agents waiting, ignore this
        if (WaiterCount() >= 2)
        {
            return;
        }

        AIAgent otherNearest = NearestAgent(agent.transform.position, agent);

        AirductPlacement placement = NearestAirduct(agent.transform.position);


        //Send agents to wait at the nearest placement exits
        Transform[] exits = placement.ExitPoints;


        if (exits.Length >= 2)
        {
            agent.SendToWait(exits[0].position);
            otherNearest.SendToWait(exits[1].position);
        }
        else
        {
            agent.SendToWait(exits[0].position);
        }

    }
    public void ReportAirductExit(AIAgent agent)
    {
        escortInProgress = false;
        for (int i = 0; i < agents.Count; i++)
        {
            if (agents[i].State == AIAgent.AIState.Wait)
            {
                agents[i].ChangeState(AIAgent.AIState.Patrol);
            }
        }
    }
    AirductPlacement NearestAirduct(Vector3 position)
    {
        float shortestDistance = Mathf.Infinity;
        AirductPlacement a = airductPlacements[0];

        for (int i = 0; i < airductPlacements.Count; i++)
        {


            float distance = Vector3.Distance(position, airductPlacements[i].transform.position);
            if (distance <= shortestDistance)
            {
                shortestDistance = distance;
                a = airductPlacements[i];
            }
        }

        return a;
    }
    AIAgent NearestAgent(Vector3 position)
    {
        float shortestDistance = Mathf.Infinity;
        AIAgent agent = agents[0];

        for (int i = 0; i < agents.Count; i++)
        {
            //ignore surveillance room agent because he can't leave the room
            if (agents[i].Tag == "Surveillance")
                continue;

            float distance = Vector3.Distance(position, agents[i].transform.position);
            if (distance <= shortestDistance)
            {
                shortestDistance = distance;
                agent = agents[i];
            }
        }

        return agent;
    }
    AIAgent NearestAgent(Vector3 position, AIAgent ignore)
    {
        float shortestDistance = Mathf.Infinity;
        AIAgent agent = agents[0];

        for (int i = 0; i < agents.Count; i++)
        {
            //ignore surveillance room agent because he can't leave the room
            if (agents[i].Tag == "Surveillance")
                continue;

            if (agents[i] == ignore)
                continue;

            float distance = Vector3.Distance(position, agents[i].transform.position);
            if (distance <= shortestDistance)
            {
                shortestDistance = distance;
                agent = agents[i];
            }
        }

        return agent;
    }
    private void ReturnToPositions(AIAgent agent)
    {
        agent.ReturnToPositions();
    }
}
