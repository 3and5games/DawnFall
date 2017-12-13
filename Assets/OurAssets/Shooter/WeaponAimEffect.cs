using System;
using UnityEngine;

internal class WeaponAimEffect: MonoBehaviour
{
    public GameObject Explosion;
    public GameObject Projector;

    public void Init(RaycastHit hit)
    {
        GameObject newExplosion = Instantiate(Explosion, hit.point, Quaternion.LookRotation(hit.normal.normalized));
        GameObject projector = Instantiate(Projector, hit.point + hit.normal.normalized, Quaternion.LookRotation(-hit.normal.normalized));
        projector.transform.SetParent(hit.collider.transform);
        projector.GetComponent<Projector>().nearClipPlane = projector.GetComponent<Projector>().farClipPlane = Vector3.Distance(projector.transform.position, hit.point);
        Destroy(newExplosion, newExplosion.GetComponent<ParticleSystem>().main.startLifetime.Evaluate(1));
        Destroy(gameObject);
    }
}