using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CinemachineController : MonoBehaviour {

	public Transform[] dialogPersons;
	private int i;

	void Start()
	{
		InitDialogCameras ();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			i++;
			if(i>=dialogPersons.Length)
			{
				i = 0;
			}
			SetCam (dialogPersons[i]);
		}
	}

	public void SetCam(Transform t)
	{
		StopAllCoroutines ();
		foreach (Transform tr in dialogPersons) {
			if (tr != t) {
				StartCoroutine (SetCameraWeight (dialogPersons.ToList ().IndexOf (tr), 0));
			} else {
				StartCoroutine (SetCameraWeight (dialogPersons.ToList ().IndexOf (tr), 1));
			}
		}
	}

	private void InitDialogCameras()
	{
	}

	private IEnumerator SetCameraWeight(int cameraId, int aimValue)
	{
		float deltaTime = 0;
		float currentValue = GetComponent<CinemachineMixingCamera> ().GetWeight (cameraId);
		while(currentValue!=aimValue)
		{
			deltaTime += Time.deltaTime/2;
			currentValue = Mathf.Lerp (currentValue, aimValue, deltaTime);
			GetComponent<CinemachineMixingCamera> ().SetWeight (cameraId, currentValue);	
			yield return null;
		}
		yield return null;
	}
}
