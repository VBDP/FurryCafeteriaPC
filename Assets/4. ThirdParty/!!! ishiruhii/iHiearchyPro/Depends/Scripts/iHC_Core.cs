// ============================================================
//  iHC_Core.cs  —  Nucleo de iHierarchyPro
//  Ubicacion: Assets/TuProyecto/Scripts/
//  NO coloques este archivo dentro de una carpeta Editor
//
//  Este archivo contiene los tipos y la logica de pintado
//  compartida por todos los scripts de color generados.
//  Distribuyelo junto con tus scripts de color (ReColor1.cs, etc.)
//
//  NO requiere ninguna otra dependencia de ishiruhii.
// ============================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iTools
{
    // ─────────────────────────────────────────────────────────
    //  Enums
    // ─────────────────────────────────────────────────────────
    public enum iHC_LabelMode
    {
        ReplaceName,     // Reemplaza el nombre del objeto en la jerarquia
        AdditionalLabel  // Muestra el nombre original + etiqueta extra
    }

    public enum iHC_ScopeMode
    {
        AffectsObject,        // Solo afecta al objeto con ese nombre exacto
        AffectsFirstChildren, // Afecta a los hijos directos del objeto con ese nombre
        AffectsAllChildren    // Afecta a todos los hijos y subhijos recursivamente
    }

    // ─────────────────────────────────────────────────────────
    //  Struct de regla
    // ─────────────────────────────────────────────────────────
    public struct iHC_Rule
    {
        public string        objectName;
        public Color         backgroundColor;
        public Color         textColor;
        public iHC_LabelMode labelMode;
        public iHC_ScopeMode scopeMode;
        public string        customLabel;
        public string        iconPath;
        public Color         iconTint;
    }

    // ─────────────────────────────────────────────────────────
    //  Logica de pintado (compartida por todos los scripts)
    // ─────────────────────────────────────────────────────────
    // ─────────────────────────────────────────────────────────
    //  Bridge de preview en tiempo real (usado por iHierarchyEditor)
    //  Si el editor no esta presente, key siempre es null y los
    //  scripts usan sus propias reglas sin ningun overhead.
    // ─────────────────────────────────────────────────────────
    public static class iHC_Preview
    {
        public static iHC_Rule[] rules = null;
        public static string     key   = null; // nombre de la clase que tiene el override activo
    }

        public static class iHC_Painter
    {
        public static bool MatchesScope(GameObject obj, iHC_Rule rule)
        {
            switch (rule.scopeMode)
            {
                case iHC_ScopeMode.AffectsObject:
                    return obj.name == rule.objectName;
                case iHC_ScopeMode.AffectsFirstChildren:
                    return obj.transform.parent != null &&
                           obj.transform.parent.name == rule.objectName;
                case iHC_ScopeMode.AffectsAllChildren:
                    return HasAncestorNamed(obj.transform, rule.objectName);
                default: return false;
            }
        }

        private static bool HasAncestorNamed(Transform t, string name)
        {
            var p = t.parent;
            while (p != null)
            {
                if (p.name == name) return true;
                p = p.parent;
            }
            return false;
        }

        public static void Paint(GameObject obj, Rect rect, iHC_Rule rule)
        {
            // Fondo
            EditorGUI.DrawRect(rect, rule.backgroundColor);

            // Icono
            Rect textRect = rect;
            if (!string.IsNullOrEmpty(rule.iconPath))
            {
                var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(rule.iconPath);
                if (icon != null)
                {
                    float s     = rect.height;
                    var   iRect = new Rect(rect.x, rect.y, s, s);
                    var   prev  = GUI.color;
                    GUI.color   = rule.iconTint;
                    GUI.DrawTexture(iRect, icon, ScaleMode.ScaleToFit);
                    GUI.color   = prev;
                    textRect    = new Rect(rect.x + s + 2f, rect.y, rect.width - s - 2f, rect.height);
                }
            }

            // Texto
            string label = rule.labelMode == iHC_LabelMode.ReplaceName
                ? (string.IsNullOrEmpty(rule.customLabel) ? obj.name : rule.customLabel)
                : obj.name + (string.IsNullOrEmpty(rule.customLabel) ? "" : "  |  " + rule.customLabel);

            GUI.Label(textRect, label, new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                normal    = { textColor = rule.textColor }
            });
        }
    }
}
#endif
