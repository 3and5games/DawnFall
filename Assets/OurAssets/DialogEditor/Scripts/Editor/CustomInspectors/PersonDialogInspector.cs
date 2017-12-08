using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dialoges
{
    [CustomEditor(typeof(PersonDialog))]
    public class PersonDialogInspector : Editor
    {
        private PersonDialog dialog;
        private SerializedObject dialogObject;
        private SerializedProperty dialogProperty;
        private bool inspectedFlag = false;
        private QuestWindow qw;
        private List<Chain> inspectedChains = new List<Chain>();
        private SerializedProperty cameraPoints;

        private void OnEnable()
        {
            dialog = (PersonDialog)target;
            dialogObject = new SerializedObject(dialog);
            dialogProperty = dialogObject.FindProperty("pathEvents");
            cameraPoints = serializedObject.FindProperty("points");
        }
        public override void OnInspectorGUI()
        {
            

            if (inspectedFlag != dialog.playing)
            {
                if (dialog.playing)
                {
                    qw = QuestWindow.Init(GuidManager.GetGameByChain(dialog.PersonChain));
                    //qw.DebugPathGame(DialogPlayer.Instance.currentState);
                }
                else
                {
                    if (qw)
                    {
                        qw.Close();
                    }
                }
                inspectedFlag = dialog.playing;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Dialogs pack:");
            dialog.game = (PathGame)EditorGUILayout.ObjectField(dialog.game, typeof(PathGame), false);
            GUILayout.EndHorizontal();
            if (dialog.game && dialog.game.chains.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Dialog:");


                if (!dialog.game.chains.Contains(dialog.PersonChain))
                {
                    Debug.Log(dialog.game.chains[0]);
                    dialog.PersonChain = dialog.game.chains[0];
					SetEvents();

                    if (dialog.PersonChain)
                    {
                        SetEvents();
                    }
                }
                Chain ch = dialog.game.chains[EditorGUILayout.Popup(dialog.game.chains.IndexOf(dialog.PersonChain), dialog.game.chains.Select(x => x.dialogName).ToArray())];
                if (dialog.PersonChain != ch)
                {
                    dialog.PersonChain = ch;

                    if (dialog.PersonChain)
                    {
                        SetEvents();
                    }
                }
                GUILayout.EndHorizontal();
                int i = 0;

                foreach (KeyValuePair<Path, PathEvent> pathEvent in dialog.PathEventsList)
                {
                    string aim = "";

                    if (pathEvent.Key.aimState != null)
                    {
                        aim = pathEvent.Key.aimState.description;
                    }
                    GUILayout.Label(pathEvent.Key.text + "->" + aim);
                    Undo.RecordObject(target, "Changed event");
                    EditorGUILayout.PropertyField(dialogProperty.GetArrayElementAtIndex(i));
                    dialogObject.ApplyModifiedProperties();
                    i++;
                }
            }

            EditorGUILayout.PropertyField(cameraPoints);
            serializedObject.ApplyModifiedProperties();
        }
        private void AddPathes(Chain c, List<Path> newPathes, List<PathEvent> newEvents)
        {
            if (inspectedChains.Contains(c))
            {
                return;
            }
            else
            {
                inspectedChains.Add(c);
            }
            foreach (State s in c.states)
            {
                foreach (Path p in s.pathes)
                {
                    if (p.aimState != null && GuidManager.GetChainByState(p.aimState) != c)
                    {
                        AddPathes(GuidManager.GetChainByState(p.aimState), newPathes, newEvents);
                    }
                    if (p.withEvent)
                    {
                        newEvents.Add(new PathEvent());
                        newPathes.Add(p);
                    }
                }
            }
            inspectedChains.Clear();
        }
        private void SetEvents()
        {
			Debug.Log ("set events");
            List<Path> newPathes = new List<Path>();
            List<PathEvent> newEvents = new List<PathEvent>();
            AddPathes(dialog.PersonChain, newPathes, newEvents);
            inspectedChains.Clear();
            dialog.pathes = newPathes.ToArray();
            dialog.pathEvents = newEvents.ToArray();
            dialogObject = new SerializedObject(dialog);
            dialogProperty = dialogObject.FindProperty("pathEvents");
        }
    }
}