using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseUpObject : MonoBehaviour {

    [SerializeField]
    Vector3 closeUpDirection;

    public Vector3 CloseUpDirection { get { return transform.TransformDirection(closeUpDirection); } }

    [SerializeField]
    protected Vector3 closeUpPoint;


    public virtual Vector3 CloseUpPoint { get { return closeUpPoint; } }

    [SerializeField]
    Vector3 playerPoint;
    public Vector3 PlayerPoint { get { return transform.TransformPoint(playerPoint); } }


    [SerializeField]
    protected float activationAngle = 90f;

    public float ActivationAngle { get { return activationAngle; } }


    [SerializeField]
    Renderer interactRenderer;

    [SerializeField]
    protected bool parentCamera = true;

    public bool ParentCamera { get { return parentCamera; } }

    public virtual void OnInteract()
    {
        if (interactRenderer)
            interactRenderer.material.SetFloat("_MaxIntensity", 0);
    }

    private void OnDrawGizmosSelected()
    {
        Color c = Color.green;
        c.a = 0.4f;
        Gizmos.color = c;
        Gizmos.DrawRay(transform.TransformPoint(closeUpPoint), transform.TransformDirection(closeUpDirection));
        Gizmos.DrawSphere(transform.TransformPoint(playerPoint), 0.2f);
    }

  
}
