using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkAmbient : MonoBehaviour {

	new AudioSource audio;

	public static bool darkAmbientActivated = false;

	void Awake()
	{
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

		if(darkAmbientActivated)
		{
			if(audio.isPlaying == false)
			audio.Play();
			audio.volume = Mathf.MoveTowards(audio.volume,1,Time.deltaTime*0.5f);
		}
		else
		{
			audio.volume = Mathf.MoveTowards(audio.volume,0,Time.deltaTime*0.5f);
		}
		
	}
}
