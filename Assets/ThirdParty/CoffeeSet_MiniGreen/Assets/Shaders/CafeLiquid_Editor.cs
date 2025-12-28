#if !COMPILER_UDONSHARP && UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CafeLiquid_Editor : ShaderGUI
{
    CafeLiquid_Group objGroup;
    Tuple<int, Material> infoGroup;

    Material matSelf;
    Material matMask { get{ return infoGroup.Item2; } }

    bool Initialized = false;
    void Initialize()
    {
        if (Initialized) return;
        Initialized = true;
        
        string path = AssetDatabase.GetAssetPath( Selection.activeObject );
        matSelf = AssetDatabase.LoadAssetAtPath<Material>( path );
        
        string pathGroup = AssetDatabase.GetAssetPath( matSelf.shader );
        pathGroup = pathGroup.Replace(Path.GetFileName(pathGroup), "CafeLiquid_GroupObj.asset");
        objGroup = AssetDatabase.LoadAssetAtPath<CafeLiquid_Group>(pathGroup);

        infoGroup = objGroup.GetMatGroup(matSelf);
    }

    public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Initialize();

        base.OnGUI(materialEditor, properties);

        MatchingProperties();
    }

    void MatchingProperties()
    {
        GUILayout.Space(20);
        if (!matMask)
        {
            if (GUILayout.Button("Create/Select mask material")) CreateMaskMat();
            return;
        }
        if (GUILayout.Button("Select group scriptable object")) Selection.activeObject = objGroup;
        // matMask.SetFloat("_Layer", matSelf.GetFloat("_Layer"));
        matMask.SetFloat("_Height", matSelf.GetFloat("_Height"));
        matMask.SetVector("_MinMax", matSelf.GetVector("_MinMax"));
    }

    void CreateMaskMat()
    {
        string path = AssetDatabase.GetAssetPath( Selection.activeObject ).Replace(".mat", "_Mask.mat");
        Material newMatMask = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!newMatMask)
        {
            var tempMat = new Material( Shader.Find("VRCCoffee/Cafe LiquidMask") );
            AssetDatabase.CreateAsset(tempMat, path);
            newMatMask = AssetDatabase.LoadAssetAtPath<Material>(path);
            Debug.Log($"{matMask}, Mask material has been generated.");
        }
        else
        {
            Debug.Log($"{matMask}, Mask material has been selected.");
        }
        objGroup.SetMatGroup(matSelf, newMatMask);
        infoGroup = objGroup.GetMatGroup(matSelf);
    }
}
#endif