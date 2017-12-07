using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Dialoges
{
    public class VariantsGui : MonoBehaviour
    {
        private int currentVariant = 0;
        private List<Path> avaliablePathes = new List<Path>();
        public Text pathText;
        private Action<Path> onApply;
		private Vector3 minusPosition, zeroPosition, plusPosition;

		void Start()
		{
			zeroPosition = new Vector3(pathText.transform.localPosition.x, pathText.transform.localPosition.y, pathText.transform.localPosition.z);
			minusPosition = new Vector3(zeroPosition.x-pathText.GetComponent<RectTransform>().rect.width, zeroPosition.y, zeroPosition.z);
			plusPosition = new Vector3(zeroPosition.x+pathText.GetComponent<RectTransform>().rect.width, zeroPosition.y, zeroPosition.z);
		}

        private void Update()
        {
            if (GetComponent<Animator>().GetBool("Active"))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    SwitchLeft();
                }

                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    SwitchRight();
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Apply();
                }
            }
        }

        public void Apply()
        {
            if (onApply!=null)
            {
                onApply.Invoke(avaliablePathes[currentVariant]);
            }
            GetComponent<Animator>().SetBool("Active", false);
        }

        public void SwitchRight()
        {
            currentVariant++;
            if (currentVariant > avaliablePathes.Count-1)
            {
                currentVariant = 0;
            }
			RectTransform newTextTransform = Instantiate (pathText.gameObject, pathText.transform.parent).GetComponent<RectTransform>();
			newTextTransform.localPosition = plusPosition;
			StartCoroutine(MoveFromTo (newTextTransform, plusPosition, zeroPosition, 1000));
			StartCoroutine(MoveFromTo (pathText.transform, zeroPosition, minusPosition, 1000));
			Destroy (pathText.gameObject, 0.5f);
			pathText = newTextTransform.GetComponent<Text> ();
			pathText.text = avaliablePathes[currentVariant].text;
        }

        public void SwitchLeft()
        {
            currentVariant--;
            if (currentVariant < 0)
            {
                currentVariant = avaliablePathes.Count - 1;
            }
			RectTransform newTextTransform = Instantiate (pathText.gameObject, pathText.transform.parent).GetComponent<RectTransform>();
			newTextTransform.localPosition = minusPosition;
			StartCoroutine(MoveFromTo (newTextTransform, minusPosition, zeroPosition, 1000));
			StartCoroutine(MoveFromTo (pathText.transform, zeroPosition, plusPosition, 1000));
			Destroy (pathText.gameObject, 0.5f);
			pathText = newTextTransform.GetComponent<Text> ();
			pathText.text = avaliablePathes[currentVariant].text;
        }

        public void ShowVariants(List<Path> pathes, Action<Path> onApply)
        {
            this.onApply = onApply;
            GetComponent<Animator>().SetBool("Active", true);
            avaliablePathes = pathes;
            currentVariant = 0;
            pathText.text = avaliablePathes[currentVariant].text;
        }

		public void Hide()
		{
			GetComponent<Animator>().SetBool("Active", false);
		}

		IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed) {
			float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
			float t = 0;
			while (t <= 1.0f) {
				t += step; // Goes from 0 to 1, incrementing by step each time
				objectToMove.localPosition = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
				yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
			}
			objectToMove.localPosition = b;
		}
    }
}
