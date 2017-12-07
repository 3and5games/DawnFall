using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace Dialoges
{
[CustomEditor(typeof(Param))]

public class ParamInspector : Editor
{
    private Param p;
    private PathGame _game;
    private PathGame game
    {
        get
        {
            if (!_game)
            {
                p = (Param)target;
                _game = AssetDatabase.LoadAssetAtPath<PathGame>(AssetDatabase.GetAssetPath(p));
            }
            return _game;
        }
    }

    public override void OnInspectorGUI()
    {
        p = (Param)target;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("name:");
        EditorGUILayout.BeginHorizontal();
        string pName = EditorGUILayout.TextField(p.paramName);
        bool pShowing = !GUILayout.Toggle(!p.showing, "hidden", GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
        string pDescription = p.description;
        Sprite pImage = p.image;
        if (pShowing)
        {
            EditorGUILayout.LabelField("description:");
            EditorGUILayout.BeginHorizontal();
            pDescription = EditorGUILayout.TextField(p.description, GUILayout.Height(60));
            pImage = (Sprite)EditorGUILayout.ObjectField(p.image, typeof(Sprite), false, GUILayout.Width(60), GUILayout.Height(60));
            EditorGUILayout.EndHorizontal();
        }

        Path pPath = p.activationPath;
        Param.ActivationType pType = (Param.ActivationType)EditorGUILayout.EnumPopup(p.activationType);

        if (pType != Param.ActivationType.None)
        {
            DrawCondition(p);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activatiuon path: ", GUILayout.Width(100));
            pPath = (Path)EditorGUILayout.ObjectField(p.activationPath, typeof(Path), false);
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginDisabledGroup(p.Game.parameters.Count == 0);
            if (GUILayout.Button("add changer", GUILayout.Width(110)))
            {
                Undo.RecordObject(p, "path condition creation");
                p.changes.Add(new ParamChanges(p.Game.parameters[0]));
            }
            GUI.color = Color.white;
            EditorGUI.EndDisabledGroup();
            DrawChanges(p);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("tags:", GUILayout.Width(100));
        string pTags = EditorGUILayout.TextField(p.tags);
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(p, "param base properties");
            p.paramName = pName;
            p.showing = pShowing;
            p.description = pDescription;
            p.image = pImage;
            p.tags = pTags;
            p.activationPath = pPath;
            p.activationType = pType;
        }
    }

    private void DrawCondition(Param param)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("condition: ", GUILayout.Width(100));
        GUI.backgroundColor = Color.white;
        try
        {
            List<float> pv = new List<float>();
            foreach (Param p in param.condition.Parameters)
            {
                pv.Add(0);
            }
            ExpressionSolver.CalculateBool(param.condition.conditionString, pv);
        }
        catch
        {
            GUI.color = Color.red;
        }
        EditorGUI.BeginChangeCheck();

        string conditionString = EditorGUILayout.TextField(param.condition.conditionString);

        EditorGUI.BeginDisabledGroup(param.Game.parameters.Count == 0);
        GUI.color = Color.green;
        if (GUILayout.Button("", GUILayout.Width(15), GUILayout.Height(15)))
        {
            Undo.RecordObject(p, "path condition creation");
            param.condition.AddParam(param.Game.parameters[0]);
        }
        EditorGUI.EndDisabledGroup();

        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();

        Param removingParam = null;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(p, "path condition string");
            param.condition.conditionString = conditionString;
        }

        for (int i = 0; i < param.condition.Parameters.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("[p" + i + "]", GUILayout.Width(35));

            if (!p.Game.parameters.Contains(param.condition.Parameters[i]))
            {
                if (p.Game.parameters.Count > 0)
                {
                    param.condition.Parameters[i] = p.Game.parameters[0];
                }
                else
                {
                    removingParam = param.condition.Parameters[i];
                    continue;
                }
            }

            EditorGUI.BeginChangeCheck();

            int paramIndex = EditorGUILayout.Popup(p.Game.parameters.IndexOf(param.condition.Parameters[i]), p.Game.parameters.Select(x => x.paramName).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "set condition param");
                param.condition.setParam(i, p.Game.parameters[paramIndex]);
            }

            GUI.color = Color.red;
            if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
            {
                removingParam = param.condition.Parameters[i];
            }
            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        if (removingParam != null)
        {
            Undo.RecordObject(p, "removing condition param");
            param.condition.RemoveParam(removingParam);
        }

    }
    private void DrawChanges(Param param)
    {
        ParamChanges removingChanger = null;
        for (int i = 0; i < param.changes.Count; i++)
        {
            GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("delete"))
            {
                removingChanger = param.changes[i];
            }
            if (GUILayout.Button("add param"))
            {
                Undo.RecordObject(p, "add path changer param");
                param.changes[i].AddParam(p.Game.parameters[0]);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (!p.Game.parameters.Contains(param.changes[i].aimParam))
            {
                if (p.Game.parameters.Count > 0)
                {
                    param.changes[i].aimParam = p.Game.parameters[0];
                }
                else
                {
                    removingChanger = param.changes[i];
                    continue;
                }
            }
            EditorGUI.BeginChangeCheck();
            int paramIndex = EditorGUILayout.Popup(p.Game.parameters.IndexOf(param.changes[i].aimParam), p.Game.parameters.Select(x => x.paramName).ToArray(), GUILayout.Width(81));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "chose path changer param");
                param.changes[i].aimParam = p.Game.parameters[paramIndex];
            }
            GUILayout.Label("=", GUILayout.Width(15));
            GUI.backgroundColor = Color.white;
            try
            {
                List<float> paramsV = new List<float>();
                foreach (Param p in param.changes[i].Parameters)
                {
                    paramsV.Add(1);
                }
                ExpressionSolver.CalculateFloat(param.changes[i].changeString, paramsV);
            }
            catch
            {
                GUI.color = Color.red;
            }
            EditorGUI.BeginChangeCheck();
            string changeString = EditorGUILayout.TextField(param.changes[i].changeString);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "change path changer string");
                param.changes[i].changeString = changeString;
            }
            GUI.color = Color.white;
            Param removingParam = null;
            EditorGUILayout.EndHorizontal();
            for (int j = 0; j < param.changes[i].Parameters.Count; j++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("[p" + j + "]", GUILayout.Width(35));

                if (!p.Game.parameters.Contains(param.changes[i].Parameters[j]))
                {
                    if (p.Game.parameters.Count > 0)
                    {
                        param.changes[i].setParam(p.Game.parameters[0], j);
                    }
                    else
                    {
                        removingParam = param.changes[i].Parameters[j];
                        continue;
                    }
                }
                EditorGUI.BeginChangeCheck();
                int v = EditorGUILayout.Popup(p.Game.parameters.IndexOf(param.changes[i].Parameters[j]), p.Game.parameters.Select(x => x.paramName).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(p, "change path changer sub param");
                    param.changes[i].setParam(p.Game.parameters[v], j);
                }
                GUI.color = Color.red;
                if (GUILayout.Button("", GUILayout.Height(15), GUILayout.Width(15)))
                {
                    removingParam = param.changes[i].Parameters[j];
                }
                GUI.color = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            if (removingParam != null)
            {
                Undo.RecordObject(p, "remove path changer sub param");
                param.changes[i].RemoveParam(removingParam);
            }
            GUI.color = Color.white;
        }
        if (removingChanger != null)
        {
            Undo.RecordObject(p, "remove path changer param");
            param.changes.Remove(removingChanger);
        }
    }
}
}