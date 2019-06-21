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
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Misc,"keypadSuccess",transform.position,null,1,0);
        }
        else
        {
            SoundEngine.instance.PlaySoundAt(SoundEngine.SoundType.Misc,"keypadFail",transform.position,null,1,0);
        }
       
    }

}
