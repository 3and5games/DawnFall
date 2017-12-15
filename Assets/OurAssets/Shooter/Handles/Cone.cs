using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cone {

	public float radius;
	public float angle;
	//public Vector3 offsetRotation;

	public Cone(float radius, float angle)
	{
		this.radius = radius;
		this.angle = angle;
		//this.offsetRotation = offsetRotation;
	}

	public Collider[] ConeCast(Vector3 position, Vector3 direction)
	{
		List<Collider> result = new List<Collider> ();
		Collider[] overlaped = Physics.OverlapSphere (position, radius);
		foreach(Collider c in overlaped)
		{
			if(Vector3.Angle(c.transform.position - position, direction)<=angle/2)
			{
				result.Add (c);
			}
		}

		return result.ToArray ();
	}
}
