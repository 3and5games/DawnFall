using UnityEngine;
using System.Collections.Generic;

namespace Dialoges
{
    [System.Serializable]
    public class Param : ScriptableObject
    {
        public enum ActivationType
        {
            None,
            Manual,
            Auto
        }

        private PathGame game;
        public PathGame Game
        {
            get
            {
                if (game == null)
                {
                    game = GuidManager.GetGameByParam(this);
                }

                return game;
            }
            set
            {
                game = value;
            }
        }
        public string _paramName = "new param";
        public string paramName
        {
            get
            {
                return _paramName;
            }
            set
            {
                _paramName = value;
                if (name != _paramName)
                {
                    name = _paramName;
                    game.Dirty = true;
                }
            }
        }
        public bool showing;
        public string description = "";
        public Sprite image;

        //activation fields
        public Path activationPath;
        public Condition condition;
        public List<ParamChanges> changes = new List<ParamChanges>();
        public ActivationType activationType;

        public Vector2 scrollPosition;
        public int paramGUID;
        public string tags;
        public string[] Tags
        {
            get
            {
                if (tags == "")
                {
                    return new string[0];
                }
                else
                {
                    return tags.Split(',');
                }
            }
        }
    }
}