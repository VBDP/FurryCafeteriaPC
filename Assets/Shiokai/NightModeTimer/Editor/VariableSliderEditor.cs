using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UdonSharp;
using UdonSharpEditor;
using VRC.Udon;
using Shiokai.NightModeTimer.Core;
using Shiokai.NightModeTimer.Accessory;

namespace Shiokai.NightModeTimer.CustomInspector
{
    [CustomEditor(typeof(VariableSlider))]
    public class VariableSliderEditor : Editor
    {
        private Slider _slider;
        private SerializedProperty _nightModeSwitcher;
        private SerializedProperty _display;
        private SerializedProperty _variableType;
        private SerializedProperty _maxValue;
        private SerializedProperty _minValue;
        private SerializedProperty _defaultValue;

        private static readonly string _emptySliderErrorText = "Sliderが設定されていません。";
        private static readonly string _sliderEventErrorText = "SliderにVariableSliderのイベントが登録されていません";
        private static readonly string _failedAutoDetectSwitcherText = "親オブジェクトにAutoDayNightSwitcherが存在しなかったため、自動設定できません。";

        private void SetSliderEvent()
        {
            UnityEditor.Events.UnityEventTools
            .AddStringPersistentListener(
                                            _slider.onValueChanged,
                                            (target as UdonSharpBehaviour).GetComponent<UdonBehaviour>().SendCustomEvent,
                                            "OnSliderValueChanged"
                                        );
            EditorUtility.SetDirty(_slider);
        }

        private void OnEnable()
        {
            _nightModeSwitcher = serializedObject.FindProperty("_nightModeSwitcher");
            _display = serializedObject.FindProperty("_display");
            _variableType = serializedObject.FindProperty("_variableType");
            _maxValue = serializedObject.FindProperty("_maxValue");
            _minValue = serializedObject.FindProperty("_minValue");
            _defaultValue = serializedObject.FindProperty("_defaultValue");
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;

            VariableSlider variableSlider = (VariableSlider)target;
            serializedObject.Update();
            _slider = variableSlider.GetComponent<Slider>();

            if (_slider == null)
            {
                EditorGUILayout.HelpBox(_emptySliderErrorText, MessageType.Error);
            }
            else
            {
                var onValueChangedEvent = _slider.onValueChanged;
                var eventCount = onValueChangedEvent.GetPersistentEventCount();
                var hasEvent = false;
                for (int i = 0; i < eventCount; i++)
                {
                    var hasEventUdon = (onValueChangedEvent.GetPersistentTarget(i) as UdonBehaviour)?.GetComponent<VariableSlider>() != null;
                    var hasSendCustomEvent = onValueChangedEvent.GetPersistentMethodName(i) == "SendCustomEvent";
                    hasEvent = hasEventUdon && hasSendCustomEvent;
                    if (hasEvent)
                    {
                        break;
                    }
                }

                if (!hasEvent)
                {
                    EditorGUILayout.HelpBox(_sliderEventErrorText, MessageType.Error);
                    if (GUILayout.Button("自動登録"))
                    {
                        SetSliderEvent();
                    }
                }
            }

            EditorGUILayout.PropertyField(_variableType);
            EditorGUILayout.PropertyField(_nightModeSwitcher);
            if (_nightModeSwitcher.objectReferenceValue == null)
            {
                if (GUILayout.Button("Auto Detect"))
                {
                    var parent = variableSlider.GetComponentInParent<NightModeSwitcher>();
                    if (parent == null)
                    {
                        EditorUtility.DisplayDialog("Variable Slider", _failedAutoDetectSwitcherText, "OK");
                    }
                    else
                    {
                        _nightModeSwitcher.objectReferenceValue = parent;
                    }
                }
            }
            EditorGUILayout.PropertyField(_display);
            EditorGUILayout.PropertyField(_maxValue);
            EditorGUILayout.PropertyField(_minValue);
            EditorGUILayout.PropertyField(_defaultValue);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
