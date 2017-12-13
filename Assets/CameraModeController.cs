using Cinemachine;
using Dialoges;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Invector.CharacterController;

public class CameraModeController : Singleton<CameraModeController> {

    public GameObject childCameraPrefab;

    public void SetAdventureMode()
    {
        SetCam(0);
    }

    public void SetDialog(PersonDialog dp)
    {
        SetDialogMode(dp.points);
    }

    public void SetDialogMode(PersonDialog.DialogCameraPoint[] transforms)
    {
        foreach (Transform t in GetComponent<CinemachineMixingCamera>().ChildCameras[1].transform)
        {
            Destroy(t.gameObject);
        }
        List<PersonDialog.DialogCameraPoint> transformsWithPlayer = transforms.ToList();
        transformsWithPlayer.Add(new PersonDialog.DialogCameraPoint(FindObjectOfType<vThirdPersonController>().transform, FindObjectOfType<vThirdPersonController>().transform, new Vector3(0.2f, 1.8f, 0.78f), new Vector3(0, 1.34f, 0)));
        transforms = transformsWithPlayer.ToArray();

        foreach (PersonDialog.DialogCameraPoint t in transforms)
        {
            CinemachineVirtualCamera vCam = Instantiate(childCameraPrefab, GetComponent<CinemachineMixingCamera>().ChildCameras[1].transform).GetComponent<CinemachineVirtualCamera>();
            vCam.Follow = t.follow;
            vCam.LookAt = t.lookAt;
            vCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = t.followOffset;
            vCam.GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = t.lookAtOffset;
        }
        SetCam(1);
    }

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
            float currentValue = GetComponent<CinemachineMixingCamera>().GetWeight(cameraId);
            while (currentValue != aimValue)
            {
                deltaTime += Time.deltaTime / 2;
                currentValue = Mathf.Lerp(currentValue, aimValue, deltaTime);
                GetComponent<CinemachineMixingCamera>().SetWeight(cameraId, currentValue);
                yield return null;
            }
            yield return null;
        }

    private void Awake()
    {
        DialogPlayer.Instance.onFinishDialog += SetAdventureMode;
        DialogPlayer.Instance.onDialogChanged += SetDialog;
		DialogPlayer.Instance.onPathGo += PathGo;
		DialogPlayer.Instance.onVariantsIn += StateIn;
    }

	private void PathGo(Path path){
		GetComponent<CinemachineMixingCamera> ().ChildCameras [1].GetComponent<CinemachineController> ().SetCam (0);
	}

	private void StateIn(State state){
		GetComponent<CinemachineMixingCamera> ().ChildCameras [1].GetComponent<CinemachineController> ().SetCam (1);
	}
}
