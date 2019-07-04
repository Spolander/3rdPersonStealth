using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEvents : MonoBehaviour
{
    Animator anim;

    private float runningValueLimit = 101;
    private float joggingValueLimit = 50;
    private float walkingValueLimit = 10;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

  

    public void WalkingFootStep()
    {
        float forward = anim.GetFloat("Forward");

        if (forward < joggingValueLimit && forward > walkingValueLimit)
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
