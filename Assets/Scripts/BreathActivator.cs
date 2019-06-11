using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathActivator : MonoBehaviour {

		[SerializeField]
		new AudioSource audio;

		public AudioSource AudioPlayer{get{return audio;}}

	public static bool breathActivated = false;

	[SerializeField]
	private bool enter = true;

	public static BreathActivator instance;

	void Awake()
	{
		instance = this;
	}


	
	// Update is called once per frame
	void Update () {

		if(audio == null)
		return;

		if(breathActivated)
		{
			if(audio.isPlaying == false)
			audio.Play();
			audio.volume = Mathf.MoveTowards(audio.volume,0.5f,Time.deltaTime*0.5f);
		}
		else
		{
			audio.volume = Mathf.MoveTowards(audio.volume,0,Time.deltaTime*0.5f);

			if(Mathf.Approximately(0, audio.volume) && audio.isPlaying)
			{
				audio.Stop();
			}
		}
		
	}

	void OnTriggerEnter(Collider col)
	{
		breathActivated = enter;
		LastNotesMusic.noteMusicActivated = !enter;
	}
}
