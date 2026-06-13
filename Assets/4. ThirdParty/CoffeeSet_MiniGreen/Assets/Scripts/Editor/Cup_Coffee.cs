
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UdonSharpEditor;

namespace VRCCoffeeSet
{
    [CustomEditor(typeof(Cup_Coffee))]
    public class Cup_Coffee_Editor : Editor
    {
        public Cup_Coffee _script;
        
        public Renderer _renderer;
        public int _skinIndex;
        public Color[] _skinColor = new Color[0];

        bool toggleVariable = false;
        bool toggleBase = false;

        void Initialize()
        {
            _script = target as Cup_Coffee;
            _renderer = _script.GetComponentInChildren<Renderer>();

            // Axis
            if (_script.m_type == TypeCup.Espresso) Array.Resize(ref _script.m_axis, 2);
            else _script.m_axis = null;

            // Particle
            if (_script.m_type == TypeCup.Espresso) Array.Resize(ref _script.m_particle, 3);
            else Array.Resize(ref _script.m_particle, 1);

            // Skin
            _skinIndex = SkinManager.GetSkinCup(_script);
            _skinColor = SkinManager.GetColorCup(_script, _renderer);
        }

        public override void OnInspectorGUI()
        {
            if (_script == null) Initialize();

            serializedObject.Update();

            #region  Skin

            EditorGUI.BeginChangeCheck();
            _skinIndex = EditorGUILayout.Popup("Skin Select", _skinIndex, SkinManager.GetSkinList_Cup(_script.m_type));
            if ( EditorGUI.EndChangeCheck() )
            {
                var tempSkinIndex = _skinIndex;
                if (SkinManager.ChangeCup(_script, _skinIndex) == null)
                {
                    _skinIndex = tempSkinIndex;
                    MainMethod.DebugLogError("Skin Change Error");
                }
                else
                {
                    DestroyImmediate(this);
                    return;
                }
            }
            
            switch (_script.m_type)
            {
                case TypeCup.Espresso:
                    switch ( (SkinsCupEspresso)_skinIndex )
                    {
                        default: break;
                    }
                    break;

                case TypeCup.Coffee:
                    switch ( (SkinsCupCoffee)_skinIndex )
                    {
                        case SkinsCupCoffee.Mug:
                            EditorGUI.BeginChangeCheck();
                            _skinColor[0] = EditorGUILayout.ColorField("Cup Color", _skinColor[0]);
                            if ( EditorGUI.EndChangeCheck() ) SkinManager.SetColor(_renderer, 0, _skinColor[0]);
                            break;
                    }
                    break;

                case TypeCup.Glass:
                    switch ( (SkinsCupGlass)_skinIndex )
                    {
                        default: break;
                    }
                    break;
            }

            #endregion

            GUILayout.Space(20);
            toggleVariable = EditorGUILayout.Foldout(toggleVariable, "Do not change any variables");
            if (!toggleVariable) return;

            // begin change check
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_type"), new GUIContent("Cup Type"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Initialize();
            }
            
            switch (_script.m_type)
            {
                case TypeCup.Espresso:
                    GUILayout.Label("Axis");
                    _script.m_axis[0] = EditorGUILayout.ObjectField("_Out", _script.m_axis[0], typeof(Transform), true) as Transform;
                    _script.m_axis[1] = EditorGUILayout.ObjectField("_Middle", _script.m_axis[1], typeof(Transform), true) as Transform;
                    EditorGUILayout.Space();
                    GUILayout.Label("Particle");
                    _script.m_particle[0] = EditorGUILayout.ObjectField("_Steam", _script.m_particle[0], typeof(ParticleSystem), true) as ParticleSystem;
                    _script.m_particle[1] = EditorGUILayout.ObjectField("_Pour", _script.m_particle[1], typeof(ParticleSystem), true) as ParticleSystem;
                    _script.m_particle[2] = EditorGUILayout.ObjectField("_Pour Caramel Mix", _script.m_particle[2], typeof(ParticleSystem), true) as ParticleSystem;
                    break;
                case TypeCup.Coffee:
                    _script.m_particle[0] = EditorGUILayout.ObjectField("Particle Steam", _script.m_particle[0], typeof(ParticleSystem), true) as ParticleSystem;
                    break;
                case TypeCup.Glass:
                    _script.m_particle[0] = EditorGUILayout.ObjectField("Particle Bubble", _script.m_particle[0], typeof(ParticleSystem), true) as ParticleSystem;
                    _script.m_contentHeight = EditorGUILayout.Vector2Field("Content Height", _script.m_contentHeight);
                    break;
            }

            EditorGUILayout.Space();
            toggleBase = EditorGUILayout.BeginFoldoutHeaderGroup(toggleBase, "Udon Behaviour");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (toggleBase)
            {
                UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target);
                base.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}