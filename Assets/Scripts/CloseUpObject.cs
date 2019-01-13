using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpObject : MonoBehaviour {

    [SerializeField]
    Vector3 closeUpDirection;

    [SerializeField]
    Vector3 closeUpPoint;

    [SerializeField]
    private float activationAngle = 90f;

    public float ActivationAngle { get { return activationAngle; } }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.TransformPoint(closeUpPoint), transform.TransformDirection(closeUpDirection));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            CameraFollow.playerCam.ActivateCloseUp(transform.TransformPoint(closeUpPoint), transform.TransformDirection(closeUpDirection), true);
    }
}
