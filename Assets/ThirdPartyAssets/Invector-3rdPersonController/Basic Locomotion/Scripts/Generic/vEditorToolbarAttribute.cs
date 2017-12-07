using UnityEngine;
using System.Collections;

public class vEditorToolbarAttribute : PropertyAttribute
{
    public readonly string title;
    public vEditorToolbarAttribute(string title)
    {
        this.title = title;
    }
}

