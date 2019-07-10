using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictedAreaManager : MonoBehaviour {

	BoxCollider[] colliders;

	public static RestrictedAreaManager instance;

	void Awake()
	{
		colliders = GetComponents<BoxCollider>();
		instance = this;
	}

	public bool OutsideRestrictedArea(Vector3 source)
	{
		for(int i = 0; i < colliders.Length; i++)
		{
			if(colliders[i].bounds.Contains(source))
			return true;
		}

		return false;
	}	
}
