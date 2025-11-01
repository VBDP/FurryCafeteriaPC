using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace VRCCoffeeSet
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Material Group", menuName = "VRCCoffee/Material Group", order = int.MaxValue)]
    public class MaterialGroup : ScriptableObject
    {
        [System.Serializable] public class MatInfo
        {
            public string name;
            public Material[] mat;
            public bool isArr;
            public bool isExpanded;
        }
        // MatArray List 변수
        [SerializeField] public List<MatInfo> m_matInfoList;

        public Material[] GetMaterial(string name)
        {
            foreach (var matInfo in m_matInfoList) if (matInfo.name == name) return matInfo.mat;
            return null;
        }
    }

    // custom editor
    [UnityEditor.CustomEditor(typeof(MaterialGroup))]
    public class MaterialGroupEditor : Editor
    {
        MaterialGroup instance;
        ReorderableList[] listMat;

        private void OnEnable()
        {
            instance = target as MaterialGroup;
            if (instance.m_matInfoList == null) instance.m_matInfoList = new List<MaterialGroup.MatInfo>();
            Initialize();
        }

        void Initialize()
        {
            listMat = new ReorderableList[instance.m_matInfoList.Count];
            for (int i = 0; i < instance.m_matInfoList.Count; i++)
            {
                var tempMatProperty = serializedObject.FindProperty("m_matInfoList").GetArrayElementAtIndex(i).FindPropertyRelative("mat");
                listMat[i] = new ReorderableList(serializedObject, tempMatProperty, true, false, true, true);
                listMat[i].drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    var element = tempMatProperty.GetArrayElementAtIndex(index);
                    rect.y += 2;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, new GUIContent($"Element {index}"));
                };
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Material"))
            {
                instance.m_matInfoList.Add(new MaterialGroup.MatInfo(){name = "New Material", mat = new Material[1]});
                serializedObject.Update();
                Initialize();
            }
            if (GUILayout.Button("Add Material Array"))
            {
                instance.m_matInfoList.Add(new MaterialGroup.MatInfo(){name = "New Material Array", mat = new Material[1], isArr = true});
                serializedObject.Update();
                Initialize();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(15);

            for (int i = 0; i < instance.m_matInfoList.Count; i++)
            {
                var temp = instance.m_matInfoList[i];
                if (!temp.isArr)
                {
                    EditorGUILayout.BeginHorizontal();
                    temp.name = EditorGUILayout.TextField(temp.name);
                    temp.mat[0] = (Material)EditorGUILayout.ObjectField(temp.mat[0], typeof(Material), true);
                    if (GUILayout.Button("✕", GUILayout.Width(25)))
                    {
                        instance.m_matInfoList.RemoveAt(i);
                        Initialize();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    temp.name = EditorGUILayout.TextField(temp.name, GUILayout.Width(EditorGUIUtility.currentViewWidth/2-27));
                    EditorGUILayout.Space(15, false);
                    temp.isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(temp.isExpanded, "");
                    if (GUILayout.Button("✕", GUILayout.Width(25)))
                    {
                        instance.m_matInfoList.RemoveAt(i);
                        Initialize();
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (temp.isExpanded)
                    {
                        listMat[i].DoLayoutList();
                        GUILayout.Space(5);
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    GUILayout.Space(5);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
