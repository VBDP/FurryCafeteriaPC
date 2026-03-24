// ============================================================================
// iRoleSystem v5.8 - Name/Menu Module Editors  [REDESIGNED]
// Contiene: iNameSetterEditor, iNameGetRoleEditor, iRoleMenuTriggerEditor
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    // ========================================================================
    // iNameSetter Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.NameSetter.iNameSetter))]
    public class iNameSetterEditor : UnityEditor.Editor
    {
        private SerializedProperty core, roleManager, roleDatabase, assignOnJoin, assignDelay;
        private SerializedProperty playerDisplayNames, playerRoleIndices, feedbackPrefab;
        private SerializedProperty roleAssignedSound, feedbackText, debugMode;

        private void OnEnable()
        {
            core               = serializedObject.FindProperty("core");
            roleManager        = serializedObject.FindProperty("roleManager");
            roleDatabase       = serializedObject.FindProperty("roleDatabase");
            assignOnJoin       = serializedObject.FindProperty("assignOnJoin");
            assignDelay        = serializedObject.FindProperty("assignDelay");
            playerDisplayNames = serializedObject.FindProperty("playerDisplayNames");
            playerRoleIndices  = serializedObject.FindProperty("playerRoleIndices");
            feedbackPrefab     = serializedObject.FindProperty("feedbackPrefab");
            roleAssignedSound  = serializedObject.FindProperty("roleAssignedSound");
            feedbackText       = serializedObject.FindProperty("feedbackText");
            debugMode          = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("✦", "iNameSetter",
                "Asigna roles por nombre de jugador", iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.Space(6);

            // ── Sistema ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  " + iEditorLang.Get(EL.SECTION_SYSTEM_REFS), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            if (core.objectReferenceValue == null) core.objectReferenceValue = iRoleEditorUtils.FindRoleDatabaseInScene();
            EditorGUILayout.PropertyField(core, new GUIContent("iRoleSystem Core"));
            if (core.objectReferenceValue != null)
            {
                Core.iRoleSystemCore cr = (Core.iRoleSystemCore)core.objectReferenceValue;
                if (roleManager.objectReferenceValue == null && cr.playerRoleManager != null) roleManager.objectReferenceValue = cr.playerRoleManager;
                if (roleDatabase.objectReferenceValue == null && cr.roleDatabase != null) roleDatabase.objectReferenceValue = cr.roleDatabase;
            }
            EditorGUILayout.PropertyField(roleManager, new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_MANAGER)));
            EditorGUILayout.PropertyField(roleDatabase, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_DATABASE)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Config asignación ─────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_CONFIG), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(assignOnJoin, new GUIContent(iEditorLang.Get(EL.FIELD_ASSIGN_ON_JOIN)));
            EditorGUILayout.PropertyField(assignDelay, new GUIContent(iEditorLang.Get(EL.FIELD_ASSIGN_DELAY)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Lista jugadores ───────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("✦  " + iEditorLang.Get(EL.SECTION_PLAYER_LIST), iRoleEditorStyle.C_ACCENT_GREEN);

            if (playerDisplayNames.arraySize != playerRoleIndices.arraySize)
            {
                int ns = Mathf.Max(playerDisplayNames.arraySize, playerRoleIndices.arraySize);
                playerDisplayNames.arraySize = ns; playerRoleIndices.arraySize = ns;
            }

            EditorGUILayout.BeginHorizontal();
            if (iRoleEditorStyle.ColorButton("+ " + iEditorLang.Get(EL.BTN_ADD_PLAYER), iRoleEditorStyle.C_ACCENT_GREEN, EditorStyles.miniButtonLeft, GUILayout.Height(22)))
            {
                int idx = playerDisplayNames.arraySize;
                playerDisplayNames.arraySize = idx + 1; playerRoleIndices.arraySize = idx + 1;
                playerDisplayNames.GetArrayElementAtIndex(idx).stringValue = "";
                playerRoleIndices.GetArrayElementAtIndex(idx).intValue = 0;
            }
            if (iRoleEditorStyle.ColorButton("- " + iEditorLang.Get(EL.FIELD_MAX_PLAYERS), iRoleEditorStyle.C_ACCENT_RED, EditorStyles.miniButtonRight, GUILayout.Height(22)))
                if (playerDisplayNames.arraySize > 0) { playerDisplayNames.arraySize--; playerRoleIndices.arraySize--; }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);

            Core.iRoleDatabase roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();

            if (playerDisplayNames.arraySize > 0)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                for (int i = 0; i < playerDisplayNames.arraySize; i++)
                {
                    SerializedProperty np = playerDisplayNames.GetArrayElementAtIndex(i);
                    SerializedProperty rp = playerRoleIndices.GetArrayElementAtIndex(i);
                    Rect row = GUILayoutUtility.GetRect(0, 24);
                    EditorGUI.DrawRect(row, i % 2 == 0 ? iRoleEditorStyle.C_BG_ROW_A : iRoleEditorStyle.C_BG_ROW_B);
                    EditorGUI.LabelField(new Rect(row.x + 4, row.y + 3, 26, 18), $"[{i}]", EditorStyles.miniLabel);
                    np.stringValue = EditorGUI.TextField(new Rect(row.x + 32, row.y + 3, row.width * 0.45f, 18), np.stringValue);
                    if (roleDb != null && roleDb.roleNames != null && roleDb.roleNames.Length > 0)
                    {
                        string[] opts = new string[roleDb.roleNames.Length + 1]; opts[0] = iEditorLang.Get(EL.MSG_EMPTY_DROPDOWN);
                        for (int j = 0; j < roleDb.roleNames.Length; j++) opts[j + 1] = $"[{j}] {roleDb.roleNames[j]}";
                        int sel = rp.intValue + 1;
                        sel = EditorGUI.Popup(new Rect(row.x + row.width * 0.5f, row.y + 3, row.width * 0.5f - 4, 18), sel, opts);
                        rp.intValue = sel - 1;
                    }
                    EditorGUILayout.Space(0);
                }
                EditorGUILayout.EndVertical();
            }
            else
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_NO_PLAYERS_CFG), iRoleEditorStyle.C_TEXT_DIM);

            EditorGUILayout.Space(4);
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_SYNC_FROM_NGR), iRoleEditorStyle.C_ACCENT_CYAN, GUILayout.Height(22)))
            {
                Modules.iNameGetRole.iNameGetRole ngr = Object.FindObjectOfType<Modules.iNameGetRole.iNameGetRole>();
                if (ngr != null && ngr.playerNames != null)
                {
                    playerDisplayNames.arraySize = ngr.playerNames.Length; playerRoleIndices.arraySize = ngr.playerNames.Length;
                    for (int i = 0; i < ngr.playerNames.Length; i++) { playerDisplayNames.GetArrayElementAtIndex(i).stringValue = ngr.playerNames[i]; playerRoleIndices.GetArrayElementAtIndex(i).intValue = 0; }
                }
            }
            EditorGUILayout.Space(4);

            // ── Audio / Feedback ──────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("♪  " + iEditorLang.Get(EL.SECTION_AUDIO), iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(roleAssignedSound, new GUIContent(iEditorLang.Get(EL.FIELD_FEEDBACK_SOUND)));
            EditorGUILayout.PropertyField(feedbackText, new GUIContent(iEditorLang.Get(EL.FIELD_FEEDBACK_TEXT)));
            EditorGUILayout.PropertyField(feedbackPrefab, new GUIContent(iEditorLang.Get(EL.FIELD_FALLBACK_PREFAB)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Debug ─────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_DEBUG), iRoleEditorStyle.C_TEXT_DIM);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(debugMode, new GUIContent(iEditorLang.Get(EL.FIELD_DEBUG_MODE)));
            EditorGUILayout.EndVertical();

            // ── Resumen ───────────────────────────────────────────────────
            Modules.NameSetter.iNameSetter module = (Modules.NameSetter.iNameSetter)target;
            if (module != null && playerDisplayNames.arraySize > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_SUMMARY), iRoleEditorStyle.SSectionLabel);
                int cfg = module.GetConfiguredPlayersCount();
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_TOTAL_PLAYERS, cfg.ToString()), iRoleEditorStyle.SDim);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_ASSIGN_JOIN, module.assignOnJoin.ToString()), iRoleEditorStyle.SDim);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_DELAY, module.assignDelay.ToString("F1")), iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }

    // ========================================================================
    // iNameGetRole Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.iNameGetRole.iNameGetRole))]
    public class iNameGetRoleEditor : UnityEditor.Editor
    {
        private SerializedProperty core, roleManager, roleDatabase;
        private SerializedProperty playerNames, maxSlotsPerPlayer, perPlayerRoleIds, perPlayerPrefabs, defaultRolePrefab;
        private SerializedProperty allowedRoleIds, maxSlotsPerRole, perRoleButtonIds, perRoleButtonPrefabs;
        private SerializedProperty roleSelectionCanvas, roleButtonsContainer, allowMasterAccess;
        private SerializedProperty menuOpenSound, roleAssignedSound, accessDeniedSound;
        private SerializedProperty feedbackText, roleCheckInterval, roleDisplayText, debugMode, maxButtons;

        private bool[] _playerFoldouts     = new bool[0];
        private bool[] _allowedRoleFoldouts = new bool[0];

        private static readonly Color C_ADD     = new Color(0.25f, 0.70f, 0.35f);
        private static readonly Color C_DEL     = new Color(0.85f, 0.25f, 0.25f);
        private static readonly Color C_HEADER  = new Color(0.18f, 0.35f, 0.55f);

        private void OnEnable()
        {
            core                 = serializedObject.FindProperty("core");
            roleManager          = serializedObject.FindProperty("roleManager");
            roleDatabase         = serializedObject.FindProperty("roleDatabase");
            playerNames          = serializedObject.FindProperty("playerNames");
            maxSlotsPerPlayer    = serializedObject.FindProperty("maxSlotsPerPlayer");
            perPlayerRoleIds     = serializedObject.FindProperty("perPlayerRoleIds");
            perPlayerPrefabs     = serializedObject.FindProperty("perPlayerPrefabs");
            defaultRolePrefab    = serializedObject.FindProperty("defaultRolePrefab");
            allowedRoleIds       = serializedObject.FindProperty("allowedRoleIds");
            maxSlotsPerRole      = serializedObject.FindProperty("maxSlotsPerRole");
            perRoleButtonIds     = serializedObject.FindProperty("perRoleButtonIds");
            perRoleButtonPrefabs = serializedObject.FindProperty("perRoleButtonPrefabs");
            roleSelectionCanvas  = serializedObject.FindProperty("roleSelectionCanvas");
            roleButtonsContainer = serializedObject.FindProperty("roleButtonsContainer");
            allowMasterAccess    = serializedObject.FindProperty("allowMasterAccess");
            menuOpenSound        = serializedObject.FindProperty("menuOpenSound");
            roleAssignedSound    = serializedObject.FindProperty("roleAssignedSound");
            accessDeniedSound    = serializedObject.FindProperty("accessDeniedSound");
            feedbackText         = serializedObject.FindProperty("feedbackText");
            roleCheckInterval    = serializedObject.FindProperty("roleCheckInterval");
            roleDisplayText      = serializedObject.FindProperty("roleDisplayText");
            debugMode            = serializedObject.FindProperty("debugMode");
            maxButtons           = serializedObject.FindProperty("maxButtons");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("✦", "iNameGetRole",
                iEditorLang.Get(EL.SECTION_PLAYER_LIST), iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.Space(6);

            // ── Sistema ───────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  " + iEditorLang.Get(EL.SECTION_SYSTEM_REFS), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            if (core.objectReferenceValue == null)
            {
                Core.iRoleSystemCore found = Object.FindObjectOfType<Core.iRoleSystemCore>();
                if (found != null) core.objectReferenceValue = found;
            }
            EditorGUILayout.PropertyField(core, new GUIContent("iRoleSystem Core"));
            if (core.objectReferenceValue != null)
            {
                Core.iRoleSystemCore coreRef = (Core.iRoleSystemCore)core.objectReferenceValue;
                if (roleManager.objectReferenceValue == null && coreRef.playerRoleManager != null) roleManager.objectReferenceValue = coreRef.playerRoleManager;
                if (roleDatabase.objectReferenceValue == null && coreRef.roleDatabase != null) roleDatabase.objectReferenceValue = coreRef.roleDatabase;
            }
            EditorGUILayout.PropertyField(roleManager, new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_MANAGER)));
            EditorGUILayout.PropertyField(roleDatabase, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_DATABASE)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Config global ─────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_GLOBAL_CFG), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUI.BeginChangeCheck();
            int newMax = EditorGUILayout.IntSlider(
                new GUIContent(iEditorLang.Get(EL.FIELD_MAX_SLOTS), iEditorLang.Get(EL.MSG_MAX_SLOTS_WARN)),
                maxSlotsPerPlayer.intValue, 1, 16);
            if (EditorGUI.EndChangeCheck())
            {
                if (playerNames.arraySize > 0)
                {
                    bool ok = EditorUtility.DisplayDialog(iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_TITLE), iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_BODY), iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_YES), iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_NO));
                    if (ok) { maxSlotsPerPlayer.intValue = newMax; RebuildFlatArrays(playerNames.arraySize, newMax); }
                }
                else maxSlotsPerPlayer.intValue = newMax;
            }
            EditorGUILayout.PropertyField(defaultRolePrefab, new GUIContent(iEditorLang.Get(EL.FIELD_FALLBACK_PREFAB)));
            GUI.color = iRoleEditorStyle.C_TEXT_DIM;
            EditorGUILayout.LabelField("⚡ El texto (TextMeshPro UI) del prefab se configura automáticamente con el nombre del rol.", EditorStyles.miniLabel);
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Lista jugadores ───────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("✦  " + iEditorLang.Get(EL.SECTION_PLAYER_LIST), iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.Space(4);
            DrawPlayerList();
            EditorGUILayout.Space(4);

            // ── Roles con Acceso ──────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◆  Roles con Acceso al Canvas", new Color(0.6f, 0.3f, 0.9f));
            EditorGUILayout.Space(4);
            DrawAllowedRoleList();
            EditorGUILayout.Space(4);

            // ── UI ────────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◈  " + iEditorLang.Get(EL.SECTION_UI), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(roleSelectionCanvas, new GUIContent(iEditorLang.Get(EL.FIELD_CANVAS)));
            EditorGUILayout.PropertyField(roleButtonsContainer, new GUIContent(iEditorLang.Get(EL.FIELD_BTN_CONTAINER)));
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(allowMasterAccess, new GUIContent("Permitir acceso al Master", "Si está activo, el Master de la instancia puede abrir el canvas aunque no esté en la lista de jugadores. Verá todos los roles disponibles."));
            if (allowMasterAccess.boolValue)
            {
                GUI.color = iRoleEditorStyle.C_ACCENT_ORANGE;
                EditorGUILayout.LabelField("♛ El Master podrá usar este canvas con todos los roles.", EditorStyles.miniLabel);
                GUI.color = Color.white;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Audio ─────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("♪  " + iEditorLang.Get(EL.SECTION_AUDIO), iRoleEditorStyle.C_ACCENT_PURPLE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(menuOpenSound, new GUIContent(iEditorLang.Get(EL.FIELD_MENU_OPEN_SOUND)));
            EditorGUILayout.PropertyField(roleAssignedSound, new GUIContent(iEditorLang.Get(EL.FIELD_ASSIGN_SOUND)));
            EditorGUILayout.PropertyField(accessDeniedSound, new GUIContent(iEditorLang.Get(EL.FIELD_ACCESS_DENIED_SND)));
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(feedbackText, new GUIContent(iEditorLang.Get(EL.FIELD_FEEDBACK_TEXT)));
            EditorGUILayout.PropertyField(roleCheckInterval, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_INTERVAL_TXT)));
            EditorGUILayout.PropertyField(roleDisplayText, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_DISPLAY_TXT)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Debug ─────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_DEBUG), iRoleEditorStyle.C_TEXT_DIM);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(debugMode, new GUIContent(iEditorLang.Get(EL.FIELD_DEBUG_MODE)));
            EditorGUILayout.PropertyField(maxButtons, new GUIContent(iEditorLang.Get(EL.FIELD_MAX_BUTTONS)));
            EditorGUILayout.EndVertical();

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }

        private void DrawPlayerList()
        {
            int playerCount = playerNames.arraySize;
            int maxSlots    = Mathf.Max(1, maxSlotsPerPlayer.intValue);
            EnsureFoldoutsSize(playerCount);

            int expected = playerCount * maxSlots;
            if (perPlayerRoleIds.arraySize != expected || perPlayerPrefabs.arraySize != expected)
            {
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_ARRAY_MISMATCH), iRoleEditorStyle.C_ACCENT_ORANGE);
                if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_REPAIR_ARRAYS), iRoleEditorStyle.C_ACCENT_ORANGE, GUILayout.Height(24)))
                { serializedObject.ApplyModifiedProperties(); RebuildFlatArraysPreserving(playerCount, maxSlots); serializedObject.Update(); }
                EditorGUILayout.Space(4);
            }

            Core.iRoleDatabase roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();
            bool removedPlayer = false;

            for (int p = 0; p < playerCount; p++)
            {
                if (removedPlayer) break;
                int start = p * maxSlots;
                int activeSlots = CountActiveSlotsSerializedProp(start, maxSlots);
                string playerLabel = playerNames.GetArrayElementAtIndex(p).stringValue;
                if (string.IsNullOrEmpty(playerLabel)) playerLabel = "?";

                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);

                // Fila header del jugador
                Rect headerRow = GUILayoutUtility.GetRect(0, 28);
                EditorGUI.DrawRect(headerRow, C_HEADER * new Color(1,1,1,0.6f));
                EditorGUI.DrawRect(new Rect(headerRow.x, headerRow.y, 3, headerRow.height), iRoleEditorStyle.C_ACCENT_GREEN);

                _playerFoldouts[p] = EditorGUI.Foldout(
                    new Rect(headerRow.x + 6, headerRow.y + 6, 16, 16),
                    _playerFoldouts[p], "", true);

                SerializedProperty nameProp = playerNames.GetArrayElementAtIndex(p);
                nameProp.stringValue = EditorGUI.TextField(
                    new Rect(headerRow.x + 26, headerRow.y + 5, headerRow.width - 90, 18),
                    nameProp.stringValue);

                // Badge slots activos
                Color slotColor = activeSlots > 0 ? iRoleEditorStyle.C_ACCENT_GREEN : iRoleEditorStyle.C_TEXT_DIM;
                GUI.color = slotColor;
                EditorGUI.LabelField(
                    new Rect(headerRow.xMax - 62, headerRow.y + 7, 36, 16),
                    $"{activeSlots} rol(es)", EditorStyles.miniLabel);
                GUI.color = Color.white;

                // Botón eliminar
                Color bp = GUI.backgroundColor;
                GUI.backgroundColor = C_DEL * new Color(1,1,1,0.7f);
                if (GUI.Button(new Rect(headerRow.xMax - 24, headerRow.y + 5, 20, 18), "✕", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog(iEditorLang.Get(EL.DLG_DELETE_PLAYER_TITLE), iEditorLang.Get(EL.DLG_DELETE_PLAYER_BODY, playerLabel), iEditorLang.Get(EL.DLG_DELETE_BTN), iEditorLang.Get(EL.DLG_CANCEL_BTN)))
                    { serializedObject.ApplyModifiedProperties(); RemovePlayer(p, maxSlots); serializedObject.Update(); removedPlayer = true; }
                }
                GUI.backgroundColor = bp;
                EditorGUILayout.Space(0);

                if (_playerFoldouts[p] && !removedPlayer)
                {
                    EditorGUILayout.Space(4);
                    if (roleDb == null)
                        iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_NO_ROLES_DEFINED), iRoleEditorStyle.C_TEXT_DIM);
                    else
                    {
                        string[] roleOptions = BuildRoleDropdownOptions(roleDb);
                        bool hasAny = false;

                        for (int s = 0; s < maxSlots; s++)
                        {
                            int idx = start + s;
                            if (idx >= perPlayerRoleIds.arraySize) break;
                            SerializedProperty roleIdProp = perPlayerRoleIds.GetArrayElementAtIndex(idx);
                            if (roleIdProp.intValue < 0) continue;
                            hasAny = true;
                            SerializedProperty prefabProp = idx < perPlayerPrefabs.arraySize ? perPlayerPrefabs.GetArrayElementAtIndex(idx) : null;

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField($"Slot {s+1}", GUILayout.Width(44));
                            int di = roleIdProp.intValue + 1; if (di < 0 || di >= roleOptions.Length) di = 0;
                            int ni = EditorGUILayout.Popup(di, roleOptions);
                            roleIdProp.intValue = ni - 1;
                            if (ni > 0 && roleDb.roleColors != null && (ni-1) < roleDb.roleColors.Length)
                                EditorGUI.DrawRect(GUILayoutUtility.GetRect(14,14,GUILayout.Width(14)), roleDb.roleColors[ni-1]);
                            EditorGUILayout.EndHorizontal();
                            if (prefabProp != null)
                            { EditorGUILayout.BeginHorizontal(); EditorGUILayout.LabelField(iEditorLang.Get(EL.FIELD_DISPLAY_PREFAB) + ":", GUILayout.Width(56)); EditorGUILayout.PropertyField(prefabProp, GUIContent.none); EditorGUILayout.EndHorizontal(); }
                            EditorGUILayout.EndVertical();
                            GUI.backgroundColor = C_DEL * new Color(1,1,1,0.7f);
                            if (GUILayout.Button("✕", GUILayout.Width(22), GUILayout.Height(34))) { roleIdProp.intValue=-1; if (prefabProp!=null) prefabProp.objectReferenceValue=null; }
                            GUI.backgroundColor = bp;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space(2);
                        }

                        if (!hasAny) iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_SLOT_EMPTY), iRoleEditorStyle.C_TEXT_DIM);
                        EditorGUILayout.Space(4);
                        if (activeSlots < maxSlots)
                        {
                            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_ADD_SLOT), C_ADD, GUILayout.Height(22)))
                                AddSlotToPlayer(p, start, maxSlots);
                        }
                        else iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_SLOT_MAX_REACHED, maxSlots.ToString()), iRoleEditorStyle.C_ACCENT_ORANGE);
                    }
                    EditorGUILayout.Space(4);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
            }

            // Botón añadir jugador
            EditorGUILayout.Space(4);
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_ADD_PLAYER), C_ADD, GUILayout.Height(28)))
            { serializedObject.ApplyModifiedProperties(); AddPlayer(maxSlots); serializedObject.Update(); }

            // Resumen
            if (playerCount > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_SUMMARY), iRoleEditorStyle.SSectionLabel);
                int totalRoles = 0;
                for (int p = 0; p < playerCount; p++) totalRoles += CountActiveSlotsSerializedProp(p * maxSlots, maxSlots);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_TOTAL_PLAYERS, playerCount.ToString()), iRoleEditorStyle.SDim);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_TOTAL_SLOTS, totalRoles.ToString()), iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }
        }

        // ── Helpers array ─────────────────────────────────────────────────
        private int CountActiveSlotsSerializedProp(int start, int max)
        { int c=0; for(int s=0;s<max;s++){int idx=start+s; if(idx>=perPlayerRoleIds.arraySize)break; if(perPlayerRoleIds.GetArrayElementAtIndex(idx).intValue>=0)c++;} return c; }

        private void AddSlotToPlayer(int p, int start, int max)
        { for(int s=0;s<max;s++){int idx=start+s; if(idx>=perPlayerRoleIds.arraySize)break; if(perPlayerRoleIds.GetArrayElementAtIndex(idx).intValue<0){perPlayerRoleIds.GetArrayElementAtIndex(idx).intValue=0;_playerFoldouts[p]=true;return;}} }

        private void AddPlayer(int maxSlots)
        {
            Modules.iNameGetRole.iNameGetRole t = (Modules.iNameGetRole.iNameGetRole)target;
            int old=t.playerNames!=null?t.playerNames.Length:0, @new=old+1;
            string[] names=new string[@new]; if(t.playerNames!=null) t.playerNames.CopyTo(names,0); names[old]="NewPlayer"; t.playerNames=names;
            int oldSz=t.perPlayerRoleIds!=null?t.perPlayerRoleIds.Length:0, newSz=@new*maxSlots;
            int[] ids=new int[newSz]; if(t.perPlayerRoleIds!=null) System.Array.Copy(t.perPlayerRoleIds,ids,Mathf.Min(oldSz,newSz));
            for(int s=0;s<maxSlots;s++) ids[old*maxSlots+s]=-1; t.perPlayerRoleIds=ids;
            int oldPSz=t.perPlayerPrefabs!=null?t.perPlayerPrefabs.Length:0;
            GameObject[] pf=new GameObject[newSz]; if(t.perPlayerPrefabs!=null) System.Array.Copy(t.perPlayerPrefabs,pf,Mathf.Min(oldPSz,newSz));
            t.perPlayerPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RemovePlayer(int p, int maxSlots)
        {
            Modules.iNameGetRole.iNameGetRole t=(Modules.iNameGetRole.iNameGetRole)target;
            int old=t.playerNames!=null?t.playerNames.Length:0; if(p<0||p>=old)return;
            string[] names=new string[old-1]; for(int i=0,j=0;i<old;i++){if(i==p)continue;names[j++]=t.playerNames[i];} t.playerNames=names;
            int rs=p*maxSlots,re=rs+maxSlots,sz=t.perPlayerRoleIds!=null?t.perPlayerRoleIds.Length:0,nsz=Mathf.Max(0,sz-maxSlots);
            int[] ids=new int[nsz]; GameObject[] pf=new GameObject[nsz]; int wi=0;
            for(int i=0;i<sz;i++){if(i>=rs&&i<re)continue;if(wi>=nsz)break;ids[wi]=t.perPlayerRoleIds[i];pf[wi]=t.perPlayerPrefabs!=null&&i<t.perPlayerPrefabs.Length?t.perPlayerPrefabs[i]:null;wi++;}
            t.perPlayerRoleIds=ids; t.perPlayerPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RebuildFlatArrays(int cnt, int max)
        { Modules.iNameGetRole.iNameGetRole t=(Modules.iNameGetRole.iNameGetRole)target; int sz=cnt*max; t.perPlayerRoleIds=new int[sz]; t.perPlayerPrefabs=new GameObject[sz]; for(int i=0;i<sz;i++) t.perPlayerRoleIds[i]=-1; EditorUtility.SetDirty(t); }

        private void RebuildFlatArraysPreserving(int cnt, int max)
        {
            Modules.iNameGetRole.iNameGetRole t=(Modules.iNameGetRole.iNameGetRole)target;
            int nsz=cnt*max; int[] oi=t.perPlayerRoleIds; GameObject[] op=t.perPlayerPrefabs;
            int[] ni=new int[nsz]; GameObject[] np=new GameObject[nsz];
            for(int i=0;i<nsz;i++){ni[i]=oi!=null&&i<oi.Length?oi[i]:-1;np[i]=op!=null&&i<op.Length?op[i]:null;}
            t.perPlayerRoleIds=ni; t.perPlayerPrefabs=np; EditorUtility.SetDirty(t);
        }

        private string[] BuildRoleDropdownOptions(Core.iRoleDatabase db)
        { if(db==null||db.roleNames==null||db.roleNames.Length==0)return new[]{iEditorLang.Get(EL.MSG_NO_ROLES_DEFINED)}; string[] opts=new string[db.roleNames.Length+1]; opts[0]=iEditorLang.Get(EL.MSG_EMPTY_DROPDOWN); for(int i=0;i<db.roleNames.Length;i++) opts[i+1]=$"[{i}] {db.roleNames[i]}"; return opts; }

        private void EnsureFoldoutsSize(int cnt)
        { if(_playerFoldouts==null||_playerFoldouts.Length!=cnt){bool[] nf=new bool[cnt];int cl=_playerFoldouts!=null?Mathf.Min(_playerFoldouts.Length,cnt):0;for(int i=0;i<cl;i++)nf[i]=_playerFoldouts[i];_playerFoldouts=nf;} }

        private void EnsureAllowedFoldoutsSize(int cnt)
        { if(_allowedRoleFoldouts==null||_allowedRoleFoldouts.Length!=cnt){bool[] nf=new bool[cnt];int cl=_allowedRoleFoldouts!=null?Mathf.Min(_allowedRoleFoldouts.Length,cnt):0;for(int i=0;i<cl;i++)nf[i]=_allowedRoleFoldouts[i];_allowedRoleFoldouts=nf;} }

        // ── Sección "Roles con Acceso" ────────────────────────────────────
        private void DrawAllowedRoleList()
        {
            int roleCount  = allowedRoleIds.arraySize;
            int maxSlots   = Mathf.Max(1, maxSlotsPerRole.intValue);
            EnsureAllowedFoldoutsSize(roleCount);

            int expected = roleCount * maxSlots;
            if (perRoleButtonIds.arraySize != expected || perRoleButtonPrefabs.arraySize != expected)
            {
                iRoleEditorStyle.DrawWarning("Arrays de botones por rol desincronizados. Repara.", iRoleEditorStyle.C_ACCENT_ORANGE);
                if (iRoleEditorStyle.ColorButton("Reparar arrays", iRoleEditorStyle.C_ACCENT_ORANGE, GUILayout.Height(22)))
                {
                    serializedObject.ApplyModifiedProperties();
                    RebuildAllowedRoleArrays(roleCount, maxSlots);
                    serializedObject.Update();
                }
                EditorGUILayout.Space(4);
            }

            Core.iRoleDatabase roleDb = iRoleEditorUtils.FindRoleDatabaseInScene();
            string[] roleOptions      = BuildRoleDropdownOptions(roleDb);

            // Slider de slots por rol
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUI.BeginChangeCheck();
            int newMaxSlots = EditorGUILayout.IntSlider(
                new GUIContent("Slots por Rol", "Máximo de botones disponibles por cada rol permitido"),
                maxSlotsPerRole.intValue, 1, 16);
            if (EditorGUI.EndChangeCheck())
            {
                if (roleCount > 0)
                {
                    bool ok = EditorUtility.DisplayDialog("Cambiar slots por rol",
                        "¿Reconstruir arrays? Se perderá la configuración actual de botones por rol.",
                        "Sí, reconstruir", "Cancelar");
                    if (ok) { maxSlotsPerRole.intValue = newMaxSlots; serializedObject.ApplyModifiedProperties(); RebuildAllowedRoleArrays(roleCount, newMaxSlots); serializedObject.Update(); }
                }
                else maxSlotsPerRole.intValue = newMaxSlots;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            Color C_ROLE_HEADER = new Color(0.25f, 0.15f, 0.45f);
            bool removedEntry = false;

            for (int r = 0; r < roleCount; r++)
            {
                if (removedEntry) break;

                int start  = r * maxSlots;
                int active = CountActiveAllowedSlots(start, maxSlots);

                // Obtener nombre del rol permitido
                int    thisRoleId   = allowedRoleIds.GetArrayElementAtIndex(r).intValue;
                string thisRoleName = (roleDb != null && roleDb.roleNames != null && thisRoleId >= 0 && thisRoleId < roleDb.roleNames.Length)
                    ? roleDb.roleNames[thisRoleId] : (thisRoleId >= 0 ? $"Rol #{thisRoleId}" : "Sin Rol");

                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);

                // Header del rol
                Rect headerRow = GUILayoutUtility.GetRect(0, 28);
                EditorGUI.DrawRect(headerRow, C_ROLE_HEADER * new Color(1,1,1,0.7f));
                EditorGUI.DrawRect(new Rect(headerRow.x, headerRow.y, 3, headerRow.height), new Color(0.6f, 0.3f, 0.9f));

                _allowedRoleFoldouts[r] = EditorGUI.Foldout(
                    new Rect(headerRow.x + 6, headerRow.y + 6, 16, 16),
                    _allowedRoleFoldouts[r], "", true);

                // Dropdown para elegir el rol de acceso
                if (roleDb != null && roleDb.roleNames != null && roleDb.roleNames.Length > 0)
                {
                    int sel = thisRoleId + 1;
                    if (sel < 0 || sel >= roleOptions.Length) sel = 0;
                    int newSel = EditorGUI.Popup(
                        new Rect(headerRow.x + 26, headerRow.y + 5, headerRow.width - 90, 18),
                        sel, roleOptions);
                    allowedRoleIds.GetArrayElementAtIndex(r).intValue = newSel - 1;
                }
                else
                {
                    EditorGUI.LabelField(
                        new Rect(headerRow.x + 26, headerRow.y + 5, headerRow.width - 90, 18),
                        thisRoleName, EditorStyles.miniLabel);
                }

                // Badge slots activos
                Color slotColor = active > 0 ? new Color(0.6f, 0.3f, 0.9f) : iRoleEditorStyle.C_TEXT_DIM;
                GUI.color = slotColor;
                EditorGUI.LabelField(
                    new Rect(headerRow.xMax - 62, headerRow.y + 7, 36, 16),
                    $"{active} btn(s)", EditorStyles.miniLabel);
                GUI.color = Color.white;

                // Botón eliminar
                Color bp = GUI.backgroundColor;
                GUI.backgroundColor = C_DEL * new Color(1,1,1,0.7f);
                if (GUI.Button(new Rect(headerRow.xMax - 24, headerRow.y + 5, 20, 18), "✕", EditorStyles.miniButton))
                {
                    if (EditorUtility.DisplayDialog("Eliminar rol de acceso",
                        $"¿Eliminar acceso para '{thisRoleName}'?", "Eliminar", "Cancelar"))
                    {
                        serializedObject.ApplyModifiedProperties();
                        RemoveAllowedRole(r, maxSlots);
                        serializedObject.Update();
                        removedEntry = true;
                    }
                }
                GUI.backgroundColor = bp;
                EditorGUILayout.Space(0);

                if (_allowedRoleFoldouts[r] && !removedEntry)
                {
                    EditorGUILayout.Space(4);
                    if (roleDb == null)
                        iRoleEditorStyle.DrawWarning("Asigna un iRoleDatabase para configurar botones.", iRoleEditorStyle.C_TEXT_DIM);
                    else
                    {
                        bool hasAny = false;
                        for (int s = 0; s < maxSlots; s++)
                        {
                            int idx = start + s;
                            if (idx >= perRoleButtonIds.arraySize) break;
                            SerializedProperty rp = perRoleButtonIds.GetArrayElementAtIndex(idx);
                            if (rp.intValue < 0) continue;
                            hasAny = true;
                            SerializedProperty pp = idx < perRoleButtonPrefabs.arraySize ? perRoleButtonPrefabs.GetArrayElementAtIndex(idx) : null;

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField($"Btn {s+1}", GUILayout.Width(44));
                            int di = rp.intValue + 1; if (di < 0 || di >= roleOptions.Length) di = 0;
                            int ni = EditorGUILayout.Popup(di, roleOptions);
                            rp.intValue = ni - 1;
                            if (ni > 0 && roleDb.roleColors != null && (ni-1) < roleDb.roleColors.Length)
                                EditorGUI.DrawRect(GUILayoutUtility.GetRect(14,14,GUILayout.Width(14)), roleDb.roleColors[ni-1]);
                            EditorGUILayout.EndHorizontal();
                            if (pp != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Prefab:", GUILayout.Width(46));
                                EditorGUILayout.PropertyField(pp, GUIContent.none);
                                GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                                EditorGUILayout.LabelField("(opcional, usa fallback si vacío)", EditorStyles.miniLabel, GUILayout.Width(180));
                                GUI.color = Color.white;
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                            GUI.backgroundColor = C_DEL * new Color(1,1,1,0.7f);
                            if (GUILayout.Button("✕", GUILayout.Width(22), GUILayout.Height(34))) { rp.intValue=-1; if(pp!=null) pp.objectReferenceValue=null; }
                            GUI.backgroundColor = bp;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.Space(2);
                        }

                        if (!hasAny) iRoleEditorStyle.DrawWarning("Sin botones configurados para este rol.", iRoleEditorStyle.C_TEXT_DIM);
                        EditorGUILayout.Space(4);

                        if (active < maxSlots)
                        {
                            if (iRoleEditorStyle.ColorButton("+ Añadir Botón", new Color(0.6f, 0.3f, 0.9f), GUILayout.Height(22)))
                                AddSlotToAllowedRole(r, start, maxSlots);
                        }
                        else
                            iRoleEditorStyle.DrawWarning($"Máximo de {maxSlots} botones alcanzado.", iRoleEditorStyle.C_ACCENT_ORANGE);
                    }
                    EditorGUILayout.Space(4);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(3);
            }

            EditorGUILayout.Space(4);
            if (iRoleEditorStyle.ColorButton("+ Añadir Rol con Acceso", new Color(0.5f, 0.25f, 0.8f), GUILayout.Height(28)))
            {
                serializedObject.ApplyModifiedProperties();
                AddAllowedRole(maxSlots);
                serializedObject.Update();
            }

            if (roleCount > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.LabelField("Resumen", iRoleEditorStyle.SSectionLabel);
                int total = 0;
                for (int r = 0; r < roleCount; r++) total += CountActiveAllowedSlots(r * maxSlots, maxSlots);
                EditorGUILayout.LabelField($"Roles con acceso: {roleCount}", iRoleEditorStyle.SDim);
                EditorGUILayout.LabelField($"Total botones configurados: {total}", iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }
        }

        // ── Helpers allowedRole ───────────────────────────────────────────
        private int CountActiveAllowedSlots(int start, int max)
        { int c=0; for(int s=0;s<max;s++){int idx=start+s; if(idx>=perRoleButtonIds.arraySize)break; if(perRoleButtonIds.GetArrayElementAtIndex(idx).intValue>=0)c++;} return c; }

        private void AddSlotToAllowedRole(int r, int start, int max)
        { for(int s=0;s<max;s++){int idx=start+s; if(idx>=perRoleButtonIds.arraySize)break; if(perRoleButtonIds.GetArrayElementAtIndex(idx).intValue<0){perRoleButtonIds.GetArrayElementAtIndex(idx).intValue=0;_allowedRoleFoldouts[r]=true;return;}} }

        private void AddAllowedRole(int maxSlots)
        {
            Modules.iNameGetRole.iNameGetRole t = (Modules.iNameGetRole.iNameGetRole)target;
            int old = t.allowedRoleIds != null ? t.allowedRoleIds.Length : 0, @new = old + 1;
            int[] ids = new int[@new]; if(t.allowedRoleIds!=null) t.allowedRoleIds.CopyTo(ids,0); ids[old] = -1; t.allowedRoleIds = ids;
            int newSz = @new * maxSlots;
            int[] btnIds = new int[newSz]; if(t.perRoleButtonIds!=null) System.Array.Copy(t.perRoleButtonIds,btnIds,Mathf.Min(t.perRoleButtonIds.Length,newSz));
            for(int s=0;s<maxSlots;s++) btnIds[old*maxSlots+s]=-1; t.perRoleButtonIds=btnIds;
            GameObject[] pf=new GameObject[newSz]; if(t.perRoleButtonPrefabs!=null) System.Array.Copy(t.perRoleButtonPrefabs,pf,Mathf.Min(t.perRoleButtonPrefabs.Length,newSz));
            t.perRoleButtonPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RemoveAllowedRole(int r, int maxSlots)
        {
            Modules.iNameGetRole.iNameGetRole t=(Modules.iNameGetRole.iNameGetRole)target;
            int old=t.allowedRoleIds!=null?t.allowedRoleIds.Length:0; if(r<0||r>=old)return;
            int[] ids=new int[old-1]; for(int i=0,j=0;i<old;i++){if(i==r)continue;ids[j++]=t.allowedRoleIds[i];} t.allowedRoleIds=ids;
            int rs=r*maxSlots,re=rs+maxSlots,sz=t.perRoleButtonIds!=null?t.perRoleButtonIds.Length:0,nsz=Mathf.Max(0,sz-maxSlots);
            int[] btnIds=new int[nsz]; GameObject[] pf=new GameObject[nsz]; int wi=0;
            for(int i=0;i<sz;i++){if(i>=rs&&i<re)continue;if(wi>=nsz)break;btnIds[wi]=t.perRoleButtonIds[i];pf[wi]=t.perRoleButtonPrefabs!=null&&i<t.perRoleButtonPrefabs.Length?t.perRoleButtonPrefabs[i]:null;wi++;}
            t.perRoleButtonIds=btnIds; t.perRoleButtonPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RebuildAllowedRoleArrays(int cnt, int maxSlots)
        {
            Modules.iNameGetRole.iNameGetRole t=(Modules.iNameGetRole.iNameGetRole)target;
            int sz=cnt*maxSlots; t.perRoleButtonIds=new int[sz]; t.perRoleButtonPrefabs=new GameObject[sz];
            for(int i=0;i<sz;i++) t.perRoleButtonIds[i]=-1; EditorUtility.SetDirty(t);
        }
    }

    // ========================================================================
    // iRoleMenuTrigger Editor
    // ========================================================================
    [CustomEditor(typeof(Modules.iNameGetRole.iRoleMenuTrigger))]
    public class iRoleMenuTriggerEditor : UnityEditor.Editor
    {
        private SerializedProperty nameGetRole, canvasTransform, authorizedUsers;
        private SerializedProperty inputMode, pcKey, oculusButtonIndex, oculusButtonNames;
        private SerializedProperty distanceMultiplier, verticalOffset, baseAvatarHeight, baseCanvasScale;
        private SerializedProperty facingPlayer, flatRotation, cooldown, debugMode;

        private static readonly string[] OCULUS_LABELS = {
            "A  —  Right Hand", "B  —  Right Hand", "X  —  Left Hand", "Y  —  Left Hand",
            "Grip  —  Right", "Grip  —  Left", "Index Trigger  —  Right", "Index Trigger  —  Left",
            "Thumbstick Click  —  Right", "Thumbstick Click  —  Left"
        };
        private static readonly Color C_SYNC = new Color(0.20f, 0.60f, 0.80f);
        private static readonly Color C_ADD  = new Color(0.25f, 0.65f, 0.30f);
        private static readonly Color C_DEL  = new Color(0.80f, 0.25f, 0.25f);

        private void OnEnable()
        {
            nameGetRole        = serializedObject.FindProperty("nameGetRole");
            canvasTransform    = serializedObject.FindProperty("canvasTransform");
            authorizedUsers    = serializedObject.FindProperty("authorizedUsers");
            inputMode          = serializedObject.FindProperty("inputMode");
            pcKey              = serializedObject.FindProperty("pcKey");
            oculusButtonIndex  = serializedObject.FindProperty("oculusButtonIndex");
            oculusButtonNames  = serializedObject.FindProperty("oculusButtonNames");
            distanceMultiplier = serializedObject.FindProperty("distanceMultiplier");
            verticalOffset     = serializedObject.FindProperty("verticalOffset");
            baseAvatarHeight   = serializedObject.FindProperty("baseAvatarHeight");
            baseCanvasScale    = serializedObject.FindProperty("baseCanvasScale");
            facingPlayer       = serializedObject.FindProperty("facingPlayer");
            flatRotation       = serializedObject.FindProperty("flatRotation");
            cooldown           = serializedObject.FindProperty("cooldown");
            debugMode          = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("☰", "iRoleMenuTrigger",
                iEditorLang.Get(EL.SECTION_INPUT), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.Space(6);

            // ── Referencias ───────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⚙  " + iEditorLang.Get(EL.SECTION_REFERENCES), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(nameGetRole, new GUIContent("iNameGetRole"));
            EditorGUILayout.PropertyField(canvasTransform, new GUIContent(iEditorLang.Get(EL.FIELD_CANVAS)));
            if (nameGetRole.objectReferenceValue == null)
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_NO_NGR_ASSIGNED), iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Whitelist ─────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("✦  " + iEditorLang.Get(EL.SECTION_WHITELIST), iRoleEditorStyle.C_ACCENT_GREEN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_WHITELIST_HINT), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.Space(4);
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_SYNC_WHITELIST), C_SYNC, GUILayout.Height(24)))
            {
                Modules.iNameGetRole.iNameGetRole ngr = (Modules.iNameGetRole.iNameGetRole)nameGetRole.objectReferenceValue;
                if (ngr != null && ngr.playerNames != null && ngr.playerNames.Length > 0)
                { authorizedUsers.arraySize = ngr.playerNames.Length; for (int i=0;i<ngr.playerNames.Length;i++) authorizedUsers.GetArrayElementAtIndex(i).stringValue=ngr.playerNames[i]; }
                else EditorUtility.DisplayDialog(iEditorLang.Get(EL.DLG_ERROR_TITLE), iEditorLang.Get(EL.DLG_NO_NGR_PLAYERS), iEditorLang.Get(EL.DLG_OK));
            }
            EditorGUILayout.Space(4);
            int count = authorizedUsers.arraySize;
            for (int i = 0; i < count; i++)
            {
                Rect row = GUILayoutUtility.GetRect(0, 22);
                EditorGUI.DrawRect(row, i%2==0 ? iRoleEditorStyle.C_BG_ROW_A : iRoleEditorStyle.C_BG_ROW_B);
                EditorGUI.LabelField(new Rect(row.x+4, row.y+2, 26, 18), $"[{i}]", EditorStyles.miniLabel);
                SerializedProperty e = authorizedUsers.GetArrayElementAtIndex(i);
                e.stringValue = EditorGUI.TextField(new Rect(row.x+32, row.y+2, row.width-60, 18), e.stringValue);
                Color bp = GUI.backgroundColor; GUI.backgroundColor = C_DEL * new Color(1,1,1,0.7f);
                if (GUI.Button(new Rect(row.xMax-24, row.y+2, 20, 18), "✕", EditorStyles.miniButton)) authorizedUsers.DeleteArrayElementAtIndex(i);
                GUI.backgroundColor = bp;
                EditorGUILayout.Space(0);
            }
            EditorGUILayout.Space(4);
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_ADD_USER), C_ADD, GUILayout.Height(22)))
            { authorizedUsers.InsertArrayElementAtIndex(authorizedUsers.arraySize); authorizedUsers.GetArrayElementAtIndex(authorizedUsers.arraySize-1).stringValue=""; }
            EditorGUILayout.BeginHorizontal();
            iRoleEditorStyle.DrawChip($"{count} usuarios autorizados", iRoleEditorStyle.C_ACCENT_GREEN);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Input ─────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⌖  " + iEditorLang.Get(EL.SECTION_INPUT), iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.BeginHorizontal();
            Color bp2 = GUI.backgroundColor;
            GUI.backgroundColor = inputMode.intValue==0 ? new Color(0.25f,0.65f,0.35f) : new Color(0.28f,0.28f,0.32f);
            if (GUILayout.Button("🖥  PC", GUILayout.Height(32))) inputMode.intValue=0;
            GUI.backgroundColor = inputMode.intValue==1 ? new Color(0.25f,0.50f,0.85f) : new Color(0.28f,0.28f,0.32f);
            if (GUILayout.Button("🥽  Oculus / VR", GUILayout.Height(32))) inputMode.intValue=1;
            GUI.backgroundColor = bp2;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(8);
            if (inputMode.intValue == 0)
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_PC_MODE), iRoleEditorStyle.SSectionLabel);
                EditorGUILayout.PropertyField(pcKey, new GUIContent(iEditorLang.Get(EL.FIELD_PC_KEY)));
                EditorGUILayout.LabelField($"Presiona: {((KeyCode)pcKey.intValue)}", iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_OCULUS_MODE), iRoleEditorStyle.SSectionLabel);
                int ci = Mathf.Clamp(oculusButtonIndex.intValue, 0, OCULUS_LABELS.Length-1);
                int ni = EditorGUILayout.Popup(new GUIContent(iEditorLang.Get(EL.FIELD_QUEST_BUTTON)), ci, OCULUS_LABELS);
                if (ni != ci) oculusButtonIndex.intValue = ni;
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(cooldown, new GUIContent(iEditorLang.Get(EL.FIELD_COOLDOWN)));
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Posición canvas ───────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("⊡  " + iEditorLang.Get(EL.SECTION_CANVAS_POS), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_CANVAS_POS_HINT), iRoleEditorStyle.C_ACCENT_BLUE);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(distanceMultiplier, new GUIContent(iEditorLang.Get(EL.FIELD_DIST_MULT)));
            EditorGUILayout.PropertyField(verticalOffset, new GUIContent(iEditorLang.Get(EL.FIELD_VERT_OFFSET)));
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(baseAvatarHeight, new GUIContent(iEditorLang.Get(EL.FIELD_BASE_HEIGHT)));
            EditorGUILayout.PropertyField(baseCanvasScale, new GUIContent(iEditorLang.Get(EL.FIELD_BASE_SCALE)));
            float bh=baseAvatarHeight.floatValue, bs=baseCanvasScale.floatValue;
            if (bh > 0f) EditorGUILayout.LabelField($"1.50m→×{bs*(1.50f/bh):F3}  |  1.75m→×{bs*(1.75f/bh):F3}  |  1.90m→×{bs*(1.90f/bh):F3}", iRoleEditorStyle.SDim);
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(facingPlayer, new GUIContent(iEditorLang.Get(EL.FIELD_FACING_PLAYER)));
            if (facingPlayer.boolValue) { EditorGUI.indentLevel++; EditorGUILayout.PropertyField(flatRotation, new GUIContent(iEditorLang.Get(EL.FIELD_FLAT_ROTATION))); EditorGUI.indentLevel--; }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(4);

            // ── Debug ─────────────────────────────────────────────────────
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_DEBUG), iRoleEditorStyle.C_TEXT_DIM);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(debugMode, new GUIContent(iEditorLang.Get(EL.FIELD_DEBUG_MODE)));
            EditorGUILayout.EndVertical();

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }
    }
}
#endif
