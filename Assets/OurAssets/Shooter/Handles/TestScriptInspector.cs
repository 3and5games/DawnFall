using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScript))]
public class TestScriptInspector : Editor {

    private Cone cone;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        this.cone = ((TestScript)target).cone;
        Transform tr = ((TestScript)target).transform;

        Vector3 forward = tr.forward * cone.radius;

        Handles.DrawLine(tr.position, tr.position + RotatePointAroundPivot(forward, Vector3.zero, tr.up * cone.angle/2));
        Handles.DrawLine(tr.position, tr.position + RotatePointAroundPivot(forward, Vector3.zero, -tr.up * cone.angle / 2));
        Handles.DrawLine(tr.position, tr.position + RotatePointAroundPivot(forward, Vector3.zero, tr.right * cone.angle / 2));
        Handles.DrawLine(tr.position, tr.position + RotatePointAroundPivot(forward, Vector3.zero, -tr.right * cone.angle / 2));

        Handles.DrawWireArc(forward, tr.forward, tr.right, 360, Mathf.Sin(cone.angle * Mathf.Deg2Rad / 2)*cone.radius);
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
