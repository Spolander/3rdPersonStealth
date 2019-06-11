using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : KeypadButton
{

	[SerializeField]
	private Elevator elevator;
    public override void Interact()
    {
        if (animationInProgress)
            return;

        elevator.CallElevator(keyNumber);
        StartCoroutine(buttonAnimation());
    }

IEnumerator buttonAnimation()
    {

        animationInProgress = true;

        float lerp = 0;

        Vector3 originalPos = transform.position;
        Vector3 targetPos = transform.position - transform.up * 0.015f;
        while (lerp < 1)
        {
            lerp += Time.deltaTime / 0.25f;


            if (lerp <= 0.5f)
            {
                transform.position = Vector3.Lerp(originalPos, targetPos, lerp / 0.5f);
            }
            else
            {
                transform.position = Vector3.Lerp(targetPos,originalPos, (lerp-0.5f) / 0.5f);
            }

            yield return null;
        }

        animationInProgress = false;
    }
}
