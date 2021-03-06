﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    Animator anim;

    private float runningValueLimit = 101;
    private float joggingValueLimit = 90;
    private float walkingValueLimit = 35;


    [SerializeField]
    private float audioTriggerDistance = 25;

    [SerializeField]
    private float airductAudioDistance = 15;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void TakeDownStartSound()
    {
        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "playerSlide_1", transform.position, transform, 1, 0);
    }
    public void TakeDownEndSound()
    {

        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "deathLand", transform.position, transform, 1, 0);
    }
    public void RunningFootstep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward > runningValueLimit && forward > 10)
        {
            string tag = GetGroundTag();
            AudioTrigger();

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
            AudioTrigger();
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
            AudioTrigger();
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

        AudioTrigger();
        SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Player, "land", transform.position, transform, 1, 0);
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

    void AudioTrigger()
    {
        if (Elevator.instance == null)
            return;

        if (Player.instance.InCrawlSpace == false && Player.instance.InCrawlSpaceTransition == false)
        {
            if (Elevator.instance.playerInsideElevator() == false)
            {
                Collider[] cols = Physics.OverlapSphere(transform.position, audioTriggerDistance, 1 << LayerMask.NameToLayer("Guard"));

                for (int i = 0; i < cols.Length; i++)
                {
                    cols[i].GetComponent<AIAgent>().AudioTrigger(AIAgent.AudioTriggerType.Footstep, transform.position);
                }
            }
        }


    }

    public void AirductAudioTrigger()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, airductAudioDistance, 1 << LayerMask.NameToLayer("Guard"));

        for (int i = 0; i < cols.Length; i++)
        {
            cols[i].GetComponent<AIAgent>().AudioTrigger(AIAgent.AudioTriggerType.AirDuct, transform.position);
        }
    }
}
