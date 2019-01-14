using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VirtualCursor : MonoBehaviour {


    Vector3 normalizedPosition = new Vector3(0.5f, 0.5f, 1f);

    [SerializeField]
    private float sensitivity = 1;

    [SerializeField]
    private float controllerSensitivityMultiplier = 3f;

    private float sensitivityMultiplier = 0.2f;

    [SerializeField]
    MyInputManager inputManager;
    // Use this for initialization

    public static VirtualCursor instance;

    Image image;

    private bool active = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        instance = this;
        Activate(false);
    }


    // Update is called once per frame
    void Update () {


        transform.position = new Vector3(Screen.width * normalizedPosition.x, Screen.height * normalizedPosition.y, 1);


        if (inputManager)
        {
            float multiplier = 1;
            if (inputManager.controllerType == MyInputManager.ControllerType.Gamepad1)
                multiplier = controllerSensitivityMultiplier;

            normalizedPosition.x += inputManager.cameraInput.x  * sensitivityMultiplier* Time.deltaTime*multiplier;
            normalizedPosition.y += inputManager.cameraInput.y *(Screen.width / Screen.height) * sensitivityMultiplier * Time.deltaTime*multiplier;
        }
        else
        {
            normalizedPosition.x += Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
            normalizedPosition.y += Input.GetAxisRaw("Mouse Y") * (Screen.width / Screen.height) * sensitivity * Time.deltaTime;
        }
  


        normalizedPosition.x = Mathf.Clamp(normalizedPosition.x, 0, 1);
        normalizedPosition.y = Mathf.Clamp(normalizedPosition.y, 0, 1);
    }

    public Vector3 NormalizedPosition { get { return normalizedPosition; } }
    public Vector3 ScreenPosition { get { return new Vector3(Screen.width * normalizedPosition.x, Screen.height * normalizedPosition.y, 0); } }

    public void Activate(bool enable)
    {
        image.enabled = enable;
        active = enable;
    }
}
