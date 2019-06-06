using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    Animator anim;

    private float runningValueLimit = 101;
    private float joggingValueLimit = 90;
    private float walkingValueLimit = 35;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void RunningFootstep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward > runningValueLimit && forward > 10)
        {
            string tag = GetGroundTag();

            if (tag == "metal")
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "footstepMetal" + Random.Range(1, 5).ToString(), transform.position, transform, 1, 0);
            }
            else if (tag == "pipe")
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "airduct_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
            }
            else
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "concrete_run_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
            }

        }
    }

    public void WalkingFootStep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward < joggingValueLimit - walkingValueLimit && forward > 10)
        {
            string tag = GetGroundTag();

            if (tag == "metal")
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "footstepMetal" + Random.Range(1, 5).ToString(), transform.position, transform, 1, 0);
            }
            else if (tag == "pipe")
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "airduct_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
            }
            else
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "concrete_walk_" + Random.Range(1, 6).ToString(), transform.position, transform, 1, 0);
            }

        }
    }

    public void JoggingFootStep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward <= runningValueLimit && forward >= joggingValueLimit && forward > 10)
        {
            string tag = GetGroundTag();

            if (tag == "metal")
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "footstepMetal" + Random.Range(1, 5).ToString(), transform.position, transform, 1, 0);
            }
            else if (tag == "pipe")
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "airduct_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
            }
            else
            {
                SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "concrete_walk_" + Random.Range(1, 6).ToString(), transform.position, transform, 1, 0);

            }
        }
    }

    public void LandingSound()
    {
        string s = GetGroundTag();
        if (s == "metal")
        {
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "footstepMetal" + Random.Range(1, 5).ToString(), transform.position, transform, 1, 0);
        }
        else if (s == "pipe")
        {
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Footstep, "airduct_" + Random.Range(1, 7).ToString(), transform.position, transform, 1, 0);
        }

        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "land",transform.position, transform,1,0);
    }


    string GetGroundTag()
    {
        string s = "";

        RaycastHit hit;

        Ray ray = new Ray(transform.TransformPoint(0, 0.2f, 0), Vector3.down);

        if (Physics.Raycast(ray, out hit, 1, 1 << LayerMask.NameToLayer("Default"), QueryTriggerInteraction.Ignore))
        {
            s = hit.collider.tag;
        }

        return s;
    }
}
