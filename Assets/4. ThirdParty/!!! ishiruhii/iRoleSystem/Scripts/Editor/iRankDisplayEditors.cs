// ============================================================================
// iRoleSystem v5.8 - Rank Display Editors  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    // ========================================================================
    // iRankDisplayConfig Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.RankDisplay.iRankDisplayConfig))]
    public class iRankDisplayConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty displayPrefab, heightOffset, displayScale, allowedRoles;
        private SerializedProperty animationClip, loopAnimation, billboard, billboardYOnly;
        private bool _foldAnim = false;

        private void OnEnable()
        {
            displayPrefab  = serializedObject.FindProperty("displayPrefab");
            heightOffset   = serializedObject.FindProperty("heightOffset");
            displayScale   = serializedObject.FindProperty("displayScale");
            allowedRoles   = serializedObject.FindProperty("allowedRoles");
            animationClip  = serializedObject.FindProperty("animationClip");
            loopAnimation  = serializedObject.FindProperty("loopAnimation");
            billboard      = serializedObject.FindProperty("billboard");
            billboardYOnly = serializedObject.FindProperty("billboardYOnly");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("◆", "iRankDisplayConfig",
                "Configuración visual de insignia", iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.Space(6);

            // ── Prefab ────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◆  " + iEditorLang.Get(EL.FIELD_DISPLAY_PREFAB), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(displayPrefab, new GUIContent(iEditorLang.Get(EL.FIELD_DISPLAY_PREFAB)));
            if (displayPrefab.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("Asigna un Prefab de insignia.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Posición y escala ─────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_POS_SCALE), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(heightOffset, new GUIContent(iEditorLang.Get(EL.FIELD_HEIGHT_OFFSET)));
            EditorGUILayout.PropertyField(displayScale, new GUIContent(iEditorLang.Get(EL.FIELD_DISPLAY_SCALE)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Billboard ─────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◈  " + iEditorLang.Get(EL.SECTION_BILLBOARD), iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(billboard, new GUIContent(iEditorLang.Get(EL.FIELD_BILLBOARD)));
            if (billboard.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(billboardYOnly, new GUIContent(iEditorLang.Get(EL.FIELD_BILLBOARD_Y)));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Animación ─────────────────────────────────────────────────
            _foldAnim = EditorGUILayout.Foldout(_foldAnim,
                "  " + iEditorLang.Get(EL.SECTION_ANIMATION), true, iRoleEditorStyle.SFoldout);
            if (_foldAnim)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(animationClip, new GUIContent(iEditorLang.Get(EL.FIELD_ANIM_CLIP)));
                if (animationClip.objectReferenceValue != null)
                    EditorGUILayout.PropertyField(loopAnimation, new GUIContent(iEditorLang.Get(EL.FIELD_LOOP_ANIM)));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Roles ─────────────────────────────────────────────────────
            Core.iRoleDatabase roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();
            if (roleDb != null)
                iRoleEditorUtils.DrawRolePermissions(allowedRoles, roleDb, iEditorLang.Get(EL.SECTION_ROLES));
            else
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_NO_ROLE_DB_SCENE), iRoleEditorStyle.C_ACCENT_ORANGE);

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }

    // ========================================================================
    // iRankDisplayManager Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.RankDisplay.iRankDisplayManager))]
    public class iRankDisplayManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty roleSystem, playerRoleManager, displayConfigs;
        private SerializedProperty displayPrefabs, updateEveryXFrames, roleCheckInterval, maxPlayers;

        private void OnEnable()
        {
            roleSystem         = serializedObject.FindProperty("roleSystem");
            playerRoleManager  = serializedObject.FindProperty("playerRoleManager");
            displayConfigs     = serializedObject.FindProperty("displayConfigs");
            displayPrefabs     = serializedObject.FindProperty("displayPrefabs");
            updateEveryXFrames = serializedObject.FindProperty("updateEveryXFrames");
            roleCheckInterval  = serializedObject.FindProperty("roleCheckInterval");
            maxPlayers         = serializedObject.FindProperty("maxPlayers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("◆", "iRankDisplayManager",
                "Gestión de insignias de rango sincronizadas", iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.Space(6);

            // ── Referencias ───────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  " + iEditorLang.Get(EL.SECTION_SYSTEM_REFS), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(roleSystem);
            EditorGUILayout.PropertyField(playerRoleManager, new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_MANAGER)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Config displays ───────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◆  " + iEditorLang.Get(EL.SECTION_CONFIG), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_CANVAS_HINT), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(displayConfigs, new GUIContent(iEditorLang.Get(EL.FIELD_DISPLAY_CONFIGS)), true);
            EditorGUILayout.PropertyField(displayPrefabs, new GUIContent(iEditorLang.Get(EL.FIELD_DISPLAY_PREFABS)), true);
            if (displayConfigs.arraySize != displayPrefabs.arraySize)
            {
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_CONFIGS_PREFABS_SIZE), iRoleEditorStyle.C_ACCENT_RED);
                if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_SYNC_SIZES), iRoleEditorStyle.C_ACCENT_ORANGE))
                    displayPrefabs.arraySize = displayConfigs.arraySize;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Rendimiento ───────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_PERF), iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(updateEveryXFrames, new GUIContent(iEditorLang.Get(EL.FIELD_UPDATE_FRAMES)));
            EditorGUILayout.PropertyField(maxPlayers, new GUIContent(iEditorLang.Get(EL.FIELD_MAX_CAPACITY)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Chequeo de roles ──────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⏱  " + iEditorLang.Get(EL.SECTION_ROLE_CHECK), iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(roleCheckInterval, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_CHECK_INT)));
            if (roleCheckInterval.floatValue > 0)
                iRoleEditorStyle.DrawWarning(
                    iEditorLang.Get(EL.MSG_RUNTIME_CHECK_INFO, roleCheckInterval.floatValue.ToString("F1")),
                    iRoleEditorStyle.C_ACCENT_BLUE);
            else
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_CHECK_DISABLED), iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8);

            // ── Runtime ───────────────────────────────────────────────────
            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_REFRESH_ALL),
                    iRoleEditorStyle.C_ACCENT_CYAN, GUILayout.Height(28), GUILayout.Width(200)))
                    ((Modules.RankDisplay.iRankDisplayManager)target).RefreshAllPlayers();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }
}
#endif
