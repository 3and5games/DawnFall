using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//rename to DialogInput
using System;

namespace Dialoges
{
    public class DialogRaycaster : MonoBehaviour
    {
        public Action OnPersonAim, OnPersonMissed;
        public float DialogDistance = 2;
        private PersonDialog person;
        private PersonDialog Person
        {
            set
            {
                if (person != value)
                {
                    if (value)
                    {
                        if (OnPersonAim != null)
                        {
                            OnPersonAim.Invoke();
                        }
                    }
                    else
                    {
                        if (OnPersonMissed != null)
                        {
                            OnPersonMissed.Invoke();
                        }
                    }

                    person = value;
                }
            }
            get
            {
                return person;
            }
        }

        public bool avaliable = true;
        public KeyCode startDialogKeycode = KeyCode.E;

        void Start()
        {
            //FindObjectOfType<SmoothCameraWithBumper>().onObjectRaycasted += Raycasted;
            //FindObjectOfType<SmoothCameraWithBumper>().onObjectRaycastedMissed += RaycastedMissed;
        }

        private void Raycasted(GameObject go, float distance)
        {
            if (distance > DialogDistance)
            {
                RaycastedMissed();
                return;
            }

            if (!go.GetComponent<PersonDialog>())
            {
                avaliable = true;
               
            }
            Person = go.GetComponent<PersonDialog>();
        }

        private void RaycastedMissed()
        {
            if (Person)
            {
                
                Person = null;
                avaliable = true;
            }
        }

        void Update()
        {
            if (Input.GetKey(startDialogKeycode) && avaliable)
            {
                if (Person)
                {
                    avaliable = false;
                    
                    Person.Talk();
                }
            }
        }
    }
}