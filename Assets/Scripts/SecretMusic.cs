using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretMusic : MonoBehaviour {

	new AudioSource audio;

	public static bool secretMusicActivated = false;

	[SerializeField]
	private bool enter = true;

	void Awake()
	{
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

		if(audio == null)
		return;

		if(secretMusicActivated)
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
		secretMusicActivated = enter;
	}
}
