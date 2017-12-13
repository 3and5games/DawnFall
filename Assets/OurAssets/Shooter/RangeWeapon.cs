using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour {

    public GameObject WeaponAimEffect, WeaponSourceEffect;
    public Transform source;
    private WeaponSourceEffect sourceEffect;

	public float pullBackTime, pullOutTime;
	public float ForceMultiplyer = 1;

    private IEnumerator Shooting()
    {
        shooting = true;
        while (shooting && !reloading)
        {

            Shoot();

            yield return new WaitForSeconds(fireRate);
        }
    }

    public void StopShooting()
    {
        shooting = false;
        StopCoroutine(Shooting());
    }

    public void StartShooting()
    {
        StartCoroutine(Shooting());
    }

    public float fireRate = 0.2f;
    public float reloadTime = 1;
    public int Capacity = 10;
    public float currentAmmo;
    public bool shooting = false;
    public bool reloading = false;



    void Start()
    {
        Reload();
    }

    private void Reload()
    {
        reloading = true;
		GetComponentInParent<Animator> ().SetTrigger ("Reload");
        Invoke("FinishReload", reloadTime);
    }

    private void FinishReload()
    {
        currentAmmo = Capacity;
        reloading = false;
        if (shooting)
        {
            StartCoroutine(Shooting());
        }
    }

    void Shoot()
    {

        currentAmmo--;
        if (currentAmmo == 0)
        {
            Reload();
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            if (!sourceEffect)
            {
                sourceEffect = Instantiate(WeaponSourceEffect).GetComponent<WeaponSourceEffect>();
            }
            sourceEffect.Init(hit, source);

			Rigidbody collidedRigidbody =  hit.collider.GetComponent<Rigidbody> ();
			if(collidedRigidbody)
			{
				collidedRigidbody.AddForce ((hit.point -Camera.main.transform.position).normalized*ForceMultiplyer);
			}
			if(WeaponAimEffect){
				Instantiate(WeaponAimEffect).GetComponent<WeaponAimEffect>().Init(hit);
			}
            
        }
        else
        {
            if (!sourceEffect)
            {
                sourceEffect = Instantiate(WeaponSourceEffect).GetComponent<WeaponSourceEffect>();
            }
            sourceEffect.Init(Camera.main.transform.position + Camera.main.transform.forward * 100, source);
        }

    }
}
