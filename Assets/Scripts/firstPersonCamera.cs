using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class firstPersonCamera : MonoBehaviour {
    [SerializeField]
    private float xySpeed = 1;
    [SerializeField]
    private float sensitivity = 1;
    [SerializeField]
    private float upperLimit = -60;
    public float UpperLimit { get { return upperLimit; } set { upperLimit = value; } }

    [SerializeField]
    private float lowerLimit = 60;
    public float LowerLimit { get { return lowerLimit; } set { lowerLimit = value; } }

    Vector3 angles;
    public enum rotAngles {x, y, xy };
    public rotAngles rotationAngles;
    public bool smoother;
    public bool localRotation;
    public Vector3 offset;
    public Transform head;

    [SerializeField]
    private bool headCamera = false;

    public bool HeadCamera { get { return headCamera; }set { headCamera = value; } }

    float headCameraAngleLimit = 45f;


    [SerializeField]
    private MyInputManager input;
    // Use this for initialization

    private float headBobTimer;
    [SerializeField]
    private float headBobSpeed = 1;
    [SerializeField]
    private float headBobStrength = 0.1f;

    Vector3 startingLocalPos;

    private void Awake()
    {
        startingLocalPos = transform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(rotationAngles == rotAngles.y)
        angles.y = transform.eulerAngles.y;
        Application.targetFrameRate = 60;
    }
    // Update is called once per frame
    void LateUpdate () {

       

        if (rotationAngles == rotAngles.y)
        {
            angles.y += input.cameraInput.x * sensitivity;
        }
        else if (rotationAngles == rotAngles.x)
        {
            angles.x += input.cameraInput.y * -1 * sensitivity;
        }
        else
        {
            angles.x += input.cameraInput.y * -1 * sensitivity;
            angles.y += input.cameraInput.x * sensitivity;
        }
      
       
        angles.x = Mathf.Clamp(angles.x, upperLimit, lowerLimit);

        if (headCamera && rotationAngles == rotAngles.xy)
            angles.y = Mathf.Clamp(angles.y, -headCameraAngleLimit, headCameraAngleLimit);
        else if (headCamera && rotationAngles == rotAngles.x)
            angles.y = 0f;

        angles.z = 0;

        if (smoother)
        {
            if (!localRotation)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(angles), Time.deltaTime * xySpeed);
            else
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(angles), Time.deltaTime * xySpeed);
        }
        else
        {
            if (!localRotation)
                transform.rotation = Quaternion.Euler(angles);
            else
                transform.localRotation = Quaternion.Euler(angles);
        }
        
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f);

        if (head)
        {
            transform.position = head.TransformPoint(offset);
        }
       
}

    public void changeHeadMode(bool change)
    {
        if (change)
        {
            rotationAngles = rotAngles.xy;
            Vector3 a = transform.localEulerAngles;
            a.y = 0f;
            transform.localEulerAngles = a;
        }
        else
        {
            rotationAngles = rotAngles.x;
            Vector3 a = transform.localEulerAngles;
            a.y = 0f;
            transform.localEulerAngles = a;
        }
            
    }

    public void resetAngles()
    {
        angles = Vector3.zero;
    }

    //called from player controller
    public void DoHeadBob(float multiplier)
    {
        headBobTimer += Time.deltaTime * headBobSpeed*multiplier;

        if (headBobTimer >= Mathf.PI)
            headBobTimer = 0;

        float y = startingLocalPos.y + Mathf.Sin(headBobTimer) * headBobStrength;
        transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
    }
}
