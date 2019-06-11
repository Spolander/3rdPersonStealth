using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Cam.Effects;
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
		Invoke("ResetPlayer",6);
	}

	private void ResetPlayer()
	{
		Player.instance.enabled = true;
		Player.instance.CompleteLoop();
		OutdoorManager.instance.EnteredOutside(true);
	}
	
}
