// ============================================================================
// iRoleSystem v5.6 - Prison Module (iAreaZone edition)
// Author: ishiruhii
// Description: Sistema de cárcel que confina jugadores según su rol.
//              Usa iAreaZone en lugar de BoxCollider para definir el área.
//              Si el jugador intenta salir → se teleporta al tp point.
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.Prison
{
    using ishiruhii.iRoleSystem.Core;

    [AddComponentMenu("iRoleSystem/Modules/iPrisonManager")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iPrisonManager : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia al gestor de roles")]
        public iPlayerRoleManager playerRoleManager;

        [Tooltip("Referencia a la base de datos de roles")]
        public iRoleDatabase roleDatabase;

        [Header("Configuración de la Cárcel")]
        [Tooltip("Zona poligonal que define el área de la cárcel (iAreaZone)")]
        public iAreaZone prisonArea;

        [Tooltip("Punto de teletransporte DENTRO de la cárcel (tp-point)")]
        public Transform teleportPoint;

        [Header("Roles Afectados")]
        [Tooltip("Qué roles son enviados/confinados a la cárcel")]
        public bool[] affectedRoles;

        [Header("Configuración de Chequeo")]
        [Tooltip("Intervalo de chequeo de posición (segundos)")]
        public float positionCheckInterval = 1f;

        [Tooltip("Activar chequeo dinámico de rol")]
        public bool enableDynamicRoleCheck = true;

        [Tooltip("Intervalo de chequeo de rol (segundos)")]
        public float roleCheckInterval = 3f;

        [Header("Efectos")]
        [Tooltip("Sonido al teletransportar")]
        public AudioSource teleportSound;

        [Tooltip("Partículas al teletransportar")]
        public ParticleSystem teleportParticles;

        // Estado interno
        private VRCPlayerApi localPlayer;
        private int  currentRole          = -1;
        private bool isConfined           = false;
        private bool initialTeleportDone  = false;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;
            if (localPlayer == null) return;

            currentRole = GetLocalPlayerRole();

            SendCustomEventDelayedSeconds(nameof(CheckPosition), 0.5f);

            if (enableDynamicRoleCheck)
                SendCustomEventDelayedSeconds(nameof(CheckRoleUpdate), roleCheckInterval);
        }

        public void CheckPosition()
        {
            if (localPlayer == null || prisonArea == null)
            {
                SendCustomEventDelayedSeconds(nameof(CheckPosition), positionCheckInterval);
                return;
            }

            bool shouldBeConfined = IsRoleAffected(currentRole);

            if (shouldBeConfined)
            {
                if (!initialTeleportDone)
                {
                    ExecuteTeleport();
                    initialTeleportDone = true;
                    isConfined = true;
                }
                else if (!prisonArea.ContainsLocalPlayer())
                {
                    ExecuteTeleport();
                }
            }
            else
            {
                isConfined          = false;
                initialTeleportDone = false;
            }

            SendCustomEventDelayedSeconds(nameof(CheckPosition), positionCheckInterval);
        }

        public void CheckRoleUpdate()
        {
            int newRole = GetLocalPlayerRole();

            if (newRole != currentRole)
            {
                currentRole = newRole;

                if (IsRoleAffected(currentRole) && !isConfined)
                    initialTeleportDone = false;
            }

            SendCustomEventDelayedSeconds(nameof(CheckRoleUpdate), roleCheckInterval);
        }

        [HideInInspector] public int _lastChangedPlayerId;
        [HideInInspector] public int _lastChangedRole;

        public void _OnPlayerRoleChanged()
        {
            if (localPlayer != null && _lastChangedPlayerId == localPlayer.playerId)
            {
                currentRole = _lastChangedRole;

                if (IsRoleAffected(currentRole) && !isConfined)
                    initialTeleportDone = false;
            }
        }

        public void ForceTeleportToPrison() => ExecuteTeleport();

        public void ReleasePlayer()
        {
            isConfined          = false;
            initialTeleportDone = false;
        }

        public bool IsCurrentlyConfined() => isConfined;

        private int GetLocalPlayerRole()
        {
            if (playerRoleManager == null || localPlayer == null) return -1;
            return playerRoleManager.GetPlayerRole(localPlayer.playerId);
        }

        private bool IsRoleAffected(int roleIndex)
        {
            if (affectedRoles == null) return false;
            if (roleIndex < 0 || roleIndex >= affectedRoles.Length) return false;
            return affectedRoles[roleIndex];
        }

        private void ExecuteTeleport()
        {
            if (teleportPoint == null || localPlayer == null) return;

            if (teleportSound != null)     teleportSound.Play();
            if (teleportParticles != null) teleportParticles.Play();

            localPlayer.TeleportTo(teleportPoint.position, teleportPoint.rotation);
        }
    }
}
