using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour {
    Animator anim;

    private float runningValueLimit = 110;
    private float joggingValueLimit = 50;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void RunningFootstep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward > runningValueLimit && forward > 10)
        {
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "concrete_run_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
        }
    }

    public void WalkingFootStep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward < joggingValueLimit && forward > 10)
        {
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "concrete_walk_" + Random.Range(1, 6).ToString(), transform.position, transform, 1, 0);
        }
    }

    public void JoggingFootStep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward <= runningValueLimit && forward > 10)
        {
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "concrete_walk_" + Random.Range(1, 6).ToString(), transform.position, transform, 1, 0);
        }
    }
}
