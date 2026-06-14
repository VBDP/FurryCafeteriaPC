using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.CustomInspector
{
    [CustomEditor(typeof(NightModeSwitcher))]
    public class NightModeSwitcherEditor : Editor
    {
        private bool _isOpenDangerZone = false;
        private int _shaderPropID = 0;

        private SerializedProperty _dawnKnob;
        private SerializedProperty _duskKnob;
        private SerializedProperty _nightModeRenderer;
        private SerializedProperty _nightModePropertyName;
        private SerializedProperty _nocturnalNightModeLevel;
        private SerializedProperty _defaultDawnTime;
        private SerializedProperty _defaultDuskTime;
        private SerializedProperty _switchingDuration;
        private SerializedProperty _lights;
        private SerializedProperty _nighttimeLightIntensityMultiplier;

        private static readonly string _dangerZoneHeaderText = "Danger Zone";
        private static readonly string _dangerZoneInfoText = "この項目のいずれかが空であるか誤っている場合、正常に動作しない可能性があります。";
        private static readonly string _emptyDawnKnobErrorText = "Dawn Knobが空のため、正常に動作しません。";
        private static readonly string _emptyDuskKnobErrorText = "Dusk Knobが空のため、正常に動作しません。";
        private static readonly string _emptyNightModeRendererErrorText = "Night Mode Rendererが設定されていないため、Night Modeにできません。";
        private static readonly string _invalidNightModePropertyErrorText = "Night Mode Rendererに設定したShaderがNight Mode Property Nameに設定した名前のPropertyを持っていません。";

        private void InitializeProperties()
        {
            _dawnKnob = serializedObject.FindProperty("_dawnTimePointer");
            _duskKnob = serializedObject.FindProperty("_duskTimePointer");
            _nightModeRenderer = serializedObject.FindProperty("_nightModeRenderer");
            _nightModePropertyName = serializedObject.FindProperty("_nightModePropertyName");
            _nocturnalNightModeLevel = serializedObject.FindProperty("_nighttimeNightModeLevel");
            _defaultDawnTime = serializedObject.FindProperty("_defaultDawnTime");
            _defaultDuskTime = serializedObject.FindProperty("_defaultDuskTime");
            _switchingDuration = serializedObject.FindProperty("_switchingDuration");
            _lights = serializedObject.FindProperty("_lights");
            _nighttimeLightIntensityMultiplier = serializedObject.FindProperty("_nighttimeLightIntensityMultiplier");
            serializedObject.Update();
            _defaultDawnTime.floatValue = (_dawnKnob.objectReferenceValue as TimePointerBase)?.DefaultTime ?? _defaultDawnTime.floatValue;
            _defaultDuskTime.floatValue = (_duskKnob.objectReferenceValue as TimePointerBase)?.DefaultTime ?? _defaultDuskTime.floatValue;
            serializedObject.ApplyModifiedProperties();
        }

        private string[] GetShaderProperties()
        {
            if (_nightModeRenderer?.objectReferenceValue == null)
            {
                return new string[0];
            }
            Shader nightShader = ((Renderer)_nightModeRenderer.objectReferenceValue).sharedMaterial.shader;
            int shaderPropertyCount = nightShader.GetPropertyCount();
            string[] shaderProperties = new string[shaderPropertyCount];
            for (int i = 0; i < shaderPropertyCount; i++)
            {
                string propName = nightShader.GetPropertyName(i);
                shaderProperties[i] = propName;
                if (propName == _nightModePropertyName.stringValue)
                {
                    _shaderPropID = i;
                }
            }

            return shaderProperties;
        }

        private void DrawDangerZone()
        {
            EditorGUILayout.HelpBox(_dangerZoneInfoText, MessageType.Info);

            Shader nightShader = ((Renderer)_nightModeRenderer?.objectReferenceValue)?.sharedMaterial.shader;

            string[] shaderProperties = GetShaderProperties();


            EditorGUILayout.PropertyField(_dawnKnob);
            EditorGUILayout.PropertyField(_duskKnob);
            EditorGUILayout.PropertyField(_nightModeRenderer);
            _shaderPropID = EditorGUILayout.Popup(_nightModePropertyName.displayName, _shaderPropID, shaderProperties);
            _nightModePropertyName.stringValue = nightShader?.GetPropertyName(_shaderPropID) ?? string.Empty;
        }

        private void SetDawnPointerField(TimePointerBase timePointer)
        {
            var so = new SerializedObject(timePointer);
            so.Update();
            so.FindProperty("_defaultTime").floatValue = _defaultDawnTime.floatValue;
            so.ApplyModifiedProperties();
        }

        private void SetDuskPointerField(TimePointerBase timePointer)
        {
            var so = new SerializedObject(timePointer);
            so.Update();
            so.FindProperty("_defaultTime").floatValue = _defaultDuskTime.floatValue;
            so.ApplyModifiedProperties();
        }

        private void DrawDangerHelpBox()
        {
            if (_dawnKnob.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(_emptyDawnKnobErrorText, MessageType.Error);
            }


            if (_duskKnob.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(_emptyDuskKnobErrorText, MessageType.Error);
            }

            if (_nightModeRenderer.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(_emptyNightModeRendererErrorText, MessageType.Error);
            }
            else if ((((Renderer)_nightModeRenderer.objectReferenceValue)?.sharedMaterial?.shader.FindPropertyIndex(_nightModePropertyName.stringValue) ?? -1) == -1)
            {
                EditorGUILayout.HelpBox(_invalidNightModePropertyErrorText, MessageType.Warning);
            }
        }

        private void OnEnable()
        {
            InitializeProperties();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            // var _autoDayNightSwitcher = (AutoDayNightSwitcher)target;
            serializedObject.Update();
            EditorGUILayout.LabelField("Default Times", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_defaultDawnTime);
            EditorGUILayout.PropertyField(_defaultDuskTime);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Switching", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_switchingDuration);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Night mode", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_nocturnalNightModeLevel);
            EditorGUILayout.PropertyField(_lights);
            EditorGUILayout.PropertyField(_nighttimeLightIntensityMultiplier);

            EditorGUILayout.Space(10);

            bool isOpened = EditorGUILayout.Foldout(_isOpenDangerZone, _dangerZoneHeaderText);
            _isOpenDangerZone = isOpened;
            if (_isOpenDangerZone)
            {
                DrawDangerZone();
            }

            DrawDangerHelpBox();

            if (_dawnKnob.objectReferenceValue != null)
            {
                SetDawnPointerField((TimePointerBase)_dawnKnob.objectReferenceValue);
            }

            if (_duskKnob.objectReferenceValue != null)
            {
                SetDuskPointerField((TimePointerBase)_duskKnob.objectReferenceValue);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
