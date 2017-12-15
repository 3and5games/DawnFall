using System;
using UnityEngine;

public class WeaponSourceEffect: MonoBehaviour
{
    public int emmitionForce = 1;

    public void Init(Transform source)
    {
        CancelInvoke("Destroy");
        transform.SetParent(source);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Emit(emmitionForce);
        }
        foreach (WFX_LightFlicker flicker in GetComponentsInChildren<WFX_LightFlicker>())
        {
            flicker.Flick();
        }

        Invoke("Destroy", 5);
    }


    private void OnDestroy()
    {
        Destroy(gameObject);
    }
}