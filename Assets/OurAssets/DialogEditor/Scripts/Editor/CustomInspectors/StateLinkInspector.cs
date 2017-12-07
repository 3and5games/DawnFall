using UnityEditor;
using System.Linq;

namespace Dialoges
{
    [CustomEditor(typeof(StateLink))]
    public class StateLinkInspector : Editor
    {
        private StateLink link;
        private PathGame game;

        private void OnEnable()
        {
            link = (StateLink)target;
            game = AssetDatabase.LoadAssetAtPath<PathGame>(AssetDatabase.GetAssetPath(link)) as PathGame;
            if (link.chain == null)
            {
                link.chain = game.chains[0];
                link.state = link.chain.StartState;
            }
        }
        public override void OnInspectorGUI()
        {
            if (!link.chain)
            {
                return;
            }
            EditorGUI.BeginChangeCheck();
            Chain chain = game.chains[EditorGUILayout.Popup(game.chains.IndexOf(link.chain), game.chains.Select(x => x.name).ToArray())];
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(link, "change link chain");
                link.chain = chain;
                if (!link.chain.states.Contains(link.state))
                {
                    link.state = link.chain.states[0];
                }
            }
            State state = null;
            EditorGUI.BeginChangeCheck();
            if (link.chain)
            {
                state = link.chain.states[EditorGUILayout.Popup(link.chain.states.IndexOf(link.state), link.chain.states.Select(x => x.description).ToArray())];
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(link, "change link state");
                link.state = state;
            }
        }
    }
}
