// ============================================================================
// iRoleSystem v4.0.5 - iPickup Module Editor
// Author: ishiruhii
// Description: Inspector personalizado para iRolePickup
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Modules.Pickup.iRolePickup))]
    public class iRolePickupEditor : UnityEditor.Editor
    {
        private SerializedProperty playerRoleManager;
        private SerializedProperty pickups;
        private SerializedProperty allowedRoles;
        private SerializedProperty checkInterval;
        private SerializedProperty allowNoRole;

        private bool _foldPickups      = true;
        private bool _foldMemberships  = true;
        private bool _foldAdvanced     = false;

        private void OnEnable()
        {
            playerRoleManager = serializedObject.FindProperty("playerRoleManager");
            pickups           = serializedObject.FindProperty("pickups");
            allowedRoles      = serializedObject.FindProperty("allowedRoles");
            checkInterval     = serializedObject.FindProperty("checkInterval");
            allowNoRole       = serializedObject.FindProperty("allowNoRole");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // ── Header ────────────────────────────────────────────────────
            iRoleEditorStyle.DrawHeader("⬡", "iPickup",
                "Control de VRCPickups por membresía de rol",
                iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.Space(6);

            // ── Sistema ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  Referencias del Sistema", iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(playerRoleManager,
                new GUIContent("Player Role Manager", "Gestor de roles. Obligatorio."));
            if (playerRoleManager.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning(
                    "⚠  Asigna un iPlayerRoleManager para activar el módulo.",
                    iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Pickups ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◈  VRCPickups Controlados", iRoleEditorStyle.C_ACCENT_PURPLE);
            _foldPickups = EditorGUILayout.Foldout(_foldPickups,
                "  Configurar pickups", true, iRoleEditorStyle.SFoldout);

            if (_foldPickups)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

                // Línea de acento
                Rect lr = GUILayoutUtility.GetRect(1, 2);
                EditorGUI.DrawRect(lr, iRoleEditorStyle.C_ACCENT_PURPLE);
                EditorGUILayout.Space(4);

                EditorGUILayout.LabelField("✔  Pickups permitidos para roles con acceso",
                    iRoleEditorStyle.SSectionLabel);
                EditorGUILayout.LabelField(
                    "Los roles marcados con permiso podrán agarrar estos objetos.\n" +
                    "El resto los verá pero no podrá interactuar con ellos.",
                    iRoleEditorStyle.SDim);
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(pickups, new GUIContent("Pickups"), true);

                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal();
                iRoleEditorStyle.DrawChip($"◈ {pickups.arraySize} pickup(s)", iRoleEditorStyle.C_ACCENT_PURPLE);
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
                    "Las membresías marcadas podrán agarrar los pickups. Las no marcadas solo los verán.",
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
                "  Intervalo y acceso sin rol", true, iRoleEditorStyle.SFoldout);

            if (_foldAdvanced)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                EditorGUILayout.PropertyField(checkInterval,
                    new GUIContent("Intervalo de chequeo (s)",
                        "Frecuencia de comprobación del rol. Recomendado: 0.5s"));
                EditorGUILayout.PropertyField(allowNoRole,
                    new GUIContent("Permitir sin rol asignado",
                        "Si está activo, jugadores SIN rol (rol = -1) también podrán agarrar los pickups."));
                EditorGUILayout.LabelField(
                    "Tip: Menor intervalo = más reactivo pero mayor carga de CPU.",
                    iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(8);

            // ── Force Update ──────────────────────────────────────────────
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = Application.isPlaying;
            if (iRoleEditorStyle.ColorButton(
                    "▶  Forzar Actualización de Pickups",
                    Application.isPlaying ? iRoleEditorStyle.C_ACCENT_PURPLE : iRoleEditorStyle.C_TEXT_DIM,
                    GUILayout.Width(260), GUILayout.Height(26)))
                ((Modules.Pickup.iRolePickup)target).ForceUpdate();
            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        // ── Render de toggles de roles ──────────────────────────────────────

        private void DrawMembershipToggles(SerializedProperty prop, Core.iRoleDatabase db)
        {
            if (db.roleNames == null || db.roleNames.Length == 0)
            {
                iRoleEditorStyle.DrawWarning(
                    "No hay roles definidos en iRoleDatabase.", iRoleEditorStyle.C_ACCENT_ORANGE);
                return;
            }

            string[] names = db.roleNames;
            if (prop.arraySize != names.Length) prop.arraySize = names.Length;

            // Contador de roles con permiso
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
                EditorGUI.LabelField(
                    new Rect(row.x + 30, row.y + 4, 30, 18), $"[{i}]", EditorStyles.miniLabel);
                GUI.contentColor = prev;

                EditorGUI.LabelField(
                    new Rect(row.x + 62, row.y + 4, row.width - 110, 18),
                    names[i], has ? EditorStyles.boldLabel : EditorStyles.label);

                GUI.color = has ? iRoleEditorStyle.C_ACCENT_GREEN : iRoleEditorStyle.C_ACCENT_RED;
                EditorGUI.LabelField(
                    new Rect(row.xMax - 60, row.y + 6, 56, 16),
                    has ? "✔ PUEDE" : "✘ NO", EditorStyles.miniLabel);
                GUI.color = Color.white;
                EditorGUILayout.Space(0);
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            if (iRoleEditorStyle.ColorButton("✔  Todos", iRoleEditorStyle.C_ACCENT_GREEN, GUILayout.MinWidth(80)))
                for (int i = 0; i < prop.arraySize; i++)
                    prop.GetArrayElementAtIndex(i).boolValue = true;
            if (iRoleEditorStyle.ColorButton("✘  Ninguno", iRoleEditorStyle.C_ACCENT_RED, GUILayout.MinWidth(80)))
                for (int i = 0; i < prop.arraySize; i++)
                    prop.GetArrayElementAtIndex(i).boolValue = false;
            if (iRoleEditorStyle.ColorButton("⇄  Invertir", iRoleEditorStyle.C_ACCENT_BLUE, GUILayout.MinWidth(80)))
                for (int i = 0; i < prop.arraySize; i++)
                {
                    var e = prop.GetArrayElementAtIndex(i);
                    e.boolValue = !e.boolValue;
                }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
