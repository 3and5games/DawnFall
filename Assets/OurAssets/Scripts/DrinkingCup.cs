using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrinkingCup : MonoBehaviour, IInteractable {

	#region IInteractable implementation

	public void Activate ()
	{
		GetComponent<AnimatorOverdriver> ().Overdrive ();
	}

	#endregion
}
