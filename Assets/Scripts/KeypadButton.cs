using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadButton : Interactable {


    [Range(0,9)]
    [SerializeField]
    private int keyNumber = 0;

    [SerializeField]
    private bool erase;

    [SerializeField]
    private bool enter;

    [SerializeField]
    private Keypad m_keypad;

    bool animationInProgress = false;



    public override void Interact()
    {
        if (animationInProgress)
            return;

        m_keypad.InputNumber(keyNumber, erase, enter);
        StartCoroutine(buttonAnimation());
    }

    IEnumerator buttonAnimation()
    {

        animationInProgress = true;

        float lerp = 0;

        Vector3 originalPos = transform.position;
        Vector3 targetPos = transform.position - transform.forward * 0.015f;
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
