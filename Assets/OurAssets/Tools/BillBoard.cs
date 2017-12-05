using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public enum PivotAxis
	{
		Free,
		Y
	}


	public class BillBoard : MonoBehaviour
	{

		[Tooltip("Specifies the axis about which the object will rotate.")]
		public PivotAxis PivotAxis = PivotAxis.Free;

		[Tooltip("Specifies the target we will orient to. If no Target is specified the main camera will be used.")]
		public Transform TargetTransform;

		private void OnEnable()
		{
			if (TargetTransform == null)
			{
				TargetTransform = Camera.main.transform;
			}

			Update();
		}

		
		private void Update()
		{
			if (TargetTransform == null)
			{
				return;
			}

			
			Vector3 directionToTarget = TargetTransform.position - transform.position;


			switch (PivotAxis)
			{
			case PivotAxis.Y:
				directionToTarget.y = 0.0f;
				break;

			case PivotAxis.Free:
			default:
				break;
			}
			
			if (directionToTarget.sqrMagnitude < 0.001f)
			{
				return;
			}
			
			transform.rotation = Quaternion.LookRotation(-directionToTarget);
		}
	}
