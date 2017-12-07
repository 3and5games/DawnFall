using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;

namespace Dialoges
{
    public class DialogPlayer : Singleton<DialogPlayer>
    {
        public Action<PersonDialog> onDialogChanged;
        public Action onFinishDialog;


        private PersonDialog currentDialog;
        public PersonDialog CurrentDialog
        {
            get
            {
                return currentDialog;
            }
        }
        public delegate void StateEventHandler(State e);
        public delegate void PathEventHandler(Path e);
        public event StateEventHandler onStateIn;
		public event StateEventHandler onVariantsIn;
        public event PathEventHandler onPathGo;
        public State currentState;
		private Path currentPath;
        public void PlayState(State state, PersonDialog pd)
        {
            currentDialog = pd;
            if (onDialogChanged != null)
            {
                onDialogChanged.Invoke(pd);
            }



            onStateIn.Invoke(state);
            currentState = state;

			bool playingAuto = false;

            foreach (Path p in state.pathes.Where(p => p.auto))
            {
                if (PlayerResource.Instance.CheckCondition(p.condition))
                {
					currentPath = p;
					Invoke ("PlayDelayedPath", currentState.time);
					playingAuto = true;
                }
            }


			if (!playingAuto) {
				Invoke ("Variants", currentState.time);
			}


            if (state.pathes.Where(p => PlayerResource.Instance.CheckCondition(p.condition)).Count() == 0)
            {
				Invoke ("FinishDialog", currentState.time);
            }
        }

		private void Variants()
		{
			Debug.Log ("v1");
			if(onVariantsIn!=null)
			{
				Debug.Log ("v2");
				if (currentState.pathes.Where (p => PlayerResource.Instance.CheckCondition (p.condition) && p.auto).Count () == 0) 
				{
					Debug.Log ("v3");
					onVariantsIn.Invoke (currentState);
				}
			}
		}

		private void FinishDialog()
		{
			if (onFinishDialog != null)
			{
				onFinishDialog.Invoke();
			}
		}

		private void PlayDelayedPath()
		{
			if(currentPath)
			{
				PlayPath (currentPath, true);
			}
		}

		public void PlayPath(Path p, bool delayed = false)
        {
			currentPath = p;


			if (p.aimState != null)
            {
				
					PlayState (p.aimState, currentDialog);
					onPathGo.Invoke (p);
			
            }
			else
            {
                currentDialog.playing = false;
                currentDialog = null;
                if (onDialogChanged != null)
                {
                    onDialogChanged.Invoke(null);
                }
				FinishDialog ();
            }
        }

		public void SkipState()
		{
			CancelInvoke ("Variants");
			CancelInvoke ("PlayDelayedPath");
			Variants ();

			if(IsInvoking("FinishDialog"))
			{
				CancelInvoke ("FinishDialog");
				FinishDialog ();
			}

			if(currentState.pathes.Where(p => p.auto && PlayerResource.Instance.CheckCondition(p.condition) && p.auto).Count()>0)
			{
				PlayPath (currentState.pathes.Where(p => p.auto && PlayerResource.Instance.CheckCondition(p.condition) && p.auto).ToList()[0]);
			}

		}
    }
}
