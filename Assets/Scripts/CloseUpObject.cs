using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpObject : MonoBehaviour {

    [SerializeField]
    Vector3 closeUpDirection;

    public Vector3 CloseUpDirection { get { return transform.TransformDirection(closeUpDirection); } }

    [SerializeField]
    Vector3 closeUpPoint;


    public Vector3 CloseUpPoint { get { return closeUpPoint; } }

    [SerializeField]
    Vector3 playerPoint;
    public Vector3 PlayerPoint { get { return transform.TransformPoint(playerPoint); } }


    [SerializeField]
    private float activationAngle = 90f;

    public float ActivationAngle { get { return activationAngle; } }



    private void OnDrawGizmosSelected()
    {
        Color c = Color.green;
        c.a = 0.4f;
        Gizmos.color = c;
        Gizmos.DrawRay(transform.TransformPoint(closeUpPoint), transform.TransformDirection(closeUpDirection));
        Gizmos.DrawSphere(transform.TransformPoint(playerPoint), 0.2f);
    }

  
}
