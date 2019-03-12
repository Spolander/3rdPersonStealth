using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardReader : Interactable {

    // Use this for initialization

    [SerializeField]
    private string keycardTag;


    bool used = false;
    public override void Interact()
    {
        if (used)
            return;

        if(Inventory.instance.HasItem(keycardTag))
        {
            used = true;
            base.Interact();
        }
       
    }

}
