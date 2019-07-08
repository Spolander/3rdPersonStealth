using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour {

	[SerializeField]
	private Vector3[] waypoints;
	
	public Vector3[] Waypoints{get{ return waypoints;}}

	[SerializeField]
	private new string tag;

	public string Tag{get{return tag;}}

	void OnDrawGizmosSelected()
	{
		if(waypoints != null)
		{
			for(int i = 1; i< waypoints.Length; i++)
			{
				Gizmos.DrawLine(transform.TransformPoint(waypoints[i]), transform.TransformPoint(waypoints[i-1]));
			}
		}
	}

	public int GetClosestWaypointIndex(Vector3 source)
	{
		int index = 0;
		float distance = Mathf.Infinity;

		for(int i = 0; i < waypoints.Length; i++)
		{
			float tempDistance = Vector3.Distance(transform.TransformPoint(waypoints[i]), source);
			if(tempDistance < distance)
			{
				index = i;
			}
		}

		return index;
	}
}
