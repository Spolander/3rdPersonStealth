using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoors : MonoBehaviour {

	[SerializeField]
	Animator[] doors;

	public Animator getDoor(int index)
	{
		return doors[index];
	}

	public void CloseDoor(int index)
	{
		doors[index].GetComponent<Collider>().enabled = true;
		doors[index].Play("Close",0,0.0f);
	}

	public void OpenDoor(int index)
	{
		doors[index].Play("Open",0,0.0f);
	}
}
