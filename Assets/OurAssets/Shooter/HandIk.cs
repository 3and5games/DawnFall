using UnityEngine;
using System.Collections;
using Invector.CharacterController;

[ExecuteInEditMode]
public class HandIk : MonoBehaviour {
	[Range(0,1)]
	public float force;


	Animator animator;

	// Use this for initialization
	void Start () {

		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {

		animator.Update(0.0f);

	}

	void OnAnimatorIK(int layerIndex)
	{
		if (FindObjectOfType<vThirdPersonController>().isStrafing) {
			force += Time.deltaTime/5;
		} else {
			force -= Time.deltaTime/5;
		}

		force = Mathf.Clamp (force, 0, 1);

		Vector3 aim = Camera.main.transform.position + Camera.main.transform.forward * 30;

		if (FindObjectOfType<vThirdPersonController>().isStrafing)
		{
			//animator.SetIKRotationWeight(AvatarIKGoal.RightHand, force);
			//animator.SetIKPositionWeight(AvatarIKGoal.RightHand, force);
			//animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, force);
			//animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, force);

			Vector3 v = animator.rootRotation.eulerAngles;
			animator.rootRotation = Quaternion.Euler (v.x, Quaternion.LookRotation (aim - transform.position).eulerAngles.y, v.z);
			//animator.SetBoneLocalRotation (HumanBodyBones.UpperChest, Quaternion.LookRotation(aim-transform.position));
		}


		//animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(aim-transform.position));
		//animator.SetIKPosition(AvatarIKGoal.RightHand, aim);
		//animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(aim-transform.position));
		//animator.SetIKPosition(AvatarIKGoal.LeftHand, aim);

	}
}

