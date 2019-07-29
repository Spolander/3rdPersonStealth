using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ElevatorIndicator : MonoBehaviour {

	TextMeshPro text;

	void Awake()
	{
		text = GetComponentInChildren<TextMeshPro>();
	}

	public void UpdateText(string text)
	{
		this.text.text = text;
	}
}
