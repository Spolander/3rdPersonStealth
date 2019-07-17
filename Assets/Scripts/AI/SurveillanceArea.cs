using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurveillanceArea : MonoBehaviour
{

    BoxCollider box;
	public static SurveillanceArea instance;
    void Awake()
    {
		instance = this;
        box = GetComponent<BoxCollider>();
    }

	public bool InsideSurveillanceArea(Vector3 pos)
	{
		return box.bounds.Contains(pos);
	}
}
