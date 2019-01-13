using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpObject : MonoBehaviour {

    [SerializeField]
    Vector3 closeUpDirection;

    public Vector3 CloseUpDirection { get { return transform.TransformDirection(closeUpDirection); } }

    [SerializeField]
    Vector3 closeUpPoint;

    public Vector3 CloseUpPoint { get { return transform.TransformPoint(closeUpPoint); } }

    [SerializeField]
    private float activationAngle = 90f;

    public float ActivationAngle { get { return activationAngle; } }



    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.TransformPoint(closeUpPoint), transform.TransformDirection(closeUpDirection));
    }

  
}
