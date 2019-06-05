using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassReflector : MonoBehaviour {

	ReflectionProbe probe;


	public float interval = 5;

	private float timer;
	void Awake()
	{
		probe = GetComponent<ReflectionProbe>();
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;

		if(timer > interval)
		{
			timer = 0;
			probe.RenderProbe();
		}
		
	}
}
