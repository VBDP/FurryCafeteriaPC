
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

using System;
using UnityEditor;
using UdonSharpEditor;

namespace VRCCoffeeSet
{
    [CustomEditor(typeof(Tool))]
    public class Tool_Editor : Editor 
    {
        Tool script;
        TypeTool _type;

        int indexSkin;
        
        bool toggleVariable = false;
        bool toggleBase = false;

        GUIStyle paddingLeft = new GUIStyle();

        private void Awake()
        {
            script = target as Tool;
            paddingLeft.padding.left = 15;
        }

        void Initialize()
        {
            _type = script.m_type;
            Transform thisObj = Selection.activeTransform;

            // Skin
            indexSkin = SkinManager.GetSkinTool(script);

            // Animator
            switch (_type)
            {
                case TypeTool.Ingredient:
                case TypeTool.Filter:
                    if (!script.m_animator) script.m_animator = script.GetComponentInChildren<Animator>();
                    break;
                default:
                    script.m_animator = null;
                    break;
            }

            // Particle
            switch (_type)
            {
                case TypeTool.Squeezer:
                case TypeTool.Ingredient:
                case TypeTool.Pitcher:
                    Array.Resize<ParticleSystem>(ref script.m_particle, 2);
                    break;
                default: 
                    script.m_particle = null;
                    break;
            }

            // Axis
            switch (_type)
            {
                case TypeTool.Ingredient: break;
                case TypeTool.Pitcher:
                    Array.Resize<Transform>(ref script.m_axis, 2);
                    break;
                default:
                    script.m_axis = null;
                    break;
            }

            // Audio
            if (_type == TypeTool.NULL) script.m_audio = null;

            // Ingredient Variable
            if (_type != TypeTool.Ingredient)
            {
                script.isCap = false;
                script.isAnim = false;
                script.isTilt = false;
                script.isTrig = false;
                script.isHold = false;
                script.lengthAnim = 0;
                script.eventAnim = null;
            }

            serializedObject.Update();
        }

        public override void OnInspectorGUI()
        {
            if (_type != script.m_type) Initialize();

        #region Skin

            int tempIndexSkin = indexSkin;
            switch (_type)
            {
                case TypeTool.KnockBox:
                case TypeTool.Filter:
                case TypeTool.Tamper:
                    EditorGUI.BeginChangeCheck();
                    indexSkin = (int)(SkinsMachine)EditorGUILayout.EnumPopup("Skin Select", (SkinsMachine)indexSkin);
                    if ( EditorGUI.EndChangeCheck() )
                    {
                        string tempStr = SkinManager.ChangeTool(script, indexSkin);
                        if (tempStr == null) return;
                        else
                        {
                            indexSkin = tempIndexSkin;
                            Debug.LogWarning(tempStr);
                        }
                    }
                    break;
                default:
                    GUILayout.Label("Skin Select unavailable");
                    break;
            }

        #endregion

            GUILayout.Space(20);
            toggleVariable = EditorGUILayout.Foldout(toggleVariable, "Do not change any variables");
            if (!toggleVariable) return;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_type"), new GUIContent("Tool Type"));
            if (script.m_type == TypeTool.Straw) EditorGUILayout.PropertyField(serializedObject.FindProperty("m_skinIndex"), new GUIContent("Skin Index"));
            serializedObject.ApplyModifiedProperties();

            if (_type != script.m_type) Initialize();

            switch (_type)
            {
                case TypeTool.Squeezer:
                    GUILayout.Label("Particle");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(0), new GUIContent("Lemon"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(1), new GUIContent("GrapeFruit"));
                    EditorGUILayout.EndVertical();
                    break;
                case TypeTool.Ingredient:
                    script.isCap = EditorGUILayout.Toggle("Cap", script.isCap);

                    script.isAnim = EditorGUILayout.Toggle("Animated", script.isAnim);
                    if (script.isAnim)
                    {
                        EditorGUILayout.BeginVertical(paddingLeft);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_animator"), new GUIContent("Animator"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("lengthAnim"), new GUIContent("Length"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("eventAnim"), new GUIContent("Event Name"));
                        EditorGUILayout.EndVertical();
                        serializedObject.ApplyModifiedProperties();
                        GUILayout.Space(10);
                    }
                    else
                    {
                        script.m_animator = null;
                        script.lengthAnim = 0;
                        script.eventAnim = null;
                    }

                    script.isTrig = EditorGUILayout.Toggle("Need to Click", script.isTrig);
                    if (script.isTrig)
                    {
                        EditorGUILayout.BeginVertical(paddingLeft);
                        script.isHold = EditorGUILayout.Toggle("Need to Hold", script.isHold);
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(10);
                    }

                    script.isTilt = EditorGUILayout.Toggle("Need to Tilt", script.isTilt);
                    if (script.isTilt)
                    {
                        Array.Resize<Transform>(ref script.m_axis, 2);
                        serializedObject.Update();
                        GUILayout.Label("Axis");
                        EditorGUILayout.BeginVertical(paddingLeft);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_axis").GetArrayElementAtIndex(0), new GUIContent("Out"));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_axis").GetArrayElementAtIndex(1), new GUIContent("Base"));
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(10);
                    }
                    else script.m_axis = null;

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(0), new GUIContent("Particle"));
                    break;
                case TypeTool.Pitcher:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_contentMesh"), new GUIContent("Content Mesh"));
                    GUILayout.Space(10);
                    GUILayout.Label("Axis");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_axis").GetArrayElementAtIndex(0), new GUIContent("Out"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_axis").GetArrayElementAtIndex(1), new GUIContent("Base"));
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(10);
                    GUILayout.Label("Particle");
                    EditorGUILayout.BeginVertical(paddingLeft);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(0), new GUIContent("Latte"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_particle").GetArrayElementAtIndex(1), new GUIContent("Cappuccino"));
                    EditorGUILayout.EndVertical();
                    break;
            }

            if (_type != TypeTool.NULL) EditorGUILayout.PropertyField(serializedObject.FindProperty("m_audio"), new GUIContent("Audio"));

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            toggleBase = EditorGUILayout.BeginFoldoutHeaderGroup(toggleBase, "Udon Variables");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (toggleBase) {
                UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target);
                base.OnInspectorGUI();
            }
        }
    }
}