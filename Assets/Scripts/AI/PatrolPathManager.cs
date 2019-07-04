using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PatrolPathManager : MonoBehaviour {

	List<PatrolPath> patrolPaths;

	public static PatrolPathManager instance;

	void Awake()
	{
		instance = this;
		patrolPaths = (FindObjectsOfType(typeof(PatrolPath)) as PatrolPath[]).ToList();
	}
	void Start()
	{
		
	}

	public PatrolPath GetPathWithTag(string tag)
	{
		PatrolPath path = patrolPaths.Find(x => x.Tag == tag);

		return path;
	}

}
