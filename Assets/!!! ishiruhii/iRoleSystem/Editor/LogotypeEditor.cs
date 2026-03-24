using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Logotype))]
public class LogotypeEditor : Editor
{
    SerializedProperty bannerProp;
    SerializedProperty detectedObjectsProp;
    SerializedProperty detectedStatesProp;

    GUIStyle titleStyle;
    GUIStyle subtitleStyle;
    GUIStyle sectionStyle;
    GUIStyle featureStyle;
    GUIStyle moduleTitleStyle;

    private void OnEnable()
    {
        bannerProp = serializedObject.FindProperty("banner");
        detectedObjectsProp = serializedObject.FindProperty("detectedObjects");
        detectedStatesProp = serializedObject.FindProperty("detectedStates");

        titleStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        subtitleStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        sectionStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        featureStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        moduleTitleStyle = new GUIStyle(EditorStyles.label)
        {
            fontSize = 15,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(10);

        DrawBanner();
        DrawMainHeader();
        DrawFeatures();

        GUILayout.Space(30);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        DrawAutoDetectedSection();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            Logotype logotype = (Logotype)target;
            logotype.ApplyDetectedState();
            EditorUtility.SetDirty(logotype);
        }
    }

    void DrawBanner()
    {
        if (bannerProp.objectReferenceValue != null)
        {
            Sprite bannerSprite = (Sprite)bannerProp.objectReferenceValue;
            Rect rect = GUILayoutUtility.GetRect(0, 1000, 80, 200);
            EditorGUI.DrawPreviewTexture(rect, bannerSprite.texture, null, ScaleMode.ScaleToFit);
        }
        else
        {
            EditorGUILayout.HelpBox("Asigna un banner aquí", MessageType.Info);
        }
    }

    void DrawMainHeader()
    {
        GUILayout.Space(15);
        GUILayout.Label("iRoleSystem PRO - v5.8", titleStyle);
        GUILayout.Space(8);
        GUILayout.Label("La Solucion definitiva para la gestion de", subtitleStyle);
        GUILayout.Space(1);
        GUILayout.Label("membresias y usuarios en tu mundo de VRChat.", subtitleStyle);
        GUILayout.Space(15);
        GUILayout.Label("• VENTAJAS EX •", sectionStyle);
        GUILayout.Space(10);
    }

    void DrawFeatures()
    {
        DrawFeature("• [PRO] Sistema Ampliamente Modular •");
        DrawFeature("• [PRO] Actualizaciones Constantes •");
        DrawFeature("• [PRO] Funciones Mejoradas y Eficientes •");
        DrawFeature("• [PRO] Soporte para futuras actualizaciones •");
		
    }

    void DrawAutoDetectedSection()
    {
        GUILayout.Label("MADE BY ISHIRUHII", moduleTitleStyle);
        GUILayout.Space(10);
		GUILayout.Label("Version 5.6-Developer(5.6.4dev)", moduleTitleStyle);
        GUILayout.Space(10);

        GUILayout.Space(10);

        for (int i = 0; i < detectedObjectsProp.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();

            SerializedProperty obj = detectedObjectsProp.GetArrayElementAtIndex(i);
            SerializedProperty state = detectedStatesProp.GetArrayElementAtIndex(i);

            GUI.enabled = obj.objectReferenceValue != null;

            EditorGUILayout.ObjectField(obj.objectReferenceValue, typeof(GameObject), true);
            state.boolValue = EditorGUILayout.Toggle(state.boolValue, GUILayout.Width(20));

            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
        }
    }

    void DrawFeature(string text)
    {
        GUILayout.Label(text, featureStyle);
        GUILayout.Space(4);
    }
}

