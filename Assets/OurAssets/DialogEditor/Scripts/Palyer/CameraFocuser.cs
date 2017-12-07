using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocuser : MonoBehaviour
{
    private float baseFow;
    private Camera attachedCamera;
    private float effectValue = 1;

    [Range(1, 2)]
    public float zoom;
    public float time = 1;
    public bool focused = false;

    private void Start()
    {
        attachedCamera = GetComponent<Camera>();
        baseFow = attachedCamera.fieldOfView;
    }
    void Update()
    {
        if (focused)
        {
            effectValue -= Time.deltaTime / time;
        }
        else
        {
            effectValue += Time.deltaTime / time;
        }
        effectValue = Mathf.Clamp(effectValue, 0, 1);
        attachedCamera.fieldOfView = Mathf.Lerp(baseFow / zoom, baseFow, effectValue);
    }

    [ContextMenu("unfocus")]
    public void UnFocus()
    {
        focused = false;
    }
    [ContextMenu("focus")]
    public void Focus()
    {
        focused = true;
    }
}
