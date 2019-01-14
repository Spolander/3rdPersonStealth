using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowCleanerElevator : MonoBehaviour {


    int direction = 0;

    [SerializeField]
    private float speed = 2;

    public void GoUp()
    {
        direction = 1;
    }

    public void GoDown()
    {
        direction = -1;
    }

    public void Stop()
    {
        direction = 0;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * direction * Time.deltaTime * speed, Space.World);
    }
}
