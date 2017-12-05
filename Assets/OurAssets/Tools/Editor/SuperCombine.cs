using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SuperCombine : EditorWindow
{
    Texture2D metalic, raughnes;

    [MenuItem("Window/металик + раугнес = эпическое слияние")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        SuperCombine window = (SuperCombine)EditorWindow.GetWindow(typeof(SuperCombine));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Metalic");
        metalic = (Texture2D)EditorGUILayout.ObjectField(metalic, typeof(Texture2D), GUILayout.Width(100), GUILayout.Height(100));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Raughnes");
        raughnes = (Texture2D)EditorGUILayout.ObjectField(raughnes, typeof(Texture2D), GUILayout.Width(100), GUILayout.Height(100));
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("combine"))
        {
            Rect r = new Rect(0,0,metalic.width, metalic.height);

            Texture2D result = new Texture2D(metalic.width, metalic.height);

    
            Color[] metalicColors =  metalic.GetPixels();
            Color[] roughnesColors = raughnes.GetPixels();

            List<Color> resultColors = new List<Color>();
            for (int i = 0; i<metalicColors.Length;i++)
            {
                resultColors.Add(new Color(metalicColors[i].r, metalicColors[i].g, metalicColors[i].b, 1-(roughnesColors[i].r+ roughnesColors[i].g+ roughnesColors[i].b)/3));
            }

            result.SetPixels(resultColors.ToArray());

            result.Apply();

            if (!Directory.Exists(Path.Combine(Application.dataPath, "CombineTextures")))
            {
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "CombineTextures"));
            }

 

            File.WriteAllBytes(Application.dataPath+ "/CombineTextures/"+raughnes.name+"_"+metalic.name+".png", result.EncodeToPNG());
        }
    }
}
