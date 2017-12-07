using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextContent : MonoBehaviour
{
    private RectTransform contentTransform, textTransform;

    void Start()
    {
        contentTransform = GetComponent<RectTransform>();
        textTransform = GetComponentInChildren<Text>().GetComponent<RectTransform>();
    }

    void Update()
    {
        contentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textTransform.sizeDelta.y);
    }
}
