using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour {

	public float force = 20;
	public GameObject projectile;
	public float angularForce = 10;
	public bool throwWeapon = false;
	public float delay = 0;
	private Vector3 aim;

	public void Launch(Vector3 aim)
	{
		this.aim = aim;
		Invoke ("LaunchProjectile", delay);
	}

	private void LaunchProjectile()
	{
		GameObject newProjectile = Instantiate (projectile);
		if(throwWeapon)
		{
			RangeWeapon oldObject = GetComponentInParent<RangeWeapon> ();

			newProjectile.transform.SetParent (oldObject.transform.parent);
			newProjectile.transform.localScale = oldObject.transform.localScale;
			newProjectile.transform.localRotation = oldObject.transform.localRotation;
			newProjectile.transform.localPosition = oldObject.transform.localPosition;
			newProjectile.transform.SetParent (null);
			oldObject.HideVisual ();
		}
		else
		{
			newProjectile.transform.localScale = Vector3.one;
			newProjectile.transform.position = transform.position;
		}
		newProjectile.GetComponent<Rigidbody> ().AddForce ((aim-transform.position).normalized*force);
		newProjectile.GetComponent<Rigidbody> ().AddRelativeTorque (new Vector3(Random.value, Random.value, Random.value).normalized*angularForce);
	}
}
