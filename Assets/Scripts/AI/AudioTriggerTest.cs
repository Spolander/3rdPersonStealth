using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerTest : MonoBehaviour {


	public float distance = 15;
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.Mouse0))
		Trigger();
	}

	void Trigger()
	{
		Collider[] cols = Physics.OverlapSphere(transform.position, distance, 1 << LayerMask.NameToLayer("Guard"));

		for(int i = 0; i < cols.Length; i++)
		{
			cols[i].GetComponent<AIAgent>().AudioTrigger(AIAgent.AudioTriggerType.Footstep,transform.position);
		}

	}
}
