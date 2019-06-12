using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class VisionAnimator : MonoBehaviour
{


    public RawImage red;

    public RawImage back;

    private bool activated = false;
    public bool Activated { get { return activated; } }

    private float sinTimer;

    private float sinSpeed = 4;

    public static VisionAnimator instance;

    private float visionLength = 3.5f;

    AudioSource visionSound;

    private float minDurationBetweenVisions = 90f;
    private float maxDurationBetweenVisions = 180f;

	[SerializeField]
    private float visionTimer;
    private float visionTimerTarget;

	public static bool visionReached = false;

	[SerializeField]
	private Texture[] backImages;

	int imageNumber = 0;
    // Use this for initialization
    void Awake()
    {
        instance = this;
        visionSound = GetComponent<AudioSource>();
        visionTimer = 0;
        visionTimerTarget = 30;
    }

    public void ActivateVision(int vision)
    {
        visionSound.Play();
        sinTimer = 0;
        activated = true;
        red.enabled = true;
        back.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (activated)
        {
            FlashImages();
        }
        visionTimer += Time.deltaTime;

        if (visionTimer > visionTimerTarget && visionReached == false)
        {
            ActivateVision(1);
            visionTimer = 0;
            visionTimerTarget = Random.Range(minDurationBetweenVisions, maxDurationBetweenVisions);
        }

    }

    void FlashImages()
    {
        sinTimer += Time.deltaTime * sinSpeed;
        red.uvRect = new Rect(Random.insideUnitCircle, Vector2.one);

        Color c = red.color;
        c.a = Mathf.Lerp(5f / 255f, 21f / 255f, (Mathf.Sin(sinTimer) + 1) / 2);
        red.color = c;

        back.uvRect = new Rect(Random.insideUnitCircle * 0.0075f, Vector2.one);

		if(sinTimer > visionLength*sinSpeed*0.5f && imageNumber == 0)
		{
			imageNumber = 1;
			back.texture = backImages[1];
		}

        if (sinTimer > visionLength * sinSpeed)
        {
            activated = false;
            red.enabled = false;
            back.enabled = false;
			imageNumber = 0;
			back.texture = backImages[0];
        }
    }
}
