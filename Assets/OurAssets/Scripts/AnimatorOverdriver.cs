using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverdriver : MonoBehaviour {

    //public AnimatorContr

    public float inTime;
    public string defaultStateName;
    public string stateName;
    private Animator overridingAnimator;
    private Action onOverrideFinished;
    private bool autoExit;

	public void Overdrive(Animator animator, Action onOverrideFinished = null, bool autoExit = false)
	{
        this.autoExit = autoExit;
        this.onOverrideFinished = onOverrideFinished;
        overridingAnimator = animator;
        animator.CrossFade(stateName, inTime);

        if (autoExit)
        {
            Invoke("ExitOverriding", overridingAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
	}

    private void Update()
    {
        if (overridingAnimator && Input.GetKeyDown(KeyCode.E) && !autoExit)
        {
            ExitOverriding();
        }
    }

    private void InvokeOnFinish()
    {
        onOverrideFinished.Invoke();
    }

    private void ExitOverriding()
    {
        overridingAnimator.CrossFade(defaultStateName, inTime);

        if (onOverrideFinished != null)
        {
            Invoke("InvokeOnFinish", inTime);
        }

        overridingAnimator = null;
    }
}
