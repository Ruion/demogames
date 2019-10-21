using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTextMesh : MonoBehaviour {

	public Color TextColor = Color.blue; 

	void Start () { 
		GetComponent<MeshRenderer>().material.color = TextColor;
	}
	
	// Update is called once per frame 
	void Update () { 
		// gameObject.transform.Translate(0, 0.1f, 0, Space.World); 
		TextColor.a = TextColor.a-0.012f; 
		GetComponent<MeshRenderer>().material.color = TextColor;
	} 
}