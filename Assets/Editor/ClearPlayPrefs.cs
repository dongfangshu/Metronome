using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClearPlayPrefs 
{
    [MenuItem("Tools/Clear")]
    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All player preferences cleared.");
    }
}
