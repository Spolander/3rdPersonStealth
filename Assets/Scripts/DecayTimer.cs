using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DecayTimer : MonoBehaviour
{

    public TMP_Text timerText;

    [SerializeField]
    private float minutes = 9;
    [SerializeField]
    private float seconds = 59;

    private bool updating = true;

    public static bool timeChallengeSuccess = true;

    void Awake()
    {
        timeChallengeSuccess = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (updating)
        {
            string str_seconds = "";
            string str_minutes = "";
            seconds -= Time.deltaTime;

            if (seconds <= 0)
            {

                if (minutes == 0)
                {
                    //failed the time challenge
                    updating = false;
                    timerText.text = "00:00";
                    timeChallengeSuccess = false;
                }
                else
                {
                    seconds = 60;
                    minutes--;
                }

                
            }

            if (seconds >= 10)
            {
                str_seconds = ((int)seconds).ToString();
                //timerText.text = "0" + minutes.ToString() + ":" + 
            }
            else
            {
                // timerText.text = "0" + minutes.ToString() + ":0" + ((int)seconds).ToString();
                str_seconds = "0" + ((int)seconds).ToString();
            }

            if (minutes >= 10)
            {
                str_minutes = minutes.ToString();
            }
            else
            {
                str_minutes = "0" + minutes.ToString();
            }

            timerText.text = str_minutes + ":" + str_seconds;


        }



    }
}
