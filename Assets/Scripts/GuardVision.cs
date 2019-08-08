using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class GuardVision : MonoBehaviour
{

    public static GuardVision instance;

    new Camera camera;

    [SerializeField]
    private float duration = 5;

    [SerializeField]
    private float cooldown = 10;

    Coroutine visionEffect;

    [SerializeField]
    private Image progress;

    [SerializeField]
    private Image glassesIcon;

    AudioSource[] sources;

    [SerializeField]
    private AudioMixer mixer;

    void Awake()
    {
        instance = this;
        camera = GetComponent<Camera>();
        progress.enabled = false;
        glassesIcon.enabled = false;

        sources = GetComponents<AudioSource>();
    }

    public void ActivateVision(bool activate)
    {
        if (activate)
        {
            if (visionEffect == null)
            {
                visionEffect = StartCoroutine(Vision());
            }
        }
        else
        {
            if (visionEffect != null)
            {
                StopCoroutine(visionEffect);
                camera.enabled = false;
                glassesIcon.enabled = false;
                progress.enabled = false;

                if (sources[0].isPlaying)
                    sources[1].Play();

                sources[0].Stop();

                mixer.SetFloat("Lowpass1", 22000);
                mixer.SetFloat("Lowpass2", 22000);
                mixer.SetFloat("Lowpass3", 22000);
                mixer.SetFloat("Lowpass4",22000);

                visionEffect = null;
            }
        }

    }

    IEnumerator Vision()
    {
        sources[0].Play();
        camera.enabled = true;

        mixer.SetFloat("Lowpass1", 500);
        mixer.SetFloat("Lowpass2", 500);
        mixer.SetFloat("Lowpass3", 500);
        mixer.SetFloat("Lowpass4", 500);

        yield return new WaitForSecondsRealtime(duration);
        camera.enabled = false;

        progress.enabled = true;
        glassesIcon.enabled = true;

        mixer.SetFloat("Lowpass1", 22000);
        mixer.SetFloat("Lowpass2", 22000);
        mixer.SetFloat("Lowpass3", 22000);
        mixer.SetFloat("Lowpass4", 22000);
        float lerp = 0;
        sources[0].Stop();
        sources[1].Play();
        while (lerp < 1)
        {
            lerp += Time.deltaTime / cooldown;

            progress.fillAmount = lerp;
            yield return null;
        }

        progress.enabled = false;
        glassesIcon.enabled = false;
        visionEffect = null;
    }

}
