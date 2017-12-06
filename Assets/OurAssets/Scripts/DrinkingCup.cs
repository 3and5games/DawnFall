using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DrinkingCup : MonoBehaviour, IInteractable {

    #region IInteractable implementation

	public UnityEvent onApplyAction;

    public bool AutoExit = true;

	public void Activate ()
	{
        FindObjectOfType<ThirdPersonController>().enabled = false;
        InteractableManager.Instance.InteractionEnable = false;
        Camera.main.GetComponent<SmoothCameraWithBumper>().Mode = SmoothCameraWithBumper.ThirdPersonCameraMode.Observer;


		GetComponent<AnimatorOverdriver> ().Overdrive (FindObjectOfType<ThirdPersonController>().GetComponentInChildren<Animator>(),()=>{onApplyAction.Invoke();}, ()=>
        {
            FindObjectOfType<ThirdPersonController>().enabled = true;
            InteractableManager.Instance.InteractionEnable = true;
            Camera.main.GetComponent<SmoothCameraWithBumper>().Mode = SmoothCameraWithBumper.ThirdPersonCameraMode.Adventure;
        },
        AutoExit
        );
	}

	#endregion
}
