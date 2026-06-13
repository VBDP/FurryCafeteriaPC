// ============================================================================
// iRoleSystem v5.6 - Core Module
// Author: ishiruhii
// Description: Base de datos central de roles del sistema
// ============================================================================

using UdonSharp;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Core
{
    /// <summary>
    /// Almacena la definición de todos los roles disponibles en el mundo.
    /// Debe existir una única instancia en la escena.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleDatabase : UdonSharpBehaviour
    {
        [Header("Configuración de Roles")]
        [Tooltip("Lista de nombres de roles disponibles en el mundo")]
        public string[] roleNames = new string[] { "Visitante", "VIP", "Moderador", "Admin" };

        [Header("Colores de Roles (Opcional)")]
        [Tooltip("Colores asociados a cada rol para UI")]
        public Color[] roleColors = new Color[] { Color.gray, Color.yellow, Color.cyan, Color.red };

        [Header("Iconos de Roles (Opcional)")]
        [Tooltip("Sprites/iconos para cada rol")]
        public Sprite[] roleIcons;

        // =====================================================================
        // Referencia al sistema de lenguaje para el fallback "Sin Rol"
        // =====================================================================
        [Header("Idioma")]
        [Tooltip("Referencia al iLanguageSystem (mismo que usa iRoleSystemCore)")]
        public iLanguageSystem languageSystem;

        // =====================================================================
        // API
        // =====================================================================

        /// <summary>
        /// Obtiene el nombre de un rol por su índice.
        /// Si el índice es inválido devuelve el texto "Sin Rol" en el idioma activo.
        /// </summary>
        public string GetRoleName(int roleIndex)
        {
            if (roleIndex < 0 || roleIndex >= roleNames.Length)
            {
                if (languageSystem != null)
                    return languageSystem.Get(iLang.NO_ROLE);
                return "Sin Rol";
            }
            return roleNames[roleIndex];
        }

        /// <summary>Obtiene el color de un rol por su índice</summary>
        public Color GetRoleColor(int roleIndex)
        {
            if (roleColors == null || roleIndex < 0 || roleIndex >= roleColors.Length)
                return Color.white;
            return roleColors[roleIndex];
        }

        /// <summary>Obtiene el número total de roles definidos</summary>
        public int GetRoleCount()
        {
            return roleNames != null ? roleNames.Length : 0;
        }

        /// <summary>Verifica si un índice de rol es válido</summary>
        public bool IsValidRole(int roleIndex)
        {
            return roleIndex >= 0 && roleIndex < roleNames.Length;
        }
    }
}
