using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
	[SerializeField]
	private new string tag;

	public string Tag{get{return tag;}}

	public void ResetCheckpoint()
	{
		GetComponent<Collider>().enabled = true;
	}
}
