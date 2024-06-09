using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreBuildPlayerPrefsReset : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Resetting PlayerPrefs before build...");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}