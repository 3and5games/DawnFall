using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBulletEffect : MonoBehaviour {

    public Cone cone;
    public int emmitionForce = 1;

    private Vector3 aimPoint = Vector3.zero;

    public void Init(RaycastHit hit, Transform source)
    {
        Debug.Log(hit.point);
        Init(hit.point, source);
    }

    public void Init(Vector3 aim, Transform source)
    {
        aimPoint = aim;
        CancelInvoke("Destroy");
        transform.SetParent(source);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.LookRotation(aim-transform.position);

        foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>())
        {
            ps.Emit(emmitionForce);
        }

        foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>())
        {
            CancelInvoke("DisableLineRenderers");
            lr.enabled = true;
            Invoke("DisableLineRenderers", 0.1f);
        }
        foreach (ProjectileLauncher launcher in GetComponentsInChildren<ProjectileLauncher>())
        {
            Debug.Log("launch");
            launcher.Launch(aim);
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

    void Update()
    {
        foreach (LineRenderer lr in GetComponentsInChildren<LineRenderer>())
        {
            lr.SetPosition(0, lr.transform.position);
            lr.SetPosition(1, aimPoint);
        }     
    }

}
