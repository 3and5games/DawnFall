using System;
using System.Collections.Generic;
using UnityEngine;
namespace Dialoges
{
    [CreateAssetMenu(fileName = "PathGame", menuName = "PathGame")]
    [Serializable]
    public class PathGame : ScriptableObject
    {
        public bool Dirty = false;
        public string gameName
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                }
            }
        }
        public string description;
        public string autor;
        [HideInInspector]
        public List<Chain> chains = new List<Chain>();
        [HideInInspector]
        public List<Param> parameters = new List<Param>();
        [HideInInspector]
        public float zoom = 1;
    }
}