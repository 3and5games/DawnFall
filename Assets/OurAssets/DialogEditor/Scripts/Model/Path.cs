using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Dialoges
{
    [System.Serializable]
    public class Path : ScriptableObject
    {
        private PathGame game;
        public PathGame Game
        {
            get
            {
                if (game == null)
                {
                    game = GuidManager.GetGameByPath(this);
                }
                return game;
            }
            set
            {
                game = value;
            }
        }
        public string _text = "";
        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                if (value != "")
                {
                    string ss = value.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                    ss = ss.Substring(0, Mathf.Min(10, ss.Length));
                    if (name != ss)
                    {
                        name = ss;
                        game.Dirty = true;
                    }
                }
                _text = value;
            }
        }
        public bool auto = false;
        public bool waitInput = false;
        public bool withEvent = false;
        public Condition condition = new Condition();
        public List<ParamChanges> changes = new List<ParamChanges>();
        public int aimStateGuid;
        public UnityAction pathEvent;
        public State aimState = null;
    }
}


