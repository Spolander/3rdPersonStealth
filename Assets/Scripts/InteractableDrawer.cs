using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDrawer : CloseUpObject {

    public AnimationCurve curve;

    [SerializeField]
    private float animationTime;

    [SerializeField]
    private float extrudeAmount = 0.3f;

    bool opened = false;
    public override Vector3 CloseUpPoint { get { return transform.InverseTransformPoint(startingPosition); } }

    Vector3 startingPosition;
    private void Start()
    {
        startingPosition = transform.TransformPoint(closeUpPoint);
    }
    public override void OnInteract()
    {
        base.OnInteract();

        if(opened == false)
        StartCoroutine(openAnimation());
    }

    IEnumerator openAnimation()
    {
        opened = true;
        float lerp = 0;

        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + transform.forward * extrudeAmount;

        while (lerp < 1)
        {
            lerp += Time.deltaTime / animationTime;
            transform.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(lerp));
            yield return null;
        }
       
    }
}
