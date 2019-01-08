using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;
public class MyInputManager : MonoBehaviour {

    public enum ControllerType {Gamepad1, Gamepad2, Keyboard}
    public ControllerType controllerType;

    private bool rightTriggerPressed = false;
    private bool jumpButtonPressed = false;
    private bool interactButtonPressed = false;

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

                if (jumpButtonPressed == false)
                {
                    if (device.Action1.IsPressed)
                    {
                        jumpButtonPressed = true;
                        return true;
                    }
                }
                else
                {
                    if (!device.Action1.IsPressed)
                    {
                        jumpButtonPressed = false;
                        return false;
                    }
                }
                return false;
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

                return device.RightStick;
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

    public bool KnifeButtonDown
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetButtonDown(controllerType.ToString() + "Knife");
            else
            {
                if (device == null)
                    return false;

                if (device.RightTrigger.IsPressed && rightTriggerPressed == false)
                {
                    rightTriggerPressed = true;
                    return true;
                }
               
                return false;
            }
        }
    }

    public bool KnifeButtonUp
    {
        get
        {
            if (controllerType == ControllerType.Keyboard)
                return Input.GetButtonUp(controllerType.ToString() + "Knife");
            else
            {

                if (device.RightTrigger.IsPressed == false && rightTriggerPressed)
                {
                    rightTriggerPressed = false;
                    return true;
                }
            }
            return false;
        
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

                if (interactButtonPressed == false)
                {
                    if (device.Action3.IsPressed)
                    {
                        interactButtonPressed= true;
                        return true;
                    }
                }
                else
                {
                    if (!device.Action3.IsPressed)
                    {
                        interactButtonPressed = false;
                        return false;
                    }
                }
                return false;

            }
        }
    }
}
