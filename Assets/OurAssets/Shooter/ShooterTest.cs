using Invector.CharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterTest : MonoBehaviour {

    public RangeWeapon weapon;
	public GameObject[] weapons;

	private vThirdPersonController cc;  
	private vThirdPersonController controller
	{
		get
		{
			if(!cc)
			{
				cc = GetComponent<vThirdPersonController> ();
			}
			return cc;
		}
	}

	private Animator an;  
	private Animator animator
	{
		get
		{
			if(!an)
			{
				an = GetComponent<Animator> ();
			}
			return an;
		}
	}

	private int currentWeapon = 0;
	private int CurrentWeapon
	{
		get
		{
			return currentWeapon;
		}
		set
		{
			if(currentWeapon!=value)
			{
			currentWeapon = value;
			

					CancelInvoke ("TakeWeapon");
					if (weapon) {
						Destroy (weapon.gameObject, weapon.pullBackTime);
					}
				if(weapons[currentWeapon]){
					Invoke ("TakeWeapon", weapons[currentWeapon].GetComponent<RangeWeapon>().pullOutTime);
				}
					
			
			animator.SetInteger ("WeaponId", value);
			animator.SetTrigger ("TakeWeapon");

		}
		}
	}

	void Start()
	{
		controller.OnStrafingChanged += (bool strafing) => {
			if (!strafing || weapons.Length == 1) {
				CurrentWeapon = 0;
			} else {
				CurrentWeapon = 1;
			}
		};
	}


	void TakeWeapon()
	{
		if(weapons[currentWeapon])
		{
		GameObject newWeapon = Instantiate (weapons[currentWeapon]);
		newWeapon.transform.SetParent (animator.GetBoneTransform(HumanBodyBones.RightHand));
		newWeapon.transform.localScale = Vector3.one;
		newWeapon.transform.localRotation = Quaternion.identity;
		newWeapon.transform.localPosition = Vector3.zero;
		weapon = newWeapon.GetComponent<RangeWeapon> ();
		}
	}

    // Update is called once per frame
    void Update () {
		
		int cw = currentWeapon+Mathf.RoundToInt(Input.GetAxisRaw("Mouse ScrollWheel")*10);
		if(cw<0)
		{
			cw = weapons.Length - 1;
		}
		if(cw>weapons.Length-1)
		{
			cw = 0;
		}
		CurrentWeapon = cw;

		if(weapon)
		{
        if (Input.GetMouseButtonDown(0) && GetComponent<vThirdPersonController>().IsStrafing)
        {	
				animator.SetBool ("Shooting", true);
            weapon.StartShooting();          
        }

        if (Input.GetMouseButtonUp(0) || !GetComponent<vThirdPersonController>().IsStrafing)
        {
				animator.SetBool ("Shooting", false);
            weapon.StopShooting();
        }    
		}
	}
   
}
