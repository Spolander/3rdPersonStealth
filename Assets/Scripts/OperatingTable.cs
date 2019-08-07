using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Cam.Effects;
using UnityEngine.SceneManagement;
public class OperatingTable : CloseUpObject {

	private float timer;
	void Update()
	{
		timer += Time.deltaTime;
	}
	public override void OnInteract()
	{	
		GetComponent<AudioSource>().Play();
		BreathActivator.breathActivated = false;
		BreathActivator.instance.AudioPlayer.Stop();
		base.OnInteract();
		Player.instance.enabled = false;
		RetroSize.instance.Pixelate(true);
		RetroSize.instance.enabled = true;

		StartCoroutine(LoopRoutine());
	}

	IEnumerator LoopRoutine()
	{
		
		yield return new WaitForSeconds(10);

		string levelToLoad = "Intermission";

		if(timer < 60f*10f)
		levelToLoad = "Victory";
		
		AsyncOperation a = SceneManager.LoadSceneAsync(levelToLoad);

		while(a.isDone == false)
		yield return null;
	}
	
}
