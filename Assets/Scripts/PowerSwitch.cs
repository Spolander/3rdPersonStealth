﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSwitch : Interactable
{

    bool activated = false;

    public override void Interact()
    {
        if (activated)
            return;

        activated = true;
        GetComponentInParent<Animator>().Play("Activate");

        //activate window cleaner stuff

        WindowCleanerElevator[] elevators = FindObjectsOfType(typeof(WindowCleanerElevator)) as WindowCleanerElevator[];

        for (int i = 0; i < elevators.Length; i++)
        {
			elevators[i].GoDown();
        }

		transform.parent.GetComponent<BoxCollider>().enabled = false;
		transform.parent.gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
