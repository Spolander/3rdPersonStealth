using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuUI : MonoBehaviour
{

    private bool loading = false;

    public GameObject[] buttons;

    [SerializeField]
    private AudioSource music;

    public MainMenuAnimator animator;

    public AudioSource breath;

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].SetActive(false);

        StartCoroutine(startingAnimation());
    }

    IEnumerator startingAnimation()
    {
        music.Play();
        buttons[0].SetActive(true);
        float delay = 60f / 182f * 8;
        yield return new WaitForSecondsRealtime(delay);
        buttons[1].SetActive(true);
        yield return new WaitForSecondsRealtime(60f / 182f * 8);
        buttons[2].SetActive(true);
    }
    public void Initiate()
    {

        for (int i = 0; i < buttons.Length; i++)
            buttons[i].SetActive(false);
        if (loading == false)
            StartCoroutine(loadLevel());


    }

    public void ExitGame()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].SetActive(false);
        breath.Play();
        music.Stop();
        animator.FadeToBlack();
        StartCoroutine(quitDelay());
       
    }

    IEnumerator quitDelay()
    {
        yield return new WaitForSecondsRealtime(breath.clip.length);
        Application.Quit();
    }

    IEnumerator loadLevel()
    {
        print("Yea boiii");
        loading = true;

        AsyncOperation a = SceneManager.LoadSceneAsync(1);
        while (a.isDone == false)
        {
            print("Loading");
            yield return null;
        }

    }
}
