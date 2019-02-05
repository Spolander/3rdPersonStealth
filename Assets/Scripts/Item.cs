using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [SerializeField]
    private Vector3 inventoryPosition;

    public Vector3 InventoryPosition { get { return inventoryPosition; } }

    [SerializeField]
    private Vector3 inventoryRotation;

    public Vector3 InventoryRotation { get { return inventoryRotation; } }

}
