using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorOverdriver : MonoBehaviour {

    //public AnimatorContr

    public float inTime;
    public string defaultStateName;
    public string stateName;
    private Animator overridingAnimator;
    private Action onOverrideFinished, onPointReach;
	private bool autoExit;
	private Animator queuedAnimator;
	public Transform moveToTransform;

	private NavMeshAgent agent;
	private bool rotating = false;

	public void Overdrive(Animator animator, Action onPointReach = null, Action onOverrideFinished = null, bool autoExit = false)
	{
		this.onPointReach = onPointReach;
		queuedAnimator = animator;
		this.autoExit = autoExit;
		this.onOverrideFinished = onOverrideFinished;
		queuedAnimator.transform.parent.GetComponent<Rigidbody> ().isKinematic = true;

		if (moveToTransform) {
			NavMeshHit hit;
			NavMesh.SamplePosition (moveToTransform.position, out hit, 3, NavMesh.AllAreas);
			if (hit.hit) {
				agent = animator.transform.parent.gameObject.AddComponent<NavMeshAgent> ();
				agent.speed = 1;
				agent.destination = hit.position;
				agent.GetComponent<Collider> ().enabled = false;
			}
		} else {
			Animate();
		}
	}

	private void Animate()
	{
		if (onPointReach != null) {
			onPointReach.Invoke ();
		}
			Animator animator = queuedAnimator;
		if(!animator)
		{
			animator = overridingAnimator;
		}

		animator.CrossFade(stateName, inTime);
		if (autoExit)
		{
			Debug.Log ("invoke");
			Invoke("ExitOverriding", animator.GetCurrentAnimatorStateInfo(0).length);
		}
	}

    private void Update()
    {
		if(rotating)
		{
			RotateTowards (overridingAnimator.transform.parent, moveToTransform);
		}
		
		if (overridingAnimator && Input.GetKeyDown(KeyCode.E) && !autoExit)
        {
			Debug.Log ("exit in update");
            ExitOverriding();
        }
		if(queuedAnimator)
		{
			overridingAnimator = queuedAnimator;
			queuedAnimator = null;
		}

		if(agent)
		{
			if(Vector3.Distance(agent.transform.position, agent.destination)<=agent.stoppingDistance)
			{
				GameObject go = agent.gameObject;
				Destroy (agent);
				rotating = true;
				Animate ();
			}
		}
    }

    private void InvokeOnFinish()
    {
        onOverrideFinished.Invoke();
    }

    private void ExitOverriding()
    {
		overridingAnimator.transform.parent.GetComponent<Collider> ().enabled = true;
		overridingAnimator.transform.parent.GetComponent<Rigidbody> ().isKinematic = false;
		rotating = false;
        overridingAnimator.CrossFade(defaultStateName, inTime);

        if (onOverrideFinished != null)
        {
            Invoke("InvokeOnFinish", inTime);
        }

        overridingAnimator = null;
    }

	private void RotateTowards(Transform rotatingObject, Transform target)
	{
		//Quaternion lookRotation = Quaternion.Euler(new Vector3(rotatingObject.rotation.eulerAngles.x, target.transform.rotation.eulerAngles.y, rotatingObject.rotation.eulerAngles.z));    // flattens the vector3
		rotatingObject.rotation = Quaternion.Slerp(rotatingObject.rotation, target.transform.rotation, Time.deltaTime * overridingAnimator.GetCurrentAnimatorStateInfo(0).length);
		rotatingObject.position = Vector3.Lerp (rotatingObject.position, target.position, Time.deltaTime * overridingAnimator.GetCurrentAnimatorStateInfo(0).length);
	}
}
