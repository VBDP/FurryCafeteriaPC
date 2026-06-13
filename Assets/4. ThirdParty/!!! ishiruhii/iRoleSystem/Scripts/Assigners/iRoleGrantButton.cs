// ============================================================================
// iRoleSystem v4.0.5 - Role Assignment
// Author: ishiruhii
// Description: Botón para asignar roles a usuarios específicos
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Assigners
{
    using ishiruhii.iRoleSystem.Core;

    /// <summary>
    /// Botón interactivo que asigna un rol específico solo a usuarios autorizados.
    /// Útil para dar roles especiales a administradores o usuarios específicos.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleGrantButton : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia al gestor de roles")]
        public iPlayerRoleManager playerRoleManager;

        [Tooltip("Referencia a la base de datos de roles")]
        public iRoleDatabase roleDatabase;

        [Header("Configuración de Rol")]
        [Tooltip("Índice del rol a otorgar")]
        public int roleToGrant = 0;

        [Header("Usuarios Autorizados")]
        [Tooltip("Lista de displayNames que pueden usar este botón")]
        public string[] authorizedUsers;

        [Header("Visual")]
        [Tooltip("Objeto visual del botón (se oculta después de usar)")]
        public GameObject buttonVisual;

        [Tooltip("Ocultar botón a usuarios no autorizados")]
        public bool hideForUnauthorized = true;

        [Header("Efectos")]
        [Tooltip("Sonido al otorgar rol")]
        public AudioSource grantSound;

        // Estado
        private VRCPlayerApi localPlayer;
        private bool hasBeenUsed = false;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;

            // Actualizar visibilidad inicial
            UpdateButtonVisibility();
        }

        public override void Interact()
        {
            if (hasBeenUsed) return;
            if (!IsAuthorized()) return;

            if (playerRoleManager == null || roleDatabase == null) return;
            if (!roleDatabase.IsValidRole(roleToGrant)) return;

            // Otorgar rol
            playerRoleManager.SetPlayerRole(localPlayer.playerId, roleToGrant);

            // Efecto de sonido
            if (grantSound != null)
            {
                grantSound.Play();
            }

            // Marcar como usado y ocultar
            hasBeenUsed = true;
            UpdateButtonVisibility();
        }

        private void UpdateButtonVisibility()
        {
            if (buttonVisual == null) return;

            if (hasBeenUsed)
            {
                buttonVisual.SetActive(false);
                return;
            }

            if (hideForUnauthorized && !IsAuthorized())
            {
                buttonVisual.SetActive(false);
                return;
            }

            buttonVisual.SetActive(true);
        }

        private bool IsAuthorized()
        {
            if (localPlayer == null) return false;
            if (authorizedUsers == null || authorizedUsers.Length == 0) return false;

            string playerName = localPlayer.displayName;

            for (int i = 0; i < authorizedUsers.Length; i++)
            {
                if (authorizedUsers[i] == playerName)
                    return true;
            }

            return false;
        }
    }
}
