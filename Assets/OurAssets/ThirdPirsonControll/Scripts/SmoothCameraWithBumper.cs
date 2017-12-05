//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 

using UnityEngine;
using System.Collections;
using System;

public class SmoothCameraWithBumper : MonoBehaviour 
{
	public Action<ThirdPersonCameraMode> onModeChanged;
	public Action<GameObject, float> onObjectRaycasted;
	public Action onObjectRaycastedMissed;
	public float clipOffset = 0.1f;

	public enum ThirdPersonCameraMode
	{
		Adventure,
		Shooter
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
			mode = value;
			if(onModeChanged!=null)
			{
				onModeChanged.Invoke (mode);
			}
		}
	}

	[SerializeField] private SingleUnityLayer ignoringLayer;
	[SerializeField] private Transform target = null;
	[SerializeField] private float damping = 5.0f;
	[SerializeField] private float rotationDamping = 10.0f;

	[SerializeField] private Vector3 cameraRotation; // allows offsetting of camera lookAt, very useful for low bumper heights
	[SerializeField] private Vector3 cameraOffset;
	[SerializeField] private Vector3 aimOffset;

	[SerializeField] private Vector3 shooterCameraRotation; // allows offsetting of camera lookAt, very useful for low bumper heights
	[SerializeField] private Vector3 shooterCameraOffset;
	[SerializeField] private Vector3 shooterAimOffset;

	[SerializeField] private float raycastDistance = 2.5f; // length of bumper ray
	[SerializeField] private float bumperCameraHeight = 1.0f; // adjust camera height while bumping

	private Vector3 aimPosition;
	private float yAxisRotation;

	private void FixedUpdate() 
	{
		Vector3 wantedPosition = Vector3.zero;
		Vector3 wantedCameraVector = Vector3.zero;

		if (Input.GetMouseButton (1)) {
			Mode = ThirdPersonCameraMode.Shooter;
		} else {
			Mode = ThirdPersonCameraMode.Adventure;
		}

		switch(mode)
		{
		case ThirdPersonCameraMode.Adventure:
			wantedPosition = target.transform.position+cameraOffset+aimOffset;
			wantedCameraVector = cameraRotation;
			break;
		case ThirdPersonCameraMode.Shooter:
			wantedPosition = target.transform.position+shooterCameraOffset+shooterAimOffset;
			wantedCameraVector = shooterCameraRotation;
			break;
		}

		//rotate wanted position with character
		wantedPosition = RotatePointAroundPivot(wantedPosition, target.position+aimOffset, target.transform.rotation.eulerAngles);


		// raycast from camera
		RaycastHit hit;
		Ray ray = new Ray (transform.position, transform.forward);
		if (Physics.Raycast (ray, out hit, raycastDistance)) {
			if (onObjectRaycasted != null) {
				onObjectRaycasted.Invoke (hit.collider.gameObject, hit.distance);
			}
			if (hit.collider.gameObject.layer == ignoringLayer.LayerIndex) {
				wantedPosition.x = hit.point.x;
				wantedPosition.z = hit.point.z;
				wantedPosition.y = Mathf.Lerp (hit.point.y + bumperCameraHeight, wantedPosition.y, Time.deltaTime * damping);

			}
		
				aimPosition = hit.point;
		
		} else 
		{
			if (onObjectRaycastedMissed != null) {
				onObjectRaycastedMissed.Invoke ();
			}
		}



		if (mode == ThirdPersonCameraMode.Shooter) 
		{
			yAxisRotation += Input.GetAxis ("Mouse Y");
			yAxisRotation = Mathf.Clamp (yAxisRotation, -30, 30);

			transform.rotation = target.transform.rotation*Quaternion.Euler(wantedCameraVector);
			transform.Rotate (Vector3.left*yAxisRotation);
		} else 
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation*Quaternion.Euler(wantedCameraVector), Time.deltaTime * rotationDamping);
		}

		// check to see if there is anything behind the target
		RaycastHit hitClip;
		Ray rayClip = new Ray (target.transform.position+aimOffset, (-target.transform.position-aimOffset+wantedPosition).normalized * raycastDistance);

		// cast the bumper ray out from rear and check to see if there is anything behind
		Debug.DrawRay(rayClip.origin, rayClip.direction, Color.red);

		if (Physics.Raycast (rayClip, out hitClip)) { 
			float distance = Vector3.Distance (hitClip.point, target.transform.position + aimOffset);
			float needDistance = Vector3.Distance (wantedPosition, target.transform.position + aimOffset);
			if (distance < needDistance) {
				Vector3 reflectVec = Vector3.Reflect(rayClip.direction, hitClip.normal);
				transform.position = Vector3.Lerp (transform.position, hitClip.point+reflectVec*clipOffset, Time.deltaTime * damping*3);
			} else {
				transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);
			}
		} else 
		{
			transform.position = Vector3.Lerp (transform.position, wantedPosition, Time.deltaTime * damping);
		}
	}

	private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(angles) * dir;
		point = dir + pivot;
		return point;
	}
}