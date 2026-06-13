// ============================================================================
// iRoleSystem v5.6 - PrivateZone Module (iAreaZone edition)
// Author: ishiruhii
// Description: Zonas privadas/VIP con acceso restringido por rol.
//              Usa iAreaZone en lugar de BoxCollider/Trigger.
//              Si el jugador no tiene el rol → se teleporta al ejection point.
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.PrivateZone
{
    using ishiruhii.iRoleSystem.Core;

    [AddComponentMenu("iRoleSystem/Modules/iPrivateZoneManager")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iPrivateZoneManager : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        public iPlayerRoleManager playerRoleManager;

        [Header("Idioma")]
        [Tooltip("Referencia al iLanguageSystem para mensajes localizados")]
        public iLanguageSystem languageSystem;

        [Header("Configuración de Zona")]
        [Tooltip("Zona poligonal que define el área privada (iAreaZone)")]
        public iAreaZone zoneArea;

        [Tooltip("Punto de teletransporte al que se envía al jugador sin permiso (tp-point)")]
        public Transform ejectionPoint;

        [Tooltip("Mensaje de expulsión personalizado (deja vacío para usar el idioma activo)")]
        public string ejectionMessage = "";

        [Header("Roles Permitidos")]
        public bool[] allowedRoles;

        [Header("Configuración de Chequeo")]
        [Tooltip("Intervalo de chequeo de posición dentro de la zona (segundos)")]
        public float checkInterval = 0.5f;

        [Header("Efectos")]
        public AudioSource ejectionSound;

        // Estado interno
        private VRCPlayerApi localPlayer;

        private void Start()
        {
            localPlayer = Networking.LocalPlayer;
            if (localPlayer == null) return;

            SendCustomEventDelayedSeconds(nameof(CheckZone), checkInterval);
        }

        public void CheckZone()
        {
            if (localPlayer != null && zoneArea != null)
            {
                if (zoneArea.ContainsLocalPlayer() && !HasPermission(localPlayer))
                {
                    EjectPlayer(localPlayer);
                }
            }

            SendCustomEventDelayedSeconds(nameof(CheckZone), checkInterval);
        }

        public bool HasPermission(VRCPlayerApi player)
        {
            if (player == null || playerRoleManager == null) return false;
            int roleIndex = playerRoleManager.GetPlayerRole(player.playerId);
            if (roleIndex < 0 || allowedRoles == null || roleIndex >= allowedRoles.Length) return false;
            return allowedRoles[roleIndex];
        }

        public bool LocalPlayerHasPermission() => HasPermission(Networking.LocalPlayer);

        public string GetEjectionMessage()
        {
            if (!string.IsNullOrEmpty(ejectionMessage))
                return ejectionMessage;

            if (languageSystem != null)
                return languageSystem.Get(iLang.NO_PERMISSION_ENTER);

            return "No tienes permiso para entrar aquí.";
        }

        private void EjectPlayer(VRCPlayerApi player)
        {
            if (ejectionPoint == null) return;

            if (ejectionSound != null)
                ejectionSound.Play();

            player.TeleportTo(ejectionPoint.position, ejectionPoint.rotation);
        }
    }
}
