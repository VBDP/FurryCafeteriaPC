// ============================================================================
// iRoleSystem v5.8 - iPlayerRoleManager Editor  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Core.iPlayerRoleManager))]
    public class iPlayerRoleManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty roleDatabase, defaultRoleIndex, defaultRoleDelay;
        private SerializedProperty skipIfAlreadyHasRole, maxPlayers, onRoleChangedListeners;
        private SerializedProperty enableMasterRole, masterRoleIndex, masterRoleDelay, masterSkipIfAlreadyHasRole;

        private void OnEnable()
        {
            roleDatabase              = serializedObject.FindProperty("roleDatabase");
            defaultRoleIndex          = serializedObject.FindProperty("defaultRoleIndex");
            defaultRoleDelay          = serializedObject.FindProperty("defaultRoleDelay");
            skipIfAlreadyHasRole      = serializedObject.FindProperty("skipIfAlreadyHasRole");
            maxPlayers                = serializedObject.FindProperty("maxPlayers");
            onRoleChangedListeners    = serializedObject.FindProperty("onRoleChangedListeners");
            enableMasterRole          = serializedObject.FindProperty("enableMasterRole");
            masterRoleIndex           = serializedObject.FindProperty("masterRoleIndex");
            masterRoleDelay           = serializedObject.FindProperty("masterRoleDelay");
            masterSkipIfAlreadyHasRole = serializedObject.FindProperty("masterSkipIfAlreadyHasRole");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("⚙", "iPlayerRoleManager",
                "Gestiona la asignación y sincronización de roles", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.Space(6);

            // ── Referencias ───────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◈  Referencias", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(roleDatabase, new GUIContent("Role Database"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Configuración ─────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  Configuración", iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

            Core.iPlayerRoleManager manager = (Core.iPlayerRoleManager)target;

            // Rol por defecto
            if (manager.roleDatabase != null && manager.roleDatabase.roleNames != null)
            {
                string[] opts = new string[manager.roleDatabase.roleNames.Length + 1];
                opts[0] = "Sin Rol (-1)";
                for (int i = 0; i < manager.roleDatabase.roleNames.Length; i++)
                    opts[i + 1] = $"[{i}] {manager.roleDatabase.roleNames[i]}";
                int sel = defaultRoleIndex.intValue + 1;
                sel = EditorGUILayout.Popup("Rol por Defecto", sel, opts);
                defaultRoleIndex.intValue = sel - 1;
            }
            else
                EditorGUILayout.PropertyField(defaultRoleIndex, new GUIContent("Rol por Defecto"));

            bool hasDefault = defaultRoleIndex.intValue >= 0;

            if (hasDefault)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(4);

                // Delay
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.PropertyField(defaultRoleDelay,
                    new GUIContent("Delay de asignación (s)", "0 = inmediato."));
                if (defaultRoleDelay.floatValue < 0f) defaultRoleDelay.floatValue = 0f;

                string delayInfo = defaultRoleDelay.floatValue <= 0f
                    ? "⚡ Asignación inmediata al entrar"
                    : $"⏱ Se asignará {defaultRoleDelay.floatValue:F1}s después de entrar";
                GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                EditorGUILayout.LabelField(delayInfo, EditorStyles.miniLabel);
                GUI.color = Color.white;

                // Presets de delay
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Presets:", iRoleEditorStyle.SDim, GUILayout.Width(50));
                float[] presets = { 0f, 3f, 5f, 10f, 30f };
                foreach (float p in presets)
                    if (iRoleEditorStyle.ColorButton($"{p}s", iRoleEditorStyle.C_ACCENT_BLUE, GUILayout.Height(18)))
                        defaultRoleDelay.floatValue = p;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(4);

                // Skip si ya tiene rol
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.PropertyField(skipIfAlreadyHasRole,
                    new GUIContent("No sobrescribir si ya tiene rol"));
                if (skipIfAlreadyHasRole.boolValue)
                {
                    GUI.color = iRoleEditorStyle.C_ACCENT_GREEN;
                    EditorGUILayout.LabelField("✔ Solo asigna a jugadores sin rol (recomendado)", EditorStyles.miniLabel);
                    GUI.color = Color.white;
                }
                else
                    iRoleEditorStyle.DrawWarning("⚠ El rol por defecto sobreescribirá cualquier rol existente.", iRoleEditorStyle.C_ACCENT_ORANGE);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel--;
            }
            else
            {
                GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                EditorGUILayout.LabelField("Sin rol por defecto. Los jugadores entrarán sin rol.", EditorStyles.miniLabel);
                GUI.color = Color.white;
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.PropertyField(maxPlayers, new GUIContent("Capacidad Máxima"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Rol del Master ─────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("♛  Rol del Master de Instancia", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(enableMasterRole, new GUIContent("Asignar rol al Master"));

            if (enableMasterRole.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.Space(4);

                // Dropdown de rol para el master
                if (manager.roleDatabase != null && manager.roleDatabase.roleNames != null)
                {
                    string[] opts = new string[manager.roleDatabase.roleNames.Length + 1];
                    opts[0] = "Sin Rol (-1)";
                    for (int i = 0; i < manager.roleDatabase.roleNames.Length; i++)
                        opts[i + 1] = $"[{i}] {manager.roleDatabase.roleNames[i]}";
                    int sel = masterRoleIndex.intValue + 1;
                    sel = EditorGUILayout.Popup("Rol del Master", sel, opts);
                    masterRoleIndex.intValue = sel - 1;
                }
                else
                    EditorGUILayout.PropertyField(masterRoleIndex, new GUIContent("Rol del Master"));

                if (masterRoleIndex.intValue >= 0)
                {
                    EditorGUILayout.Space(4);

                    // Delay para el master
                    EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                    EditorGUILayout.PropertyField(masterRoleDelay,
                        new GUIContent("Delay de asignación (s)", "0 = inmediato."));
                    if (masterRoleDelay.floatValue < 0f) masterRoleDelay.floatValue = 0f;

                    string masterDelayInfo = masterRoleDelay.floatValue <= 0f
                        ? "⚡ Asignación inmediata al ser Master"
                        : $"⏱ Se asignará {masterRoleDelay.floatValue:F1}s después";
                    GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                    EditorGUILayout.LabelField(masterDelayInfo, EditorStyles.miniLabel);
                    GUI.color = Color.white;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Presets:", iRoleEditorStyle.SDim, GUILayout.Width(50));
                    float[] mPresets = { 0f, 3f, 5f, 10f, 30f };
                    foreach (float p in mPresets)
                        if (iRoleEditorStyle.ColorButton($"{p}s", iRoleEditorStyle.C_ACCENT_ORANGE, GUILayout.Height(18)))
                            masterRoleDelay.floatValue = p;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(4);

                    // Skip si ya tiene rol (master)
                    EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                    EditorGUILayout.PropertyField(masterSkipIfAlreadyHasRole,
                        new GUIContent("No sobrescribir si ya tiene rol"));
                    if (masterSkipIfAlreadyHasRole.boolValue)
                    {
                        GUI.color = iRoleEditorStyle.C_ACCENT_GREEN;
                        EditorGUILayout.LabelField("✔ Solo asigna si el Master no tiene rol previo", EditorStyles.miniLabel);
                        GUI.color = Color.white;
                    }
                    else
                        iRoleEditorStyle.DrawWarning("⚠ El rol del Master sobreescribirá cualquier rol existente.", iRoleEditorStyle.C_ACCENT_ORANGE);
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                    EditorGUILayout.LabelField("Sin rol asignado al Master.", EditorStyles.miniLabel);
                    GUI.color = Color.white;
                }

                EditorGUI.indentLevel--;
            }
            else
            {
                GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                EditorGUILayout.LabelField("El Master entrará con el rol por defecto (si está configurado).", EditorStyles.miniLabel);
                GUI.color = Color.white;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Notificaciones ────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◈  Notificaciones de Cambio de Rol", iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            iRoleEditorStyle.DrawWarning("Los behaviours listados recibirán el evento '_OnPlayerRoleChanged'", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(onRoleChangedListeners, true);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Runtime ───────────────────────────────────────────────────
            if (Application.isPlaying)
            {
                iRoleEditorStyle.DrawSectionHeader("▶  Info Runtime", iRoleEditorStyle.C_ACCENT_CYAN);
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.LabelField("(Datos visibles en Play Mode)", iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }
}
#endif
