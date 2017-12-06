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
    private Action onOverrideFinished;
	public bool autoExit;
	private Animator queuedAnimator;
	public Transform moveToTransform;

	private NavMeshAgent agent;


	public void Overdrive(Animator animator, Action onOverrideFinished = null, bool autoExit = false)
	{
		queuedAnimator = animator;
		this.autoExit = autoExit;
		this.onOverrideFinished = onOverrideFinished;


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
				go.GetComponent<Collider> ().enabled = true;
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
		
        overridingAnimator.CrossFade(defaultStateName, inTime);

        if (onOverrideFinished != null)
        {
            Invoke("InvokeOnFinish", inTime);
        }

        overridingAnimator = null;
    }
}
