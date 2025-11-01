using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
public class YeetBakery : EditorWindow
{
    [MenuItem("Tools/Yeet Bakery")]
    static void DeleteBakery()
    {
        string directoryPathBakery = "Assets/Bakery/";
        string directoryPathScripts = "Assets/Editor/x64/Bakery/scripts/";
        string directoryPathEditor = "Assets/Editor/x64/Bakery/"; //this one needs the script exclusion

        string symbolToRemove = "BAKERY_INCLUDED";
        BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(activeBuildTarget);
        string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        defineSymbols = defineSymbols.Replace(symbolToRemove, string.Empty).Replace(";;", ";").Trim(';');
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbols);

        string[] exceptions = new string[]
        {
        "ftLightmaps.cs",
        "ftLightmapsStorage.cs",
        "ftLocalStorage.cs",
        "ftGlobalStorage.cs",
        "BakeryProjectSettings.cs",
        "ftModelPostProcessor.cs",
        "ftSavedPadding2.cs",

        "ftLightmaps.meta",
        "ftLightmapsStorage.meta",
        "ftLocalStorage.meta",
        "ftGlobalStorage.meta",
        "BakeryProjectSettings.meta",
        "ftModelPostProcessor.meta",
        "ftSavedPadding2.meta",
        "scripts"
        };

        string[] files = Directory.GetFiles(directoryPathBakery, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            if (!isFileExclusionSimple(fileName, exceptions))
            {
                try
                {
                    File.Delete(file);
                    Debug.Log($"Deleted: {fileName}");
                } catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        files = Directory.GetFiles(directoryPathScripts, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            if (!isFileExclusionSimple(fileName, exceptions))
            {
                try
                {
                    File.Delete(file);
                    Debug.Log($"Deleted: {fileName}");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        files = Directory.GetFiles(directoryPathEditor, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            string dirName = Path.GetDirectoryName(file);

            if (!IsFileExclusion(fileName, dirName, exceptions))
            {
                try
                {
                    File.Delete(file);
                    Debug.Log($"Deleted: {fileName}");
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }

        Debug.Log("Sucessfully deleted Bakery");
        AssetDatabase.Refresh();
    }

    static bool IsFileExclusion(string fileName, string directoryName, string[] exceptions)
    {
        string fullPath = Path.Combine(directoryName, fileName);

        foreach (string exception in exceptions)
        {
            if (fullPath.Equals(exception) || directoryName.EndsWith(exception))
            {
                return true;
            }
        }
        return false;
    }

    static bool isFileExclusionSimple(string fileName, string[] exceptions)
    {
        foreach (string exception in exceptions)
        {
            if (fileName.Equals(exception))
            {
                return true;
            }
        }
        return false;
    }
}
