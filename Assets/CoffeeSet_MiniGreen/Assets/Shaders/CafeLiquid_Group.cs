#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CreateAssetMenu(fileName = "Shader Group", menuName = "VRCCoffee/Shader Group", order = int.MaxValue)]
public class CafeLiquid_Group : ScriptableObject
{
    [SerializeField] public List<Material> _matLiquid;
    [SerializeField] public List<Material> _matMask;
    [SerializeField] public List<bool> _folds;

    public Tuple<int, Material> GetMatGroup(Material matLiquid)
    {
        int index = -1;
        Material targetMat = null;
        for (int i = 0; i < _matLiquid.Count; i++)
        {
            if (_matLiquid[i] != matLiquid) continue;
            index = i;
            targetMat = _matMask[i];
        }
        return new Tuple<int, Material>(index, targetMat);
    }

    public void SetMatGroup(Material matLiquid, Material matMask)
    {
        int index = -1;
        for (int i = 0; i < _matLiquid.Count; i++)
        {
            if (_matLiquid[i] != matLiquid) continue;
            index = i;
        }

        // Set
        if (index > -1)
        {
            _matMask[index] = matMask;
        }
        // Add
        else
        {
            _matLiquid.Add(matLiquid);
            _matMask.Add(matMask);
            _folds.Add(false);
        }
    }
    public void AddMatGroup()
    {
        _matLiquid.Add(null);
        _matMask.Add(null);
        _folds.Add(false);
    }
    public void RemoveMatGroup(int index)
    {
        _matLiquid.RemoveAt(index);
        _matMask.RemoveAt(index);
        _folds.RemoveAt(index);
    }
}

[CustomEditor(typeof(CafeLiquid_Group))]
public class CafeLiquid_GroupEditor : Editor
{
    CafeLiquid_Group instance;

    private void Awake()
    {
        instance = target as CafeLiquid_Group;
    }

    public override void OnInspectorGUI()
    {
        string foldName = "";
        for (int i = 0; i < instance._matLiquid.Count; i++)
        {
            foldName = instance._matLiquid[i] ? instance._matLiquid[i].name : "NULL";
            EditorGUILayout.BeginHorizontal();
            instance._folds[i] = EditorGUILayout.BeginFoldoutHeaderGroup(instance._folds[i], foldName);
            if (GUILayout.Button("Remove", GUILayout.Width(75)))
            {
                instance.RemoveMatGroup(i);
                return;
            }
            EditorGUILayout.EndHorizontal();
            if (instance._folds[i])
            {
                instance._matLiquid[i] = (Material)EditorGUILayout.ObjectField("Liquid", instance._matLiquid[i], typeof(Material), true);
                instance._matMask[i] = (Material)EditorGUILayout.ObjectField("Mask", instance._matMask[i], typeof(Material), true);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Group", GUILayout.Height(25)))
        {
            instance.AddMatGroup();
            return;
        }
        if (GUILayout.Button("Remove Empty Group", GUILayout.Height(25)))
        {
            List<int> index = new List<int>();
            for (int i = 0; i < instance._matLiquid.Count; i++)
                if (instance._matLiquid[i] == null && instance._matMask[i] == null) index.Add(i);
            for (int i = 0; i < index.Count; i++) instance.RemoveMatGroup(index[index.Count-1 - i]);
            return;
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif