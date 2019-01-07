using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour {

    public float speed = 3;
    Camera cam;
	// Use this for initialization
	void Start () {

        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {

        if (!cam)
            return;

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;

        Vector2 inputVector = new Vector2(Input.GetAxisRaw("KeyboardHorizontal"), Input.GetAxisRaw("KeyboardVertical"));

        if (inputVector.magnitude > 1)
            inputVector.Normalize();

        Vector3 move = camRight * inputVector.x * speed + camForward * inputVector.y*speed;

        transform.Translate(move * Time.deltaTime, Space.World);
	}
}
