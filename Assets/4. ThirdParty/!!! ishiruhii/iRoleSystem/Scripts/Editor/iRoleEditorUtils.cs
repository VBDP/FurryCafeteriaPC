// ============================================================================
// iRoleSystem v5.8 - Editor Utils  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    public static class iRoleEditorUtils
    {
        /// <summary>Header de módulo usando el nuevo design system.</summary>
        public static void DrawModuleHeader(string moduleName, string description)
        {
            // Mapping de módulos a iconos/colores
            Color accent = iRoleEditorStyle.C_ACCENT_BLUE;
            string icon  = "◈";
            if (moduleName.Contains("Prison"))   { accent = iRoleEditorStyle.C_ACCENT_RED;    icon = "⬡"; }
            if (moduleName.Contains("Private"))  { accent = iRoleEditorStyle.C_ACCENT_PURPLE; icon = "◉"; }
            if (moduleName.Contains("Admin"))    { accent = iRoleEditorStyle.C_ACCENT_ORANGE; icon = "★"; }
            if (moduleName.Contains("Rank"))     { accent = iRoleEditorStyle.C_ACCENT_CYAN;   icon = "◆"; }
            if (moduleName.Contains("Name"))     { accent = iRoleEditorStyle.C_ACCENT_GREEN;  icon = "✦"; }
            if (moduleName.Contains("Menu"))     { accent = iRoleEditorStyle.C_ACCENT_BLUE;   icon = "☰"; }
            if (moduleName.Contains("Object"))   { accent = iRoleEditorStyle.C_ACCENT_GREEN;  icon = "⬡"; }
            if (moduleName.Contains("Trigger"))  { accent = iRoleEditorStyle.C_ACCENT_BLUE;   icon = "⌖"; }
            if (moduleName.Contains("Area") || moduleName.Contains("Zone"))
                                                 { accent = iRoleEditorStyle.C_ACCENT_CYAN;   icon = "⊡"; }

            iRoleEditorStyle.DrawHeader(icon, moduleName, description, accent);
        }

        /// <summary>Dibuja checkboxes de roles con diseño mejorado.</summary>
        public static void DrawRolePermissions(
            SerializedProperty allowedRoles,
            Core.iRoleDatabase roleDatabase,
            string label = null)
        {
            if (roleDatabase == null || roleDatabase.roleNames == null)
            {
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_ASSIGN_ROLE_DB), iRoleEditorStyle.C_ACCENT_ORANGE);
                return;
            }

            string[] roleNames = roleDatabase.roleNames;
            if (allowedRoles.arraySize != roleNames.Length)
                allowedRoles.arraySize = roleNames.Length;

            iRoleEditorStyle.DrawSectionHeader(
                label ?? iEditorLang.Get(EL.SECTION_ROLES),
                iRoleEditorStyle.C_ACCENT_BLUE);

            // Contador
            int active = 0;
            for (int i = 0; i < allowedRoles.arraySize; i++)
                if (allowedRoles.GetArrayElementAtIndex(i).boolValue) active++;

            EditorGUILayout.BeginHorizontal();
            iRoleEditorStyle.DrawChip($"{active} / {roleNames.Length} permitidos", iRoleEditorStyle.C_ACCENT_BLUE);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            for (int i = 0; i < roleNames.Length; i++)
            {
                SerializedProperty element = allowedRoles.GetArrayElementAtIndex(i);
                bool has = element.boolValue;
                Color roleColor = (roleDatabase.roleColors != null && i < roleDatabase.roleColors.Length)
                    ? roleDatabase.roleColors[i] : Color.gray;

                Rect row = GUILayoutUtility.GetRect(0, 24);
                EditorGUI.DrawRect(row, has
                    ? new Color(0.18f, 0.82f, 0.48f, 0.08f)
                    : new Color(0.14f, 0.14f, 0.18f, 1f));
                EditorGUI.DrawRect(new Rect(row.x, row.y + 2, 3, row.height - 4), roleColor);

                bool newVal = EditorGUI.Toggle(
                    new Rect(row.x + 10, row.y + (row.height - 14) * 0.5f, 14, 14), has);
                if (newVal != has) element.boolValue = newVal;

                Color prev = GUI.contentColor;
                GUI.contentColor = roleColor;
                EditorGUI.LabelField(new Rect(row.x + 30, row.y + 3, 30, 18), $"[{i}]", EditorStyles.miniLabel);
                GUI.contentColor = prev;

                EditorGUI.LabelField(new Rect(row.x + 62, row.y + 3, row.width - 100, 18),
                    roleNames[i], has ? EditorStyles.boldLabel : EditorStyles.label);
                EditorGUILayout.Space(0);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_SELECT_ALL),
                iRoleEditorStyle.C_ACCENT_GREEN, EditorStyles.miniButtonLeft))
                for (int i = 0; i < allowedRoles.arraySize; i++)
                    allowedRoles.GetArrayElementAtIndex(i).boolValue = true;
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_DESELECT_ALL),
                iRoleEditorStyle.C_ACCENT_RED, EditorStyles.miniButtonRight))
                for (int i = 0; i < allowedRoles.arraySize; i++)
                    allowedRoles.GetArrayElementAtIndex(i).boolValue = false;
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>Dropdown de selección de rol (sin cambios en lógica).</summary>
        public static int DrawRoleDropdown(int currentIndex, Core.iRoleDatabase roleDatabase, string label = null)
        {
            if (roleDatabase == null || roleDatabase.roleNames == null || roleDatabase.roleNames.Length == 0)
            {
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_ROLE_DB_FOR_ROLES), iRoleEditorStyle.C_ACCENT_ORANGE);
                return currentIndex;
            }
            string[] roleNames = roleDatabase.roleNames;
            if (currentIndex >= roleNames.Length) currentIndex = 0;
            return EditorGUILayout.Popup(label ?? iEditorLang.Get(EL.SECTION_ROLES), currentIndex, roleNames);
        }

        // ── Caché de búsqueda en escena ───────────────────────────────────
        // FIX LAG: FindObjectOfType es costoso si se llama cada frame desde OnInspectorGUI.
        // Cacheamos los resultados y solo volvemos a buscar si el objeto se destruye.

        private static Core.iRoleDatabase        _cachedRoleDatabase;
        private static Core.iPlayerRoleManager   _cachedPlayerRoleManager;
        private static Core.iRoleSystemCore      _cachedSystemCore;

        /// <summary>Invalida la caché (útil al cargar escenas nuevas o añadir objetos).</summary>
        public static void InvalidateCache()
        {
            _cachedRoleDatabase      = null;
            _cachedPlayerRoleManager = null;
            _cachedSystemCore        = null;
        }

        private static Core.iRoleSystemCore GetCachedCore()
        {
            if (_cachedSystemCore == null)
                _cachedSystemCore = Object.FindObjectOfType<Core.iRoleSystemCore>();
            return _cachedSystemCore;
        }

        /// <summary>Busca iRoleDatabase en la escena (con caché).</summary>
        public static Core.iRoleDatabase FindRoleDatabaseInScene()
        {
            if (_cachedRoleDatabase != null) return _cachedRoleDatabase;
            Core.iRoleSystemCore core = GetCachedCore();
            if (core != null && core.roleDatabase != null)
                return _cachedRoleDatabase = core.roleDatabase;
            return _cachedRoleDatabase = Object.FindObjectOfType<Core.iRoleDatabase>();
        }

        /// <summary>Busca iPlayerRoleManager en la escena (con caché).</summary>
        public static Core.iPlayerRoleManager FindPlayerRoleManagerInScene()
        {
            if (_cachedPlayerRoleManager != null) return _cachedPlayerRoleManager;
            Core.iRoleSystemCore core = GetCachedCore();
            if (core != null && core.playerRoleManager != null)
                return _cachedPlayerRoleManager = core.playerRoleManager;
            return _cachedPlayerRoleManager = Object.FindObjectOfType<Core.iPlayerRoleManager>();
        }
    }
}
#endif
