using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableManager : Singleton<InteractableManager> {

	public Action<GameObject> onInteractableObjectChanged;
	public float interactionDistance = 2;

    private bool interactionEnable = true;
    public bool InteractionEnable
    {
        get
        {
            return interactionEnable;
        }
        set
        {
            interactionEnable = value;
            if (!interactionEnable)
            {
                interactableObject = null;
            }
        }
    }

	private GameObject _interactableObject;
	private GameObject interactableObject
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


	void ObjectRaycastMissed()
	{
		interactableObject = null;
	}

	void ObjectRaycasted(GameObject go, float distance)
	{
        if (!InteractionEnable)
        {
            return;
        }

		if (distance <= interactionDistance) {
			interactableObject = go;
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
                foreach (IInteractable interactableBehaviour in interactableObject.GetComponentsInChildren<IInteractable>())
                {
                    interactableBehaviour.Activate();
                }
			}
		}
	}
}
