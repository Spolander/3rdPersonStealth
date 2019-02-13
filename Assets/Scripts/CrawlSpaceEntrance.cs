using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlSpaceEntrance : MonoBehaviour {

    [SerializeField]
    private Vector3 entrancePoint;

    public Vector3 EntrancePoint { get { return transform.TransformPoint(entrancePoint); } }

    [SerializeField]
    private Vector3 exitPoint;
    public Vector3 ExitPoint { get { return transform.TransformPoint(exitPoint); } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.TransformPoint(entrancePoint), Vector3.one * 0.2f);
        Gizmos.DrawCube(transform.TransformPoint(exitPoint), Vector3.one * 0.2f);
    }
}
