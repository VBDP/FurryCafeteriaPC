// ============================================================================
// iRoleSystem v5.6 - Core Module
// Author: ishiruhii
// Description: Componente central que conecta todos los módulos del sistema
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Core
{
    /// <summary>
    /// Componente principal del sistema iRoleSystem.
    /// Actúa como hub central para todos los módulos.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleSystemCore : UdonSharpBehaviour
    {
        [Header("=== iRoleSystem v5.6 ===")]
        [Space(10)]

        [Header("Componentes Core")]
        [Tooltip("Base de datos de definición de roles")]
        public iRoleDatabase roleDatabase;

        [Tooltip("Gestor de roles de jugadores")]
        public iPlayerRoleManager playerRoleManager;

        // =====================================================================
        // IDIOMA
        // =====================================================================
        [Header("Idioma / Language / 言語 / 语言")]
        [Tooltip(
            "ES: Idioma del sistema\n" +
            "EN: System language\n" +
            "JP: システムの言語\n" +
            "ZH: 系统语言"
        )]
        public iSystemLanguage systemLanguage = iSystemLanguage.Spanish;

        [Tooltip("Módulo de localización — asigna el componente iLanguageSystem de la escena")]
        public iLanguageSystem languageSystem;

        // =====================================================================
        // CONFIGURACIÓN GLOBAL
        // =====================================================================
        [Header("Configuración Global")]
        [Tooltip("Activar mensajes de debug en consola")]
        public bool enableDebugLogs = false;

        [Tooltip("Intervalo de actualización global (frames)")]
        [Range(1, 10)]
        public int globalUpdateRate = 2;

        // =====================================================================
        // INICIO
        // =====================================================================
        private void Start()
        {
            // Sincronizar el idioma elegido en el Core con el módulo de lenguaje
            ApplyLanguage();

            ValidateSetup();

            if (enableDebugLogs)
            {
                string initMsg = languageSystem != null
                    ? languageSystem.Get(iLang.SYSTEM_INITIALIZED)
                    : "iRoleSystem v5.6 inicializado correctamente.";
                Log(initMsg);
            }
        }

        // =====================================================================
        // GESTIÓN DE IDIOMA
        // =====================================================================

        /// <summary>
        /// Aplica el idioma seleccionado en el Core al módulo iLanguageSystem.
        /// Llamar después de cambiar systemLanguage en runtime.
        /// </summary>
        public void ApplyLanguage()
        {
            if (languageSystem == null) return;
            languageSystem.SetLanguage(systemLanguage);
        }

        /// <summary>
        /// Cambia el idioma en runtime y lo aplica inmediatamente.
        /// </summary>
        public void SetLanguage(iSystemLanguage lang)
        {
            systemLanguage = lang;
            ApplyLanguage();
        }

        /// <summary>
        /// Cambia el idioma por índice entero (0=ES, 1=EN, 2=JP, 3=ZH).
        /// Útil para botones de UI en VRChat.
        /// </summary>
        public void SetLanguageByIndex(int index)
        {
            switch (index)
            {
                case 0: systemLanguage = iSystemLanguage.Spanish;  break;
                case 1: systemLanguage = iSystemLanguage.English;  break;
                case 2: systemLanguage = iSystemLanguage.Japanese; break;
                case 3: systemLanguage = iSystemLanguage.Chinese;  break;
                default: systemLanguage = iSystemLanguage.Spanish; break;
            }
            ApplyLanguage();
        }

        /// <summary>
        /// Obtiene el nombre localizado del idioma activo.
        /// </summary>
        public string GetCurrentLanguageName()
        {
            if (languageSystem != null) return languageSystem.GetLanguageName();
            return systemLanguage.ToString();
        }

        // =====================================================================
        // ACCESO RÁPIDO A LOCALIZACIÓN
        // =====================================================================

        /// <summary>
        /// Obtiene un string localizado (shortcut a languageSystem.Get)
        /// </summary>
        public string L(int stringId)
        {
            if (languageSystem == null) return "[?]";
            return languageSystem.Get(stringId);
        }

        /// <summary>
        /// Obtiene un string localizado con argumento (shortcut a languageSystem.Get)
        /// </summary>
        public string L(int stringId, string arg0)
        {
            if (languageSystem == null) return arg0;
            return languageSystem.Get(stringId, arg0);
        }

        // =====================================================================
        // VALIDACIÓN
        // =====================================================================
        private void ValidateSetup()
        {
            if (roleDatabase == null)
                LogError("iRoleDatabase no asignado!");

            if (playerRoleManager == null)
                LogError("iPlayerRoleManager no asignado!");

            if (languageSystem == null)
                LogWarning("iLanguageSystem no asignado. Se usarán textos por defecto.");
        }

        // =====================================================================
        // API PÚBLICA DE ACCESO RÁPIDO
        // =====================================================================

        /// <summary>Asigna un rol a un jugador (atajo)</summary>
        public void SetPlayerRole(VRCPlayerApi player, int roleIndex)
        {
            if (playerRoleManager == null || player == null) return;
            playerRoleManager.SetPlayerRole(player.playerId, roleIndex);
        }

        /// <summary>Obtiene el rol de un jugador (atajo)</summary>
        public int GetPlayerRole(VRCPlayerApi player)
        {
            if (playerRoleManager == null || player == null) return -1;
            return playerRoleManager.GetPlayerRole(player.playerId);
        }

        /// <summary>Obtiene el nombre del rol de un jugador (localizado)</summary>
        public string GetPlayerRoleName(VRCPlayerApi player)
        {
            int roleIndex = GetPlayerRole(player);
            if (roleDatabase == null)
                return L(iLang.NO_ROLE);
            return roleDatabase.GetRoleName(roleIndex);
        }

        /// <summary>Verifica si un jugador tiene permiso según array de roles permitidos</summary>
        public bool CheckRolePermission(VRCPlayerApi player, bool[] allowedRoles)
        {
            if (player == null || allowedRoles == null) return false;
            int roleIndex = GetPlayerRole(player);
            if (roleIndex < 0 || roleIndex >= allowedRoles.Length) return false;
            return allowedRoles[roleIndex];
        }

        /// <summary>Verifica si el jugador local tiene permiso</summary>
        public bool CheckLocalPermission(bool[] allowedRoles)
        {
            return CheckRolePermission(Networking.LocalPlayer, allowedRoles);
        }

        // =====================================================================
        // LOGGING
        // =====================================================================

        public void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[iRoleSystem] {message}");
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning($"[iRoleSystem] {message}");
        }

        public void LogError(string message)
        {
            Debug.LogError($"[iRoleSystem] {message}");
        }
    }
}
