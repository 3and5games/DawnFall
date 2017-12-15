using System;
using UnityEngine;

internal class WeaponAimEffect: MonoBehaviour
{
    [Range(0, 1)]
    public float ExplosionChance = 1;


    public GameObject Explosion;
    public GameObject Projector;
    public bool DestroyExplosion = true;

    public void Init(RaycastHit hit)
    {
        GameObject newExplosion = null;
        if (UnityEngine.Random.value<=ExplosionChance)
        {
            newExplosion = Instantiate(Explosion, hit.point, Quaternion.LookRotation(hit.normal.normalized));
        }
       

		if(Projector){
            GameObject projector = Instantiate(Projector, hit.point + hit.normal.normalized, Quaternion.LookRotation(-hit.normal.normalized));
            projector.transform.SetParent(hit.collider.transform);
            projector.GetComponent<Projector>().nearClipPlane = projector.GetComponent<Projector>().farClipPlane = Vector3.Distance(projector.transform.position, hit.point);
		}

        if (DestroyExplosion && newExplosion!=null)
        {
            Destroy(newExplosion, newExplosion.GetComponent<ParticleSystem>().main.startLifetime.Evaluate(1));
        }
        
        Destroy(gameObject);
    }
}