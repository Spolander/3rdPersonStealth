using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StabilizeCamera : MonoBehaviour {

	void LateUpdate()
	{
		Vector3 euler = transform.eulerAngles;
		euler.z = 0;
		transform.eulerAngles = euler;
	}
}
