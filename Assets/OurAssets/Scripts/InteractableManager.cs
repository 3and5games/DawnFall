using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableManager : Singleton<InteractableManager> {

	public Action<IInteractable> onInteractableObjectChanged;
	public float interactionDistance = 2;
	private IInteractable _interactableObject;
	private IInteractable interactableObject
	{
		get
		{
			return _interactableObject;
		}
		set
		{
			_interactableObject = value;
			if (onInteractableObjectChanged!=null) 
			{
				onInteractableObjectChanged.Invoke (value);
			}
		}
	}
	public KeyCode InteractionCode;

	void OnEnable()
	{
		FindObjectOfType<SmoothCameraWithBumper> ().onObjectRaycasted += ObjectRaycasted;
		FindObjectOfType<SmoothCameraWithBumper> ().onObjectRaycastedMissed += ObjectRaycastMissed;
	}

	void OnDisable()
	{
		FindObjectOfType<SmoothCameraWithBumper> ().onObjectRaycasted -= ObjectRaycasted;
	}

	void ObjectRaycastMissed()
	{
		interactableObject = null;
	}

	void ObjectRaycasted(GameObject go, float distance)
	{
		if (distance <= interactionDistance) {
			interactableObject = go.GetComponent<IInteractable> ();
		} else 
		{
			interactableObject = null;
		}
	}

	void Update()
	{
		if(Input.GetKeyDown(InteractionCode))
		{
			if(interactableObject!=null)
			{
				interactableObject.Activate ();
			}
		}
	}
}
