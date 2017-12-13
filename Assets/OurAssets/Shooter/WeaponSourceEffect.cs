using System;
using UnityEngine;

public class WeaponSourceEffect: MonoBehaviour
{
    

    public void Init(RaycastHit hit, Transform source)
    {
        Init(hit.point, source);
    }

    public void Init(Vector3 aim, Transform source)
    {
		Debug.Log ("Init");
        CancelInvoke("Destroy");
        transform.SetParent(source);
        transform.localPosition = Vector3.zero;
        transform.LookAt(aim);
        transform.localScale = Vector3.one;

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Emit(1);
        }
        foreach (WFX_LightFlicker flicker in GetComponentsInChildren<WFX_LightFlicker>())
        {
            flicker.Flick();
        }

        foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>())
        {
            CancelInvoke("DisableLineRenderers");
            lr.enabled = true;
            lr.SetPosition(0, lr.transform.position);
            lr.SetPosition(1, aim);
            Invoke("DisableLineRenderers", 0.5f);
        }

		foreach(ProjectileLauncher launcher in GetComponentsInChildren<ProjectileLauncher>())
		{
			Debug.Log ("launch");
			launcher.Launch (aim);
		}
        Invoke("Destroy", 5);
    }

    private void DisableLineRenderers()
    {
        foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>())
        {
            lr.enabled = false;
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}