using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UdonSharpEditor;
using Shiokai.NightModeTimer.Core;
using Shiokai.NightModeTimer.Orbit;

namespace Shiokai.NightModeTimer.CustomInspector
{
    [CustomEditor(typeof(NightImageFiller))]
    public class NocturnalImageFillerEditor : Editor
    {
        private SerializedProperty _dawnTimePointer;
        private SerializedProperty _duskTimePointer;

        private static readonly string _emptyTimePointerErrorText = "TimePointerが設定されていません。";
        private static readonly string _invalidImageWarningText = "Imageの設定が想定されるものと異なります。";
        private static readonly string _invalidKnobTypeWarningText = "がOrbitKnobでないため、正常に動作しない可能性があります。";
        private static readonly string _autoFixButtonText = "Auto fix";

        private bool CheckImageSetting(Image image)
        {
            Image.Type type = image.type;
            Image.FillMethod fillMethod = image.fillMethod;
            int fillOrigin = image.fillOrigin;
            bool clockwise = image.fillClockwise;
            if (type == Image.Type.Filled && fillMethod == Image.FillMethod.Radial360 && fillOrigin == 0 && clockwise == true)
            {
                return true;
            }

            return false;
        }

        private void CheckKnobType(TimePointerBase timePointer, string displayName)
        {
            if (timePointer as OrbitKnob == null)
            {
                EditorGUILayout.HelpBox(displayName + _invalidKnobTypeWarningText, MessageType.Warning);
            }
        }

        private void SetupImage(Image image)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Radial360;
            image.fillOrigin = 0;
            image.fillClockwise = true;
        }

        private void SetDefaultTimeFromTimePointer()
        {
            var dawn = _dawnTimePointer.objectReferenceValue;
            var dusk = _duskTimePointer.objectReferenceValue;
            if (dawn == null || dusk == null)
            {
                EditorGUILayout.HelpBox(_emptyTimePointerErrorText, MessageType.Error);
                return;
            }
        }

        private void OnEnable()
        {
            _dawnTimePointer = serializedObject.FindProperty("_dawnTimePointer");
            _duskTimePointer = serializedObject.FindProperty("_duskTimePointer");
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            DrawDefaultInspector();
            serializedObject.Update();

            CheckKnobType(_dawnTimePointer.objectReferenceValue as TimePointerBase, _dawnTimePointer.displayName);
            CheckKnobType(_duskTimePointer.objectReferenceValue as TimePointerBase, _duskTimePointer.displayName);

            SetDefaultTimeFromTimePointer();
            serializedObject.ApplyModifiedProperties();

            Image image = ((NightImageFiller)target).GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            if (CheckImageSetting(image) == true)
            {
                return;
            }

            EditorGUILayout.HelpBox(_invalidImageWarningText, MessageType.Warning);
            if (GUILayout.Button(_autoFixButtonText))
            {
                SetupImage(image);
            }
        }
    }
}
