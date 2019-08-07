using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Cam.Effects;
using UnityEngine.SceneManagement;
public class OperatingTableLoop : CloseUpObject
{

    public AudioSource failBreath;
    public AudioSource successBreath;
    public override void OnInteract()
    {
        if (DecayTimer.timeChallengeSuccess)
        {
            successBreath.Play();
        }
		else
		{
			failBreath.Play();
		}

        BreathActivator.breathActivated = false;
        BreathActivator.instance.AudioPlayer.Stop();
        base.OnInteract();
        Player.instance.enabled = false;
        RetroSize.instance.Pixelate(true);
        RetroSize.instance.enabled = true;
		GameObject music = GameObject.FindWithTag("music");
		music.SetActive(false);

        StartCoroutine(LoopRoutine());
    }

    IEnumerator LoopRoutine()
    {

        yield return new WaitForSeconds(10);

		string levelToLoad = DecayTimer.timeChallengeSuccess ? "Victory":"IntermissionText";
        AsyncOperation a = SceneManager.LoadSceneAsync(levelToLoad);

        while (a.isDone == false)
            yield return null;
    }

}
