// ============================================================================
// iRoleSystem v5.8 - Shared Editor Style System
// Author: ishiruhii
// Description: Design system unificado para todos los editores del sistema.
// ============================================================================

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Editor
{
    /// <summary>
    /// Sistema de diseño centralizado. Todos los editores usan esta clase.
    /// </summary>
    public static class iRoleEditorStyle
    {
        // ── Paleta de colores ──────────────────────────────────────────────
        public static readonly Color C_BG_DARK       = new Color(0.11f, 0.11f, 0.15f, 1f);
        public static readonly Color C_BG_SECTION    = new Color(0.15f, 0.15f, 0.20f, 1f);
        public static readonly Color C_BG_ROW_A      = new Color(0.14f, 0.14f, 0.19f, 1f);
        public static readonly Color C_BG_ROW_B      = new Color(0.12f, 0.12f, 0.17f, 1f);
        public static readonly Color C_ACCENT_BLUE   = new Color(0.22f, 0.55f, 1.00f, 1f);
        public static readonly Color C_ACCENT_GREEN  = new Color(0.18f, 0.82f, 0.48f, 1f);
        public static readonly Color C_ACCENT_ORANGE = new Color(1.00f, 0.62f, 0.10f, 1f);
        public static readonly Color C_ACCENT_RED    = new Color(0.95f, 0.28f, 0.28f, 1f);
        public static readonly Color C_ACCENT_PURPLE = new Color(0.70f, 0.35f, 1.00f, 1f);
        public static readonly Color C_ACCENT_CYAN   = new Color(0.10f, 0.85f, 0.90f, 1f);
        public static readonly Color C_TEXT_DIM      = new Color(0.58f, 0.58f, 0.68f, 1f);
        public static readonly Color C_TEXT_MID      = new Color(0.78f, 0.78f, 0.85f, 1f);
        public static readonly Color C_TEXT_WHITE    = new Color(0.96f, 0.96f, 1.00f, 1f);
        public static readonly Color C_BORDER        = new Color(0.25f, 0.25f, 0.32f, 1f);

        // ── Texturas en caché ──────────────────────────────────────────────
        private static Texture2D _texDark;
        private static Texture2D _texSection;
        private static Texture2D _texBlue;
        private static Texture2D _texGreen;
        private static Texture2D _texRed;
        private static Texture2D _texOrange;
        private static Texture2D _texPurple;
        private static Texture2D _texCyan;
        private static Texture2D _texBorder;

        public static Texture2D TexDark    => _texDark    ?? (_texDark    = MakeTex(C_BG_DARK));
        public static Texture2D TexSection => _texSection ?? (_texSection = MakeTex(C_BG_SECTION));
        public static Texture2D TexBlue    => _texBlue    ?? (_texBlue    = MakeTex(C_ACCENT_BLUE));
        public static Texture2D TexGreen   => _texGreen   ?? (_texGreen   = MakeTex(C_ACCENT_GREEN));
        public static Texture2D TexRed     => _texRed     ?? (_texRed     = MakeTex(C_ACCENT_RED));
        public static Texture2D TexOrange  => _texOrange  ?? (_texOrange  = MakeTex(C_ACCENT_ORANGE));
        public static Texture2D TexPurple  => _texPurple  ?? (_texPurple  = MakeTex(C_ACCENT_PURPLE));
        public static Texture2D TexCyan    => _texCyan    ?? (_texCyan    = MakeTex(C_ACCENT_CYAN));
        public static Texture2D TexBorder  => _texBorder  ?? (_texBorder  = MakeTex(C_BORDER));

        // ── Estilos en caché ───────────────────────────────────────────────
        private static GUIStyle _sTitle;
        private static GUIStyle _sSubtitle;
        private static GUIStyle _sSectionLabel;
        private static GUIStyle _sDim;
        private static GUIStyle _sMini;
        private static GUIStyle _sBadge;
        private static GUIStyle _sFoldout;
        private static GUIStyle _sBox;
        private static GUIStyle _sBoxDark;

        public static GUIStyle STitle => _sTitle ?? (_sTitle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 15, fontStyle = FontStyle.Bold,
            normal = { textColor = C_TEXT_WHITE },
            padding = new RectOffset(0, 0, 2, 0)
        });

        public static GUIStyle SSubtitle => _sSubtitle ?? (_sSubtitle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 10, normal = { textColor = C_TEXT_DIM }
        });

        public static GUIStyle SSectionLabel => _sSectionLabel ?? (_sSectionLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 11, fontStyle = FontStyle.Bold,
            normal = { textColor = C_TEXT_WHITE }
        });

        public static GUIStyle SDim => _sDim ?? (_sDim = new GUIStyle(EditorStyles.miniLabel)
        {
            normal = { textColor = C_TEXT_DIM }, wordWrap = true, fontSize = 10
        });

        public static GUIStyle SMini => _sMini ?? (_sMini = new GUIStyle(EditorStyles.miniLabel)
        {
            normal = { textColor = C_TEXT_MID }, fontSize = 10
        });

        public static GUIStyle SBadge => _sBadge ?? (_sBadge = new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 9, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold,
            padding = new RectOffset(4, 4, 1, 1)
        });

        public static GUIStyle SFoldout => _sFoldout ?? (_sFoldout = new GUIStyle(EditorStyles.foldoutHeader)
        {
            fontStyle = FontStyle.Bold, fontSize = 11
        });

        public static GUIStyle SBox => _sBox ?? (_sBox = new GUIStyle("box")
        {
            padding = new RectOffset(10, 10, 8, 8),
            margin  = new RectOffset(0, 0, 3, 3),
            normal  = { background = TexSection }
        });

        public static GUIStyle SBoxDark => _sBoxDark ?? (_sBoxDark = new GUIStyle("box")
        {
            padding = new RectOffset(8, 8, 6, 6),
            margin  = new RectOffset(0, 0, 2, 2),
            normal  = { background = TexDark }
        });

        // ── API de dibujo ──────────────────────────────────────────────────

        /// <summary>Dibuja el header principal de un módulo.</summary>
        public static void DrawHeader(string icon, string title, string subtitle, Color accent, string version = "v5.8")
        {
            Rect bgRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(new Rect(bgRect.x - 2, bgRect.y, bgRect.width + 4, 60), C_BG_DARK);

            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(12);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField($"{icon}  {title}", STitle);
            EditorGUILayout.LabelField(subtitle, SSubtitle);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            Color prev = GUI.contentColor;
            GUI.contentColor = new Color(1f, 1f, 1f, 0.35f);
            EditorGUILayout.LabelField(version, EditorStyles.miniLabel, GUILayout.Width(32));
            GUI.contentColor = prev;
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            EditorGUILayout.EndVertical();

            // Línea acento
            Rect lr = GUILayoutUtility.GetRect(1, 2);
            EditorGUI.DrawRect(lr, accent);
        }

        /// <summary>Dibuja el header de sección con borde izquierdo colorido.</summary>
        public static void DrawSectionHeader(string title, Color accent)
        {
            EditorGUILayout.Space(2);
            Rect r = GUILayoutUtility.GetRect(0, 22);
            EditorGUI.DrawRect(r, new Color(accent.r, accent.g, accent.b, 0.09f));
            EditorGUI.DrawRect(new Rect(r.x, r.y, 3, r.height), accent);
            GUI.Label(new Rect(r.x + 10, r.y + 3, r.width, r.height), title, SSectionLabel);
            EditorGUILayout.Space(2);
        }

        /// <summary>Advertencia inline con color.</summary>
        public static void DrawWarning(string msg, Color color)
        {
            Rect r = GUILayoutUtility.GetRect(0, 30);
            EditorGUI.DrawRect(r, new Color(color.r, color.g, color.b, 0.10f));
            EditorGUI.DrawRect(new Rect(r.x, r.y, 3, r.height), color);
            GUI.Label(new Rect(r.x + 10, r.y + 4, r.width - 14, r.height - 6), msg, SDim);
            EditorGUILayout.Space(1);
        }

        /// <summary>Chip/badge inline.</summary>
        // FIX LAG: Cacheamos el GUIStyle del chip para no allocar uno nuevo cada frame.
        private static GUIStyle _sChipCached;
        private static Color    _sChipCachedColor;

        public static void DrawChip(string text, Color color)
        {
            if (_sChipCached == null || _sChipCachedColor != color)
            {
                _sChipCached = new GUIStyle(SBadge) { normal = { textColor = color } };
                _sChipCachedColor = color;
            }
            GUIStyle s = _sChipCached;
            Vector2 sz = s.CalcSize(new GUIContent(text));
            Rect r = GUILayoutUtility.GetRect(sz.x + 10, sz.y + 4);
            EditorGUI.DrawRect(r, new Color(color.r, color.g, color.b, 0.14f));
            EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 1), new Color(color.r, color.g, color.b, 0.35f));
            GUI.Label(r, text, s);
        }

        /// <summary>Botón con color de fondo.</summary>
        public static bool ColorButton(string text, Color color, params GUILayoutOption[] opts)
        {
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = color * new Color(1, 1, 1, 0.6f);
            bool r = GUILayout.Button(text, EditorStyles.miniButton, opts);
            GUI.backgroundColor = prev;
            return r;
        }

        public static bool ColorButton(string text, Color color, GUIStyle style, params GUILayoutOption[] opts)
        {
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = color * new Color(1, 1, 1, 0.6f);
            bool r = GUILayout.Button(text, style, opts);
            GUI.backgroundColor = prev;
            return r;
        }

        /// <summary>Separador fino.</summary>
        public static void Divider(float opacity = 0.25f)
        {
            Rect r = GUILayoutUtility.GetRect(1, 1);
            EditorGUI.DrawRect(r, new Color(C_BORDER.r, C_BORDER.g, C_BORDER.b, opacity));
        }

        // ── Helpers ────────────────────────────────────────────────────────
        public static Texture2D MakeTex(Color col)
        {
            Texture2D t = new Texture2D(1, 1);
            t.SetPixel(0, 0, col); t.Apply(); return t;
        }
    }
}
#endif
