using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour {

	public float force = 20;
	public GameObject projectile;
	public float angularForce = 10;

	public void Launch(Vector3 aim)
	{
		GameObject newProjectile = Instantiate (projectile);
		newProjectile.transform.localScale = Vector3.one;
		newProjectile.transform.position = transform.position;
		newProjectile.GetComponent<Rigidbody> ().AddForce ((aim-transform.position).normalized*force);
		newProjectile.GetComponent<Rigidbody> ().AddRelativeTorque (new Vector3(Random.value, Random.value, Random.value).normalized*angularForce);
	}
}
