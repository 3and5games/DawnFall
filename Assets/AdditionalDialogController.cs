using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialoges;
using Invector.CharacterController;
using Invector.EventSystems;
using Invector.CharacterController.Actions;

public class AdditionalDialogController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DialogPlayer.Instance.onFinishDialog += FinishDialog;
		DialogPlayer.Instance.onStateIn += StartDialog;
	}
	
	void FinishDialog()
	{
		PingingTooltip.Instance.gameObject.SetActive(true);
		FindObjectOfType<vThirdPersonInput> ().blocked = false;
		FindObjectOfType<vGenericAction> ().enabled = true;
		FindObjectOfType<vThirdPersonCamera> ().enabled = true;
	
	}

	void StartDialog(State s)
	{
		PingingTooltip.Instance.gameObject.SetActive(false);
		FindObjectOfType<vThirdPersonInput> ().blocked = true;
		FindObjectOfType<vGenericAction> ().enabled = false;
		FindObjectOfType<vThirdPersonCamera> ().enabled = false;
	}
}
