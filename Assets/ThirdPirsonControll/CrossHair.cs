using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	void Start () {
		FindObjectOfType<SmoothCameraWithBumper> ().onModeChanged += ModeChanged;
	}

	private void ModeChanged(SmoothCameraWithBumper.ThirdPersonCameraMode mode)
	{
		CrossHairImmage.enabled = (mode == SmoothCameraWithBumper.ThirdPersonCameraMode.Shooter);
	}
}
