using System.Collections.Generic;
using UnityEngine;


namespace Dialoges
{
    [System.Serializable]
    public class PersonDialog : MonoBehaviour
    {
        [SerializeField]
        private Chain _personChain;
        public Chain PersonChain
        {
            set
            {
                _personChain = value;
            }
            get
            {
                return _personChain;
            }
        }

        public bool playing;
        public Path[] pathes;
        public PathEvent[] pathEvents;
        public Dictionary<Path, PathEvent> pathEventsList = new Dictionary<Path, PathEvent>();
        public Dictionary<Path, PathEvent> PathEventsList
        {
            get
            {
                Dictionary<Path, PathEvent> newPathEvents = new Dictionary<Path, PathEvent>();

                for (int i = 0; i < pathes.Length; i++)
                {
                    newPathEvents.Add(pathes[i], (PathEvent)pathEvents[i]);
                }
                pathEventsList = newPathEvents;
                return newPathEvents;
            }
        }
        [HideInInspector]
        public PathGame game;
        [SerializeField]


		[ContextMenu("talk")]
        public void Talk()
        {
            playing = true;
            DialogPlayer.Instance.onPathGo += new DialogPlayer.PathEventHandler(InvokeEvent);
            DialogPlayer.Instance.onFinishDialog += FinishDialog;
            DialogPlayer.Instance.PlayState(PersonChain.StartState, this);
        }

        private void InvokeEvent(Path p)
        {
            if (!PersonChain.states.Contains(p.aimState))
            {
                PersonChain = GuidManager.GetChainByState(p.aimState);
            }
            if (pathEventsList.ContainsKey(p))
            {
                pathEventsList[p].Invoke();
            }
        }

        private void FinishDialog()
        {
            DialogPlayer.Instance.onPathGo -= new DialogPlayer.PathEventHandler(InvokeEvent);
        }
    }
}
