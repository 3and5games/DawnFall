using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour {

    public int effectId;

    public float tick = 1;
    public float radius = 5;
    public float lifetime = 5;
    public bool allowIntersection = true;

    private float time = 0;
    private float longestParticleTime;


	// Use this for initialization
	void Start () {
        if (!allowIntersection)
        {
            foreach (AreaEffect ae in FindObjectsOfType<AreaEffect>())
            {
                if (ae.effectId == effectId && Vector3.Distance(ae.transform.position, transform.position)<radius)
                {
                    Destroy(ae.gameObject);
                }
            }
        }

        InvokeRepeating("Tick", 0, tick);
        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            float lt = ps.main.startLifetime.Evaluate(1);
            if (lt>longestParticleTime)
            {
                longestParticleTime = lt;
            }
        }
	}
	
	void Tick()
    {
        time += tick;
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in colliders)
        {

        }


        if (time >= lifetime-longestParticleTime)
        {
            foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
            {
                ps.Stop();
            }
        }

        if (time>=lifetime)
        {
            Destroy(gameObject);
        }
    }
}
