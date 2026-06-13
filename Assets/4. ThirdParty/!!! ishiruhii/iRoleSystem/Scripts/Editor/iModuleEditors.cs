// ============================================================================
// iRoleSystem v5.8 - Module Editors (Prison + PrivateZone)  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    // ========================================================================
    // iPrisonManager Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.Prison.iPrisonManager))]
    public class iPrisonManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty playerRoleManager, roleDatabase, prisonArea;
        private SerializedProperty teleportPoint, affectedRoles, positionCheckInterval;
        private SerializedProperty enableDynamicRoleCheck, roleCheckInterval;
        private SerializedProperty teleportSound, teleportParticles;
        private bool _foldEffects = false;

        private void OnEnable()
        {
            playerRoleManager      = serializedObject.FindProperty("playerRoleManager");
            roleDatabase           = serializedObject.FindProperty("roleDatabase");
            prisonArea             = serializedObject.FindProperty("prisonArea");
            teleportPoint          = serializedObject.FindProperty("teleportPoint");
            affectedRoles          = serializedObject.FindProperty("affectedRoles");
            positionCheckInterval  = serializedObject.FindProperty("positionCheckInterval");
            enableDynamicRoleCheck = serializedObject.FindProperty("enableDynamicRoleCheck");
            roleCheckInterval      = serializedObject.FindProperty("roleCheckInterval");
            teleportSound          = serializedObject.FindProperty("teleportSound");
            teleportParticles      = serializedObject.FindProperty("teleportParticles");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("⬡", "iPrisonManager",
                iEditorLang.Get(EL.SECTION_PRISON_CONFIG), iRoleEditorStyle.C_ACCENT_RED);
            EditorGUILayout.Space(6);

            // ── Sistema ──────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  " + iEditorLang.Get(EL.SECTION_SYSTEM_REFS), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(playerRoleManager, new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_MANAGER)));
            EditorGUILayout.PropertyField(roleDatabase, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_DATABASE)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Configuración prisión ─────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⬡  " + iEditorLang.Get(EL.SECTION_PRISON_CONFIG), iRoleEditorStyle.C_ACCENT_RED);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(prisonArea,
                new GUIContent("Área de Cárcel (iAreaZone)", "Zona poligonal 3D de confinamiento."));
            if (prisonArea.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("Asigna un GameObject con iAreaZone para definir el área de cárcel.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(teleportPoint, new GUIContent(iEditorLang.Get(EL.FIELD_TELEPORT_POINT)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Chequeo ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_CHECK_CONFIG), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(positionCheckInterval, new GUIContent(iEditorLang.Get(EL.FIELD_POS_INTERVAL)));
            EditorGUILayout.PropertyField(enableDynamicRoleCheck, new GUIContent(iEditorLang.Get(EL.FIELD_DYN_ROLE_CHECK)));
            if (enableDynamicRoleCheck.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(roleCheckInterval, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_INTERVAL)));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Efectos ───────────────────────────────────────────────────
            _foldEffects = EditorGUILayout.Foldout(_foldEffects,
                "  " + iEditorLang.Get(EL.SECTION_EFFECTS), true, iRoleEditorStyle.SFoldout);
            if (_foldEffects)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(teleportSound, new GUIContent(iEditorLang.Get(EL.FIELD_TELEPORT_SOUND)));
                EditorGUILayout.PropertyField(teleportParticles, new GUIContent(iEditorLang.Get(EL.FIELD_TELEPORT_FX)));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Roles ─────────────────────────────────────────────────────
            Modules.Prison.iPrisonManager mgr = (Modules.Prison.iPrisonManager)target;
            if (mgr.roleDatabase != null)
                iRoleEditorUtils.DrawRolePermissions(affectedRoles, mgr.roleDatabase, iEditorLang.Get(EL.SECTION_ROLES));
            else
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_ROLE_DB_FOR_ROLES), iRoleEditorStyle.C_ACCENT_ORANGE);

            // FIX LAG: Solo marcar dirty si hay cambios reales
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }

    // ========================================================================
    // iPrivateZoneManager Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.PrivateZone.iPrivateZoneManager))]
    public class iPrivateZoneManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty playerRoleManager, zoneArea, ejectionPoint;
        private SerializedProperty ejectionMessage, allowedRoles, checkInterval, ejectionSound;
        private bool _foldEffects = false;

        private void OnEnable()
        {
            playerRoleManager = serializedObject.FindProperty("playerRoleManager");
            zoneArea          = serializedObject.FindProperty("zoneArea");
            ejectionPoint     = serializedObject.FindProperty("ejectionPoint");
            ejectionMessage   = serializedObject.FindProperty("ejectionMessage");
            allowedRoles      = serializedObject.FindProperty("allowedRoles");
            checkInterval     = serializedObject.FindProperty("checkInterval");
            ejectionSound     = serializedObject.FindProperty("ejectionSound");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("◉", "iPrivateZoneManager",
                iEditorLang.Get(EL.SECTION_ZONE_CONFIG), iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.Space(6);

            // ── Sistema ──────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  " + iEditorLang.Get(EL.SECTION_REFERENCES), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(playerRoleManager, new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_MANAGER)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Zona ─────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_ZONE_CONFIG), iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(zoneArea,
                new GUIContent("Área Privada (iAreaZone)", "Zona poligonal 3D que define el área privada."));
            if (zoneArea.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("Asigna un GameObject con iAreaZone para definir el área privada.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.PropertyField(ejectionPoint, new GUIContent(iEditorLang.Get(EL.FIELD_EJECTION_POINT)));
            EditorGUILayout.PropertyField(ejectionMessage, new GUIContent(iEditorLang.Get(EL.FIELD_EJECTION_MSG)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Chequeo ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_CHECK_CONFIG), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(checkInterval,
                new GUIContent("Intervalo de chequeo (s)", "Frecuencia de comprobación. Default: 0.5s"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Efectos ───────────────────────────────────────────────────
            _foldEffects = EditorGUILayout.Foldout(_foldEffects,
                "  " + iEditorLang.Get(EL.SECTION_EFFECTS), true, iRoleEditorStyle.SFoldout);
            if (_foldEffects)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(ejectionSound, new GUIContent(iEditorLang.Get(EL.FIELD_EJECTION_SOUND)));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Roles ─────────────────────────────────────────────────────
            Core.iRoleDatabase roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();
            if (roleDb != null)
                iRoleEditorUtils.DrawRolePermissions(allowedRoles, roleDb, iEditorLang.Get(EL.SECTION_ROLES));
            else
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_NO_ROLE_DB_SCENE), iRoleEditorStyle.C_ACCENT_ORANGE);

            // FIX LAG: Solo marcar dirty si hay cambios reales
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }
}
#endif
