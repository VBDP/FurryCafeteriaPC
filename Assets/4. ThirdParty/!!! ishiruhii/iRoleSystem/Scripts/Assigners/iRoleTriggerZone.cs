// ============================================================================
// iRoleSystem v4.0.5 - Role Assignment
// Author: ishiruhii
// Description: Asignador de roles por zona trigger
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Assigners
{
    using ishiruhii.iRoleSystem.Core;

    /// <summary>
    /// Asigna un rol específico a jugadores que entran en la zona trigger.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleTriggerZone : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia al gestor de roles")]
        public iPlayerRoleManager playerRoleManager;

        [Tooltip("Referencia a la base de datos de roles")]
        public iRoleDatabase roleDatabase;

        [Header("Configuración")]
        [Tooltip("Índice del rol a asignar")]
        public int roleToAssign = 0;

        [Tooltip("Solo asignar si el jugador no tiene rol")]
        public bool onlyIfNoRole = false;

        [Header("Efectos")]
        [Tooltip("Sonido al asignar rol")]
        public AudioSource assignSound;

        private void Start()
        {
            // Asegurar que sea trigger
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (playerRoleManager == null) return;
            if (roleDatabase == null) return;

            // Validar rol
            if (!roleDatabase.IsValidRole(roleToAssign)) return;

            // Verificar si solo asignar a jugadores sin rol
            if (onlyIfNoRole)
            {
                int currentRole = playerRoleManager.GetPlayerRole(player.playerId);
                if (currentRole >= 0) return;
            }

            // Asignar rol
            playerRoleManager.SetPlayerRole(player.playerId, roleToAssign);

            // Efecto de sonido
            if (assignSound != null && player.isLocal)
            {
                assignSound.Play();
            }
        }
    }
}
