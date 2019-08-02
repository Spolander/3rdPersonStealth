using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class VictoryVision : MonoBehaviour
{

    public TMP_Text text;

    [TextArea(1, 5)]
    public string programInfo;

    [TextArea(1, 5)]
    public string copyright;

	//1
    private float beepDuration = 1f;

	//0.06
    private float letterDuration = 0.06f;

	private float enterDuration = 0.02f;

	private float pauseDuration = 2.5f;


    void Start()
    {
        print(System.Environment.UserName);
        print(System.Environment.CurrentDirectory);
        print(System.Environment.Version);

        StartCoroutine(TextAnimation());

    }

    IEnumerator TextAnimation()
    {
        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = programInfo;
        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = text.text + "<br>";
        text.text = text.text + copyright;

        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = text.text + "<br>";


        //Show user path to directory 
        string directory = System.Environment.CurrentDirectory;
        for (int i = 0; i < directory.Length; i++)
        {
            text.text = text.text + directory[i];
            yield return new WaitForSecondsRealtime(letterDuration);
        }

        text.text = text.text + "<br>";
        text.text = text.text + "<br>" + "<";
        yield return new WaitForSecondsRealtime(beepDuration);

        //start showing commmands
        string exitCommand = "exit";
        for (int i = 0; i < exitCommand.Length; i++)
        {
            text.text = text.text + exitCommand[i];
            yield return new WaitForSecondsRealtime(letterDuration);
        }

        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = text.text + "<br>";
        text.text = text.text + "No such command 'exit'";


        text.text = text.text + "<br>" + "<";

        yield return new WaitForSecondsRealtime(beepDuration);



        for (int i = 0; i < exitCommand.Length; i++)
        {
            text.text = text.text + exitCommand[i];
            yield return new WaitForSecondsRealtime(letterDuration);
        }

        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = text.text + "<br>";
        text.text = text.text + "No such command 'exit'";

        text.text = text.text + "<br>" + "<";
        yield return new WaitForSecondsRealtime(beepDuration);
        //End exit commands

        string shutdown = "shutdown";

        for (int i = 0; i < shutdown.Length; i++)
        {
            text.text = text.text + shutdown[i];
            yield return new WaitForSecondsRealtime(letterDuration);
        }

        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = text.text + "<br>";
        text.text = text.text + "No such command 'shutdown'";

        text.text = text.text + "<br>" + "<";
        yield return new WaitForSecondsRealtime(beepDuration);
        //End exit commands

        string stop = "stop";

        for (int i = 0; i < stop.Length; i++)
        {
            text.text = text.text + stop[i];
            yield return new WaitForSecondsRealtime(letterDuration);
        }

        yield return new WaitForSecondsRealtime(beepDuration);
        text.text = text.text + "<br>";
        text.text = text.text + "No such command 'stop'";

        text.text = text.text + "<br>" + "<";

        yield return new WaitForSecondsRealtime(beepDuration);
        string help = "help";

        for (int i = 0; i < help.Length; i++)
        {
            text.text = text.text + help[i];
            yield return new WaitForSecondsRealtime(letterDuration);
        }

        yield return new WaitForSecondsRealtime(beepDuration * 3);
        text.text = text.text + "<br>";
        text.text = text.text + "No such command 'help'";
        text.text = text.text + "<br>";

		yield return new WaitForSecondsRealtime(beepDuration);
        //smash enter

        for (int i = 0; i < 50; i++)
        {
            text.text = text.text+"<" + "<br>";
			yield return new WaitForSecondsRealtime(enterDuration);

        }

		text.text = "";
		text.gameObject.SetActive(false);

		yield return new WaitForSecondsRealtime(pauseDuration);

		SceneManager.LoadScene("Credits");
    }

}
