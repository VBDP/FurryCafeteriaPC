// ============================================================================
// iRoleSystem v4.0.5 - iPickup Module
// Author: ishiruhii
// Description: Control de VRCPickups según el rol del jugador local.
//              Los roles con permiso pueden agarrar el objeto.
//              Los roles sin permiso solo lo ven (pickup desactivado).
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

namespace ishiruhii.iRoleSystem.Modules.Pickup
{
    using ishiruhii.iRoleSystem.Core;

    /// <summary>
    /// Controla si el jugador local puede agarrar (VRCPickup) un objeto,
    /// basándose en su rol asignado en iPlayerRoleManager.
    /// Los roles no permitidos ven el objeto pero no pueden interactuar.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRolePickup : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia al gestor de roles del sistema.")]
        public iPlayerRoleManager playerRoleManager;

        [Header("Pickups Controlados")]
        [Tooltip("Lista de VRCPickups cuyo acceso será controlado por rol.")]
        public VRCPickup[] pickups;

        [Header("Membresías con Permiso")]
        [Tooltip("Array paralelo a los roles definidos en iRoleDatabase. " +
                 "Marca 'true' en los índices de los roles que pueden agarrar los pickups.")]
        public bool[] allowedRoles;

        [Header("Configuración")]
        [Tooltip("Intervalo en segundos para comprobar el rol del jugador.")]
        public float checkInterval = 0.5f;

        [Tooltip("¿Pueden agarrar los pickups los jugadores SIN rol asignado (rol = -1)?")]
        public bool allowNoRole = false;

        // ── Estado interno ──────────────────────────────────────────────────
        private int  _lastCheckedRole    = -999;
        private bool _lastHadPermission  = false;

        // ── Callbacks para el sistema de notificación de roles ─────────────
        [HideInInspector] public int _lastChangedPlayerId;
        [HideInInspector] public int _lastChangedRole;

        // ───────────────────────────────────────────────────────────────────

        private void Start()
        {
            // Estado seguro inicial: nadie puede agarrar hasta verificar el rol
            SetPickupable(false);
            SendCustomEventDelayedSeconds(nameof(CheckRole), 0.5f);
        }

        /// <summary>
        /// Comprueba el rol actual del jugador local y actualiza los pickups.
        /// Se llama periódicamente mediante SendCustomEventDelayedSeconds.
        /// </summary>
        public void CheckRole()
        {
            int currentRole = GetLocalPlayerRole();

            if (currentRole != _lastCheckedRole)
            {
                _lastCheckedRole = currentRole;
                bool hasPermission = CheckPermission(currentRole);

                if (hasPermission != _lastHadPermission)
                {
                    _lastHadPermission = hasPermission;
                    SetPickupable(hasPermission);
                }
            }

            SendCustomEventDelayedSeconds(nameof(CheckRole), checkInterval);
        }

        /// <summary>
        /// Fuerza una actualización inmediata del estado de los pickups.
        /// Puede llamarse desde otros scripts o desde el Editor en PlayMode.
        /// </summary>
        public void ForceUpdate()
        {
            int currentRole = GetLocalPlayerRole();
            bool hasPermission = CheckPermission(currentRole);
            SetPickupable(hasPermission);

            _lastCheckedRole   = currentRole;
            _lastHadPermission = hasPermission;
        }

        // ── Callback del sistema de notificación ────────────────────────────

        /// <summary>
        /// Llamado por iPlayerRoleManager cuando cambia el rol de cualquier jugador.
        /// Solo actúa si el jugador afectado es el jugador local.
        /// </summary>
        public void _OnPlayerRoleChanged()
        {
            if (Networking.LocalPlayer == null) return;
            if (_lastChangedPlayerId == Networking.LocalPlayer.playerId)
                ForceUpdate();
        }

        // ── Helpers privados ────────────────────────────────────────────────

        private int GetLocalPlayerRole()
        {
            if (playerRoleManager == null) return -1;
            if (Networking.LocalPlayer == null) return -1;
            return playerRoleManager.GetPlayerRole(Networking.LocalPlayer.playerId);
        }

        private bool CheckPermission(int roleIndex)
        {
            // Jugador sin rol asignado
            if (roleIndex < 0)
                return allowNoRole;

            if (allowedRoles == null) return false;
            if (roleIndex >= allowedRoles.Length) return false;

            return allowedRoles[roleIndex];
        }

        private void SetPickupable(bool canPickup)
        {
            if (pickups == null) return;

            for (int i = 0; i < pickups.Length; i++)
            {
                if (pickups[i] == null) continue;
                pickups[i].pickupable = canPickup;
            }
        }
    }
}
