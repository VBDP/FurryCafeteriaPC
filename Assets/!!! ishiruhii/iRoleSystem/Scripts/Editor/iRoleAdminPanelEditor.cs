// ============================================================================
// iRoleSystem v5.8 - iRoleAdminPanel Editor  [REDESIGNED]
// Author: ishiruhii
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Modules.AdminPanel.iRoleAdminPanel))]
    public class iRoleAdminPanelEditor : UnityEditor.Editor
    {
        private SerializedProperty core, roleManager, roleDatabase;
        private SerializedProperty adminNames, maxSlotsPerAdmin, perAdminRoleIds, perAdminPrefabs, defaultButtonPrefab;
        private SerializedProperty allowedRoleIds, maxSlotsPerRole, perRoleButtonIds, perRoleButtonPrefabs;
        private SerializedProperty canvasRoot, roleButtonsContainer, targetPlayerInput, feedbackText, playerInfoText;
        private SerializedProperty playerSelectorCanvas, playerButtonsContainer, playerSelectorButtonPrefab;
        private SerializedProperty autoRefreshOnOpen, maxPlayerButtons;
        private SerializedProperty enableKeyToggle, pcToggleKey, questToggleButton;
        private SerializedProperty positionInFrontOfPlayer, canvasDistance, canvasHeightOffset, canvasExtraRotation, canvasScale;
        private SerializedProperty openSound, assignSound, errorSound, closeSound;
        private SerializedProperty maxButtons, clearInputOnClose, showPlayerInfoOnType, infoUpdateInterval;
        private SerializedProperty autoCloseAfterAssign, autoCloseDelay, debugMode, allowMasterAccess;

        private bool[] _adminFoldouts       = new bool[0];
        private bool[] _allowedRoleFoldouts = new bool[0];
        private bool _foldKey = true, _foldPos = false, _foldUI = false, _foldAudio = false, _foldOpts = false;

        private static readonly Color C_ADD = new Color(0.20f, 0.62f, 0.28f);
        private static readonly Color C_DEL = new Color(0.80f, 0.22f, 0.22f);

        private void OnEnable()
        {
            core                    = serializedObject.FindProperty("core");
            roleManager             = serializedObject.FindProperty("roleManager");
            roleDatabase            = serializedObject.FindProperty("roleDatabase");
            adminNames              = serializedObject.FindProperty("adminNames");
            maxSlotsPerAdmin        = serializedObject.FindProperty("maxSlotsPerAdmin");
            perAdminRoleIds         = serializedObject.FindProperty("perAdminRoleIds");
            perAdminPrefabs         = serializedObject.FindProperty("perAdminPrefabs");
            defaultButtonPrefab     = serializedObject.FindProperty("defaultButtonPrefab");
            canvasRoot              = serializedObject.FindProperty("canvasRoot");
            roleButtonsContainer    = serializedObject.FindProperty("roleButtonsContainer");
            targetPlayerInput       = serializedObject.FindProperty("targetPlayerInput");
            feedbackText            = serializedObject.FindProperty("feedbackText");
            playerInfoText          = serializedObject.FindProperty("playerInfoText");
            playerSelectorCanvas       = serializedObject.FindProperty("playerSelectorCanvas");
            playerButtonsContainer     = serializedObject.FindProperty("playerButtonsContainer");
            playerSelectorButtonPrefab = serializedObject.FindProperty("playerSelectorButtonPrefab");
            autoRefreshOnOpen          = serializedObject.FindProperty("autoRefreshOnOpen");
            maxPlayerButtons           = serializedObject.FindProperty("maxPlayerButtons");
            enableKeyToggle         = serializedObject.FindProperty("enableKeyToggle");
            pcToggleKey             = serializedObject.FindProperty("pcToggleKey");
            questToggleButton       = serializedObject.FindProperty("questToggleButton");
            positionInFrontOfPlayer = serializedObject.FindProperty("positionInFrontOfPlayer");
            canvasDistance          = serializedObject.FindProperty("canvasDistance");
            canvasHeightOffset      = serializedObject.FindProperty("canvasHeightOffset");
            canvasExtraRotation     = serializedObject.FindProperty("canvasExtraRotation");
            canvasScale             = serializedObject.FindProperty("canvasScale");
            openSound               = serializedObject.FindProperty("openSound");
            assignSound             = serializedObject.FindProperty("assignSound");
            errorSound              = serializedObject.FindProperty("errorSound");
            closeSound              = serializedObject.FindProperty("closeSound");
            maxButtons              = serializedObject.FindProperty("maxButtons");
            clearInputOnClose       = serializedObject.FindProperty("clearInputOnClose");
            showPlayerInfoOnType    = serializedObject.FindProperty("showPlayerInfoOnType");
            infoUpdateInterval      = serializedObject.FindProperty("infoUpdateInterval");
            autoCloseAfterAssign    = serializedObject.FindProperty("autoCloseAfterAssign");
            autoCloseDelay          = serializedObject.FindProperty("autoCloseDelay");
            debugMode               = serializedObject.FindProperty("debugMode");
            allowMasterAccess       = serializedObject.FindProperty("allowMasterAccess");
            allowedRoleIds          = serializedObject.FindProperty("allowedRoleIds");
            maxSlotsPerRole         = serializedObject.FindProperty("maxSlotsPerRole");
            perRoleButtonIds        = serializedObject.FindProperty("perRoleButtonIds");
            perRoleButtonPrefabs    = serializedObject.FindProperty("perRoleButtonPrefabs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            iRoleEditorStyle.DrawHeader("★", "iRoleAdminPanel",
                iEditorLang.Get(EL.SECTION_ADMIN_LIST), iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.Space(6);

            DrawSystemRefs();
            EditorGUILayout.Space(4);
            DrawGlobalSettings();
            EditorGUILayout.Space(4);
            DrawAdminList();
            EditorGUILayout.Space(4);
            DrawAllowedRoleList();
            EditorGUILayout.Space(4);

            _foldKey = DrawFoldSection(_foldKey, "☰  " + iEditorLang.Get(EL.LABEL_KEY_CONTROL), iRoleEditorStyle.C_ACCENT_BLUE);
            if (_foldKey) DrawKeyControl();

            _foldPos = DrawFoldSection(_foldPos, "⊡  " + iEditorLang.Get(EL.SECTION_CANVAS_POS), iRoleEditorStyle.C_ACCENT_CYAN);
            if (_foldPos) DrawPositioning();

            _foldUI = DrawFoldSection(_foldUI, "◈  " + iEditorLang.Get(EL.SECTION_UI), iRoleEditorStyle.C_ACCENT_GREEN);
            if (_foldUI) DrawUI();

            _foldAudio = DrawFoldSection(_foldAudio, "♪  " + iEditorLang.Get(EL.SECTION_AUDIO), iRoleEditorStyle.C_ACCENT_PURPLE);
            if (_foldAudio) DrawAudio();

            _foldOpts = DrawFoldSection(_foldOpts, "◉  " + iEditorLang.Get(EL.SECTION_OPTIONS), iRoleEditorStyle.C_TEXT_DIM);
            if (_foldOpts) DrawOptions();

            EditorGUILayout.Space(4);
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_DEBUG), iRoleEditorStyle.C_TEXT_DIM);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(debugMode, new GUIContent(iEditorLang.Get(EL.FIELD_DEBUG_MODE)));
            EditorGUILayout.EndVertical();

            // FIX LAG: Condicional
            if (serializedObject.ApplyModifiedProperties())
                EditorUtility.SetDirty(target);
        }

        private bool DrawFoldSection(bool state, string title, Color accent)
        {
            iRoleEditorStyle.DrawSectionHeader(title, accent);
            return EditorGUILayout.Foldout(state, "", true, iRoleEditorStyle.SFoldout);
        }

        private void DrawSystemRefs()
        {
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
                Core.iRoleSystemCore cr = (Core.iRoleSystemCore)core.objectReferenceValue;
                if (roleManager.objectReferenceValue == null && cr.playerRoleManager != null)
                    roleManager.objectReferenceValue = cr.playerRoleManager;
                if (roleDatabase.objectReferenceValue == null && cr.roleDatabase != null)
                    roleDatabase.objectReferenceValue = cr.roleDatabase;
            }
            EditorGUILayout.PropertyField(roleManager, new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_MANAGER)));
            EditorGUILayout.PropertyField(roleDatabase, new GUIContent(iEditorLang.Get(EL.FIELD_ROLE_DATABASE)));
            EditorGUILayout.EndVertical();
        }

        private void DrawGlobalSettings()
        {
            iRoleEditorStyle.DrawSectionHeader("◉  " + iEditorLang.Get(EL.SECTION_GLOBAL_CFG), iRoleEditorStyle.C_ACCENT_CYAN);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUI.BeginChangeCheck();
            int newMax = EditorGUILayout.IntSlider(
                new GUIContent(iEditorLang.Get(EL.FIELD_MAX_SLOTS), iEditorLang.Get(EL.MSG_MAX_SLOTS_ADMIN_WARN)),
                maxSlotsPerAdmin.intValue, 1, 16);
            if (EditorGUI.EndChangeCheck())
            {
                if (adminNames.arraySize > 0)
                {
                    bool ok = EditorUtility.DisplayDialog(
                        iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_TITLE),
                        iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_BODY),
                        iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_YES),
                        iEditorLang.Get(EL.DLG_CHANGE_MAX_SLOTS_NO));
                    if (ok) { maxSlotsPerAdmin.intValue = newMax; serializedObject.ApplyModifiedProperties(); RebuildArrays(adminNames.arraySize, newMax); serializedObject.Update(); }
                }
                else maxSlotsPerAdmin.intValue = newMax;
            }
            EditorGUILayout.PropertyField(defaultButtonPrefab, new GUIContent(iEditorLang.Get(EL.FIELD_FALLBACK_PREFAB)));
            GUI.color = iRoleEditorStyle.C_TEXT_DIM;
            EditorGUILayout.LabelField("⚡ El texto (TextMeshPro UI) del prefab se configura automáticamente con el nombre del rol.", EditorStyles.miniLabel);
            GUI.color = Color.white;
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(allowMasterAccess, new GUIContent("Permitir acceso al Master", "Si está activo, el Master de la instancia puede abrir el panel aunque no esté en la lista de admins. Verá todos los roles disponibles."));
            if (allowMasterAccess.boolValue)
            {
                GUI.color = iRoleEditorStyle.C_ACCENT_ORANGE;
                EditorGUILayout.LabelField("♛ El Master podrá usar este panel con todos los roles.", EditorStyles.miniLabel);
                GUI.color = Color.white;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdminList()
        {
            iRoleEditorStyle.DrawSectionHeader("★  " + iEditorLang.Get(EL.SECTION_ADMIN_LIST), iRoleEditorStyle.C_ACCENT_ORANGE);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            int adminCount = adminNames.arraySize;
            EnsureFoldoutsSize(adminCount);
            if (adminCount == 0)
                iRoleEditorStyle.DrawWarning(iEditorLang.Get(EL.MSG_NO_ADMIN_CFG), iRoleEditorStyle.C_TEXT_DIM);

            Core.iRoleDatabase roleDb = roleDatabase.objectReferenceValue as Core.iRoleDatabase;
            string[] roleOptions = BuildRoleDropdownOptions(roleDb);
            for (int a = 0; a < adminCount; a++)
                DrawAdminEntry(a, roleDb, roleOptions);

            EditorGUILayout.Space(6);
            if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_ADD_ADMIN), C_ADD, GUILayout.Height(24)))
            { AddAdmin(maxSlotsPerAdmin.intValue); serializedObject.Update(); }
            EditorGUILayout.EndVertical();
        }

        private void DrawAdminEntry(int a, Core.iRoleDatabase roleDb, string[] roleOptions)
        {
            int start  = a * maxSlotsPerAdmin.intValue;
            int active = CountActiveSlots(start, maxSlotsPerAdmin.intValue);

            EditorGUILayout.Space(3);
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
            EditorGUILayout.BeginHorizontal();
            _adminFoldouts[a] = EditorGUILayout.Foldout(_adminFoldouts[a],
                $"Admin #{a + 1}: {adminNames.GetArrayElementAtIndex(a).stringValue}  ({active}/{maxSlotsPerAdmin.intValue} slots)",
                true, iRoleEditorStyle.SFoldout);
            if (iRoleEditorStyle.ColorButton("✖", C_DEL, GUILayout.Width(24)))
            {
                string aname = adminNames.GetArrayElementAtIndex(a).stringValue;
                if (EditorUtility.DisplayDialog(iEditorLang.Get(EL.DLG_DELETE_ADMIN_TITLE),
                    iEditorLang.Get(EL.DLG_DELETE_ADMIN_BODY, aname),
                    iEditorLang.Get(EL.DLG_DELETE_BTN), iEditorLang.Get(EL.DLG_CANCEL_BTN)))
                { RemoveAdmin(a, maxSlotsPerAdmin.intValue); serializedObject.Update(); EditorGUILayout.EndHorizontal(); EditorGUILayout.EndVertical(); return; }
            }
            EditorGUILayout.EndHorizontal();

            if (_adminFoldouts[a])
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(adminNames.GetArrayElementAtIndex(a), new GUIContent(iEditorLang.Get(EL.FIELD_AUTH_USERS)));
                EditorGUILayout.Space(4);
                EditorGUILayout.LabelField(iEditorLang.Get(EL.SECTION_ROLES) + ":", iRoleEditorStyle.SSectionLabel);
                for (int s = 0; s < maxSlotsPerAdmin.intValue; s++)
                {
                    int idx = start + s;
                    if (idx >= perAdminRoleIds.arraySize) break;
                    SerializedProperty rp = perAdminRoleIds.GetArrayElementAtIndex(idx);
                    SerializedProperty pp = perAdminPrefabs.GetArrayElementAtIndex(idx);
                    if (rp.intValue < 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(iEditorLang.Get(EL.MSG_VOID_SLOT, (s+1).ToString()), GUILayout.Width(150));
                        if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_ADD_SLOT), C_ADD, GUILayout.Width(120))) rp.intValue = 0;
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"Slot {s+1}", GUILayout.Width(60));
                        int ci = rp.intValue + 1;
                        int ni = EditorGUILayout.Popup(ci, roleOptions);
                        if (ni != ci) rp.intValue = ni - 1;
                        if (iRoleEditorStyle.ColorButton("✖", C_DEL, GUILayout.Width(24))) { rp.intValue = -1; pp.objectReferenceValue = null; }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(pp, new GUIContent(iEditorLang.Get(EL.FIELD_DISPLAY_PREFAB)));
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawKeyControl()
        {
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(enableKeyToggle, new GUIContent(iEditorLang.Get(EL.FIELD_ENABLE_KEY_TOGGLE)));
            if (enableKeyToggle.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(pcToggleKey, new GUIContent(iEditorLang.Get(EL.FIELD_PC_KEY)));
                EditorGUILayout.LabelField(iEditorLang.Get(EL.LABEL_QUEST_BUTTON), iRoleEditorStyle.SSectionLabel);
                string[] qbtns = { iEditorLang.Get(EL.LABEL_NONE), "Button 1 (Left Trigger)", "Button 2 (Right Trigger)", "Button 3 (A/X)", "Button 4 (B/Y)" };
                questToggleButton.intValue = EditorGUILayout.Popup(iEditorLang.Get(EL.FIELD_QUEST_BUTTON), questToggleButton.intValue, qbtns);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawPositioning()
        {
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(positionInFrontOfPlayer, new GUIContent(iEditorLang.Get(EL.FIELD_POS_IN_FRONT)));
            if (positionInFrontOfPlayer.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(canvasDistance, new GUIContent(iEditorLang.Get(EL.FIELD_CANVAS_DIST)));
                EditorGUILayout.PropertyField(canvasHeightOffset, new GUIContent(iEditorLang.Get(EL.FIELD_HEIGHT_OFFSET2)));
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(canvasExtraRotation, new GUIContent(iEditorLang.Get(EL.FIELD_EXTRA_ROT)));
                Vector3 rot = canvasExtraRotation.vector3Value;
                string hint = Mathf.Approximately(rot.x,0f)&&Mathf.Approximately(rot.y,180f)&&Mathf.Approximately(rot.z,0f) ? "✔ Y=180°"
                    : rot == Vector3.zero ? "⚠ Sin Rotación" : "↻ Custom";
                EditorGUILayout.LabelField(hint, iRoleEditorStyle.SDim);
                EditorGUILayout.BeginHorizontal();
                if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_RESET_ROT), iRoleEditorStyle.C_ACCENT_BLUE, GUILayout.Height(20)))
                    canvasExtraRotation.vector3Value = new Vector3(0f,180f,0f);
                if (iRoleEditorStyle.ColorButton(iEditorLang.Get(EL.BTN_NO_ROT), iRoleEditorStyle.C_TEXT_DIM, GUILayout.Height(20)))
                    canvasExtraRotation.vector3Value = Vector3.zero;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(4);
                EditorGUILayout.PropertyField(canvasScale, new GUIContent(iEditorLang.Get(EL.FIELD_CANVAS_SCALE)));
                EditorGUILayout.BeginHorizontal();
                float[] scalePresets = { 0.5f, 0.75f, 1f, 1.5f };
                foreach (float sp in scalePresets)
                    if (iRoleEditorStyle.ColorButton($"×{sp}", iRoleEditorStyle.C_ACCENT_CYAN, GUILayout.Height(18)))
                        canvasScale.vector3Value = new Vector3(sp,sp,sp);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawUI()
        {
            // ── Panel principal ───────────────────────────────────────────
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.LabelField("Panel Principal", iRoleEditorStyle.SSectionLabel);
            EditorGUILayout.PropertyField(canvasRoot,           new GUIContent(iEditorLang.Get(EL.FIELD_CANVAS_ROOT)));
            EditorGUILayout.PropertyField(roleButtonsContainer, new GUIContent(iEditorLang.Get(EL.FIELD_BTN_CONTAINER)));
            EditorGUILayout.PropertyField(targetPlayerInput,    new GUIContent(iEditorLang.Get(EL.FIELD_TARGET_INPUT)));
            EditorGUILayout.PropertyField(feedbackText,         new GUIContent(iEditorLang.Get(EL.FIELD_FEEDBACK_TEXT)));
            EditorGUILayout.PropertyField(playerInfoText,       new GUIContent(iEditorLang.Get(EL.FIELD_PLAYER_INFO_TXT)));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6);

            // ── Selector de Jugadores ─────────────────────────────────────
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            GUI.color = new Color(0.5f, 0.85f, 1f);
            EditorGUILayout.LabelField("👥  Selector de Jugadores", iRoleEditorStyle.SSectionLabel);
            GUI.color = Color.white;
            iRoleEditorStyle.DrawWarning(
                "Canvas secundario con botones generados automáticamente a partir de los jugadores en instancia. " +
                "Al hacer clic en uno, se rellena el campo de nombre del input.",
                new Color(0.3f, 0.6f, 0.9f));
            EditorGUILayout.Space(4);

            EditorGUILayout.PropertyField(playerSelectorCanvas,
                new GUIContent("Canvas Selector", "GameObject del canvas secundario donde aparecen los botones de jugadores."));
            EditorGUILayout.PropertyField(playerButtonsContainer,
                new GUIContent("Contenedor Botones", "Transform padre donde se instancian los botones de jugadores."));
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(playerSelectorButtonPrefab,
                new GUIContent("Prefab Botón Jugador",
                    "Prefab con iPlayerSelectorButton. Si está vacío se usa el defaultButtonPrefab.\n" +
                    "El texto (TMP) se configura automáticamente con el nombre del jugador."));
            if (playerSelectorButtonPrefab.objectReferenceValue == null)
            {
                GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                EditorGUILayout.LabelField("⚡ Usando defaultButtonPrefab como fallback.", EditorStyles.miniLabel);
                GUI.color = Color.white;
            }
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(autoRefreshOnOpen,
                new GUIContent("Refrescar al Abrir",
                    "Si está activo, los botones de jugadores se regeneran cada vez que se abre el selector."));
            EditorGUILayout.PropertyField(maxPlayerButtons,
                new GUIContent("Máx. Botones Jugadores", "Número máximo de botones de jugador a generar (máx. 80)."));
            if (maxPlayerButtons.intValue > 80)  maxPlayerButtons.intValue = 80;
            if (maxPlayerButtons.intValue < 1)   maxPlayerButtons.intValue = 1;

            bool selectorConfigured = playerSelectorCanvas.objectReferenceValue != null &&
                                      playerButtonsContainer.objectReferenceValue != null;
            EditorGUILayout.Space(4);
            if (selectorConfigured)
            {
                GUI.color = iRoleEditorStyle.C_ACCENT_GREEN;
                EditorGUILayout.LabelField("✔ Selector configurado. Llama a OpenPlayerSelector() desde un botón del panel.", EditorStyles.miniLabel);
            }
            else
            {
                GUI.color = iRoleEditorStyle.C_TEXT_DIM;
                EditorGUILayout.LabelField("Asigna el Canvas y el Contenedor para activar el selector.", EditorStyles.miniLabel);
            }
            GUI.color = Color.white;
            EditorGUILayout.EndVertical();
        }

        private void DrawAudio()
        {
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(openSound, new GUIContent(iEditorLang.Get(EL.FIELD_OPEN_SOUND)));
            EditorGUILayout.PropertyField(assignSound, new GUIContent(iEditorLang.Get(EL.FIELD_ASSIGN_SOUND)));
            EditorGUILayout.PropertyField(errorSound, new GUIContent(iEditorLang.Get(EL.FIELD_ERROR_SOUND)));
            EditorGUILayout.PropertyField(closeSound, new GUIContent(iEditorLang.Get(EL.FIELD_CLOSE_SOUND)));
            EditorGUILayout.EndVertical();
        }

        private void DrawOptions()
        {
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
            EditorGUILayout.PropertyField(maxButtons, new GUIContent(iEditorLang.Get(EL.FIELD_MAX_BUTTONS)));
            EditorGUILayout.PropertyField(clearInputOnClose, new GUIContent(iEditorLang.Get(EL.FIELD_CLEAR_ON_CLOSE)));
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(showPlayerInfoOnType, new GUIContent(iEditorLang.Get(EL.FIELD_INFO_ON_TYPE)));
            EditorGUILayout.PropertyField(infoUpdateInterval, new GUIContent(iEditorLang.Get(EL.FIELD_INFO_INTERVAL)));
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(autoCloseAfterAssign, new GUIContent(iEditorLang.Get(EL.FIELD_AUTO_CLOSE)));
            if (autoCloseAfterAssign.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(autoCloseDelay, new GUIContent(iEditorLang.Get(EL.FIELD_AUTO_CLOSE_DELAY)));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private int CountActiveSlots(int start, int max)
        {
            int c = 0;
            for (int s = 0; s < max; s++) { int idx = start+s; if (idx >= perAdminRoleIds.arraySize) break; if (perAdminRoleIds.GetArrayElementAtIndex(idx).intValue >= 0) c++; }
            return c;
        }

        private void AddAdmin(int max)
        {
            Modules.AdminPanel.iRoleAdminPanel t = (Modules.AdminPanel.iRoleAdminPanel)target;
            int old = t.adminNames != null ? t.adminNames.Length : 0, @new = old + 1;
            string[] n = new string[@new]; if (t.adminNames != null) t.adminNames.CopyTo(n,0); n[old]="NewAdmin"; t.adminNames=n;
            int nsz=@new*max; int[] ids=new int[nsz]; GameObject[] pf=new GameObject[nsz];
            if (t.perAdminRoleIds!=null) System.Array.Copy(t.perAdminRoleIds,ids,Mathf.Min(t.perAdminRoleIds.Length,nsz));
            if (t.perAdminPrefabs!=null) System.Array.Copy(t.perAdminPrefabs,pf,Mathf.Min(t.perAdminPrefabs.Length,nsz));
            for (int s=0;s<max;s++) ids[old*max+s]=-1; t.perAdminRoleIds=ids; t.perAdminPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RemoveAdmin(int a, int max)
        {
            Modules.AdminPanel.iRoleAdminPanel t=(Modules.AdminPanel.iRoleAdminPanel)target;
            int old=t.adminNames!=null?t.adminNames.Length:0; if(a<0||a>=old)return;
            string[] n=new string[old-1]; for(int i=0,j=0;i<old;i++){if(i==a)continue;n[j++]=t.adminNames[i];} t.adminNames=n;
            int rs=a*max,re=rs+max,sz=t.perAdminRoleIds!=null?t.perAdminRoleIds.Length:0,nsz=Mathf.Max(0,sz-max);
            int[] ids=new int[nsz]; GameObject[] pf=new GameObject[nsz]; int wi=0;
            for(int i=0;i<sz;i++){if(i>=rs&&i<re)continue;if(wi>=nsz)break;ids[wi]=t.perAdminRoleIds[i];pf[wi]=t.perAdminPrefabs!=null&&i<t.perAdminPrefabs.Length?t.perAdminPrefabs[i]:null;wi++;}
            t.perAdminRoleIds=ids;t.perAdminPrefabs=pf;EditorUtility.SetDirty(t);
        }

        private void RebuildArrays(int cnt, int max)
        {
            Modules.AdminPanel.iRoleAdminPanel t=(Modules.AdminPanel.iRoleAdminPanel)target;
            int sz=cnt*max; t.perAdminRoleIds=new int[sz]; t.perAdminPrefabs=new GameObject[sz];
            for(int i=0;i<sz;i++) t.perAdminRoleIds[i]=-1; EditorUtility.SetDirty(t);
        }

        private string[] BuildRoleDropdownOptions(Core.iRoleDatabase db)
        {
            if(db==null||db.roleNames==null||db.roleNames.Length==0) return new[]{iEditorLang.Get(EL.MSG_EMPTY_DROPDOWN)};
            string[] opts=new string[db.roleNames.Length+1]; opts[0]=iEditorLang.Get(EL.MSG_EMPTY_DROPDOWN);
            for(int i=0;i<db.roleNames.Length;i++) opts[i+1]=$"[{i}] {db.roleNames[i]}"; return opts;
        }

        private void EnsureFoldoutsSize(int cnt)
        {
            if(_adminFoldouts==null||_adminFoldouts.Length!=cnt){bool[] n=new bool[cnt];int cl=_adminFoldouts!=null?Mathf.Min(_adminFoldouts.Length,cnt):0;for(int i=0;i<cl;i++)n[i]=_adminFoldouts[i];_adminFoldouts=n;}
        }

        private void EnsureAllowedFoldoutsSize(int cnt)
        { if(_allowedRoleFoldouts==null||_allowedRoleFoldouts.Length!=cnt){bool[] nf=new bool[cnt];int cl=_allowedRoleFoldouts!=null?Mathf.Min(_allowedRoleFoldouts.Length,cnt):0;for(int i=0;i<cl;i++)nf[i]=_allowedRoleFoldouts[i];_allowedRoleFoldouts=nf;} }

        private void DrawAllowedRoleList()
        {
            iRoleEditorStyle.DrawSectionHeader("◆  Roles con Acceso al Panel", new Color(0.6f, 0.3f, 0.9f));
            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);

            int roleCount = allowedRoleIds.arraySize;
            int maxSlots  = Mathf.Max(1, maxSlotsPerRole.intValue);
            EnsureAllowedFoldoutsSize(roleCount);

            // Slot slider
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
            EditorGUILayout.Space(4);

            int expected = roleCount * maxSlots;
            if (perRoleButtonIds.arraySize != expected || perRoleButtonPrefabs.arraySize != expected)
            {
                iRoleEditorStyle.DrawWarning("Arrays de botones por rol desincronizados.", iRoleEditorStyle.C_ACCENT_ORANGE);
                if (iRoleEditorStyle.ColorButton("Reparar", iRoleEditorStyle.C_ACCENT_ORANGE, GUILayout.Height(22)))
                { serializedObject.ApplyModifiedProperties(); RebuildAllowedRoleArrays(roleCount, maxSlots); serializedObject.Update(); }
                EditorGUILayout.Space(4);
            }

            Core.iRoleDatabase roleDb = roleDatabase.objectReferenceValue as Core.iRoleDatabase;
            string[] roleOptions = BuildRoleDropdownOptions(roleDb);
            Color C_ROLE_HEADER = new Color(0.22f, 0.12f, 0.40f);
            bool removed = false;

            for (int r = 0; r < roleCount; r++)
            {
                if (removed) break;
                int  start  = r * maxSlots;
                int  active = CountActiveAllowedSlots(start, maxSlots);
                int  thisRoleId = allowedRoleIds.GetArrayElementAtIndex(r).intValue;
                string thisRoleName = (roleDb!=null&&roleDb.roleNames!=null&&thisRoleId>=0&&thisRoleId<roleDb.roleNames.Length)
                    ? roleDb.roleNames[thisRoleId] : (thisRoleId>=0?$"Rol #{thisRoleId}":"Sin Rol");

                EditorGUILayout.Space(3);
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.BeginHorizontal();
                _allowedRoleFoldouts[r] = EditorGUILayout.Foldout(_allowedRoleFoldouts[r],
                    $"◆ Rol: {thisRoleName}  ({active}/{maxSlots} botones)", true, iRoleEditorStyle.SFoldout);

                // Dropdown rol
                if (roleDb != null && roleDb.roleNames != null && roleDb.roleNames.Length > 0)
                {
                    int sel = thisRoleId + 1; if(sel<0||sel>=roleOptions.Length)sel=0;
                    int newSel = EditorGUILayout.Popup(sel, roleOptions, GUILayout.Width(140));
                    allowedRoleIds.GetArrayElementAtIndex(r).intValue = newSel - 1;
                }

                if (iRoleEditorStyle.ColorButton("✖", C_DEL, GUILayout.Width(24)))
                {
                    if (EditorUtility.DisplayDialog("Eliminar rol de acceso",
                        $"¿Eliminar acceso para '{thisRoleName}'?", "Eliminar", "Cancelar"))
                    { RemoveAllowedRole(r, maxSlots); serializedObject.Update(); EditorGUILayout.EndHorizontal(); EditorGUILayout.EndVertical(); removed = true; continue; }
                }
                EditorGUILayout.EndHorizontal();

                if (_allowedRoleFoldouts[r] && !removed)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space(4);
                    EditorGUILayout.LabelField("Botones disponibles para este rol:", iRoleEditorStyle.SSectionLabel);

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

                            EditorGUILayout.BeginVertical(iRoleEditorStyle.SBox);
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField($"Btn {s+1}", GUILayout.Width(60));
                            int ci = rp.intValue + 1;
                            int ni = EditorGUILayout.Popup(ci, roleOptions);
                            if (ni != ci) rp.intValue = ni - 1;
                            Color bp2 = GUI.backgroundColor;
                            GUI.backgroundColor = C_DEL * new Color(1,1,1,0.7f);
                            if (iRoleEditorStyle.ColorButton("✖", C_DEL, GUILayout.Width(24))) { rp.intValue=-1; if(pp!=null)pp.objectReferenceValue=null; }
                            GUI.backgroundColor = bp2;
                            EditorGUILayout.EndHorizontal();
                            if (pp != null)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.PropertyField(pp, new GUIContent("Prefab (opc.)"));
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();
                        }

                        if (!hasAny) iRoleEditorStyle.DrawWarning("Sin botones configurados para este rol.", iRoleEditorStyle.C_TEXT_DIM);
                        EditorGUILayout.Space(4);
                        if (active < maxSlots)
                        {
                            if (iRoleEditorStyle.ColorButton("+ Añadir Botón", new Color(0.6f, 0.3f, 0.9f), GUILayout.Height(22)))
                                AddSlotToAllowedRole(r, start, maxSlots);
                        }
                        else iRoleEditorStyle.DrawWarning($"Máximo de {maxSlots} botones alcanzado.", iRoleEditorStyle.C_ACCENT_ORANGE);
                    }
                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space(4);
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(6);
            if (iRoleEditorStyle.ColorButton("+ Añadir Rol con Acceso", new Color(0.5f, 0.25f, 0.8f), GUILayout.Height(24)))
            { AddAllowedRole(maxSlots); serializedObject.Update(); }

            if (roleCount > 0)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginVertical(iRoleEditorStyle.SBoxDark);
                EditorGUILayout.LabelField("Resumen", iRoleEditorStyle.SSectionLabel);
                int total = 0; for (int r=0; r<roleCount; r++) total += CountActiveAllowedSlots(r*maxSlots, maxSlots);
                EditorGUILayout.LabelField($"Roles con acceso: {roleCount}", iRoleEditorStyle.SDim);
                EditorGUILayout.LabelField($"Total botones configurados: {total}", iRoleEditorStyle.SDim);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        // ── Helpers allowedRole ───────────────────────────────────────────
        private int CountActiveAllowedSlots(int start, int max)
        { int c=0; for(int s=0;s<max;s++){int idx=start+s;if(idx>=perRoleButtonIds.arraySize)break;if(perRoleButtonIds.GetArrayElementAtIndex(idx).intValue>=0)c++;} return c; }

        private void AddSlotToAllowedRole(int r, int start, int max)
        { for(int s=0;s<max;s++){int idx=start+s;if(idx>=perRoleButtonIds.arraySize)break;if(perRoleButtonIds.GetArrayElementAtIndex(idx).intValue<0){perRoleButtonIds.GetArrayElementAtIndex(idx).intValue=0;_allowedRoleFoldouts[r]=true;return;}} }

        private void AddAllowedRole(int maxSlots)
        {
            Modules.AdminPanel.iRoleAdminPanel t=(Modules.AdminPanel.iRoleAdminPanel)target;
            int old=t.allowedRoleIds!=null?t.allowedRoleIds.Length:0,@new=old+1;
            int[] ids=new int[@new]; if(t.allowedRoleIds!=null)t.allowedRoleIds.CopyTo(ids,0); ids[old]=-1; t.allowedRoleIds=ids;
            int newSz=@new*maxSlots;
            int[] btnIds=new int[newSz]; if(t.perRoleButtonIds!=null)System.Array.Copy(t.perRoleButtonIds,btnIds,Mathf.Min(t.perRoleButtonIds.Length,newSz));
            for(int s=0;s<maxSlots;s++)btnIds[old*maxSlots+s]=-1; t.perRoleButtonIds=btnIds;
            GameObject[] pf=new GameObject[newSz]; if(t.perRoleButtonPrefabs!=null)System.Array.Copy(t.perRoleButtonPrefabs,pf,Mathf.Min(t.perRoleButtonPrefabs.Length,newSz));
            t.perRoleButtonPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RemoveAllowedRole(int r, int maxSlots)
        {
            Modules.AdminPanel.iRoleAdminPanel t=(Modules.AdminPanel.iRoleAdminPanel)target;
            int old=t.allowedRoleIds!=null?t.allowedRoleIds.Length:0; if(r<0||r>=old)return;
            int[] ids=new int[old-1]; for(int i=0,j=0;i<old;i++){if(i==r)continue;ids[j++]=t.allowedRoleIds[i];} t.allowedRoleIds=ids;
            int rs=r*maxSlots,re=rs+maxSlots,sz=t.perRoleButtonIds!=null?t.perRoleButtonIds.Length:0,nsz=Mathf.Max(0,sz-maxSlots);
            int[] btnIds=new int[nsz]; GameObject[] pf=new GameObject[nsz]; int wi=0;
            for(int i=0;i<sz;i++){if(i>=rs&&i<re)continue;if(wi>=nsz)break;btnIds[wi]=t.perRoleButtonIds[i];pf[wi]=t.perRoleButtonPrefabs!=null&&i<t.perRoleButtonPrefabs.Length?t.perRoleButtonPrefabs[i]:null;wi++;}
            t.perRoleButtonIds=btnIds; t.perRoleButtonPrefabs=pf; EditorUtility.SetDirty(t);
        }

        private void RebuildAllowedRoleArrays(int cnt, int maxSlots)
        {
            Modules.AdminPanel.iRoleAdminPanel t=(Modules.AdminPanel.iRoleAdminPanel)target;
            int sz=cnt*maxSlots; t.perRoleButtonIds=new int[sz]; t.perRoleButtonPrefabs=new GameObject[sz];
            for(int i=0;i<sz;i++)t.perRoleButtonIds[i]=-1; EditorUtility.SetDirty(t);
        }
    }
}
#endif
