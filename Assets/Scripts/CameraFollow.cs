using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
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

    //Distance at wich the camera is from the close up target object
    private float closeUpDistance = 0.5f;
    private float closeUpStartDistance = 0.6f;
    private Transform closeUpTarget;
    private Vector3 closeUpTargetLocation;
    private Vector3 closeUpDirection;
    private bool closeUp;
    private float closeUpStartTime;


    //crawlspace variables
    private bool inCrawlSpace = false;
    private Vector3 crawlSpacePlayerPoint = new Vector3(0, 0.75f, 0);

    CinemachineBrain brain;

    private void Awake()
    {
        brain = GetComponent<CinemachineBrain>();
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



        if (closeUp)
        {
            if (closeUpTarget)
            {
                transform.position = Vector3.Slerp(closeUpTarget.TransformPoint(closeUpTargetLocation) + closeUpDirection * closeUpStartDistance, closeUpTarget.TransformPoint(closeUpTargetLocation) + closeUpDirection * closeUpDistance, (Time.time - closeUpStartTime) / 0.5f);
                transform.rotation = Quaternion.LookRotation(closeUpTarget.TransformPoint(closeUpTargetLocation) - transform.position);
            }
            else
            {
                transform.position = Vector3.Slerp(closeUpTargetLocation + closeUpDirection * closeUpStartDistance, closeUpTargetLocation + closeUpDirection * closeUpDistance, (Time.time - closeUpStartTime) / 0.5f);
                transform.rotation = Quaternion.LookRotation(closeUpTargetLocation- transform.position);
            }
        }
        else if (inCrawlSpace)
        {
            rotationAngleX -= Time.deltaTime * sensitivityX * Input.GetAxisRaw("Mouse Y");
            ClampRotationX();
            transform.localEulerAngles = new Vector3(rotationAngleX, 0, 0);
        }
		
	}

    void ClampRotationX()
    {
        if (rotationAngleX > minimumXRotation)
            rotationAngleX = minimumXRotation;
        else if (rotationAngleX < maximumXRotation)
            rotationAngleX = maximumXRotation;
    }

    public void ActivateCloseUp(Transform target, Vector3 location, Vector3 targetDirection, bool activate)
    {
        closeUpStartTime = Time.time;
        closeUp = activate;

        if (closeUp == false)
        {
            if (brain)
                brain.enabled = true;
            return;
        }
          

        closeUpTarget = target;
        closeUpTargetLocation = location;
        closeUpDirection = targetDirection;

        if (brain)
            brain.enabled = false;

    }

    public void ActivateCrawlSpaceMode(bool activate)
    {
        if (activate)
        {
            brain.enabled = false;
            inCrawlSpace = true;

            transform.SetParent(Player.instance.transform);
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = crawlSpacePlayerPoint;

        }
        else
        {
            brain.enabled = true;
            inCrawlSpace = false;
            transform.SetParent(null);
        }
    }
}
