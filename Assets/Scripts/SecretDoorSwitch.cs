using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoorSwitch : Interactable
{

    bool activated = false;

    public override void Interact()
    {
        if (activated)
            return;

        activated = true;
        GetComponentInParent<Animator>().Play("Activate");

        GetComponent<AudioSource>().Play();
		transform.parent.GetComponent<BoxCollider>().enabled = false;
		transform.parent.gameObject.layer = LayerMask.NameToLayer("Default");

        behaviour.Invoke(functionName, 2);
    }
}
