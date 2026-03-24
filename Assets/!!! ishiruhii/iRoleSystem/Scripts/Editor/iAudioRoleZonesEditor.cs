// ============================================================================
// iRoleSystem v5.8 - iAudioRoleZones Editor
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    using ishiruhii.iRoleSystem.Modules.AudioZones;

    [CustomEditor(typeof(iAudioRoleZones))]
    public class iAudioRoleZonesEditor : UnityEditor.Editor
    {
        // ── Propiedades cacheadas (OnEnable) ──────────────────────────────
        private SerializedProperty _playerRoleManager;
        private SerializedProperty _roleDatabase;
        private SerializedProperty _zoneArea;
        private SerializedProperty _mode;
        private SerializedProperty _zoneAudioRoles;
        private SerializedProperty _exemptRoles;
        private SerializedProperty _enableWorldAudio;
        private SerializedProperty _worldAudioSources;
        private SerializedProperty _checkInterval;
        private SerializedProperty _outsideVoiceGain;
        private SerializedProperty _insideVoiceGain;

        private bool _foldWorld    = false;
        private bool _foldAdvanced = false;

        private static readonly Color AccentAudio  = new Color(0.10f, 0.85f, 0.70f, 1f);
        private static readonly Color AccentExempt = new Color(1.00f, 0.82f, 0.10f, 1f);
        private static readonly Color ColUnica     = new Color(0.22f, 0.55f, 1.00f, 1f);
        private static readonly Color ColTotal     = new Color(0.95f, 0.28f, 0.28f, 1f);

        private void OnEnable()
        {
            _playerRoleManager = serializedObject.FindProperty("playerRoleManager");
            _roleDatabase      = serializedObject.FindProperty("roleDatabase");
            _zoneArea          = serializedObject.FindProperty("zoneArea");
            _mode              = serializedObject.FindProperty("mode");
            _zoneAudioRoles    = serializedObject.FindProperty("zoneAudioRoles");
            _exemptRoles       = serializedObject.FindProperty("exemptRoles");
            _enableWorldAudio  = serializedObject.FindProperty("enableWorldAudio");
            _worldAudioSources = serializedObject.FindProperty("worldAudioSources");
            _checkInterval     = serializedObject.FindProperty("checkInterval");
            _outsideVoiceGain  = serializedObject.FindProperty("outsideVoiceGain");
            _insideVoiceGain   = serializedObject.FindProperty("insideVoiceGain");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // ── Header ────────────────────────────────────────────────────
            iRoleEditorStyle.DrawHeader("♪", "iAudioRoleZones",
                "Separación de audio por zona + permisos de rol", AccentAudio, "v5.8");
            EditorGUILayout.Space(6);

            // ── Sistema ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  Referencias del Sistema", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(_playerRoleManager, new GUIContent("Player Role Manager"));
            EditorGUILayout.PropertyField(_roleDatabase, new GUIContent("Role Database"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Zona ──────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⊡  Zona de Audio", AccentAudio);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(_zoneArea,
                new GUIContent("Área (iAreaZone)", "Zona poligonal 3D que define el espacio de audio."));
            if (_zoneArea.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("Asigna un GameObject con iAreaZone.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Modo ──────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  Modo de Separación", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

            AudioZoneMode currentMode = (AudioZoneMode)_mode.enumValueIndex;
            Color modeColor = currentMode == AudioZoneMode.SeparacionUnica ? ColUnica : ColTotal;
            EditorGUILayout.PropertyField(_mode, new GUIContent("Modo"));
            EditorGUILayout.Space(3);
            Rect infoRect = GUILayoutUtility.GetRect(0, 42);
            EditorGUI.DrawRect(infoRect, new Color(modeColor.r, modeColor.g, modeColor.b, 0.10f));
            EditorGUI.DrawRect(new Rect(infoRect.x, infoRect.y, 3, infoRect.height), modeColor);
            GUI.Label(new Rect(infoRect.x + 10, infoRect.y + 4, infoRect.width - 14, infoRect.height - 6),
                currentMode == AudioZoneMode.SeparacionUnica
                    ? "SEPARACIÓN ÚNICA: Dentro ↔ Dentro | Fuera ↔ Fuera\nSolo roles autorizados participan en el canal de la zona."
                    : "SEPARACIÓN TOTAL: MUTE GLOBAL dentro de la zona.\nRoles exentos siempre escuchan.",
                iRoleEditorStyle.SDim);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Ganancia de voz ───────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("♪  Niveles de Voz", AccentAudio);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(_outsideVoiceGain,
                new GUIContent("Gain Jugadores Fuera", "Voz de jugadores fuera cuando local está dentro. 0 = silencio."));
            EditorGUILayout.PropertyField(_insideVoiceGain,
                new GUIContent("Gain Jugadores Dentro", "Voz de jugadores dentro cuando local está fuera. 0 = silencio."));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Roles con acceso ──────────────────────────────────────────
            Core.iRoleDatabase roleDb = _roleDatabase.objectReferenceValue as Core.iRoleDatabase;
            if (roleDb == null)
                roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();

            if (roleDb != null && roleDb.roleNames != null)
            {
                // Sincronizar tamaño de arrays
                if (_zoneAudioRoles.arraySize != roleDb.roleNames.Length)
                    _zoneAudioRoles.arraySize = roleDb.roleNames.Length;
                if (_exemptRoles.arraySize != roleDb.roleNames.Length)
                    _exemptRoles.arraySize = roleDb.roleNames.Length;

                // Roles con canal de audio
                DrawRoleToggleList(
                    "♪  Roles en Canal de Zona",
                    "Solo estos roles participan en la separación de audio de la zona.\nLos demás siguen las reglas normales.",
                    _zoneAudioRoles, roleDb, AccentAudio);
                EditorGUILayout.Space(4);

                // Roles exentos
                DrawRoleToggleList(
                    "★  Roles Exentos (escuchan todo)",
                    "Estos roles nunca son filtrados por la zona. Siempre escuchan con voz normal.\nIdeal para Admins/Owners.",
                    _exemptRoles, roleDb, AccentExempt);
            }
            else
            {
                iRoleEditorStyle.DrawWarning("Asigna una Role Database para configurar los permisos de rol.", iRoleEditorStyle.C_ACCENT_ORANGE);
            }

            EditorGUILayout.Space(4);

            // ── Audio de Mundo ────────────────────────────────────────────
            _foldWorld = EditorGUILayout.Foldout(_foldWorld, "  ♬  Audio de Mundo", true, iRoleEditorStyle.SFoldout);
            if (_foldWorld)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(_enableWorldAudio,
                    new GUIContent("Activar Audio de Mundo"));
                if (_enableWorldAudio.boolValue)
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.PropertyField(_worldAudioSources, new GUIContent("AudioSources a Silenciar"), true);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Avanzado ──────────────────────────────────────────────────
            _foldAdvanced = EditorGUILayout.Foldout(_foldAdvanced, "  ⚙  Avanzado", true, iRoleEditorStyle.SFoldout);
            if (_foldAdvanced)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(_checkInterval, new GUIContent("Intervalo de Chequeo (s)"));
                EditorGUILayout.EndVertical();
            }

            // FIX LAG: Solo repintar si hay cambios reales
            if (serializedObject.ApplyModifiedProperties())
                Repaint();
        }

        // ── Helper: lista de toggles de roles con diseño del sistema ─────

        private void DrawRoleToggleList(
            string title, string tooltip,
            SerializedProperty rolesProp,
            Core.iRoleDatabase roleDb,
            Color accent)
        {
            iRoleEditorStyle.DrawSectionHeader(title, accent);

            // Contador activos
            int active = 0;
            for (int i = 0; i < rolesProp.arraySize; i++)
                if (rolesProp.GetArrayElementAtIndex(i).boolValue) active++;

            EditorGUILayout.BeginHorizontal();
            iRoleEditorStyle.DrawChip($"{active} / {roleDb.roleNames.Length} activados", accent);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            for (int i = 0; i < roleDb.roleNames.Length; i++)
            {
                SerializedProperty elem = rolesProp.GetArrayElementAtIndex(i);
                bool has = elem.boolValue;
                Color roleColor = (roleDb.roleColors != null && i < roleDb.roleColors.Length)
                    ? roleDb.roleColors[i] : Color.gray;

                Rect row = GUILayoutUtility.GetRect(0, 24);
                EditorGUI.DrawRect(row, has
                    ? new Color(accent.r, accent.g, accent.b, 0.08f)
                    : new Color(0.14f, 0.14f, 0.18f, 1f));
                EditorGUI.DrawRect(new Rect(row.x, row.y + 2, 3, row.height - 4), roleColor);

                bool newVal = EditorGUI.Toggle(
                    new Rect(row.x + 10, row.y + (row.height - 14) * 0.5f, 14, 14), has);
                if (newVal != has) elem.boolValue = newVal;

                Color prev = GUI.contentColor;
                GUI.contentColor = roleColor;
                EditorGUI.LabelField(new Rect(row.x + 30, row.y + 3, 30, 18), $"[{i}]", EditorStyles.miniLabel);
                GUI.contentColor = prev;

                EditorGUI.LabelField(
                    new Rect(row.x + 62, row.y + 3, row.width - 100, 18),
                    roleDb.roleNames[i],
                    has ? EditorStyles.boldLabel : EditorStyles.label);
                EditorGUILayout.Space(0);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (iRoleEditorStyle.ColorButton("Seleccionar todos", accent, EditorStyles.miniButtonLeft))
                for (int i = 0; i < rolesProp.arraySize; i++)
                    rolesProp.GetArrayElementAtIndex(i).boolValue = true;
            if (iRoleEditorStyle.ColorButton("Deseleccionar todos", iRoleEditorStyle.C_ACCENT_RED, EditorStyles.miniButtonRight))
                for (int i = 0; i < rolesProp.arraySize; i++)
                    rolesProp.GetArrayElementAtIndex(i).boolValue = false;
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
