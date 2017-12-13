using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CinemachineController : Singleton<CinemachineController> {

   
	public Transform[] dialogPersons;
	private int i;


	public void SetCam(int index)
	{
		StopAllCoroutines();
		int i = 0;
		foreach (CinemachineVirtualCameraBase c in GetComponent<CinemachineMixingCamera>().ChildCameras)
		{
			if (index != i)
			{
				StartCoroutine(SetCameraWeight(i, 0));
			}
			else
			{
				StartCoroutine(SetCameraWeight(i, 1));
			}
			i++;
		}
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
