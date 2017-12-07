using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateGui : MonoBehaviour {

    private Action onSkipped;

    public void ShowState(string text, Action onSkipped = null)
    {
        GetComponentInChildren<Text>().text = text;
        this.onSkipped = onSkipped;
        GetComponent<Animator>().SetBool("Active", true);
    }

    public void HideState()
    {
        GetComponent<Animator>().SetBool("Active", false);
    }

    public void SkipState()
    {
        HideState();
        if (onSkipped!=null)
        {
            onSkipped.Invoke();
        }
    }
}
