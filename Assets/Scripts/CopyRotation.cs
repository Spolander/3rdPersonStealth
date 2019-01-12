using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour {
    
    [SerializeField]
    private Transform target;
	
	// Update is called once per frame
	void LateUpdate () {

        if (target)
            transform.rotation = target.rotation;
	}
}
