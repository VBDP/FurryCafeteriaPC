
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor;

namespace VRCCoffeeSet
{
    [CustomEditor(typeof(Machine))]
    public class MachineEditor : Editor
    {
        Machine script;
        TypeMachine _type;

        SkinsMachine skinMachine;

        bool toggleVariable = false;
        bool toggleBase;

        private void Awake()
        {
            script = target as Machine;
        }

        void Initialize()
        {
            _type = script.m_type;

            // Display
            if (_type != TypeMachine.Espresso) script.m_display = null;

            // Skin
            skinMachine = SkinManager.GetSkinMachine(script);

            serializedObject.Update();
        }

        public override void OnInspectorGUI()
        {
            if (_type != script.m_type) Initialize();

            SkinsMachine tempSkinMachine = skinMachine;
            EditorGUI.BeginChangeCheck();
            skinMachine = (SkinsMachine)EditorGUILayout.EnumPopup("Skin Select", skinMachine);
            if ( EditorGUI.EndChangeCheck() )
            {
                string tempStr = SkinManager.ChangeMachine(script, skinMachine);
                if (tempStr == null) return;
                else
                {
                    skinMachine = tempSkinMachine;
                    Debug.LogWarning(tempStr);
                }
            }

            GUILayout.Space(20);
            toggleVariable = EditorGUILayout.Foldout(toggleVariable, "Do not change any variables");
            if (!toggleVariable) return;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_type"), new GUIContent("Machine Type"));
            serializedObject.ApplyModifiedProperties();

            if (_type != script.m_type) Initialize();

            switch (_type)
            {
                case TypeMachine.Espresso:
                    GUILayout.Space(10);
                    EditorGUILayout.PropertyField( serializedObject.FindProperty("m_display") );
                    break;
            }

            GUILayout.Space(20);
            toggleBase = EditorGUILayout.Foldout(toggleBase, "Udon Script");
            if (toggleBase) base.OnInspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }
    }
}