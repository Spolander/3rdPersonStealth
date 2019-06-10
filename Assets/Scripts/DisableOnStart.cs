using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
		GetComponent<Animator>().SetFloat("Forward",Random.Range(0,150));
        Invoke("Disable", 1);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

}
