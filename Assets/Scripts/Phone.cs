using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : Interactable
{

    [SerializeField]
    private AudioSource beep;

    [SerializeField]
    private AudioSource clue;



    public override void Interact()
    {
		if(beep.isPlaying || clue.isPlaying)
		return;

		StartCoroutine(soundRoutine());
    }


	IEnumerator soundRoutine()
	{
		beep.Play();
		yield return new WaitForSeconds(beep.clip.length);
		clue.Play();
		yield return new WaitForSeconds(clue.clip.length);
		beep.Play();
	}
}
