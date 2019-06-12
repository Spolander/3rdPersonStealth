using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Cam.Effects;
using UnityEngine.SceneManagement;
public class OperatingTable : CloseUpObject {

	public override void OnInteract()
	{	
		GetComponent<AudioSource>().Play();
		BreathActivator.breathActivated = false;
		BreathActivator.instance.AudioPlayer.Stop();
		base.OnInteract();
		Player.instance.enabled = false;
		RetroSize.instance.Pixelate(true);
		RetroSize.instance.enabled = true;

		//Invoke("ResetPlayer",6);
		StartCoroutine(LoopRoutine());
	}

	private void ResetPlayer()
	{
		Player.instance.enabled = true;
		Player.instance.CompleteLoop();
		OutdoorManager.instance.EnteredOutside(true);
	}

	IEnumerator LoopRoutine()
	{
		yield return new WaitForSeconds(6);

		AsyncOperation a = SceneManager.LoadSceneAsync("Intermission");

		while(a.isDone == false)
		yield return null;
	}
	
}
