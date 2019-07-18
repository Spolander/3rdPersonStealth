using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirductPlacement : MonoBehaviour
{

    [SerializeField]
    private Transform[] exitPoints;


    public Transform[] ExitPoints { get { return exitPoints; } }


    void OnDrawGizmos()
    {
        if (exitPoints != null)
        {
            for (int i = 0; i < exitPoints.Length; i++)
            {
                Gizmos.DrawCube(exitPoints[i].position, Vector3.one);
            }
        }

    }
}
