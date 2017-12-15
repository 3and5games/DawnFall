using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeTest : MonoBehaviour {

	public Cone cone;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T))
		{
			foreach(Collider c in cone.ConeCast(transform.position, transform.forward))
			{
				c.GetComponent<MeshRenderer> ().material.color = Color.Lerp (c.GetComponent<MeshRenderer> ().material.color, Color.red, 0.3f);
			}
		}
	}
}
