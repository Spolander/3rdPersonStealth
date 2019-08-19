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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            beepDuration = 0;
            letterDuration = 0;
            enterDuration = 0;
            pauseDuration = 0;
        }
    }

    IEnumerator TextAnimation()
    {

        for (int i = 0; i < lines.Count; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                text.text = text.text + lines[i][j];

                //if we have chosen to skip the text, don't wait for delay
                if (letterDuration > 0)
                {
                    yield return new WaitForSecondsRealtime(letterDuration);
                }

            }
            text.text = text.text + "<br>";

            //if we have chosen to skip the text, don't wait for delay
            if (beepDuration > 0)
            {
                yield return new WaitForSecondsRealtime(beepDuration);
            }


        }

        SceneManager.LoadScene(levelToLoad);
    }

}
