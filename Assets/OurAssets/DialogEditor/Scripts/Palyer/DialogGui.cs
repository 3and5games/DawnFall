using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;

namespace Dialoges
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogGui : Singleton<DialogGui>
    {
        private State currentState;

        private AudioSource source;
        private AudioSource Source
        {
            get
            {
                if (!source)
                {
                    source = GetComponent<AudioSource>();
                }
                return source;
            }
        }

        void OnEnable()
        {
            DialogPlayer.Instance.onStateIn += StateIn;
			DialogPlayer.Instance.onVariantsIn += VariantsIn;
        }

        void OnDisable()
        {
            if (DialogPlayer.Instance)
            {
                DialogPlayer.Instance.onStateIn -= StateIn;
				DialogPlayer.Instance.onVariantsIn -= VariantsIn;
            }
        }

		private void VariantsIn(State e)
		{
			if(e.pathes.Where(p => PlayerResource.Instance.CheckCondition(p.condition) && p.auto).Count() == 0)
			{
				GetComponentInChildren<StateGui>().HideState();
				List <Path> avaliablePathes = currentState.pathes.Where(p => PlayerResource.Instance.CheckCondition(p.condition)).ToList();
				avaliablePathes = avaliablePathes.Where(p=>!p.auto).ToList();
				if (avaliablePathes.Count>0)
				{
					GetComponentInChildren<VariantsGui>().ShowVariants(avaliablePathes, Apply);
				}
			}
		}

        private void StateIn(State e)
        {
			GetComponentInChildren<StateGui>().ShowState(e.description, SkipState);
            currentState = e;
        }

		public void SkipState()
		{
			DialogPlayer.Instance.SkipState();
		}


        private void Apply(Path appliedPath)
        {
            DialogPlayer.Instance.PlayPath(appliedPath);
        }

		public void ShowLastState()
		{
			GetComponentInChildren<VariantsGui> ().Hide ();
			StateIn (currentState);
		}
    }
}
