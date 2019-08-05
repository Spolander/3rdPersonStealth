using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class TrainingIntro : MonoBehaviour
{

    public TMP_Text text;


    //1
    private float beepDuration = 1f;

    //0.06
    private float letterDuration = 0.06f;

    private float enterDuration = 0.02f;

    private float pauseDuration = 2.5f;

    [TextArea(2, 10)]
    public List<string> lines;


    public string levelToLoad;

    void Start()
    {


        StartCoroutine(TextAnimation());

    }

    IEnumerator TextAnimation()
    {

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                text.text = text.text + lines[i][j];
                yield return new WaitForSecondsRealtime(letterDuration);
            }
            text.text = text.text + "<br>";
            yield return new WaitForSecondsRealtime(beepDuration);

        }

        SceneManager.LoadScene(levelToLoad);
    }

}
