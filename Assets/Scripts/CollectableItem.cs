using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class CollectableItem : Interactable {

    public override void Interact()
    {
        Inventory.instance.AddItem(GetComponent<Item>());
    }
}
