using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanciateObjectOnClick : MonoBehaviour {
	Ray ray;
	public GameObject prefab;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject obj=Instantiate(prefab,new Vector3(pos.x+0.5f,pos.y,-3), Quaternion.identity) as GameObject;	
			Destroy(obj,0.3f);
		}

	}
}
