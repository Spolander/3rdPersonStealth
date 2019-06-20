using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class VictoryVision : MonoBehaviour {

	public RawImage red;

    private float sinTimer;

    private float sinSpeed = 4;

	public AudioSource vision;

	public AudioSource visionLoop;

	
	// Update is called once per frame
	void Update () {

		FlashImages();
		
	}
	void Start()
	{
		StartCoroutine(visionRoutine());
	}
	void FlashImages()
    {
        sinTimer += Time.deltaTime * sinSpeed;
        red.uvRect = new Rect(Random.insideUnitCircle, Vector2.one);

        Color c = red.color;
        c.a = Mathf.Lerp(0.5f,1, (Mathf.Sin(sinTimer) + 1) / 2);
        red.color = c;



    }

	IEnumerator visionRoutine()
	{
		VisionAnimator.visionReached = false;
		vision.Play();
		yield return new WaitForSeconds(vision.clip.length);
		
		sinTimer = 0;

		float timer = 0;
		float max = visionLoop.volume;
		while(timer < 1)
		{
			visionLoop.volume = (1-timer)*max;
			timer += Time.deltaTime/3;
			yield return null;
		}

		yield return new WaitForSeconds(1);

		Application.Quit();

	}
}
