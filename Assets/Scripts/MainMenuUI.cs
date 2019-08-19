using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{

    private bool loading = false;

    public GameObject[] buttons;

    [SerializeField]
    private AudioSource music;

    public MainMenuAnimator animator;

    public AudioSource breath;

    public AudioSource highlightSound;

    public GameObject defaultWrapper;
    public GameObject difficultyWrapper;

    [TextArea(2, 10)]
    [SerializeField]
    private string explorationDescription;

    [TextArea(2, 10)]
    [SerializeField]
    private string normalDescription;

    [TextArea(2, 10)]
    [SerializeField]
    private string nightmareDescription;

    [SerializeField]
    private TMP_Text descriptionText;

    void Start()
    {

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        //disable buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            Button b = buttons[i].GetComponent<Button>();

            if (b != null)
            {
                b.enabled = false;
            }

            buttons[i].SetActive(false);
        }

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
        yield return new WaitForSecondsRealtime(60f / 182f * 8);
        buttons[3].SetActive(true);

        //enable button for pressing
        for (int i = 0; i < buttons.Length; i++)
        {
            Button b = buttons[i].GetComponent<Button>();

            if (b != null)
            {
                b.enabled = true;
            }

        }
    }

    public void OnHighlighted()
    {
        highlightSound.Play();
    }
    public void Initiate()
    {

        // for (int i = 0; i < buttons.Length; i++)
        //     buttons[i].SetActive(false);
        // if (loading == false)
        //     StartCoroutine(loadLevel("GameIntro"));
        defaultWrapper.SetActive(false);
        difficultyWrapper.SetActive(true);


    }

    public void CancelDifficulty()
    {
        defaultWrapper.SetActive(true);
        difficultyWrapper.SetActive(false);
    }
    public void Training()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].SetActive(false);
        if (loading == false)
            StartCoroutine(loadLevel("TrainingIntro"));
    }

    public void HighlightDifficulty(int index)
    {
        string s = explorationDescription;

        if (index == 1)
            s = normalDescription;
        else if (index == 2)
            s = nightmareDescription;

        descriptionText.text = s;
    }

    public void ChooseDifficulty(int value)
    {
        DifficultySettings.GameDifficulty = (DifficultySettings.Difficulty)value;
        defaultWrapper.SetActive(false);
        difficultyWrapper.SetActive(false);
        if (loading == false)
        {
            StartCoroutine(loadLevel("GameIntro"));
        }
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

    IEnumerator loadLevel(string level)
    {
        print("Yea boiii");
        loading = true;

        AsyncOperation a = SceneManager.LoadSceneAsync(level);
        while (a.isDone == false)
        {
            print("Loading");
            yield return null;
        }

    }
}
