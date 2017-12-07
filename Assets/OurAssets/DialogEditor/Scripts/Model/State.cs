using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dialoges
{
    [System.Serializable]
    public class State : ScriptableObject
    {
#if UNITY_EDITOR
        public string shortName;
#endif
        [SerializeField]
        public int guid;
        public int Guid
        {
            get
            {
                if (guid == 0)
                {
                    guid = GuidManager.GetStateGuid();
                }
                return guid;
            }
            set
            {
                guid = value;
            }
        }

        public float time = 0;

        private PathGame game;
        public PathGame Game
        {
            get
            {
                if (game == null)
                {
#if UNITY_EDITOR
                    game = (PathGame)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(this), typeof(PathGame));
#endif
                }
                return game;
            }
            set
            {
                game = value;
            }
        }
        private Chain chain;
        public Chain Chain
        {
            get
            {
                return chain;
            }
            set
            {
                chain = value;
            }
        }

        public string _description;
        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != "")
                {
                    string ss = value.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                    ss = ss.Substring(0, Mathf.Min(ss.Length, 20));
                    if (name != ss)
                    {
                        name = ss;
                        game.Dirty = true;
                    }
                }
                _description = value;
            }
        }
        public List<Path> pathes = new List<Path>();
        public Rect position;
        public AudioClip sound;
        public Sprite image;

        public void Init(Chain chain)
        {
            Guid = GuidManager.GetStateGuid();
            this.chain = chain;
            game = chain.Game;
            description = "";
            float z = 1;
            z = game.zoom;
            position = new Rect(300, 300, 208 * z, 30 * z);
        }

        public Path AddPath()
        {
            Path newPath = CreateInstance<Path>();
            newPath.Game = game;
            pathes.Add(newPath);
            Game.Dirty = true;
            return newPath;
        }

        public void RemovePathWithoutDestroy(Path path)
        {
            game.Dirty = true;
            pathes.Remove(path);
        }

        public void RemovePath(Path path)
        {
            Game.Dirty = true;
            pathes.Remove(path);
        }

    }
}

