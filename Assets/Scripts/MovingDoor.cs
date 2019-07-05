using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDoor : MonoBehaviour
{

    [SerializeField]
    private Vector3 targetPosition;


    [SerializeField]
    private AnimationCurve curve;


    [SerializeField]
    private float animationLength = 1;
    // Use this for initialization

    Vector3 startPos;

    public enum DoorState { Opened, Closed };

    private DoorState state;

    private bool animationInProgress;

    private bool openQueue = false;
    private bool closeQueue = false;

    public bool OpenQueue { set { openQueue = value; } }
    public bool CloseQueue { set { closeQueue = value; } }
    void Awake()
    {
        state = DoorState.Closed;
        startPos = transform.localPosition;
    }
    public void OpenDoor()
    {
        if (state == DoorState.Opened || animationInProgress)
            return;

        //targetPosition = transform.TransformPoint(targetPosition);

        StartCoroutine(doorAnimation());

    }
    void Update()
    {
        if (closeQueue)
        {
            CloseDoor();
        }

        if (openQueue)
        {
            OpenDoor();
        }
    }
    public void CloseDoor()
    {
        if (state == DoorState.Closed|| animationInProgress)
            return;

        //targetPosition = transform.TransformPoint(targetPosition);

        StartCoroutine(doorClose());
    }

    IEnumerator doorAnimation()
    {
        openQueue = false;
        animationInProgress = true;
        state = DoorState.Opened;
        float lerp = 0;
        while (lerp < 1)
        {

            lerp += Time.deltaTime / animationLength;

            transform.localPosition = Vector3.Lerp(startPos, targetPosition, curve.Evaluate(lerp));
            yield return null;
        }
        animationInProgress = false;
    }

    IEnumerator doorClose()
    {
        closeQueue = false;
        animationInProgress = true;
        state = DoorState.Closed;
        float lerp = 0;
        while (lerp < 1)
        {

            lerp += Time.deltaTime / animationLength;

            transform.localPosition = Vector3.Lerp(targetPosition, startPos, curve.Evaluate(lerp));
            yield return null;
        }
        animationInProgress = false;
    }

}
