using UnityEngine;
using UnityEditor;

namespace Dialoges
{
    [CustomEditor(typeof(Chain))]
    public class ChainInspector : Editor
    {
        private Chain c;
        private PathGame _game;
        private PathGame game
        {
            get
            {
                if (!_game)
                {
                    c = (Chain)target;
                    _game = AssetDatabase.LoadAssetAtPath<PathGame>(AssetDatabase.GetAssetPath(c));
                }
                return _game;
            }
        }

        public override void OnInspectorGUI()
        {
            c = (Chain)target;
            EditorGUI.BeginChangeCheck();
            string dName = EditorGUILayout.TextField(c.dialogName, GUILayout.Height(15));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(c, "Edit chain");
                c.dialogName = dName;
            }
            GUI.color = Color.white;
        }
    }
}