using UnityEngine;

public class Tabs
{
    public static int DrawTabs(Rect r, string[] options, int selected, bool vertical = false)
    {
        const float DarkGray = 0.4f;
        const float LightGray = 0.9f;
        const float StartSpace = 5;
        Color storeColor = new Color(LightGray, LightGray, LightGray);
        Color bgCol = new Color(LightGray, LightGray, LightGray);
        Color highlightCol = new Color(DarkGray, DarkGray, DarkGray);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.padding.bottom = 8;
        GUI.backgroundColor = storeColor;
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, highlightCol);
        texture.Apply();
        GUI.DrawTexture(r, texture);
        for (int i = 0; i < options.Length; ++i)
        {
            GUI.backgroundColor = i == selected ? highlightCol : bgCol;
            if (!vertical)
            {
                if (GUI.Button(new Rect(r.position.x + StartSpace + i * ((r.width - StartSpace * 2) / options.Length), r.position.y + StartSpace / 3, (r.width - StartSpace * 2) / options.Length, r.height - StartSpace / 2), options[i], buttonStyle))
                {
                    selected = i;
                }
            }
            else
            {
                if (GUI.Button(new Rect
                    (
                    r.position.x + StartSpace,
                    r.position.y + StartSpace + i * ((r.height - StartSpace * 2) / options.Length),
                    r.width - StartSpace * 2,
                    (r.height - StartSpace * 2) / options.Length
                    ), options[i], buttonStyle))
                {
                    selected = i; 
                }
            }

        }
        return selected;
    }
}
