// ============================================================================
// iRoleSystem v5.8 - iAudioZones Editor
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    using ishiruhii.iRoleSystem.Modules.AudioZones;

    [CustomEditor(typeof(iAudioZones))]
    public class iAudioZonesEditor : UnityEditor.Editor
    {
        // ── Propiedades cacheadas (OnEnable) ──────────────────────────────
        private SerializedProperty _zoneArea;
        private SerializedProperty _mode;
        private SerializedProperty _enableWorldAudio;
        private SerializedProperty _worldAudioSources;
        private SerializedProperty _checkInterval;
        private SerializedProperty _outsideVoiceGain;
        private SerializedProperty _insideVoiceGain;

        private bool _foldWorld = false;
        private bool _foldAdvanced = false;

        private static readonly Color AccentAudio = new Color(0.10f, 0.85f, 0.70f, 1f);

        // ── Constantes de color ───────────────────────────────────────────
        private static readonly Color ColUnica  = new Color(0.22f, 0.55f, 1.00f, 1f);
        private static readonly Color ColTotal  = new Color(0.95f, 0.28f, 0.28f, 1f);

        private void OnEnable()
        {
            _zoneArea          = serializedObject.FindProperty("zoneArea");
            _mode              = serializedObject.FindProperty("mode");
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
            iRoleEditorStyle.DrawHeader("♪", "iAudioZones",
                "Separación de audio por zona poligonal", AccentAudio, "v5.8");
            EditorGUILayout.Space(6);

            // ── Zona ──────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⊡  Zona de Audio", AccentAudio);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(_zoneArea,
                new GUIContent("Área (iAreaZone)", "Zona poligonal 3D que define el espacio de separación de audio."));
            if (_zoneArea.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("Asigna un GameObject con iAreaZone para definir la zona de audio.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Modo ──────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  Modo de Separación", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

            AudioZoneMode currentMode = (AudioZoneMode)_mode.enumValueIndex;
            Color modeColor = currentMode == AudioZoneMode.SeparacionUnica ? ColUnica : ColTotal;

            EditorGUILayout.PropertyField(_mode, new GUIContent("Modo", GetModeTooltip(currentMode)));
            EditorGUILayout.Space(3);

            // Info card del modo seleccionado
            Rect infoRect = GUILayoutUtility.GetRect(0, 42);
            EditorGUI.DrawRect(infoRect, new Color(modeColor.r, modeColor.g, modeColor.b, 0.10f));
            EditorGUI.DrawRect(new Rect(infoRect.x, infoRect.y, 3, infoRect.height), modeColor);
            GUI.Label(new Rect(infoRect.x + 10, infoRect.y + 4, infoRect.width - 14, infoRect.height - 6),
                GetModeDescription(currentMode), iRoleEditorStyle.SDim);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Ganancia de voz ───────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("♪  Niveles de Voz", AccentAudio);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

            EditorGUILayout.PropertyField(_outsideVoiceGain,
                new GUIContent("Gain Jugadores Fuera",
                    "Ganancia de voz de jugadores que están FUERA de la zona cuando el jugador local está DENTRO.\n0 = silencio total. 1 = volumen normal."));
            EditorGUILayout.PropertyField(_insideVoiceGain,
                new GUIContent("Gain Jugadores Dentro",
                    "Ganancia de voz de jugadores que están DENTRO de la zona cuando el jugador local está FUERA.\n0 = silencio total. 1 = volumen normal."));

            if (_outsideVoiceGain.floatValue > 0f || _insideVoiceGain.floatValue > 0f)
                iRoleEditorStyle.DrawWarning(
                    "Valores > 0 permiten escuchar parcialmente entre zonas. Usa 0 para separación total.",
                    iRoleEditorStyle.C_ACCENT_ORANGE);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Audio de Mundo ────────────────────────────────────────────
            _foldWorld = EditorGUILayout.Foldout(_foldWorld, "  ♬  Audio de Mundo", true, iRoleEditorStyle.SFoldout);
            if (_foldWorld)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(_enableWorldAudio,
                    new GUIContent("Activar Audio de Mundo",
                        "Si está activo, silencia los AudioSources de mundo seleccionados cuando el jugador entra en la zona."));

                if (_enableWorldAudio.boolValue)
                {
                    EditorGUILayout.Space(3);
                    EditorGUILayout.PropertyField(_worldAudioSources, new GUIContent("AudioSources a Silenciar"), true);
                    if (_worldAudioSources.arraySize == 0)
                        iRoleEditorStyle.DrawWarning("Añade al menos un AudioSource del mundo para silenciar.", iRoleEditorStyle.C_ACCENT_ORANGE);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Avanzado ──────────────────────────────────────────────────
            _foldAdvanced = EditorGUILayout.Foldout(_foldAdvanced, "  ⚙  Avanzado", true, iRoleEditorStyle.SFoldout);
            if (_foldAdvanced)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(_checkInterval,
                    new GUIContent("Intervalo de Chequeo (s)",
                        "Frecuencia de comprobación de posición. Valores bajos = más preciso pero más CPU."));
                EditorGUILayout.HelpBox(
                    "0.25s recomendado. Reduce a 0.1s si necesitas respuesta más rápida en zonas pequeñas.",
                    MessageType.Info);
                EditorGUILayout.EndVertical();
            }

            // FIX LAG: Solo repintar si hay cambios reales
            if (serializedObject.ApplyModifiedProperties())
                Repaint();
        }

        private string GetModeTooltip(AudioZoneMode m)
        {
            return m == AudioZoneMode.SeparacionUnica
                ? "Los jugadores dentro se oyen entre sí. Los de fuera no escuchan a los de dentro y viceversa."
                : "Zona MUTE GLOBAL: nadie dentro escucha a nadie, ni los de fuera escuchan a los de dentro.";
        }

        private string GetModeDescription(AudioZoneMode m)
        {
            return m == AudioZoneMode.SeparacionUnica
                ? "SEPARACIÓN ÚNICA: Dentro ↔ Dentro | Fuera ↔ Fuera\nLos grupos se oyen internamente pero están aislados entre sí."
                : "SEPARACIÓN TOTAL: Zona MUTE GLOBAL\nNadie dentro escucha a nadie. Los de fuera tampoco oyen a los de dentro.";
        }
    }
}
#endif
