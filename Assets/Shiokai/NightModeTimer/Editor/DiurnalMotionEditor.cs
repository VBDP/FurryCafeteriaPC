using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharpEditor;
using Shiokai.NightModeTimer.Accessory;

namespace Shiokai.NightModeTimer.CustomInspector
{
    [CustomEditor(typeof(SunDiurnalMotion))]
    public class DiurnalMotionEditor : Editor
    {
        private static readonly string _notDirectionalWarningText = "Lightの種類がDirectionalではありません";
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target)) return;
            base.OnInspectorGUI();
            var light = (target as SunDiurnalMotion).GetComponent<Light>();
            if (light != null && (light.type != LightType.Directional))
            {
                EditorGUILayout.HelpBox(_notDirectionalWarningText, MessageType.Warning);
            }

        }

    }
}
