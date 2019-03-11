using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class CollectableItem : Interactable {

    [SerializeField]
    private GameObject removeAsInteractable;

    public override void Interact()
    {
        if (removeAsInteractable)
            removeAsInteractable.layer = LayerMask.NameToLayer("Default");

        Inventory.instance.AddItem(GetComponent<Item>());
    }
}
