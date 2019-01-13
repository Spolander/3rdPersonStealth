using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLookCamera : MonoBehaviour {


    public float speed = 10;

    public float sensitivity = 1;

    Vector3 angles;
    // Use this for initialization

    private void Start()
    {
        angles.y = transform.eulerAngles.y;
    }
    // Update is called once per frame
    void Update () {

        Vector3 moveVector = transform.right * Input.GetAxis("KeyboardHorizontal") * speed + transform.forward * Input.GetAxis("KeyboardVertical")*speed;


        transform.Translate(moveVector * Time.deltaTime, Space.World);

        angles.y += Input.GetAxis("Mouse X") * sensitivity;
        angles.x += Input.GetAxis("Mouse Y") * -1 * sensitivity;

        angles.x = Mathf.Clamp(angles.x, -90, 90);

        transform.rotation = Quaternion.Euler(angles);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);
    }
}
