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

	public PatrolPath GetClosestFloorPath(float height)
	{
		float y = Mathf.Infinity;
		int pathIndex = 0;

		for(int i = 0;i < patrolPaths.Count; i++)
		{

			float distance = Mathf.Abs(patrolPaths[i].transform.position.y-height);


			if(distance < y)
			{
				pathIndex = i;
				y = distance;
			}
		}

		return patrolPaths[pathIndex];
	}

}
