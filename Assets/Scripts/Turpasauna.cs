using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Turpasauna : MonoBehaviour
{

    public static Turpasauna instance;

    [SerializeField]
    private Image background;

    [SerializeField]
    private Image progress;

    [SerializeField]
    private Image fist;

    private float timeSinceActivation = 0;

    private float disappearTime = 3;

    private float fadeSpeed = 0.5f;


    // Use this for initialization
    void Start()
    {

        Color bg = background.color;
        bg.a = 0;

        background.color = bg;

        Color c = progress.material.color;
        c.a = 0;

        progress.material.color = c;

        timeSinceActivation = disappearTime;
    }

	void Awake()
	{
		instance = this;
	}

    void OnEnable()
    {
        Player.OnRestart += OnRestart;
    }

    void OnDisable()
    {
        Player.OnRestart -= OnRestart;
    }

    void OnRestart()
    {
        Color bg = background.color;
        bg.a = 0;

        background.color = bg;

        Color c = progress.material.color;
        c.a = 0;

        progress.material.color = c;

        timeSinceActivation = disappearTime;
    }

    // Update is called once per frame
    void Update()
    {

        timeSinceActivation += Time.deltaTime;

        if (timeSinceActivation > disappearTime)
        {
            Fade(0);
        }
        else
        {
            Fade(1);
        }
    }

    void Fade(float target)
    {
        Color bg = background.color;
        bg.a = Mathf.MoveTowards(bg.a, target, Time.deltaTime * fadeSpeed);

        background.color = bg;

        Color c = progress.material.color;
        c.a = Mathf.MoveTowards(c.a, target, Time.deltaTime * fadeSpeed);

        progress.material.color = c;
    }

    public void UpdateProgress(float value)
    {
        timeSinceActivation = 0;
        progress.fillAmount = value;
    }
}
