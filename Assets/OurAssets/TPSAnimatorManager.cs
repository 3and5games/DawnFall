using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TPSAnimatorManager : MonoBehaviour {

	private Animator humanAnimator;
	private Animator HumanAnimator
	{
		get
		{
			if(!humanAnimator)
			{
				humanAnimator = GetComponent <Animator> ();
			}
			return humanAnimator;
		}
	}

	private Vector2 inputVector;
	private Vector2 InputVector
	{
		get
		{
			return inputVector;
		}
		set
		{
			inputVector = value;
			HumanAnimator.SetFloat ("InputMagnitude", inputVector.magnitude);
		}
	}

	private void OnEnable()
	{
		TPSInput.Instance.onHorizontalChanged += SetHorizontal;
		TPSInput.Instance.onVerticalChanged += SetVertical;
		TPSInput.Instance.onJump += Jump;
	}

	private void SetHorizontal(float h)
	{
		InputVector = new Vector2 (h, inputVector.y);
	}

	private void SetVertical(float v)
	{
		InputVector = new Vector3 (inputVector.x, v);
	}

	private void Jump()
	{
		HumanAnimator.SetBool ("isJump", true);
	}
}
