using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour {

    [SerializeField]
    private Vector3 targetPosition;

    [SerializeField]
    private AnimationCurve curve;


    [SerializeField]
    private float animationLength = 1;
    // Use this for initialization

    Vector3 startPos;

    private bool opened = false;
    public void OpenDoor()
    {
        if (opened)
            return;

        //targetPosition = transform.TransformPoint(targetPosition);
        startPos = transform.localPosition;


        StartCoroutine(doorAnimation());

    }

    IEnumerator doorAnimation()
    {
        opened = true;
        float lerp = 0;
        while (lerp < 1)
        {

            lerp += Time.deltaTime / animationLength;

            transform.localPosition = Vector3.Lerp(startPos, targetPosition, curve.Evaluate(lerp));
            yield return null;
        }
    }

}
