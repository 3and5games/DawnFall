﻿using Downfall;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeWeapon : MonoBehaviour
{
    public bool throwingWeapon = false;
    public float throwDelay = 1f;

    public GameObject WeaponAimEffect, WeaponSourceEffect, WeaponBulletEfect;
    public Transform source;
    private WeaponSourceEffect sourceEffect;

    [Range(0,45)]
    public float ShiftAngle = 0;

    public int bulletInShoot = 1;

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

	public int weaponAnimationId;


    void Start()
    {
        Reload();
    }

    private void Reload()
	{
		Debug.Log ("reload "+Time.time);
        reloading = true;
		GetComponentInParent<Animator> ().SetTrigger ("Reload");
        Invoke("FinishReload", reloadTime);
    }

    private void FinishReload()
    {
		Debug.Log ("F "+Time.time);
		ShowVisual ();
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

        
		WeaponBulletEffect be = null;
		if(WeaponBulletEfect)
		{
			be = Instantiate(WeaponBulletEfect).GetComponent<WeaponBulletEffect>();
		}

        for (int i = 0; i < bulletInShoot;i++)
        {
        Vector3 aimVector = Tools.RotatePointAroundPivot(Camera.main.transform.forward, Camera.main.transform.position, new Vector3(UnityEngine.Random.Range(-ShiftAngle, ShiftAngle), UnityEngine.Random.Range(-ShiftAngle, ShiftAngle), 0));

            
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, aimVector, out hit))
            {    
                Rigidbody collidedRigidbody = hit.collider.GetComponent<Rigidbody>();
                if (collidedRigidbody)
                {
                    collidedRigidbody.AddForce((hit.point - Camera.main.transform.position).normalized * ForceMultiplyer);
                }
                if (WeaponAimEffect) {
                    Instantiate(WeaponAimEffect).GetComponent<WeaponAimEffect>().Init(hit);
                }

				if(be)
				{
                	be.Init(hit, source);
				}
            }
            else
            {
				if(be)
				{
                	be.Init(Camera.main.transform.forward*100, source);
				}
			}

        }
     
		if (!sourceEffect && WeaponSourceEffect!=null)
            {
                sourceEffect = Instantiate(WeaponSourceEffect).GetComponent<WeaponSourceEffect>();
			sourceEffect.Init(source);    
		}

    }

	public void HideVisual(){
		foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
		{
			mr.enabled = false;
		}
	}

	public void ShowVisual(){
		foreach(MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
		{
			mr.enabled = true;
		}
	}
}
