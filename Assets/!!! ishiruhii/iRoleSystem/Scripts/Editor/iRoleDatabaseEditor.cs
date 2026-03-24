// ============================================================================
// iRoleSystem v5.8 - iRoleDatabase Editor  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Core.iRoleDatabase))]
    public class iRoleDatabaseEditor : UnityEditor.Editor
    {
        private SerializedProperty roleNames, roleColors, roleIcons;
        private bool _showColors = true;
        private bool _showIcons  = false;

        private void OnEnable()
        {
            roleNames  = serializedObject.FindProperty("roleNames");
            roleColors = serializedObject.FindProperty("roleColors");
            roleIcons  = serializedObject.FindProperty("roleIcons");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("◆", "iRoleDatabase", "Definición de roles, colores e iconos", iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.Space(6);

            // ── Lista de roles ────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader($"★  Roles Definidos  [{roleNames.arraySize}]", iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

            if (roleNames.arraySize == 0)
                iRoleEditorStyle.DrawWarning("No hay roles definidos. Añade al menos uno.", iRoleEditorStyle.C_ACCENT_ORANGE);

            for (int i = 0; i < roleNames.arraySize; i++)
            {
                Color roleColor = (roleColors != null && i < roleColors.arraySize)
                    ? roleColors.GetArrayElementAtIndex(i).colorValue : Color.gray;

                Rect row = GUILayoutUtility.GetRect(0, 26);
                EditorGUI.DrawRect(row, i % 2 == 0 ? iRoleEditorStyle.C_BG_ROW_A : iRoleEditorStyle.C_BG_ROW_B);
                EditorGUI.DrawRect(new Rect(row.x, row.y + 2, 3, row.height - 4), roleColor);

                // índice
                EditorGUI.LabelField(new Rect(row.x + 8, row.y + 4, 26, 18),
                    $"[{i}]", EditorStyles.miniLabel);

                // nombre
                SerializedProperty nameProp = roleNames.GetArrayElementAtIndex(i);
                nameProp.stringValue = EditorGUI.TextField(
                    new Rect(row.x + 36, row.y + 4, row.width - 110, 18),
                    nameProp.stringValue);

                // color
                if (i < roleColors.arraySize)
                {
                    SerializedProperty colorProp = roleColors.GetArrayElementAtIndex(i);
                    colorProp.colorValue = EditorGUI.ColorField(
                        new Rect(row.xMax - 70, row.y + 4, 42, 18),
                        GUIContent.none, colorProp.colorValue, false, false, false);
                }

                // eliminar
                Color prev = GUI.backgroundColor;
                GUI.backgroundColor = iRoleEditorStyle.C_ACCENT_RED * new Color(1,1,1,0.7f);
                if (GUI.Button(new Rect(row.xMax - 24, row.y + 4, 20, 18), "✕", EditorStyles.miniButton))
                {
                    roleNames.DeleteArrayElementAtIndex(i);
                    if (i < roleColors.arraySize) roleColors.DeleteArrayElementAtIndex(i);
                    if (i < roleIcons.arraySize)  roleIcons.DeleteArrayElementAtIndex(i);
                    GUI.backgroundColor = prev;
                    break;
                }
                GUI.backgroundColor = prev;
                EditorGUILayout.Space(0);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // Botón añadir
            if (iRoleEditorStyle.ColorButton($"+ Añadir Rol  [{iEditorLang.Get(EL.LABEL_ADD_ROLE)}]",
                iRoleEditorStyle.C_ACCENT_GREEN, GUILayout.Height(26)))
            {
                roleNames.InsertArrayElementAtIndex(roleNames.arraySize);
                roleNames.GetArrayElementAtIndex(roleNames.arraySize - 1).stringValue = iEditorLang.Get(EL.LABEL_NEW_ROLE);
                if (roleColors.arraySize < roleNames.arraySize)
                {
                    roleColors.InsertArrayElementAtIndex(roleColors.arraySize);
                    roleColors.GetArrayElementAtIndex(roleColors.arraySize - 1).colorValue = Color.white;
                }
            }

            EditorGUILayout.Space(6);

            // ── Colores ───────────────────────────────────────────────────
            _showColors = EditorGUILayout.Foldout(_showColors, "  Previsualización de colores", true, iRoleEditorStyle.SFoldout);
            if (_showColors)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                while (roleColors.arraySize < roleNames.arraySize)
                    roleColors.InsertArrayElementAtIndex(roleColors.arraySize);
                for (int i = 0; i < roleNames.arraySize; i++)
                {
                    Color rc = roleColors.GetArrayElementAtIndex(i).colorValue;
                    Rect row = GUILayoutUtility.GetRect(0, 24);
                    EditorGUI.DrawRect(new Rect(row.x, row.y + 2, 6, row.height - 4), rc);
                    EditorGUI.LabelField(new Rect(row.x + 14, row.y + 3, 140, 18),
                        roleNames.GetArrayElementAtIndex(i).stringValue, EditorStyles.boldLabel);
                    roleColors.GetArrayElementAtIndex(i).colorValue = EditorGUI.ColorField(
                        new Rect(row.x + 160, row.y + 3, row.width - 165, 18),
                        GUIContent.none, rc, false, false, false);
                    EditorGUILayout.Space(0);
                }
                EditorGUILayout.EndVertical();
            }

            // ── Iconos ────────────────────────────────────────────────────
            _showIcons = EditorGUILayout.Foldout(_showIcons, "  Iconos de Rol", true, iRoleEditorStyle.SFoldout);
            if (_showIcons)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                while (roleIcons.arraySize < roleNames.arraySize)
                    roleIcons.InsertArrayElementAtIndex(roleIcons.arraySize);
                for (int i = 0; i < roleNames.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(roleNames.GetArrayElementAtIndex(i).stringValue, GUILayout.Width(140));
                    EditorGUILayout.PropertyField(roleIcons.GetArrayElementAtIndex(i), GUIContent.none);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }
}
#endif
