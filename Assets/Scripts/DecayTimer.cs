using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DecayTimer : MonoBehaviour
{

    public TMP_Text timerText;

    private float minutes = 9;
    private float seconds = 4;



    // Update is called once per frame
    void Update()
    {

        seconds -= Time.deltaTime;

        if (seconds <= 0)
        {
            seconds = 60;
            minutes--;
        }

        if (seconds >= 10)
        {
            timerText.text = "0" + minutes.ToString() + ":" + ((int)seconds).ToString();
        }
		else
		{
			timerText.text = "0"+minutes.ToString() + ":0"+((int)seconds).ToString();
		}



    }
}
