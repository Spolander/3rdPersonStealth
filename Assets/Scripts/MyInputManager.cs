using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;
public class MyInputManager : MonoBehaviour {

    public enum ControllerType {Gamepad1, Gamepad2, Keyboard}
    public ControllerType controllerType;

    private bool rightTriggerPressed = false;


    InputDevice device = null;

    [SerializeField]
    [Range(1,2)]
    int controllerNumber = 1;

    [SerializeField]
    private Vector2 sensMultiplier;

    [SerializeField]
    private Vector2 minimumSens;

    private void Start()
    {
        UpdateMappings();
    }
    void UpdateMappings()
    {
        if(InputManager.Devices.Count > 0)
        device = (InputManager.Devices.Count >= controllerNumber) ? InputManager.Devices[controllerNumber - 1] : null;
    }
    private void Update()
    {
        if (device != null)
            if (device.Action1)
                controllerType = ControllerType.Gamepad1;

        if (Input.GetKeyDown(KeyCode.Space))
            controllerType = ControllerType.Keyboard;
    }


    public Vector2 inputVector
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return new Vector2(Input.GetAxisRaw(controllerType.ToString() + "Horizontal"), Input.GetAxisRaw(controllerType.ToString() + "Vertical"));
            else
            {
                if (device == null)
                    return new Vector2();

                return device.LeftStick.Vector;
            }
        }

    }

    public bool JumpButtonDown
    {

        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetButtonDown(controllerType.ToString() + "Jump");
            else
            {
                if (device == null)
                    return false;
                return device.Action1.WasPressed;

            }
        }
    }
    public bool RunButtonHold
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetButton(controllerType.ToString() + "Run");
            else
            {
                if (device == null)
                    return false;

                return device.RightTrigger;
            }
        }
    }

    public Vector2 cameraInput
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return new Vector2(Input.GetAxisRaw(controllerType.ToString() + "CameraX"), Input.GetAxisRaw(controllerType.ToString() + "CameraY"));
            else
            {
                if (device == null)
                    return new Vector2();

                float x = Mathf.Abs(device.RightStick.X) > minimumSens.x ? device.RightStick.X * sensMultiplier.x : 0;
                float y = Mathf.Abs(device.RightStick.Y) > minimumSens.y ? device.RightStick.Y * sensMultiplier.y : 0;
                return new Vector2(x, y);
            }
        }
    }

  

    public bool InteractButtonDown
    {
        get
        {

            if (controllerType == ControllerType.Keyboard)
                return Input.GetButtonDown(controllerType.ToString() + "Interact");
            else
            {
                if (device == null)
                    return false;

                return device.Action3.WasPressed;

            }
        }
    }

    public bool CrouchButtonDown
    {

        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetButtonDown(controllerType.ToString() + "Crouch");
            else
            {
                if (device == null)
                    return false;

                return device.Action2.WasPressed;

            }
        }
    }
}
