using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

using System;
using System.Linq;
using UnityEngine.Rendering;
using UnityEditor;
using UdonSharpEditor;

using VRC.SDKBase.Editor.BuildPipeline;
using VRC.SDK3.Data;
using VRC.SDK3.Components;
using VRC.Udon.Serialization.OdinSerializer.Utilities;
using UnityEditor.SceneManagement;

namespace VRCCoffeeSet
{
    [CustomEditor(typeof(MainController))]
    public class MainController_Editor : Editor
    {
        public MainController controller;
        public string pathRoot;
        
        bool isOnScene;
        /// <summary> 0_Ade, 1_Boba </summary>
        bool[] detectDLC = new bool[2];
        bool[] detectSkin = new bool[1];

        List<ParticleSystemRenderer> particles = new List<ParticleSystemRenderer>();

        bool fold_variable, fold_save, fold_soundRange, fold_particle, fold_lang;
        bool[] status_fold = new bool[5];

        GUIStyle styleInfo = new GUIStyle();
        GUIStyle styleInit = new GUIStyle();

        string[] m_langMain, m_langAde, m_langBoba;

        bool isCtrl, isDebug;

        private void Awake()
        {
            styleInfo.fontSize = 13;
            styleInfo.normal.textColor = Color.white;
            styleInfo.alignment = styleInit.alignment = TextAnchor.MiddleCenter;
             
            // Get Controller, check validation
            controller = target as MainController;
            isOnScene = controller.gameObject.scene.IsValid() && PrefabStageUtility.GetCurrentPrefabStage() == null;
            if (isDebug) isOnScene = true;
            if (!isOnScene) return;
            
            // unpack if controller is the prefab but not completely
            if (PrefabUtility.IsPartOfPrefabInstance(controller.gameObject))
                PrefabUtility.UnpackPrefabInstance(controller.gameObject, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            particles = controller.GetComponentsInChildren<ParticleSystemRenderer>().ToList();

            var thisScript = MonoScript.FromScriptableObject(this);
            pathRoot = AssetDatabase.GetAssetPath(thisScript).Replace($"/Assets/Scripts/Editor/{thisScript.name}.cs", "");
            detectDLC[0] = AssetDatabase.IsValidFolder($"{pathRoot}/Assets_Ade");
            detectDLC[1] = AssetDatabase.IsValidFolder($"{pathRoot}/Assets_BubbleTea");
            detectSkin[0] = AssetDatabase.IsValidFolder($"{pathRoot}/Assets_SkinWhiteWood");

            m_langMain = MainMethod.GetJsonsAtPath($"{pathRoot}/Assets/Languages", addFileName: true);
            if (detectDLC[0]) m_langAde = MainMethod.GetJsonsAtPath($"{pathRoot}/Assets_Ade/Languages");
            if (detectDLC[1]) m_langBoba = MainMethod.GetJsonsAtPath($"{pathRoot}/Assets_BubbleTea/Languages");

            controller.audios = controller.transform.GetComponentsInChildren<AudioSource>();
            controller.transform.GetComponentsInChildren<VRCSpatialAudioSource>(true).ForEach(item => item.UseAudioSourceVolumeCurve = true);
            UpdateAudios();
        }

        public override void OnInspectorGUI()
        {
            #region Input

            if (!isDebug)
            {
                Event e = Event.current;
                if (e.type == EventType.KeyDown)
                {
                    if (!isCtrl && e.keyCode == KeyCode.LeftControl) isCtrl = true;
                    if (isCtrl && e.keyCode == KeyCode.D)
                    {
                        isDebug = true;
                        Awake();
                        MainMethod.DebugLog("Debug Mode Enabled");
                    }
                }
                else if (e.type == EventType.KeyUp)
                {
                    if (isCtrl && e.keyCode == KeyCode.LeftControl) isCtrl = false;
                }
            }

            #endregion

            #region Information

            EditorGUILayout.Space(15);
            GUILayout.Label(
                "Coffee Set by MiniGreen417" + "\n" +
                "Last Update : 2025-01-24" + "\n" +
                "ⓒ 2025. (MiniGreen417) all rights reserved." + "\n" +
                "Do not use for commercial purposes."
                , styleInfo
            );
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Instruction Document", GUILayout.Width(200), GUILayout.Height(25)))
                Application.OpenURL("https://docs.google.com/document/d/1qk2pvZLmfb3-kat5gqp-OG7Tz3IFpAKMIhr1lGNIt5U");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(15);

            if (!isOnScene)
            {
                EditorGUILayout.HelpBox("Drag and drop onto hierarchy.", MessageType.Info);
                return;
            }
            else if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Can't edit while playing.", MessageType.Info);
                return;
            }

            #endregion

            #region DLC

            if (detectDLC[0]) DrawDLCInstallButton("DLC - Ade", controller.m_udonDLC[0], () => {
                Type.GetType("VRCCoffeeSet.Asset_Ade").GetMethod("Install").Invoke(null, new object[] {this} );
            });
            if (detectDLC[1]) DrawDLCInstallButton("DLC - BubbleTea", controller.m_udonDLC[1], () => {
                Type.GetType("VRCCoffeeSet.Asset_Boba").GetMethod("Install").Invoke(null, new object[] {this} );
            });
            if (detectSkin[0]) DrawDLCStatus("Skin - White Wood", true);

            #endregion
            
            #region Sound Settings

            fold_soundRange = EditorGUILayout.BeginFoldoutHeaderGroup(fold_soundRange, "Sound Settings");
            if (fold_soundRange)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                controller.m_soundRangeMax = EditorGUILayout.FloatField(new GUIContent("Sound Range", "Default is 5"), controller.m_soundRangeMax);
                if (EditorGUI.EndChangeCheck()) UpdateAudios();
                if (GUILayout.Button("Default", GUILayout.MaxWidth(75)))
                {
                    controller.m_soundRangeMax = 5;
                    UpdateAudios();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                controller.gizmo_visualize = EditorGUILayout.Toggle(new GUIContent("Visualize Sphere", "Adjust the transparency of the sphere by adjusting the alpha value."), controller.gizmo_visualize);
                controller.gizmo_color = EditorGUILayout.ColorField(controller.gizmo_color);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Soundproof Collider", GUILayout.Height(25)))
                {
                    EditorUtility.FocusProjectWindow();
                    EditorGUIUtility.PingObject( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>($"{pathRoot}/Coffee Set_Soundproof.prefab") );
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Particle Settings

            fold_particle = EditorGUILayout.BeginFoldoutHeaderGroup(fold_particle, "Particle Renderer Settings");
            if (fold_particle)
            {
                EditorGUI.BeginChangeCheck();
                particles[0].shadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Cast Shadows", particles[0].shadowCastingMode);
                particles[0].receiveShadows = EditorGUILayout.Toggle("Receive Shadows", particles[0].receiveShadows);
                particles[0].lightProbeUsage = (LightProbeUsage)EditorGUILayout.EnumPopup("Light Probes", particles[0].lightProbeUsage);
                particles[0].reflectionProbeUsage = (ReflectionProbeUsage)EditorGUILayout.EnumPopup("Reflection Probes", particles[0].reflectionProbeUsage);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var item in particles)
                    {
                        item.shadowCastingMode = particles[0].shadowCastingMode;
                        item.receiveShadows = particles[0].receiveShadows;
                        item.lightProbeUsage = particles[0].lightProbeUsage;
                        item.reflectionProbeUsage = particles[0].reflectionProbeUsage;
                    }
                }
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            #region Language Info

            fold_lang = EditorGUILayout.BeginFoldoutHeaderGroup(fold_lang, "Language Settings");
            if (fold_lang)
            {
                GUILayoutOption layoutOpt = GUILayout.MaxWidth(50);

                // Default Language
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                controller.m_langCurrent = (byte)EditorGUILayout.Popup(
                    "Default Language", controller.m_langCurrent,
                    m_langMain.Select(item => $"{MainMethod.GetJsonValue(item, "fileName")} ({MainMethod.GetJsonValue(item, "langName")})").ToArray()
                );
                if (EditorGUI.EndChangeCheck())
                    foreach (var item in controller.transform.GetComponentsInChildren<Menu_Plate>(true))
                    {
                        EditorUtility.SetDirty(item);
                        item.m_currentLang = controller.m_langCurrent;
                    }
                EditorGUILayout.Space();

                // Language Info
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                styleInfo.alignment = TextAnchor.MiddleLeft;
                GUILayout.Label("Language File", styleInfo);
                EditorGUILayout.Space();
                for (int i = 0; i < m_langMain.Length; i++)
                {
                    var fileName = MainMethod.GetJsonValue(m_langMain[i], "fileName");
                    var langName = MainMethod.GetJsonValue(m_langMain[i], "langName");
                    GUILayout.Label($"{fileName} ({langName})");
                }
                EditorGUILayout.EndVertical();
                styleInfo.alignment = styleInit.alignment = TextAnchor.MiddleCenter;
                DrawLangStatus("Main", m_langMain, layoutOpt);
                if (controller.m_udonDLC[0]) DrawLangStatus("Ade", m_langAde, layoutOpt);
                if (controller.m_udonDLC[1]) DrawLangStatus("Boba", m_langBoba, layoutOpt);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            EditorGUILayout.Space(20);

            fold_variable = EditorGUILayout.Foldout(fold_variable, "Do not change any variables");
            if (!fold_variable) { serializedObject.ApplyModifiedProperties(); return; }

            #region Udon

            status_fold[1] = EditorGUILayout.BeginFoldoutHeaderGroup(status_fold[1], "Udon");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (status_fold[1])
            {
                if ( UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target) ) return;
                base.OnInspectorGUI();
                GUILayout.Space(20);
            }

            #endregion

            #region Material

            status_fold[0] = EditorGUILayout.BeginFoldoutHeaderGroup(status_fold[0], "Materials");
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (status_fold[0])
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.BeginVertical();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_MachineButton"));
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_FilterContent"));
                EditorGUILayout.Space(5);
                
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Ring"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Straw"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Top"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Cream"));
                EditorGUILayout.Space(5);
            
                GUILayout.Label("Content");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Empty"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Water"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_WaterIce"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Milk"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_MilkIce"));
                
                EditorGUILayout.Space(5);
                GUILayout.Label("Coffee");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Syrup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Espresso"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_EspressoIce"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Americano"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_AmericanoIce"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Latte"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Cappuccino"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_LatteIce"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Mocha"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_MochaArt"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_MochaIce"));
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Macchiato"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_MacchiatoIce"));
                if (controller.m_udonDLC[0])
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_Ade"));
                }
                if (controller.m_udonDLC[1])
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_BobaPearls"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_BobaDrizzle"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mat_BobaBrownSugar"));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(15);
            }

            #endregion

            #region Debug

            status_fold[2] = EditorGUILayout.BeginFoldoutHeaderGroup(status_fold[2], "Debug");
            if (status_fold[2])
            {
                if (GUILayout.Button("Initialize Test")) MainMethod.Initialize("test");
                if (GUILayout.Button("Reset")) MainMethod.Reset(this);
                if (GUILayout.Button("Find Audiosources")) controller.audios = controller.transform.GetComponentsInChildren<AudioSource>();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion
        
            serializedObject.ApplyModifiedProperties();
        }
        
        void UpdateAudios()
        {
            if (!controller) return;
            foreach(AudioSource item in controller.audios)
            {
                item.minDistance = 0;
                item.maxDistance = controller.m_soundRangeMax;
            }
        }

        void DrawDLCInstallButton(string name, bool status, Action func)
        {
            // Button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            string labelBtn = status ? "\nClick to uninstall" : "\nClick to install";
            if (GUILayout.Button(labelBtn, GUILayout.MaxWidth(200), GUILayout.Height(50))) func();
            Rect btnRect = GUILayoutUtility.GetLastRect();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            // Calculate labels Size
            string labelText = $"{name} :\n";
            string statusText = status ? "✓\n" : "✕\n";
            Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(labelText));
            Vector2 statusSize = GUI.skin.label.CalcSize(new GUIContent(statusText));
            // Label for DLC name
            styleInit.alignment = TextAnchor.MiddleCenter;
            styleInit.normal.textColor = GUI.skin.button.normal.textColor;
            GUI.Label(
                new Rect(btnRect.x + btnRect.width/2 - statusSize.x/2, btnRect.y, 0, btnRect.height),
                labelText, styleInit
            );
            // Label for check icon
            styleInit.normal.textColor = status ? Color.green : Color.red;
            GUI.Label(
                new Rect(btnRect.x + btnRect.width/2 + labelSize.x/2, btnRect.y, 0, btnRect.height),
                statusText, styleInit
            );
        }
        void DrawDLCStatus(string name, bool status)
        {
            EditorGUILayout.BeginHorizontal();
            styleInit.alignment = TextAnchor.MiddleRight;
            styleInit.normal.textColor = GUI.skin.label.normal.textColor;
            GUILayout.Label($"{name} : ", styleInit);
            styleInit.alignment = TextAnchor.MiddleLeft;
            styleInit.normal.textColor = status ? Color.green : Color.red;
            GUILayout.Label(status ? "✓" : "✕", styleInit);
            EditorGUILayout.EndHorizontal();
        }
        void DrawLangStatus(string label, string[] refData, GUILayoutOption layoutOpt)
        {
            EditorGUILayout.BeginVertical(layoutOpt);
            GUILayout.Label(label, styleInfo);
            EditorGUILayout.Space();
            foreach (var item in m_langMain)
            {
                var temp = MainMethod.GetJsonByLangcode(refData, MainMethod.GetJsonValue(item, "langCode"));
                styleInit.normal.textColor = temp != null ? Color.green : Color.red;
                GUILayout.Label(temp != null ? "✓" : "✕", styleInit, layoutOpt);
            }
            EditorGUILayout.EndVertical();
        }

        public void AssignMaterial(Transform trans, Material mat) { trans.GetComponent<Renderer>().material = mat; }

        public void AddToolInList(Tool[] tools)
        {
            List<Tool> listTool = controller.tool_other.ToList<Tool>();
            foreach (var item in tools) listTool.Add(item);
            controller.tool_other = listTool.ToArray();
        }
    }

    #region Initialize on Build or Play

    public class MainController_PreBuild : IVRCSDKBuildRequestedCallback
    {
        public int callbackOrder { get { return 0; } }
        public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
        {
            return MainMethod.Initialize("build");
        }
    }
    [InitializeOnLoad] public static class MainController_PrePlay
    {
        static MainController_PrePlay() => EditorApplication.playModeStateChanged += LogPlayModeState;
        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode) 
            if (!MainMethod.Initialize("play"))
            EditorApplication.isPlaying = false;
        }
    }

    #endregion
}
