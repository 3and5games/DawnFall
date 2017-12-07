using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineController : MonoBehaviour {

	public void SetMode(int i)
	{
		GetComponent<Animator> ().SetInteger ("State", i);
	}

	void Start()
	{
		GetComponent<SmoothCameraWithBumper> ().onModeChanged += ModeChanged;
	}

	void ModeChanged(SmoothCameraWithBumper.ThirdPersonCameraMode mode)
	{
		SetMode ((int)mode);
	}
}
