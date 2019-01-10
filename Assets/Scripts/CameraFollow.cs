using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CameraFollow : MonoBehaviour {

    private Transform lockOntarget;
    public Transform LockOnTarget { get { return lockOntarget; } set { lockOntarget = value; } }

    private Transform player;

    public float defaultDistance;
  


    public float lockOnDistance;
    public float height = 2;

   

    [SerializeField]
    private float defaultLookAtHeight = 1;



    [SerializeField]
    private float lockOnLookAtHeight = 1;

    [Header("Camera values when the camera is nearest to the player")]
    public float minimumDistance;
    public float minimumHeight = 1;
    [SerializeField]
    private float minimumLookAtHeight = 1;

    [Space]

    public float moveSpeed = 300;
    public float rotateSpeed = 300;

    private float rotationAngleY;

    public float RotationAngleY { set { rotationAngleY = value; } }

    private float rotationAngleX;

    [SerializeField]
    private float minimumXRotation = -20;

    [SerializeField]
    private float maximumXRotation = 60;

    [SerializeField]
    private float sensitivityX = 100;

    [SerializeField]
    private float sensitivityY = 100;


    public static CameraFollow playerCam;

    [SerializeField]
    private LayerMask cameraBlockingLayers;

    MyInputManager input;
	// Use this for initialization

    private void Awake()
    {
        input = FindObjectOfType(typeof(MyInputManager)) as MyInputManager;
        playerCam = this;
       // rotationAngleY = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rotationAngleX = -transform.localEulerAngles.x;
        rotationAngleY = transform.localEulerAngles.y-180;
    }
    // Update is called once per frame
    void LateUpdate () {

        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


        if (player)
        {
            if (input == null)
            {
                rotationAngleY += Time.deltaTime * Input.GetAxisRaw("Mouse X") * sensitivityX;
                rotationAngleX += Time.deltaTime * Input.GetAxisRaw("Mouse Y") * sensitivityY;
            }
            else
            {
                rotationAngleY += Time.deltaTime * input.cameraInput.x * sensitivityX;
                rotationAngleX += Time.deltaTime * input.cameraInput.y * sensitivityY;
            }

            ClampRotationX();
            Quaternion rotation = Quaternion.Euler(rotationAngleX, rotationAngleY, 0);

                Vector3 targetPosition = player.position + rotation * Vector3.forward * defaultDistance + Vector3.up * height;
                RaycastHit hit;

                float lerp = 1;
                if (Physics.Linecast(player.position + Vector3.up, targetPosition, out hit, cameraBlockingLayers, QueryTriggerInteraction.Ignore))
                {
                    lerp = hit.distance / defaultDistance;
                }

                if(hit.collider)
                targetPosition = player.position + rotation * Vector3.forward * Mathf.Lerp(minimumDistance,hit.distance,lerp) + Vector3.up * Mathf.Lerp(minimumHeight, height,lerp);
                else
                targetPosition = player.position + rotation * Vector3.forward * Mathf.Lerp(minimumDistance, defaultDistance, lerp) + Vector3.up * Mathf.Lerp(minimumHeight, height, lerp);

            transform.position = targetPosition;
                transform.rotation = Quaternion.LookRotation((player.position + Vector3.up * Mathf.Lerp(minimumLookAtHeight, defaultLookAtHeight,lerp)) - transform.position);
            
        }
		
	}

    void ClampRotationX()
    {
        if (rotationAngleX < minimumXRotation)
            rotationAngleX = minimumXRotation;
        else if (rotationAngleX > maximumXRotation)
            rotationAngleX = maximumXRotation;
    }
}
