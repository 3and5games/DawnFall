using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChangeScenes : MonoBehaviour
{
    public void LoadThirdPersonScene()
    {
        Application.LoadLevel("3rdPersonController-Demo");
    }

    public void LoadTopDownScene()
    {
        Application.LoadLevel("TopDownController-Demo");
    }

    public void LoadPlatformScene()
    {
        Application.LoadLevel("2.5DController-Demo");
    }

    public void LoadIsometricScene()
    {
        Application.LoadLevel("IsometricController-Demo");
    }

    public void LoadVMansion()
    {
        Application.LoadLevel("V-Mansion");
    }
}
