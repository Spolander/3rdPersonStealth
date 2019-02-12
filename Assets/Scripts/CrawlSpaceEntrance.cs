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
        Gizmos.DrawWireCube(transform.TransformPoint(entrancePoint), Vector3.one * 0.5f);
        Gizmos.DrawWireCube(transform.TransformPoint(exitPoint), Vector3.one * 0.5f);
    }
}
