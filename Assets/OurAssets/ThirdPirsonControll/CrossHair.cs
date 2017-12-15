using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Invector.CharacterController;

public class CrossHair : MonoBehaviour {
	private Image crossHairImage;
	private Image CrossHairImmage
	{
		get
		{
			if(!crossHairImage)
			{
				crossHairImage = GetComponent<Image> ();
			}
			return crossHairImage;
		}
	}

	// Use this for initialization
	void Update () {
		CrossHairImmage.enabled = FindObjectOfType<vThirdPersonController> ().IsStrafing;
	}
				
}
