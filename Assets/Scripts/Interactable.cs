using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public MonoBehaviour behaviour;

    [SerializeField]
    private string functionName;
    public virtual void Interact()
    {

        if (behaviour != null)
            behaviour.Invoke(functionName,0);
    }
}
