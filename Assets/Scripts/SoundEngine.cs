using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class SoundEngine : MonoBehaviour
{
    public static SoundEngine instance;


    public AudioClip[] footstepSounds;

    public AudioClip[] playerSounds;

    public AudioClip[] misc;

    public AudioClip[] guard;

    [Space(15)]

    public AudioMixer mixer;

    public enum SoundType { Player, Footstep, Misc, Guard };


    private float footStepPlayTime;
    // Use this for initialization
    void Awake()
    {
        instance = this;
    }

    public void PlaySoundAt(SoundType soundType, string clipName, Vector3 position, Transform parent, float spatial, float delay)
    {
        // if (soundType == SoundType.Footstep && Time.time < footStepPlayTime + 0.2f)
        //     return;

        footStepPlayTime = Time.time;

        AudioClip a = null;
        GameObject g = new GameObject("one shot audio");
        g.transform.position = position;
        AudioSource AS = g.AddComponent<AudioSource>();

        if (soundType == SoundType.Footstep)
        {
            for (int i = 0; i < footstepSounds.Length; i++)
                if (footstepSounds[i].name == clipName)
                    a = footstepSounds[i];

            AS.volume = 0.5f;
        }
        else if (soundType == SoundType.Player)
        {
            for (int i = 0; i < playerSounds.Length; i++)
                if (playerSounds[i].name == clipName)
                    a = playerSounds[i];

        }
        else if (soundType == SoundType.Misc)
        {
            for (int i = 0; i < misc.Length; i++)
                if (misc[i].name == clipName)
                    a = misc[i];
        }
         else if (soundType == SoundType.Guard)
        {
            for (int i = 0; i < guard.Length; i++)
                if (guard[i].name == clipName)
                    a = guard[i];
        }

        if (a == null)
        {
            Destroy(g);
            return;
        }

        AS.clip = a;
        AS.spatialBlend = spatial;
        AS.maxDistance = 25;
        AS.minDistance = 3;
        AS.rolloffMode = AudioRolloffMode.Linear;

        if (soundType != SoundType.Guard)
        {
            AS.outputAudioMixerGroup = mixer.FindMatchingGroups("FX")[0];
        }
        else
        {

            AS.outputAudioMixerGroup = mixer.FindMatchingGroups("GuardVoice")[0];
        }

        AS.PlayDelayed(delay);
        g.transform.SetParent(parent);
        Destroy(g, a.length + delay);
    }

}
