using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameplayCanvas : MonoBehaviour {

    [SerializeField]
    private Image deathScreen;

    [SerializeField]
    private TMP_Text deadText;

    Coroutine m_colorAnimation;

    public static GameplayCanvas instance;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        instance = this;
    }

    public void HideUI(bool hide)
    {
        canvas.enabled = !hide;
    }

    private void OnEnable()
    {
        Player.OnDeath += PlayerDeath;
        Player.OnRestart += PlayerReset;
    }

    private void OnDisable()
    {
        Player.OnDeath -= PlayerDeath;
        Player.OnRestart -= PlayerReset;
    }

    private void PlayerDeath()
    {
        if (deathScreen)
            deathScreen.enabled = true;

        if (m_colorAnimation != null)
            StopCoroutine(m_colorAnimation);

        m_colorAnimation = StartCoroutine(colorAnimation());
    }

    private void PlayerReset()
    {
        if (deathScreen)
            deathScreen.enabled = false;

        Color c = deadText.color;
        c.a = 0;
        deadText.color = c;
    }

    IEnumerator colorAnimation()
    {
        float timer = 0;
        Color c = deadText.color;
        while (timer < 1)
        {
            c.a = timer;
            deadText.color = c;
            timer += Time.deltaTime / 4;
            yield return null;
        }

        m_colorAnimation = null;
    }
}
