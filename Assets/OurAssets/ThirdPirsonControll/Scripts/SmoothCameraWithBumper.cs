//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 

using UnityEngine;
using System.Collections;
using System;

public class SmoothCameraWithBumper : MonoBehaviour 
{
    private float cumulativeRotation;

	public Action<ThirdPersonCameraMode> onModeChanged;
	public Action<GameObject, float> onObjectRaycasted;
	public Action onObjectRaycastedMissed;
	public float clipOffset = 0.1f;

	public enum ThirdPersonCameraMode
	{
		Adventure = 0,
		Shooter = 1,
        Observer = 2
	}

	[SerializeField]
	private ThirdPersonCameraMode mode = ThirdPersonCameraMode.Adventure;
	public ThirdPersonCameraMode Mode
	{
		get
		{
			return mode;
		}
		set
		{
			if( mode!=value)
			{
				if (onModeChanged != null) {
					onModeChanged.Invoke (value);
				}

				mode = value;
			}
		}
	}

	public Vector3 aimPosition;

	[SerializeField] private LayerMask raycastLayers;

	[SerializeField] private float raycastDistance = 2.5f; // length of bumper ray

	private void FixedUpdate() 
	{
   
 
            if (Mode != ThirdPersonCameraMode.Observer)
            {
                if (Input.GetMouseButton(1))
                {
                    Mode = ThirdPersonCameraMode.Shooter;
                }
                else
                {
                    Mode = ThirdPersonCameraMode.Adventure;
                }
            }

         
          
            // raycast from camera
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
		if (Physics.Raycast(ray, out hit, raycastDistance, raycastLayers.value))
            {
                if (onObjectRaycasted != null)
                {
                    onObjectRaycasted.Invoke(hit.collider.gameObject, hit.distance);
                }
				
                aimPosition = hit.point;

            }
            else
            {
                if (onObjectRaycastedMissed != null)
                {
                    onObjectRaycastedMissed.Invoke();
                }
            }
	}
}