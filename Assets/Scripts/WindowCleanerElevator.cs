using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCleanerElevator : MonoBehaviour {


    int direction = 0;

    [SerializeField]
    private float speed = 2;


    [SerializeField]
    private AnimationCurve curve;

    [SerializeField]
    private float brakeSpeed = 1;


    bool stopping = false;

    Coroutine stoppingAnimation;

    [SerializeField]
    private float maxYPosition;

    [SerializeField]
    private float minYPosition;


    [SerializeField]
    private LineRenderer[] lines;

    [SerializeField]
    private Transform[] linePoints;


    private void Start()
    {

        Vector3 clampPos = transform.position;
        clampPos.y = Mathf.Clamp(clampPos.y, minYPosition, maxYPosition);
        transform.position = clampPos;

        MatchLines();


    }
    public void GoUp()
    {
        if (direction == -1 && stopping)
            return;


        direction = 1;
    }

    public void GoDown()
    {
        if (direction == 1 && stopping)
            return;


        direction = -1;
    }

    public void Stop()
    {
        if (stopping)
            return;

        StartCoroutine(StopAnimation(direction));
        direction = 0;
    }

    private void Update()
    {

        transform.Translate(Vector3.up * direction * Time.deltaTime * speed, Space.World);

        if (direction == 1)
        {

            MatchLines();
            if (transform.position.y >= maxYPosition)
                Stop();
        }
        else if (direction == -1)
        {
            MatchLines();
            if (transform.position.y <= minYPosition)
                Stop();
        }

      
    }

    void MatchLines()
    {
        
            lines[0].SetPosition(0, linePoints[0].transform.position);
            lines[0].SetPosition(1, linePoints[1].transform.position);
            lines[1].SetPosition(0, linePoints[2].transform.position);
            lines[1].SetPosition(1, linePoints[3].transform.position);
    }

    IEnumerator StopAnimation(int direction)
    {
        
        stopping = true;
        float lerp = 0;
        Vector3 clampPos;
        while (lerp <= 1)
        {
            MatchLines();
            lerp += Time.deltaTime;
            transform.Translate(Vector3.up * direction * Time.deltaTime * brakeSpeed*curve.Evaluate(lerp));
            clampPos = transform.position;
            clampPos.y = Mathf.Clamp(clampPos.y, minYPosition, maxYPosition);
            transform.position = clampPos;


            yield return null;
        }

        stopping = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector3(transform.position.x, minYPosition, transform.position.z), new Vector3(transform.position.x, maxYPosition, transform.position.z));
    }
}
