// ============================================================================
// iRoleSystem v5.6 - Setup Window
// Author: ishiruhii
// Description: Ventana de instalación de módulos — Full i18n (ES / EN / JP / ZH)
// Path: ishiruhii/iRoleSystem Setup
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace ishiruhii.iRoleSystem.Editor
{
    public class iRoleSystemSetup : EditorWindow
    {
        private const string PREFABS_PATH            = "Assets/!!! ishiruhii/iRoleSystem/Data/Prefabs/";
        private const string SYSTEM_PREFAB_NAME      = "iRoleSystem Pro - v4.0.5";
        private const string MODULES_CONTAINER_NAME  = "# MODULOS";
        private const string RANKDISPLAY_PREFAB      = "iRankDisplay-Module";
        private const string OBJECTS_PREFAB          = "iObjects-Module";
        private const string PRISON_PREFAB           = "iPrison-Module";
        private const string PRIVATEZONE_PREFAB      = "iRoleZone-Module";

        private bool moduleRankDisplay = false;
        private bool moduleObjects     = false;
        private bool modulePrison      = false;
        private bool modulePrivateZone = false;

        private GUIStyle headerStyle, instructionStyle;
        private GUIStyle buttonSelectedStyle, buttonUnselectedStyle, installButtonStyle;
        private bool stylesInitialized = false;

        private readonly Color selectedColor    = new Color(0.2f, 0.7f, 0.3f, 1f);
        private readonly Color unselectedColor  = new Color(0.4f, 0.4f, 0.4f, 1f);
        private readonly Color installBtnColor  = new Color(0.2f, 0.5f, 0.9f, 1f);

        [MenuItem("ishiruhii/iRoleSystem Setup")]
        public static void ShowWindow()
        {
            iRoleSystemSetup w = GetWindow<iRoleSystemSetup>("iRoleSystem Setup");
            w.minSize = new Vector2(400, 500);
            w.maxSize = new Vector2(500, 600);
        }

        private void InitStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 22, alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold, wordWrap = true
            };

            instructionStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13, alignment = TextAnchor.MiddleCenter,
                wordWrap = true, fontStyle = FontStyle.Italic
            };

            buttonSelectedStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter, wordWrap = true, fixedHeight = 60
            };
            buttonSelectedStyle.normal.textColor = Color.white;
            buttonSelectedStyle.hover.textColor  = Color.white;
            buttonSelectedStyle.active.textColor = Color.white;

            buttonUnselectedStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 12, fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter, wordWrap = true, fixedHeight = 60
            };

            installButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16, fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter, fixedHeight = 45
            };
            installButtonStyle.normal.textColor = Color.white;

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            InitStyles();

            EditorGUILayout.Space(20);
            DrawHeader();
            EditorGUILayout.Space(20);
            DrawInstructions();
            EditorGUILayout.Space(25);
            DrawModuleButtons();
            EditorGUILayout.Space(30);

            GUILayout.Label("", GUI.skin.horizontalSlider);

            EditorGUILayout.Space(20);
            DrawInstallButton();
            EditorGUILayout.Space(20);
            DrawFooter();
        }

        private void DrawHeader()
        {
            EditorGUILayout.LabelField("iRoleSystem v5.6", headerStyle);
            EditorGUILayout.Space(1);
            EditorGUILayout.LabelField(
                iEditorLang.Get(EL.LABEL_SETUP_MODULES),
                new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 11 });
        }

        private void DrawInstructions()
        {
            EditorGUILayout.BeginVertical("helpbox");
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(
                iEditorLang.Get(EL.LABEL_SELECT_TO_INSTALL), instructionStyle);
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }

        private void DrawModuleButtons()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            moduleRankDisplay = DrawModuleToggleButton(
                iEditorLang.Get(EL.LABEL_MODULE_RANKDISP),
                moduleRankDisplay,
                iEditorLang.Get(EL.LABEL_MODULE_RANKDISP_DESC));
            GUILayout.Space(10);
            moduleObjects = DrawModuleToggleButton(
                iEditorLang.Get(EL.LABEL_MODULE_OBJECTS),
                moduleObjects,
                iEditorLang.Get(EL.LABEL_MODULE_OBJECTS_DESC));
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(15);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            modulePrison = DrawModuleToggleButton(
                iEditorLang.Get(EL.LABEL_MODULE_PRISON),
                modulePrison,
                iEditorLang.Get(EL.LABEL_MODULE_PRISON_DESC));
            GUILayout.Space(10);
            modulePrivateZone = DrawModuleToggleButton(
                iEditorLang.Get(EL.LABEL_MODULE_PZONE),
                modulePrivateZone,
                iEditorLang.Get(EL.LABEL_MODULE_PZONE_DESC));
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private bool DrawModuleToggleButton(string mainText, bool isSelected, string desc)
        {
            Color orig = GUI.backgroundColor;
            GUI.backgroundColor = isSelected ? selectedColor : unselectedColor;

            EditorGUILayout.BeginVertical(GUILayout.Width(170));
            GUIStyle style = isSelected ? buttonSelectedStyle : buttonUnselectedStyle;

            if (GUILayout.Button(mainText, style, GUILayout.Width(170), GUILayout.Height(60)))
                isSelected = !isSelected;

            EditorGUILayout.LabelField(desc, new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                fontSize = 9, wordWrap = true
            }, GUILayout.Width(170), GUILayout.Height(25));

            EditorGUILayout.EndVertical();
            GUI.backgroundColor = orig;
            return isSelected;
        }

        private void DrawInstallButton()
        {
            bool any = moduleRankDisplay || moduleObjects || modulePrison || modulePrivateZone;
            EditorGUI.BeginDisabledGroup(!any);

            Color orig = GUI.backgroundColor;
            GUI.backgroundColor = installBtnColor;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(50);
            if (GUILayout.Button(iEditorLang.Get(EL.BTN_INSTALL),
                installButtonStyle, GUILayout.Height(45)))
                InstallSelectedModules();
            GUILayout.Space(50);
            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = orig;
            EditorGUI.EndDisabledGroup();

            if (!any)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox(
                    iEditorLang.Get(EL.LABEL_SELECT_AT_LEAST), MessageType.Info);
            }
        }

        private void DrawFooter()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(
                iEditorLang.Get(EL.LABEL_SELECTED_MODULES), EditorStyles.boldLabel);

            string sel = "";
            if (moduleRankDisplay) sel += "• iRankDisplay\n";
            if (moduleObjects)     sel += "• iObjects\n";
            if (modulePrison)      sel += "• iPrison\n";
            if (modulePrivateZone) sel += "• iPrivateRoleZone\n";
            if (string.IsNullOrEmpty(sel)) sel = iEditorLang.Get(EL.LABEL_NONE);

            EditorGUILayout.LabelField(sel, EditorStyles.miniLabel);
            EditorGUILayout.EndVertical();
        }

        // ── Lógica de instalación ─────────────────────────────────────────
        private void InstallSelectedModules()
        {
            GameObject systemPrefab = FindSystemPrefabInScene();
            if (systemPrefab == null)
            {
                EditorUtility.DisplayDialog(
                    iEditorLang.Get(EL.DLG_ERROR_TITLE),
                    iEditorLang.Get(EL.DLG_ERROR_NO_PREFAB, SYSTEM_PREFAB_NAME),
                    iEditorLang.Get(EL.DLG_OK));
                return;
            }

            Transform modulesContainer = FindModulesContainer(systemPrefab);
            if (modulesContainer == null)
            {
                EditorUtility.DisplayDialog(
                    iEditorLang.Get(EL.DLG_ERROR_TITLE),
                    iEditorLang.Get(EL.DLG_ERROR_NO_CONTAINER,
                        MODULES_CONTAINER_NAME, SYSTEM_PREFAB_NAME),
                    iEditorLang.Get(EL.DLG_OK));
                return;
            }

            int installed = 0;
            string names  = "";

            if (moduleRankDisplay && InstallModule(RANKDISPLAY_PREFAB, modulesContainer))
            { installed++; names += "• iRankDisplay\n"; }
            if (moduleObjects && InstallModule(OBJECTS_PREFAB, modulesContainer))
            { installed++; names += "• iObjects\n"; }
            if (modulePrison && InstallModule(PRISON_PREFAB, modulesContainer))
            { installed++; names += "• iPrison\n"; }
            if (modulePrivateZone && InstallModule(PRIVATEZONE_PREFAB, modulesContainer))
            { installed++; names += "• iPrivateRoleZone\n"; }

            if (installed > 0)
            {
                EditorUtility.DisplayDialog(
                    iEditorLang.Get(EL.DLG_INSTALL_OK_TITLE),
                    iEditorLang.Get(EL.DLG_INSTALL_OK_BODY,
                        installed.ToString(), names),
                    iEditorLang.Get(EL.DLG_INSTALL_OK_BTN));

                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                    UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            }
            else
            {
                EditorUtility.DisplayDialog(
                    iEditorLang.Get(EL.DLG_NO_CHANGES_TITLE),
                    iEditorLang.Get(EL.DLG_NO_CHANGES_BODY),
                    iEditorLang.Get(EL.DLG_OK));
            }
        }

        private GameObject FindSystemPrefabInScene()
        {
            GameObject found = GameObject.Find(SYSTEM_PREFAB_NAME);
            if (found != null) return found;

            foreach (GameObject obj in FindObjectsOfType<GameObject>())
                if (obj.name.Contains("iRoleSystem") && obj.transform.parent == null)
                    return obj;

            return null;
        }

        private Transform FindModulesContainer(GameObject sys)
        {
            Transform t = sys.transform.Find(MODULES_CONTAINER_NAME);
            if (t != null) return t;

            foreach (Transform child in sys.transform)
                if (child.name == MODULES_CONTAINER_NAME || child.name.ToUpper() == "MODULOS")
                    return child;

            return null;
        }

        private bool InstallModule(string prefabName, Transform container)
        {
            foreach (Transform child in container)
                if (child.name.Contains(prefabName) || child.name == prefabName)
                {
                    Debug.Log($"[iRoleSystem Setup] Module '{prefabName}' already installed.");
                    return false;
                }

            string path = PREFABS_PATH + prefabName + ".prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
            {
                string[] guids = AssetDatabase.FindAssets(
                    prefabName + " t:prefab", new[] { "Assets/!!! ishiruhii" });
                if (guids.Length > 0)
                    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                        AssetDatabase.GUIDToAssetPath(guids[0]));
            }

            if (prefab == null)
            {
                Debug.LogWarning($"[iRoleSystem Setup] Prefab not found: {prefabName}");
                return false;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(container.parent);
            instance.transform.SetSiblingIndex(container.GetSiblingIndex() + 1);
            Undo.RegisterCreatedObjectUndo(instance, $"Install {prefabName}");
            Debug.Log($"[iRoleSystem Setup] Module '{prefabName}' installed.");
            return true;
        }
    }
}
#endif
