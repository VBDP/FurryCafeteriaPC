// ============================================================================
// iRoleSystem v5.6 - Language Module
// Author: ishiruhii
// Description: Sistema central de localización multilenguaje
//              Soporta: Español, English, 日本語, 中文
// ============================================================================

using UdonSharp;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Core
{
    /// <summary>
    /// Idiomas disponibles en el sistema
    /// </summary>
    public enum iSystemLanguage
    {
        Spanish  = 0,   // Español
        English  = 1,   // English
        Japanese = 2,   // 日本語
        Chinese  = 3    // 中文
    }

    // =========================================================================
    // Índices de strings (usa estas constantes en todos los módulos)
    // =========================================================================
    public static class iLang
    {
        public const int NO_ROLE              = 0;
        public const int NO_PERMISSION        = 1;
        public const int NO_ROLES_AVAILABLE   = 2;
        public const int ACCESS_DENIED        = 3;
        public const int ROLE_ASSIGNED        = 4;
        public const int WRITE_PLAYER_NAME    = 5;
        public const int PLAYER_NOT_FOUND     = 6;
        public const int CURRENT_ROLE         = 7;
        public const int WRITE_NAME_PROMPT    = 8;
        public const int YOU_LABEL            = 9;
        public const int PLAYER_NOT_IN_WORLD  = 10;
        public const int NO_PERMISSION_ENTER  = 11;
        public const int NO_SPACE_PLAYERS     = 12;
        public const int SYSTEM_INITIALIZED   = 13;
        public const int ROLE_REMOVED         = 14;
        public const int INPUT_NOT_ASSIGNED   = 15;
        public const int ROLE_MANAGER_NULL    = 16;

        // Total de strings
        public const int COUNT = 17;
    }

    /// <summary>
    /// Módulo central de localización.
    /// Referencia esta clase desde iRoleSystemCore para acceder a todos los textos.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iLanguageSystem : UdonSharpBehaviour
    {
        // =====================================================================
        // Idioma activo
        // =====================================================================
        [Header("=== iLanguageSystem ===")]
        [Tooltip("Idioma activo del sistema / Active system language")]
        public iSystemLanguage activeLanguage = iSystemLanguage.Spanish;

        // =====================================================================
        // Tablas de strings por idioma
        // Cada array tiene exactamente iLang.COUNT entradas, en el mismo orden
        // =====================================================================

        [Header("--- Español ---")]
        public string[] strings_ES = new string[]
        {
            /* 00 NO_ROLE             */ "Sin Rol",
            /* 01 NO_PERMISSION       */ "Sin permiso",
            /* 02 NO_ROLES_AVAILABLE  */ "No tienes roles disponibles.",
            /* 03 ACCESS_DENIED       */ "No tienes autorización.",
            /* 04 ROLE_ASSIGNED       */ "Rol '{0}' asignado ✓",
            /* 05 WRITE_PLAYER_NAME   */ "Escribe el nombre del jugador primero.",
            /* 06 PLAYER_NOT_FOUND    */ "Jugador '{0}' no encontrado en el mundo.",
            /* 07 CURRENT_ROLE        */ "Rol actual: {0}",
            /* 08 WRITE_NAME_PROMPT   */ "Escribe un nombre...",
            /* 09 YOU_LABEL           */ " (Tú)",
            /* 10 PLAYER_NOT_IN_WORLD */ "{0} no está en el mundo",
            /* 11 NO_PERMISSION_ENTER */ "No tienes permiso para entrar aquí.",
            /* 12 NO_SPACE_PLAYERS    */ "No hay espacio para más jugadores.",
            /* 13 SYSTEM_INITIALIZED  */ "iRoleSystem inicializado correctamente.",
            /* 14 ROLE_REMOVED        */ "Rol removido.",
            /* 15 INPUT_NOT_ASSIGNED  */ "Error: Input no asignado.",
            /* 16 ROLE_MANAGER_NULL   */ "Error: RoleManager no asignado."
        };

        [Header("--- English ---")]
        public string[] strings_EN = new string[]
        {
            /* 00 NO_ROLE             */ "No Role",
            /* 01 NO_PERMISSION       */ "No permission",
            /* 02 NO_ROLES_AVAILABLE  */ "No roles available.",
            /* 03 ACCESS_DENIED       */ "Access denied.",
            /* 04 ROLE_ASSIGNED       */ "Role '{0}' assigned ✓",
            /* 05 WRITE_PLAYER_NAME   */ "Enter player name first.",
            /* 06 PLAYER_NOT_FOUND    */ "Player '{0}' not found in the world.",
            /* 07 CURRENT_ROLE        */ "Current role: {0}",
            /* 08 WRITE_NAME_PROMPT   */ "Type a name...",
            /* 09 YOU_LABEL           */ " (You)",
            /* 10 PLAYER_NOT_IN_WORLD */ "{0} is not in the world",
            /* 11 NO_PERMISSION_ENTER */ "You don't have permission to enter here.",
            /* 12 NO_SPACE_PLAYERS    */ "No space available for more players.",
            /* 13 SYSTEM_INITIALIZED  */ "iRoleSystem initialized successfully.",
            /* 14 ROLE_REMOVED        */ "Role removed.",
            /* 15 INPUT_NOT_ASSIGNED  */ "Error: Input not assigned.",
            /* 16 ROLE_MANAGER_NULL   */ "Error: RoleManager not assigned."
        };

        [Header("--- 日本語 ---")]
        public string[] strings_JP = new string[]
        {
            /* 00 NO_ROLE             */ "ロールなし",
            /* 01 NO_PERMISSION       */ "権限なし",
            /* 02 NO_ROLES_AVAILABLE  */ "利用可能なロールがありません。",
            /* 03 ACCESS_DENIED       */ "アクセスが拒否されました。",
            /* 04 ROLE_ASSIGNED       */ "ロール「{0}」を割り当てました ✓",
            /* 05 WRITE_PLAYER_NAME   */ "プレイヤー名を先に入力してください。",
            /* 06 PLAYER_NOT_FOUND    */ "プレイヤー「{0}」がワールドに見つかりません。",
            /* 07 CURRENT_ROLE        */ "現在のロール：{0}",
            /* 08 WRITE_NAME_PROMPT   */ "名前を入力...",
            /* 09 YOU_LABEL           */ "（あなた）",
            /* 10 PLAYER_NOT_IN_WORLD */ "{0} はワールドにいません",
            /* 11 NO_PERMISSION_ENTER */ "ここに入る権限がありません。",
            /* 12 NO_SPACE_PLAYERS    */ "これ以上プレイヤーを追加するスペースがありません。",
            /* 13 SYSTEM_INITIALIZED  */ "iRoleSystem が正常に初期化されました。",
            /* 14 ROLE_REMOVED        */ "ロールを削除しました。",
            /* 15 INPUT_NOT_ASSIGNED  */ "エラー：入力が割り当てられていません。",
            /* 16 ROLE_MANAGER_NULL   */ "エラー：RoleManager が割り当てられていません。"
        };

        [Header("--- 中文 ---")]
        public string[] strings_ZH = new string[]
        {
            /* 00 NO_ROLE             */ "无角色",
            /* 01 NO_PERMISSION       */ "无权限",
            /* 02 NO_ROLES_AVAILABLE  */ "没有可用角色。",
            /* 03 ACCESS_DENIED       */ "访问被拒绝。",
            /* 04 ROLE_ASSIGNED       */ "角色「{0}」已分配 ✓",
            /* 05 WRITE_PLAYER_NAME   */ "请先输入玩家名称。",
            /* 06 PLAYER_NOT_FOUND    */ "在世界中未找到玩家「{0}」。",
            /* 07 CURRENT_ROLE        */ "当前角色：{0}",
            /* 08 WRITE_NAME_PROMPT   */ "输入名称...",
            /* 09 YOU_LABEL           */ "（你）",
            /* 10 PLAYER_NOT_IN_WORLD */ "{0} 不在世界中",
            /* 11 NO_PERMISSION_ENTER */ "您没有权限进入此处。",
            /* 12 NO_SPACE_PLAYERS    */ "没有更多空间容纳玩家。",
            /* 13 SYSTEM_INITIALIZED  */ "iRoleSystem 已成功初始化。",
            /* 14 ROLE_REMOVED        */ "角色已移除。",
            /* 15 INPUT_NOT_ASSIGNED  */ "错误：未分配输入。",
            /* 16 ROLE_MANAGER_NULL   */ "错误：RoleManager 未分配。"
        };

        // =====================================================================
        // API PÚBLICA
        // =====================================================================

        /// <summary>
        /// Obtiene un string localizado por ID (sin argumentos)
        /// </summary>
        public string Get(int stringId)
        {
            string[] table = GetActiveTable();
            if (table == null || stringId < 0 || stringId >= table.Length)
                return "[?]";
            return table[stringId];
        }

        /// <summary>
        /// Obtiene un string localizado reemplazando {0} por el argumento
        /// </summary>
        public string Get(int stringId, string arg0)
        {
            string raw = Get(stringId);
            return raw.Replace("{0}", arg0);
        }

        /// <summary>
        /// Cambia el idioma activo en tiempo real
        /// </summary>
        public void SetLanguage(iSystemLanguage lang)
        {
            activeLanguage = lang;
        }

        /// <summary>
        /// Cambia el idioma usando índice entero (útil desde UdonSharp)
        /// </summary>
        public void SetLanguageByIndex(int index)
        {
            switch (index)
            {
                case 0: activeLanguage = iSystemLanguage.Spanish;  break;
                case 1: activeLanguage = iSystemLanguage.English;  break;
                case 2: activeLanguage = iSystemLanguage.Japanese; break;
                case 3: activeLanguage = iSystemLanguage.Chinese;  break;
                default: activeLanguage = iSystemLanguage.Spanish; break;
            }
        }

        /// <summary>
        /// Obtiene el índice entero del idioma activo
        /// </summary>
        public int GetLanguageIndex()
        {
            return (int)activeLanguage;
        }

        /// <summary>
        /// Nombre legible del idioma activo
        /// </summary>
        public string GetLanguageName()
        {
            switch (activeLanguage)
            {
                case iSystemLanguage.Spanish:  return "Español";
                case iSystemLanguage.English:  return "English";
                case iSystemLanguage.Japanese: return "日本語";
                case iSystemLanguage.Chinese:  return "中文";
                default:                       return "Español";
            }
        }

        // =====================================================================
        // PRIVADO
        // =====================================================================

        private string[] GetActiveTable()
        {
            switch (activeLanguage)
            {
                case iSystemLanguage.Spanish:  return strings_ES;
                case iSystemLanguage.English:  return strings_EN;
                case iSystemLanguage.Japanese: return strings_JP;
                case iSystemLanguage.Chinese:  return strings_ZH;
                default:                       return strings_ES;
            }
        }
    }
}
