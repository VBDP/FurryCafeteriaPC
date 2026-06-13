// ============================================================================
// iRoleSystem v5.8 - iObjects Module Editor  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Modules.Objects.iRoleObjectVisibility))]
    public class iRoleObjectVisibilityEditor : UnityEditor.Editor
    {
        private SerializedProperty playerRoleManager;
        private SerializedProperty visibleWithPermission;
        private SerializedProperty visibleWithoutPermission;
        private SerializedProperty allowedRoles;
        private SerializedProperty checkInterval;
        private SerializedProperty defaultVisibility;

        private bool _foldObjects     = true;
        private bool _foldMemberships = true;
        private bool _foldAdvanced    = false;

        private void OnEnable()
        {
            playerRoleManager        = serializedObject.FindProperty("playerRoleManager");
            visibleWithPermission    = serializedObject.FindProperty("visibleWithPermission");
            visibleWithoutPermission = serializedObject.FindProperty("visibleWithoutPermission");
            allowedRoles             = serializedObject.FindProperty("allowedRoles");
            checkInterval            = serializedObject.FindProperty("checkInterval");
            defaultVisibility        = serializedObject.FindProperty("defaultVisibility");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("⬡", "iObjects",
                "Visibilidad de objetos por membresía de rol",
                iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.Space(6);

            // ── Sistema ──────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  Referencias del Sistema", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(playerRoleManager,
                new GUIContent("Player Role Manager", "Gestor de roles. Obligatorio."));
            if (playerRoleManager.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("⚠  Asigna un iPlayerRoleManager para activar el módulo.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Objetos ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◈  Objetos Controlados", iRoleEditorStyle.C_ACCENT_GREEN);
            _foldObjects = EditorGUILayout.Foldout(_foldObjects,
                "  Configurar objetos visibles / ocultos", true, iRoleEditorStyle.SFoldout);

            if (_foldObjects)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                DrawObjectSubSection(visibleWithPermission,
                    "✔  Objetos visibles para las membresías permitidas",
                    "Se activarán cuando el jugador TENGA una membresía con permiso.",
                    iRoleEditorStyle.C_ACCENT_GREEN);
                EditorGUILayout.Space(8);
                iRoleEditorStyle.Divider();
                EditorGUILayout.Space(8);
                DrawObjectSubSection(visibleWithoutPermission,
                    "✘  Objetos que NO verán las membresías seleccionadas",
                    "Se activarán cuando el jugador NO tenga una membresía con permiso.\nIdeal para contenido exclusivo para otros rangos.",
                    iRoleEditorStyle.C_ACCENT_RED);
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                iRoleEditorStyle.DrawChip($"✔ {visibleWithPermission.arraySize} obj.", iRoleEditorStyle.C_ACCENT_GREEN);
                GUILayout.Space(4);
                iRoleEditorStyle.DrawChip($"✘ {visibleWithoutPermission.arraySize} obj.", iRoleEditorStyle.C_ACCENT_RED);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(4);

            // ── Membresías ────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("♦  Membresías con Permiso", iRoleEditorStyle.C_ACCENT_BLUE);
            _foldMemberships = EditorGUILayout.Foldout(_foldMemberships,
                "  Seleccionar membresías con acceso", true, iRoleEditorStyle.SFoldout);

            if (_foldMemberships)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.LabelField(
                    "Las membresías marcadas verán el grupo ✔. Las no marcadas verán el grupo ✘.",
                    iRoleEditorStyle.SDim);
                EditorGUILayout.Space(6);

                Core.iRoleDatabase roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();
                if (roleDb != null)
                    DrawMembershipToggles(allowedRoles, roleDb);
                else
                    iRoleEditorStyle.DrawWarning(
                        "No se encontró iRoleDatabase en la escena. Configura un iRoleSystemCore.",
                        iRoleEditorStyle.C_ACCENT_ORANGE);

                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Avanzado ──────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  Configuración Avanzada", iRoleEditorStyle.C_TEXT_DIM);
            _foldAdvanced = EditorGUILayout.Foldout(_foldAdvanced,
                "  Intervalo y visibilidad por defecto", true, iRoleEditorStyle.SFoldout);

            if (_foldAdvanced)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(checkInterval,
                    new GUIContent("Intervalo de chequeo (s)", "Frecuencia de comprobación. Recomendado: 0.5s"));
                EditorGUILayout.PropertyField(defaultVisibility,
                    new GUIContent("Visibilidad por Defecto", "Estado inicial antes de comprobar el rol."));
                EditorGUILayout.LabelField("Tip: Menor intervalo = más reactivo pero mayor CPU.", iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(8);

            // ── Force Update ──────────────────────────────────────────────
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = Application.isPlaying;
            if (iRoleEditorStyle.ColorButton("▶  Forzar Actualización de Visibilidad",
                Application.isPlaying ? iRoleEditorStyle.C_ACCENT_BLUE : iRoleEditorStyle.C_TEXT_DIM,
                GUILayout.Width(260), GUILayout.Height(26)))
                ((Modules.Objects.iRoleObjectVisibility)target).ForceUpdate();
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }

        private void DrawObjectSubSection(SerializedProperty prop, string title, string desc, Color accent)
        {
            Rect lr = GUILayoutUtility.GetRect(1, 2);
            EditorGUI.DrawRect(lr, accent);
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(title, iRoleEditorStyle.SSectionLabel);
            EditorGUILayout.LabelField(desc, iRoleEditorStyle.SDim);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(prop, new GUIContent("Objetos"), true);
        }

        private void DrawMembershipToggles(SerializedProperty prop, Core.iRoleDatabase db)
        {
            if (db.roleNames == null || db.roleNames.Length == 0)
            { iRoleEditorStyle.DrawWarning("No hay roles definidos en iRoleDatabase.", iRoleEditorStyle.C_ACCENT_ORANGE); return; }

            string[] names = db.roleNames;
            if (prop.arraySize != names.Length) prop.arraySize = names.Length;

            int active = 0;
            for (int i = 0; i < prop.arraySize; i++)
                if (prop.GetArrayElementAtIndex(i).boolValue) active++;

            EditorGUILayout.BeginHorizontal();
            iRoleEditorStyle.DrawChip($"{active} / {names.Length} con permiso", iRoleEditorStyle.C_ACCENT_BLUE);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(6);

            for (int i = 0; i < names.Length; i++)
            {
                SerializedProperty el  = prop.GetArrayElementAtIndex(i);
                bool has = el.boolValue;
                Color roleColor = (db.roleColors != null && i < db.roleColors.Length)
                    ? db.roleColors[i] : Color.gray;

                Rect row = GUILayoutUtility.GetRect(0, 26);
                EditorGUI.DrawRect(row, has
                    ? new Color(0.18f, 0.82f, 0.48f, 0.08f)
                    : new Color(0.95f, 0.28f, 0.28f, 0.05f));
                EditorGUI.DrawRect(new Rect(row.x, row.y + 2, 3, row.height - 4), roleColor);

                bool newVal = EditorGUI.Toggle(
                    new Rect(row.x + 10, row.y + (row.height - 14) * 0.5f, 14, 14), has);
                if (newVal != has) el.boolValue = newVal;

                Color prev = GUI.contentColor;
                GUI.contentColor = roleColor;
                EditorGUI.LabelField(new Rect(row.x + 30, row.y + 4, 30, 18), $"[{i}]", EditorStyles.miniLabel);
                GUI.contentColor = prev;

                EditorGUI.LabelField(new Rect(row.x + 62, row.y + 4, row.width - 110, 18),
                    names[i], has ? EditorStyles.boldLabel : EditorStyles.label);

                GUI.color = has ? iRoleEditorStyle.C_ACCENT_GREEN : iRoleEditorStyle.C_ACCENT_RED;
                EditorGUI.LabelField(new Rect(row.xMax - 50, row.y + 6, 46, 16),
                    has ? "✔ VE" : "✘ NO", EditorStyles.miniLabel);
                GUI.color = Color.white;
                EditorGUILayout.Space(0);
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            if (iRoleEditorStyle.ColorButton("✔  Todos", iRoleEditorStyle.C_ACCENT_GREEN, GUILayout.MinWidth(80)))
                for (int i = 0; i < prop.arraySize; i++) prop.GetArrayElementAtIndex(i).boolValue = true;
            if (iRoleEditorStyle.ColorButton("✘  Ninguno", iRoleEditorStyle.C_ACCENT_RED, GUILayout.MinWidth(80)))
                for (int i = 0; i < prop.arraySize; i++) prop.GetArrayElementAtIndex(i).boolValue = false;
            if (iRoleEditorStyle.ColorButton("⇄  Invertir", iRoleEditorStyle.C_ACCENT_BLUE, GUILayout.MinWidth(80)))
                for (int i = 0; i < prop.arraySize; i++)
                { var e = prop.GetArrayElementAtIndex(i); e.boolValue = !e.boolValue; }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
