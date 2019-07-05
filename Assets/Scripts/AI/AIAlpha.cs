using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
public class AIAlpha : MonoBehaviour {

	public static AIAlpha instance;

	private List<AIAgent> agents;

	public enum SituationState{Normal, Alert};

	[SerializeField]
	private SituationState situation;
	public SituationState Situation{get{return situation;}}

	void Awake()
	{
		instance = this;
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

	}

	//agent calls this when it starts to escort the player out of the premises
	public void ReportEscort(AIAgent agent)
	{

	}
}
