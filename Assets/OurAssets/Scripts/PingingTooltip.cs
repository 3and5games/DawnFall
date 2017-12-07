using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingingTooltip : MonoBehaviour {

	private Transform aim;

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
        
		if (aim) 
		{
            RectTransform CanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(aim.transform.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
            img.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;


            float zeroToOneSin = (Mathf.Sin(Time.time) + 1f) / 2;     
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
		//InteractableManager.Instance.onInteractableObjectChanged += ObjectChanged;
	}

    public void Hide()
    {
        ObjectChanged(null);
    }

	public void ObjectChanged(GameObject interactableObject)
	{
        if (interactableObject) {
			aim = interactableObject.transform;
		} else {
			aim = null;
		}

	}
}
