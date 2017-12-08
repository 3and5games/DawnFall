using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))] 

public class IKTest : MonoBehaviour {

	protected Animator animator;

	public bool handActive = false, bodyActive = false;
	public Transform rightHandObj = null;
	public AvatarIKGoal goal;
	[Range(0,1)]
	public float value;

	public Transform bodyTo;

	private bool sitting = false;

	private float sitTime = 0;

	[ContextMenu("sit")]
	public void Sit()
	{
		animator.SetTrigger ("Active");
		sitting = true;
		sitTime = 0;
	}


	void Start () 
	{
		animator = GetComponent<Animator>();
	}

	//a callback for calculating IK
	void OnAnimatorIK()
	{
		if(animator) {

			//if the IK is active, set the position and rotation directly to the goal. 
			if(handActive) {



				//weight = 1.0 for the right hand means position and rotation will be at the IK goal (the place the character wants to grab)
				animator.SetIKPositionWeight(goal, value);
				animator.SetIKRotationWeight(goal, value);

				//set the position and the rotation of the right hand where the external object is
				if(rightHandObj != null) {
					animator.SetIKPosition(goal,rightHandObj.position);
					animator.SetIKRotation(goal,rightHandObj.rotation);
				}       
			}

			if(bodyActive)
			{
				if(bodyTo && sitting)
				{
					sitTime += Time.deltaTime;
					animator.bodyPosition = Vector3.Lerp (animator.bodyPosition, bodyTo.position, sitTime/2);
					animator.bodyRotation = Quaternion.Lerp (animator.bodyRotation, Quaternion.LookRotation(bodyTo.position - transform.position), sitTime/2);
				}


			}

			//if the IK is not active, set the position and rotation of the hand back to the original position
			else {          
				animator.SetIKPositionWeight(goal,0);
				animator.SetIKRotationWeight(goal,0);             
			}
		}
	}    
}