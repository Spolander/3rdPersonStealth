using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GlitchingText : MonoBehaviour
{

    public TMP_Text text;


    public string data;

    public string noise;

    public float interval = 0.5f;

    float timer = 0;

    // Update is called once per frame

    public int firstIndex = 0;
    public int lastIndex = 2;

    void Update()
    {

        timer += Time.deltaTime;

        if (timer > interval)
        {
            timer = 0;

            Noisefy();
        }
    }

    void Noisefy()
    {
        firstIndex++;
        lastIndex++;

        if (firstIndex > data.Length - 1)
            firstIndex = 0;

        if (lastIndex > data.Length - 1)
            lastIndex = 0;

        char[] charArr = data.ToCharArray();
        charArr[firstIndex] = noise[Random.Range(0, noise.Length)]; // freely modify the array
        charArr[lastIndex] = noise[Random.Range(0, noise.Length)]; // freely modify the array

		text.text = charArr.ArrayToString();
    }
}
