// ============================================================================
// iRoleAdminPanelTriggerEditor.cs
// Author: ishiruhii
// Description: Inspector personalizado para iRoleAdminPanelTrigger
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    [CustomEditor(typeof(Modules.AdminPanel.iRoleAdminPanelTrigger))]
    public class iRoleAdminPanelTriggerEditor : UnityEditor.Editor
    {
        // Propiedades serializadas
        private SerializedProperty adminPanel;
        private SerializedProperty canvasTransform;
        
        private SerializedProperty authorizedUsers;
        private SerializedProperty syncWithAdminPanel;
        
        private SerializedProperty inputMode;
        private SerializedProperty pcKey;
        private SerializedProperty oculusButtonIndex;
        
        private SerializedProperty distanceMultiplier;
        private SerializedProperty verticalOffset;
        private SerializedProperty baseAvatarHeight;
        private SerializedProperty baseCanvasScale;
        private SerializedProperty facingPlayer;
        private SerializedProperty flatRotation;
        private SerializedProperty extraRotation;
        
        private SerializedProperty cooldown;
        private SerializedProperty overridePositioning;
        private SerializedProperty debugMode;

        // Colores
        private static readonly Color COLOR_HEADER = new Color(0.15f, 0.30f, 0.50f);
        private static readonly Color COLOR_SYNC_BTN = new Color(0.18f, 0.52f, 0.78f);
        private static readonly Color COLOR_WARNING = new Color(0.90f, 0.70f, 0.20f);

        private void OnEnable()
        {
            adminPanel = serializedObject.FindProperty("adminPanel");
            canvasTransform = serializedObject.FindProperty("canvasTransform");
            
            authorizedUsers = serializedObject.FindProperty("authorizedUsers");
            syncWithAdminPanel = serializedObject.FindProperty("syncWithAdminPanel");
            
            inputMode = serializedObject.FindProperty("inputMode");
            pcKey = serializedObject.FindProperty("pcKey");
            oculusButtonIndex = serializedObject.FindProperty("oculusButtonIndex");
            
            distanceMultiplier = serializedObject.FindProperty("distanceMultiplier");
            verticalOffset = serializedObject.FindProperty("verticalOffset");
            baseAvatarHeight = serializedObject.FindProperty("baseAvatarHeight");
            baseCanvasScale = serializedObject.FindProperty("baseCanvasScale");
            facingPlayer = serializedObject.FindProperty("facingPlayer");
            flatRotation = serializedObject.FindProperty("flatRotation");
            extraRotation = serializedObject.FindProperty("extraRotation");
            
            cooldown = serializedObject.FindProperty("cooldown");
            overridePositioning = serializedObject.FindProperty("overridePositioning");
            debugMode = serializedObject.FindProperty("debugMode");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Header
            DrawHeader();
            EditorGUILayout.Space(10);

            // Referencias
            DrawReferences();
            DrawDivider("WHITELIST");
            DrawWhitelist();
            DrawDivider("CONFIGURACIÓN DE INPUT");
            DrawInputConfig();
            DrawDivider("POSICIONAMIENTO DEL CANVAS");
            DrawPositioning();
            DrawDivider("OPCIONES");
            DrawOptions();
            DrawDivider("DEBUG");
            DrawDebug();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            GUI.backgroundColor = COLOR_HEADER;
            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = Color.white;

            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel);
            titleStyle.fontSize = 14;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            EditorGUILayout.LabelField("iRoleAdminPanelTrigger", titleStyle);
            EditorGUILayout.LabelField("Activador de panel con teclas PC/VR", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.EndVertical();
        }

        private void DrawReferences()
        {
            EditorGUILayout.LabelField("Referencias", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");

            // Auto-find del admin panel
            if (adminPanel.objectReferenceValue == null)
            {
                Modules.AdminPanel.iRoleAdminPanel found = 
                    FindObjectOfType<Modules.AdminPanel.iRoleAdminPanel>();
                if (found != null)
                {
                    adminPanel.objectReferenceValue = found;
                    EditorGUILayout.HelpBox("✓ iRoleAdminPanel encontrado automáticamente", MessageType.Info);
                }
            }

            EditorGUILayout.PropertyField(adminPanel, new GUIContent("Admin Panel"));

            // Auto-find del canvas si el panel está asignado
            if (canvasTransform.objectReferenceValue == null && adminPanel.objectReferenceValue != null)
            {
                Modules.AdminPanel.iRoleAdminPanel panel = 
                    (Modules.AdminPanel.iRoleAdminPanel)adminPanel.objectReferenceValue;
                if (panel.canvasRoot != null)
                {
                    canvasTransform.objectReferenceValue = panel.canvasRoot.transform;
                    EditorGUILayout.HelpBox("✓ Canvas encontrado desde el Admin Panel", MessageType.Info);
                }
            }

            EditorGUILayout.PropertyField(canvasTransform, new GUIContent("Canvas Transform"));

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);
        }

        private void DrawWhitelist()
        {
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.PropertyField(syncWithAdminPanel, 
                new GUIContent("Sincronizar con Admin Panel",
                    "Si está activo, sincroniza automáticamente con la lista de admins del panel al iniciar"));

            if (syncWithAdminPanel.boolValue && adminPanel.objectReferenceValue != null)
            {
                EditorGUILayout.Space(5);
                GUI.backgroundColor = COLOR_SYNC_BTN;
                if (GUILayout.Button("↻ Sincronizar Ahora", GUILayout.Height(24)))
                {
                    Modules.AdminPanel.iRoleAdminPanel panel = 
                        (Modules.AdminPanel.iRoleAdminPanel)adminPanel.objectReferenceValue;
                    
                    Modules.AdminPanel.iRoleAdminPanelTrigger trigger = 
                        (Modules.AdminPanel.iRoleAdminPanelTrigger)target;
                    
                    if (panel.adminNames != null)
                    {
                        trigger.authorizedUsers = new string[panel.adminNames.Length];
                        panel.adminNames.CopyTo(trigger.authorizedUsers, 0);
                        EditorUtility.SetDirty(trigger);
                        serializedObject.Update();
                        EditorUtility.DisplayDialog("Sincronizado", 
                            "Whitelist sincronizada: " + trigger.authorizedUsers.Length + " usuarios", "OK");
                    }
                }
                GUI.backgroundColor = Color.white;
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(authorizedUsers, 
                new GUIContent("Usuarios Autorizados",
                    "DisplayNames de los usuarios que pueden activar el panel con la tecla"), true);

            if (authorizedUsers.arraySize == 0)
            {
                EditorGUILayout.HelpBox("⚠ No hay usuarios autorizados. Nadie podrá abrir el panel con la tecla.", 
                    MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawInputConfig()
        {
            EditorGUILayout.BeginVertical("box");

            string[] inputModeOptions = new string[] { "Teclado PC", "Oculus / VR" };
            inputMode.intValue = EditorGUILayout.Popup("Modo de Input", inputMode.intValue, inputModeOptions);

            EditorGUILayout.Space(5);

            if (inputMode.intValue == 0)
            {
                // Modo PC
                EditorGUILayout.LabelField("Configuración PC", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(pcKey, new GUIContent("Tecla PC"));
                
                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("💡 Tecla recomendada: Tab (no interfiere con controles de VRChat)", 
                    MessageType.Info);
            }
            else
            {
                // Modo Oculus
                EditorGUILayout.LabelField("Configuración Oculus", EditorStyles.boldLabel);
                
                string[] oculusButtons = new string[]
                {
                    "A (mano derecha)",
                    "B (mano derecha)",
                    "X (mano izquierda)",
                    "Y (mano izquierda)",
                    "Grip derecho",
                    "Grip izquierdo",
                    "Index derecho",
                    "Index izquierdo",
                    "Stick derecho (clic)",
                    "Stick izquierdo (clic)"
                };

                oculusButtonIndex.intValue = EditorGUILayout.Popup(
                    "Botón Oculus", 
                    oculusButtonIndex.intValue, 
                    oculusButtons);

                EditorGUILayout.Space(5);
                EditorGUILayout.HelpBox("💡 Los botones de grip o stick suelen ser buenas opciones", 
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawPositioning()
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.PropertyField(overridePositioning, 
                new GUIContent("Override Posicionamiento",
                    "Si está activo, este trigger posiciona el canvas (ignora el posicionamiento del admin panel)"));

            if (!overridePositioning.boolValue)
            {
                EditorGUILayout.HelpBox("ℹ El posicionamiento se controlará desde el iRoleAdminPanel", 
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space(5);
                GUI.backgroundColor = COLOR_WARNING;
                EditorGUILayout.HelpBox("⚠ Este trigger sobreescribirá el posicionamiento del admin panel", 
                    MessageType.Warning);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.Space(5);

                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(distanceMultiplier, 
                    new GUIContent("Multiplicador Distancia"));
                EditorGUILayout.PropertyField(verticalOffset, 
                    new GUIContent("Offset Vertical"));
                EditorGUILayout.PropertyField(baseAvatarHeight, 
                    new GUIContent("Altura Base Avatar"));
                EditorGUILayout.PropertyField(baseCanvasScale, 
                    new GUIContent("Escala Base Canvas"));
                
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(facingPlayer, 
                    new GUIContent("Mirar al Jugador"));
                EditorGUILayout.PropertyField(flatRotation, 
                    new GUIContent("Rotación Plana"));
                EditorGUILayout.PropertyField(extraRotation, 
                    new GUIContent("Rotación Extra"));

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawOptions()
        {
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.PropertyField(cooldown, 
                new GUIContent("Cooldown (segundos)",
                    "Tiempo mínimo entre activaciones del trigger"));

            if (cooldown.floatValue < 0.1f)
            {
                EditorGUILayout.HelpBox("⚠ Cooldown muy bajo puede causar activaciones múltiples", 
                    MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawDebug()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(debugMode, new GUIContent("Modo Debug"));
            EditorGUILayout.EndVertical();
        }

        private void DrawDivider(string title)
        {
            EditorGUILayout.Space(8);
            GUILayout.Label("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(4);
        }
    }
}
#endif
