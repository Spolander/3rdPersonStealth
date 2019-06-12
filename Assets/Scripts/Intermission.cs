using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Intermission : MonoBehaviour {

	public RawImage red;

    public RawImage back;


    private float sinTimer;

    private float sinSpeed = 4;

	public AudioSource vision;

	public AudioSource visionLoop;


	bool fade = false;
	
	// Update is called once per frame
	void Update () {

		FlashImages();
		
		if(fade)
		FadeImages();
	}
	void Start()
	{
		StartCoroutine(visionRoutine());
	}
	void FadeImages()
	{
		sinTimer += Time.deltaTime;

		Color black = Color.Lerp(Color.white, Color.black, sinTimer/2);
		back.color = black;
	}
	void FlashImages()
    {
        sinTimer += Time.deltaTime * sinSpeed;
        red.uvRect = new Rect(Random.insideUnitCircle, Vector2.one);

        Color c = red.color;
        c.a = Mathf.Lerp(5f / 255f, 78f / 255f, (Mathf.Sin(sinTimer) + 1) / 2);
        red.color = c;



    }

	IEnumerator visionRoutine()
	{
		VisionAnimator.visionReached = false;
		yield return new WaitForSeconds(25);
		visionLoop.Stop();
		vision.Play();
		sinTimer = 0;
		fade = true;

		yield return new WaitForSeconds(vision.clip.length);

		AsyncOperation a = SceneManager.LoadSceneAsync("Loop2");

		while(a.isDone == false)
		yield return null;

	}
}
