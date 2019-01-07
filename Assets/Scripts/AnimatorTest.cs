using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTest : MonoBehaviour {

    Animator[] anims;

    MyInputManager inputManager;
	// Use this for initialization
	void Start () {
        anims = GetComponentsInChildren<Animator>();
        inputManager = transform.root.GetComponentInChildren<MyInputManager>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(anims[1].GetBool("aiming"))
            anims[1].Play("ShotgunAimShoot",0,0);
            else
                anims[1].Play("ShotgunShoot", 0, 0);
            anims[2].Play("Shoot", 0, 0);
        }

        anims[1].SetBool("aiming", Input.GetKey(KeyCode.Mouse1));

	}
}
