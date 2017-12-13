using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public float ExplosionDelay = 1;
	public float ExplosionForce = 100;
	public float ExplosionRadius = 3;
	public GameObject ExplosionPrefab;

	public bool DestroyRigidbodyAfterHit = false;
	public bool DestroyColliderAfterHit = true;

	void OnCollisionEnter()
	{
		if(DestroyRigidbodyAfterHit)
		{
			Destroy(GetComponent<Rigidbody> ());
		}
		if(DestroyColliderAfterHit)
		{
			Destroy (GetComponent<Collider>());
		}
		Invoke ("Boom", ExplosionDelay);
	}

	void Boom()
	{
		Collider[] colliders = Physics.OverlapSphere (transform.position, ExplosionRadius);
		foreach(Collider c in colliders)
		{
			Rigidbody rb = c.GetComponent<Rigidbody> ();
			if(rb)
			{
				rb.AddExplosionForce (ExplosionForce, transform.position, ExplosionRadius);
			}
		}

		GameObject explosion = Instantiate (ExplosionPrefab, transform.position, Quaternion.identity);
		foreach(ParticleSystem ps in  explosion.GetComponentsInChildren<ParticleSystem>())
		{
			ps.Emit (1);
		}
		foreach (WFX_LightFlicker flicker in GetComponentsInChildren<WFX_LightFlicker>())
		{
			flicker.Flick();
		}

        Destroy(explosion, 10);
		Destroy (gameObject);
	}
}
