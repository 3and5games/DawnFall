using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingingTooltip : MonoBehaviour {

	public Transform aim;

	private Image _img;
	private Image img
	{
		get
		{
			if(!_img)
			{
				_img = GetComponent<Image> ();
			}
			return _img;
		}
	}

	void Update()
	{
		float zeroToOneSin = (Mathf.Sin (Time.time) + 1f) / 2;

		if (aim) 
		{
			transform.position = Vector3.Lerp (transform.position, aim.position, Time.deltaTime*3);
			transform.localScale = Vector3.Lerp (img.transform.localScale, Vector3.one*(zeroToOneSin/5+0.8f), Time.deltaTime*3);
			img.color = Color.Lerp (img.color, new Color(img.color.r, img.color.g, img.color.b, zeroToOneSin/5+0.8f), Time.deltaTime*3);
		} 
		else 
		{
			img.color = Color.Lerp (img.color, new Color(img.color.r, img.color.g, img.color.b, 0), Time.deltaTime*3);
			img.transform.localScale = Vector3.Lerp (img.transform.localScale, Vector3.zero, Time.deltaTime*3);
		}
	}

	void Awake()
	{
		InteractableManager.Instance.onInteractableObjectChanged += ObjectChanged;
	}

	private void ObjectChanged(IInteractable interactable)
	{
		if (interactable!=null) {
			aim = ((MonoBehaviour)interactable).transform;
		} else {
			aim = null;
		}

	}
}
