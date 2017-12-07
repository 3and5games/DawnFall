using UnityEngine;
using UnityEditor;
using System.Collections;
using Invector;

[CustomEditor(typeof(Character),true)]
public class CharacterEditor : Editor
{
    public override void OnInspectorGUI()
    {
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical();
		GUILayout.BeginVertical("Third Person Controller by Invector", "window");
        base.OnInspectorGUI();
		GUILayout.EndVertical();
		EditorGUILayout.EndVertical();

		EditorGUILayout.Space();
		EditorGUILayout.Space();
    }

    //**********************************************************************************//
    // DEBUG RAYCASTS                                                                   //
    // draw the casts of the controller on play mode 							        //
    //**********************************************************************************//	
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    //TODO: Replace first argument with the type you are editing
    private static void GizmoTest(Transform aTarget, GizmoType aGizmoType)
    {
    #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            ThirdPersonMotor motor = (ThirdPersonMotor)aTarget.GetComponent<ThirdPersonMotor>();
            if (!motor) return;

            // debug auto crouch
            Vector3 posHead = motor.transform.position + Vector3.up * ((motor.colliderHeight * 0.5f) - motor.colliderRadius);
            Ray ray1 = new Ray(posHead, Vector3.up);
            Gizmos.DrawWireSphere(ray1.GetPoint((motor.headDetect - (motor.colliderRadius * 0.1f))), motor.colliderRadius * 0.9f);
            Handles.Label(ray1.GetPoint((motor.headDetect + (motor.colliderRadius))), "Head Detection");            
            // debug check trigger action
            Vector3 yOffSet = new Vector3(0f, -0.5f, 0f);            
            Ray ray2 = new Ray(motor.transform.position - yOffSet, motor.transform.forward);
            Debug.DrawRay(ray2.origin, ray2.direction * 0.45f, Color.white);
            Handles.Label(ray2.GetPoint(0.45f), "Check for Trigger Actions");
            // debug stopmove            
            Ray ray3 = new Ray(motor.transform.position + new Vector3(0, motor.colliderHeight / 3, 0), motor.transform.forward);
            Debug.DrawRay(ray3.origin, ray3.direction * motor.stopMoveDistance, Color.blue);
            Handles.Label(ray3.GetPoint(motor.stopMoveDistance), "Check for StopMove");
            // debug stopmove            
            Ray ray4 = new Ray(motor.transform.position + new Vector3(0, motor.colliderHeight / 3.5f, 0), motor.transform.forward);
            Debug.DrawRay(ray4.origin, ray4.direction * 1f, Color.cyan);
            Handles.Label(ray4.GetPoint(1f), "Check for SlopeLimit");
            // debug stepOffset
            Ray ray5 = new Ray((motor.transform.position + new Vector3(0, motor.stepOffsetEnd, 0) + motor.transform.forward * ((motor.capsuleCollider).radius + motor.stepOffsetFwd)), Vector3.down);
            Debug.DrawRay(ray5.origin, ray5.direction * (motor.stepOffsetEnd - motor.stepOffsetStart), Color.yellow);
            Handles.Label(ray5.origin, "Step OffSet");
        }
    #endif
    }
}