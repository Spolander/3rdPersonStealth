using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuUI : MonoBehaviour {

	private bool loading = false;
	public void Initiate()
	{
	
		if(loading == false)
		StartCoroutine(loadLevel());
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	IEnumerator loadLevel()
	{
		loading = true;

		AsyncOperation a = SceneManager.LoadSceneAsync(1);
		while(a.isDone == false)
		yield return null;
	}
}
