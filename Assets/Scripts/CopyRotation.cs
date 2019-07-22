using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{

    [SerializeField]
    private Transform target;

    public static CopyRotation instance;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (target)
        {
            if (target.gameObject.activeInHierarchy == false)
            {
                target = Camera.current.transform;
            }
            transform.rotation = target.rotation;


        }

    }
    public void SetTarget(Transform t)
    {
        target = t;
    }
}
