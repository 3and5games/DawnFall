using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Dialoges
{
    [CustomEditor(typeof(Path))]

    public class PathInspector : Editor
    {
        private Path p;

        public void OnEnable()
        {
            p = (Path)target;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string pText = EditorGUILayout.TextField(p.text, GUILayout.Height(30));
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            bool pAuto = GUILayout.Toggle(p.auto, "auto", GUILayout.Width(60));
            bool pWithEvent = GUILayout.Toggle(p.withEvent, "action", GUILayout.Width(60));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "path main properties");
                p.text = pText;
                p.auto = pAuto;
                p.withEvent = pWithEvent;
            }
            EditorGUI.BeginDisabledGroup(p.Game.parameters.Count == 0);
            if (GUILayout.Button("add condition param"))
            {
                Undo.RecordObject(p, "path condition creation");
                p.condition.AddParam(p.Game.parameters[0]);
            }
            if (GUILayout.Button("add param changer"))
            {
                Undo.RecordObject(p, "path changer creation");
                p.changes.Add(new ParamChanges(p.Game.parameters[0]));
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            DrawCondition(p);
            DrawChanges(p);
            GUI.color = Color.white;
        }
        private void DrawCondition(Path path)
        {
            EditorGUILayout.LabelField("condition:");
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = Color.white;
            try
            {
                List<float> pv = new List<float>();
                foreach (Param p in path.condition.Parameters)
                {
                    pv.Add(0);
                }
                ExpressionSolver.CalculateBool(path.condition.conditionString, pv);
            }
            catch
            {
                GUI.color = Color.red;
            }
            EditorGUI.BeginChangeCheck();
            string conditionString = EditorGUILayout.TextField(path.condition.conditionString);



            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            Param removingParam = null;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "path condition string");
                path.condition.conditionString = conditionString;
            }

            for (int i = 0; i < path.condition.Parameters.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[p" + i + "]", GUILayout.Width(35));

                if (!p.Game.parameters.Contains(path.condition.Parameters[i]))
                {
                    if (p.Game.parameters.Count > 0)
                    {
                        path.condition.Parameters[i] = p.Game.parameters[0];
                    }
                    else
                    {
                        removingParam = path.condition.Parameters[i];
                        continue;
                    }
                }

                EditorGUI.BeginChangeCheck();

                int paramIndex = EditorGUILayout.Popup(p.Game.parameters.IndexOf(path.condition.Parameters[i]), p.Game.parameters.Select(x => x.paramName).ToArray());

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(p, "set condition param");
                    path.condition.setParam(i, p.Game.parameters[paramIndex]);
                }

                GUI.color = Color.red;
                if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
                {
                    removingParam = path.condition.Parameters[i];
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }

            if (removingParam != null)
            {
                Undo.RecordObject(p, "removing condition param");
                path.condition.RemoveParam(removingParam);
            }

        }
        private void DrawChanges(Path path)
        {
            ParamChanges removingChanger = null;
            for (int i = 0; i < path.changes.Count; i++)
            {
                GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("delete"))
                {
                    removingChanger = path.changes[i];
                }
                if (GUILayout.Button("add param"))
                {
                    Undo.RecordObject(p, "add path changer param");
                    path.changes[i].AddParam(p.Game.parameters[0]);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (!p.Game.parameters.Contains(path.changes[i].aimParam))
                {
                    if (p.Game.parameters.Count > 0)
                    {
                        path.changes[i].aimParam = p.Game.parameters[0];
                    }
                    else
                    {
                        removingChanger = path.changes[i];
                        continue;
                    }
                }
                EditorGUI.BeginChangeCheck();
                int paramIndex = EditorGUILayout.Popup(p.Game.parameters.IndexOf(path.changes[i].aimParam), p.Game.parameters.Select(x => x.paramName).ToArray(), GUILayout.Width(100));
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(p, "chose path changer param");
                    path.changes[i].aimParam = p.Game.parameters[paramIndex];
                }
                GUILayout.Label("=", GUILayout.Width(15));
                GUI.backgroundColor = Color.white;
                try
                {
                    List<float> paramsV = new List<float>();
                    foreach (Param p in path.changes[i].Parameters)
                    {
                        paramsV.Add(1);
                    }
                    ExpressionSolver.CalculateFloat(path.changes[i].changeString, paramsV);
                }
                catch
                {
                    GUI.color = Color.red;
                }
                EditorGUI.BeginChangeCheck();
                string changeString = EditorGUILayout.TextField(path.changes[i].changeString);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(p, "change path changer string");
                    path.changes[i].changeString = changeString;
                }
                GUI.color = Color.white;
                Param removingParam = null;
                EditorGUILayout.EndHorizontal();
                for (int j = 0; j < path.changes[i].Parameters.Count; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("[p" + j + "]", GUILayout.Width(35));

                    if (!p.Game.parameters.Contains(path.changes[i].Parameters[j]))
                    {
                        if (p.Game.parameters.Count > 0)
                        {
                            path.changes[i].setParam(p.Game.parameters[0], j);
                        }
                        else
                        {
                            removingParam = path.changes[i].Parameters[j];
                            continue;
                        }
                    }
                    EditorGUI.BeginChangeCheck();
                    int v = EditorGUILayout.Popup(p.Game.parameters.IndexOf(path.changes[i].Parameters[j]), p.Game.parameters.Select(x => x.paramName).ToArray());
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(p, "change path changer sub param");
                        path.changes[i].setParam(p.Game.parameters[v], j);
                    }
                    GUI.color = Color.red;
                    if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
                    {
                        removingParam = path.changes[i].Parameters[j];
                    }
                    GUI.color = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
                if (removingParam != null)
                {
                    Undo.RecordObject(p, "remove path changer sub param");
                    path.changes[i].RemoveParam(removingParam);
                }
                GUI.color = Color.white;
            }
            if (removingChanger != null)
            {
                Undo.RecordObject(p, "remove path changer param");
                path.changes.Remove(removingChanger);
            }
        }

    }
}
