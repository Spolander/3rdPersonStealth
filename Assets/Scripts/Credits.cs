using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class Credits : MonoBehaviour
{

    public TMP_Text title;

    public TMP_Text meta;

    public float firstPauseDuration = 10;

    public float secondPauseDuration = 7;

    public float thirdPauseDuration = 7;

    public float fourthPauseDuration = 7;

	public float fifthPauseDuration = 7;

	public float finalPause = 7;
    void Awake()
    {
        StartCoroutine(CreditsEffect());
    }
    IEnumerator CreditsEffect()
    {
        title.text = "Mnemonic";
        meta.text = "";

        yield return new WaitForSecondsRealtime(firstPauseDuration);

        title.text = "Game concept and design";
        meta.text = "Spoland";

        yield return new WaitForSecondsRealtime(secondPauseDuration);

        title.text = "Additional art";
        meta.text = "Hermanni Penttala";

        yield return new WaitForSecondsRealtime(thirdPauseDuration);
        title.text = "Misc. audio";
        meta.text = "Freesound.org";

        yield return new WaitForSecondsRealtime(fourthPauseDuration);
        title.text = "Audio design and music";
        meta.text = "Spoland";

		yield return new WaitForSecondsRealtime(fifthPauseDuration);
		title.text = "";
		meta.text = "";

		yield return new WaitForSecondsRealtime(finalPause);

		SceneManager.LoadScene("MainMenu");


    }
}
