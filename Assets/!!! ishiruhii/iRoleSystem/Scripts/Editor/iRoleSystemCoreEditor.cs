// ============================================================================
// iRoleSystem v5.8 - Core Editor  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Core.iRoleSystemCore))]
    public class iRoleSystemCoreEditor : UnityEditor.Editor
    {
        private SerializedProperty roleDatabase, playerRoleManager, languageSystem;
        private SerializedProperty systemLanguage, enableDebugLogs, globalUpdateRate;
        private bool _showFeatures = false;
        private bool _showDebug    = false;

        private static readonly string[] LANG_NAMES  = { "🇪🇸  Español", "🇬🇧  English", "🇯🇵  日本語", "🇨🇳  中文" };
        private static readonly Color[]  LANG_COLORS = {
            new Color(0.90f,0.35f,0.15f,1f), new Color(0.10f,0.40f,0.90f,1f),
            new Color(0.85f,0.15f,0.25f,1f), new Color(0.85f,0.65f,0.10f,1f)
        };

        private static readonly string[][] FEAT_LIST = {
            new[]{ "Creación y gestión de Rangos","Administración de Usuarios en tiempo real",
                   "Objetos con visibilidad por Rol","Áreas Exclusivas/Privadas por Rol",
                   "Sistema de Cárcel para Roles específicos","Zonas de asignación de Rangos",
                   "Insignias sincronizadas sobre la cabeza","Soporte multilenguaje (ES/EN/JP/ZH)" },
            new[]{ "Rank Creation & Management","Real-time User Administration",
                   "Role-based Object Visibility","Exclusive/Private Areas by Role",
                   "Prison System for specific Roles","Rank Assignment Zones",
                   "Synchronized badges above head","Multilanguage support (ES/EN/JP/ZH)" },
            new[]{ "ランクの作成と管理","リアルタイムユーザー管理","ロール別オブジェクト表示",
                   "ロール別専用エリア","プリズンシステム","ランク割り当てゾーン",
                   "頭上に同期バッジ","多言語サポート" },
            new[]{ "等级创建与管理","实时用户管理","基于角色的对象可见性",
                   "基于角色的专属区域","监狱系统","等级分配区域","头顶同步徽章","多语言支持" }
        };
        private static readonly string[] DESC = {
            "Sistema de Roles de Usuario para Mundos de VRChat",
            "User Role System for VRChat Worlds",
            "VRChatワールド用ユーザーロールシステム",
            "VRChat 世界的用户角色系统"
        };

        private void OnEnable()
        {
            roleDatabase      = serializedObject.FindProperty("roleDatabase");
            playerRoleManager = serializedObject.FindProperty("playerRoleManager");
            languageSystem    = serializedObject.FindProperty("languageSystem");
            systemLanguage    = serializedObject.FindProperty("systemLanguage");
            enableDebugLogs   = serializedObject.FindProperty("enableDebugLogs");
            globalUpdateRate  = serializedObject.FindProperty("globalUpdateRate");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            int lang = systemLanguage.enumValueIndex;
            Color ac = LANG_COLORS[lang];

            // ── Header ────────────────────────────────────────────────────
            iRoleEditorStyle.DrawHeader("⬡", "iRoleSystem", DESC[lang], ac);
            EditorGUILayout.Space(6);

            // ── Idioma ────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("🌐  Idioma / Language / 言語 / 语言", ac);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < LANG_NAMES.Length; i++)
            {
                bool active = lang == i;
                Color prev = GUI.backgroundColor;
                GUI.backgroundColor = active ? LANG_COLORS[i] : new Color(0.28f, 0.28f, 0.32f, 1f);
                GUIStyle bs = new GUIStyle(GUI.skin.button) { fontSize = 11, fontStyle = active ? FontStyle.Bold : FontStyle.Normal };
                if (active) bs.normal.textColor = Color.white;
                if (GUILayout.Button(LANG_NAMES[i], bs, GUILayout.Height(28)))
                {
                    systemLanguage.enumValueIndex = i;
                    if (Application.isPlaying) ((Core.iRoleSystemCore)target).SetLanguageByIndex(i);
                }
                GUI.backgroundColor = prev;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(languageSystem, new GUIContent("iLanguageSystem"));
            if (languageSystem.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("Asigna un iLanguageSystem.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Componentes Core ──────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  Componentes Core", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(roleDatabase, new GUIContent("Role Database"));
            if (roleDatabase.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("⚠  Asigna un iRoleDatabase.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(playerRoleManager, new GUIContent("Player Role Manager"));
            if (playerRoleManager.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning("⚠  Asigna un iPlayerRoleManager.", iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Config global ─────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  Configuración Global", iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(enableDebugLogs, new GUIContent("Logs de Debug"));
            EditorGUILayout.PropertyField(globalUpdateRate, new GUIContent("Tasa de Actualización"));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Acciones rápidas ──────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("▶  Acciones Rápidas", iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.BeginHorizontal();
            if (iRoleEditorStyle.ColorButton("⚙  Abrir Setup", iRoleEditorStyle.C_ACCENT_BLUE, GUILayout.Height(28)))
                iRoleSystemSetup.ShowWindow();
            if (iRoleEditorStyle.ColorButton("📖  Documentación", iRoleEditorStyle.C_TEXT_DIM, GUILayout.Height(28)))
                Application.OpenURL("https://github.com/ishiruhii/iRoleSystem");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            // ── Features (colapsable) ─────────────────────────────────────
            _showFeatures = EditorGUILayout.Foldout(_showFeatures, "  Características del sistema", true, iRoleEditorStyle.SFoldout);
            if (_showFeatures)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                foreach (string feat in FEAT_LIST[lang])
                {
                    EditorGUILayout.BeginHorizontal();
                    GUI.color = iRoleEditorStyle.C_ACCENT_BLUE;
                    EditorGUILayout.LabelField("•", GUILayout.Width(12));
                    GUI.color = Color.white;
                    EditorGUILayout.LabelField(feat, iRoleEditorStyle.SMini);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);

            // ── Debug ─────────────────────────────────────────────────────
            _showDebug = EditorGUILayout.Foldout(_showDebug, "  Debug (Solo Runtime)", true, iRoleEditorStyle.SFoldout);
            if (_showDebug)
            {
                if (Application.isPlaying)
                {
                    EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                    Core.iRoleSystemCore core = (Core.iRoleSystemCore)target;
                    if (core.playerRoleManager != null)
                        EditorGUILayout.TextArea(core.playerRoleManager.GetDebugInfo(), EditorStyles.helpBox);
                    EditorGUILayout.EndVertical();
                }
                else
                    iRoleEditorStyle.DrawWarning("La información de debug solo está disponible en Play Mode.", iRoleEditorStyle.C_TEXT_DIM);
            }

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }
}
#endif
