// ============================================================================
// iRoleSystem v4.0.5 - Objects Module
// Author: ishiruhii
// Description: Control de visibilidad de objetos según rol del jugador
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.Objects
{
    using ishiruhii.iRoleSystem.Core;

    /// <summary>
    /// Controla la visibilidad de objetos basándose en el rol del jugador local.
    /// Soporta objetos positivos (visibles con permiso) y negativos (visibles sin permiso).
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleObjectVisibility : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia al gestor de roles")]
        public iPlayerRoleManager playerRoleManager;

        [Header("Objetos a Controlar")]
        [Tooltip("Objetos visibles cuando el jugador TIENE permiso")]
        public GameObject[] visibleWithPermission;

        [Tooltip("Objetos que NO verán las membresías seleccionadas (visibles solo para roles sin permiso)")]
        public GameObject[] visibleWithoutPermission;

        [Header("Membresías con Permiso")]
        [Tooltip("Selecciona qué membresías (roles) tienen acceso a los objetos del grupo con permiso")]
        public bool[] allowedRoles;

        [Header("Configuración")]
        [Tooltip("Intervalo de chequeo en segundos")]
        public float checkInterval = 0.5f;

        [Tooltip("Activar al inicio aunque no haya rol")]
        public bool defaultVisibility = false;

        // Estado actual
        private int lastCheckedRole = -999;
        private bool lastHadPermission = false;

        private void Start()
        {
            // Aplicar estado inicial
            ApplyVisibility(defaultVisibility);

            // Iniciar chequeo periódico
            SendCustomEventDelayedSeconds(nameof(CheckRole), 0.5f);
        }

        /// <summary>
        /// Chequea el rol actual y actualiza visibilidad si es necesario
        /// </summary>
        public void CheckRole()
        {
            int currentRole = GetLocalPlayerRole();

            // Solo actualizar si cambió el rol
            if (currentRole != lastCheckedRole)
            {
                lastCheckedRole = currentRole;

                bool hasPermission = CheckPermission(currentRole);

                if (hasPermission != lastHadPermission)
                {
                    lastHadPermission = hasPermission;
                    ApplyVisibility(hasPermission);
                }
            }

            // Programar siguiente chequeo
            SendCustomEventDelayedSeconds(nameof(CheckRole), checkInterval);
        }

        /// <summary>
        /// Fuerza una actualización inmediata de visibilidad
        /// </summary>
        public void ForceUpdate()
        {
            int currentRole = GetLocalPlayerRole();
            bool hasPermission = CheckPermission(currentRole);
            ApplyVisibility(hasPermission);

            lastCheckedRole = currentRole;
            lastHadPermission = hasPermission;
        }

        private int GetLocalPlayerRole()
        {
            if (playerRoleManager == null) return -1;
            if (Networking.LocalPlayer == null) return -1;

            return playerRoleManager.GetPlayerRole(Networking.LocalPlayer.playerId);
        }

        private bool CheckPermission(int roleIndex)
        {
            if (allowedRoles == null) return false;
            if (roleIndex < 0 || roleIndex >= allowedRoles.Length) return false;

            return allowedRoles[roleIndex];
        }

        private void ApplyVisibility(bool hasPermission)
        {
            // Objetos visibles con permiso
            if (visibleWithPermission != null)
            {
                for (int i = 0; i < visibleWithPermission.Length; i++)
                {
                    if (visibleWithPermission[i] != null)
                    {
                        visibleWithPermission[i].SetActive(hasPermission);
                    }
                }
            }

            // Objetos visibles sin permiso
            if (visibleWithoutPermission != null)
            {
                for (int i = 0; i < visibleWithoutPermission.Length; i++)
                {
                    if (visibleWithoutPermission[i] != null)
                    {
                        visibleWithoutPermission[i].SetActive(!hasPermission);
                    }
                }
            }
        }

        /// <summary>
        /// Callback para evento de cambio de rol
        /// </summary>
        [HideInInspector] public int _lastChangedPlayerId;
        [HideInInspector] public int _lastChangedRole;

        public void _OnPlayerRoleChanged()
        {
            // Solo nos interesa si cambió el rol del jugador local
            if (Networking.LocalPlayer != null &&
                _lastChangedPlayerId == Networking.LocalPlayer.playerId)
            {
                ForceUpdate();
            }
        }
    }
}
