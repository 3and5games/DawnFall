using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(WeaponBulletEffect))]
public class WeaponBulletEfectInspector : Editor {

    private Cone cone;
    private ArcHandle m_ArcHandle = new ArcHandle();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        this.cone = ((WeaponBulletEffect)target).cone;
        Transform tr = ((WeaponBulletEffect)target).transform;

        Vector3 forward = tr.forward * cone.radius;

        Vector3 p1, p2, p3, p4, p5;
        Vector3 fwd = tr.forward * Mathf.Cos(cone.angle * Mathf.Deg2Rad) * cone.radius;
        Vector3 center = tr.position + fwd;

        float r = Mathf.Sin(cone.angle * Mathf.Deg2Rad) * cone.radius;
        p1 = center + tr.right * r;
        p2 = center - tr.right * r;
        p3 = center + tr.up * r;
        p4 = center - tr.up * r;
        p5 = tr.position + tr.forward * cone.radius;
        Handles.color = new Color(255f / 255, 99f / 255, 71f / 255);

        Handles.DrawLine(tr.position, p1);
        Handles.DrawLine(tr.position, p2);
        Handles.DrawLine(tr.position, p3);
        Handles.DrawLine(tr.position, p4);
        Handles.DrawLine(tr.position, p5);
        Handles.DrawWireArc(center, tr.forward, tr.up * r, 360, r);

        Handles.DrawWireArc(tr.position, tr.up, fwd - tr.right * r, cone.angle * 2, cone.radius);
        Handles.DrawWireArc(tr.position, tr.right, fwd + tr.up * r, cone.angle * 2, cone.radius);

        m_ArcHandle.angle = cone.angle;
        m_ArcHandle.radius = cone.radius;

        Vector3 v = Handles.FreeMoveHandle(tr.position + tr.forward * (cone.radius), Quaternion.LookRotation(tr.forward), cone.radius / 25, Vector3.one, Handles.ConeHandleCap) - tr.position - tr.forward * cone.radius;
        v = Vector3.Project(v, tr.forward);
        cone.radius += v.z;

        EditorGUI.BeginChangeCheck();
        Handles.matrix = tr.localToWorldMatrix;
        m_ArcHandle.DrawHandle();
        if (EditorGUI.EndChangeCheck())
        {
            cone.angle = m_ArcHandle.angle;
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }
}
