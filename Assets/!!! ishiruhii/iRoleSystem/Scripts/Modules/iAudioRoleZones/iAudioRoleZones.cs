// ============================================================================
// iRoleSystem v5.7 - iAudioRoleZones Module
// Author: ishiruhii
// Description: Variante de iAudioZones con control por roles.
//              Permite definir qué roles escuchan a quién dentro de la zona.
//
//  Funciona igual que iAudioZones pero añade capa de permisos de rol:
//   · Solo los jugadores con roles autorizados escuchan audio dentro de la zona.
//   · Los demás siguen las reglas de audio normales (o mute, según config).
//   · Roles exentos del filtro: pueden escuchar SIEMPRE independientemente de zona.
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.AudioZones
{
    using ishiruhii.iRoleSystem.Core;

    [AddComponentMenu("iRoleSystem/Modules/iAudioRoleZones")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iAudioRoleZones : UdonSharpBehaviour
    {
        // ── Referencias del sistema ───────────────────────────────────────
        [Header("Referencias del Sistema")]
        [Tooltip("Gestor de roles del sistema iRoleSystem.")]
        public iPlayerRoleManager playerRoleManager;

        [Tooltip("Base de datos de roles.")]
        public iRoleDatabase roleDatabase;

        // ── Zona ──────────────────────────────────────────────────────────
        [Header("Zona de Audio")]
        [Tooltip("Zona poligonal 3D que define el área (iAreaZone).")]
        public iAreaZone zoneArea;

        // ── Modo ──────────────────────────────────────────────────────────
        [Header("Modo de Separación")]
        [Tooltip("SeparacionUnica: grupos separados. SeparacionTotal: mute global dentro.")]
        public AudioZoneMode mode = AudioZoneMode.SeparacionUnica;

        // ── Permisos de Roles ─────────────────────────────────────────────
        [Header("Roles con Acceso al Canal de Audio de la Zona")]
        [Tooltip("Roles que participan en el canal de audio de la zona.\n" +
                 "Los que NO están marcados escuchan según las reglas de fuera de zona.\n" +
                 "Array sincronizado con roleDatabase.roleNames.")]
        public bool[] zoneAudioRoles;

        [Header("Roles Exentos (escuchan todo siempre)")]
        [Tooltip("Roles que nunca son filtrados por la zona (p.ej. Admins, Owners).\n" +
                 "Array sincronizado con roleDatabase.roleNames.")]
        public bool[] exemptRoles;

        // ── Audio de Mundo ────────────────────────────────────────────────
        [Header("Audio de Mundo")]
        [Tooltip("Silenciar AudioSources específicos del mundo al entrar en la zona.")]
        public bool enableWorldAudio = false;

        [Tooltip("AudioSources del mundo que se silencian al entrar.\nSolo aplica si enableWorldAudio = true.")]
        public AudioSource[] worldAudioSources;

        // ── Configuración ─────────────────────────────────────────────────
        [Header("Configuración")]
        [Range(0.05f, 2f)]
        [Tooltip("Intervalo de chequeo de posición (segundos).")]
        public float checkInterval = 0.25f;

        [Range(0f, 1f)]
        [Tooltip("Ganancia de voz para jugadores 'fuera' del canal cuando el local está dentro.\n0 = silencio.")]
        public float outsideVoiceGain = 0f;

        [Range(0f, 1f)]
        [Tooltip("Ganancia de voz para jugadores 'dentro' del canal cuando el local está fuera.\n0 = silencio.")]
        public float insideVoiceGain = 0f;

        // ── Estado interno ────────────────────────────────────────────────
        private VRCPlayerApi   _localPlayer;
        private bool           _wasInsideZone = false;
        private int            _localRole     = -1;
        private VRCPlayerApi[] _playerBuffer  = new VRCPlayerApi[80];

        private const float DefaultVoiceGain = 15f;

        // ─────────────────────────────────────────────────────────────────

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
            if (_localPlayer == null) return;

            SendCustomEventDelayedSeconds(nameof(TickCheck), checkInterval);
        }

        // ── Loop de chequeo ───────────────────────────────────────────────

        public void TickCheck()
        {
            if (_localPlayer == null || zoneArea == null)
            {
                SendCustomEventDelayedSeconds(nameof(TickCheck), checkInterval);
                return;
            }

            // Refrescar rol local (puede haber cambiado)
            _localRole = GetLocalRole();

            bool isInside = zoneArea.ContainsLocalPlayer();

            if (isInside != _wasInsideZone)
            {
                _wasInsideZone = isInside;
                OnZoneChanged(isInside);
            }

            SendCustomEventDelayedSeconds(nameof(TickCheck), checkInterval);
        }

        // ── Cambio de estado ──────────────────────────────────────────────

        private void OnZoneChanged(bool enteredZone)
        {
            switch (mode)
            {
                case AudioZoneMode.SeparacionUnica:
                    ApplySeparacionUnica(enteredZone);
                    break;
                case AudioZoneMode.SeparacionTotal:
                    ApplySeparacionTotal(enteredZone);
                    break;
            }

            if (enableWorldAudio)
                ApplyWorldAudio(enteredZone);
        }

        // ── Separación Única con Roles ────────────────────────────────────

        private void ApplySeparacionUnica(bool localIsInside)
        {
            bool localHasZoneRole   = RoleHasZoneAccess(_localRole);
            bool localIsExempt      = RoleIsExempt(_localRole);

            int count = VRCPlayerApi.GetPlayerCount();
            VRCPlayerApi.GetPlayers(_playerBuffer);

            for (int i = 0; i < count; i++)
            {
                VRCPlayerApi player = _playerBuffer[i];
                if (player == null || player.isLocal) continue;

                int  pRole      = GetPlayerRole(player.playerId);
                bool pExempt    = RoleIsExempt(pRole);
                bool pHasAccess = RoleHasZoneAccess(pRole);
                bool pInside    = zoneArea.ContainsPoint(player.GetPosition());

                // Exentos: siempre voz normal
                if (localIsExempt || pExempt)
                {
                    player.SetVoiceGain(DefaultVoiceGain);
                    continue;
                }

                // Ambos tienen acceso al canal de zona
                if (localHasZoneRole && pHasAccess)
                {
                    if (localIsInside)
                    {
                        // Local dentro: oye a los que también están dentro con acceso
                        float gain = pInside ? DefaultVoiceGain : outsideVoiceGain;
                        player.SetVoiceGain(gain);
                    }
                    else
                    {
                        // Local fuera: no oye a los de dentro con acceso
                        float gain = pInside ? insideVoiceGain : DefaultVoiceGain;
                        player.SetVoiceGain(gain);
                    }
                }
                else
                {
                    // Sin acceso al canal de zona: reglas normales fuera de zona
                    player.SetVoiceGain(DefaultVoiceGain);
                }
            }
        }

        // ── Separación Total con Roles ────────────────────────────────────

        private void ApplySeparacionTotal(bool localIsInside)
        {
            bool localIsExempt = RoleIsExempt(_localRole);

            int count = VRCPlayerApi.GetPlayerCount();
            VRCPlayerApi.GetPlayers(_playerBuffer);

            for (int i = 0; i < count; i++)
            {
                VRCPlayerApi player = _playerBuffer[i];
                if (player == null || player.isLocal) continue;

                int  pRole    = GetPlayerRole(player.playerId);
                bool pExempt  = RoleIsExempt(pRole);
                bool pInside  = zoneArea.ContainsPoint(player.GetPosition());

                // Exentos: siempre oyen
                if (localIsExempt || pExempt)
                {
                    player.SetVoiceGain(DefaultVoiceGain);
                    continue;
                }

                // MUTE GLOBAL si el local está dentro
                if (localIsInside)
                    player.SetVoiceGain(0f);
                else
                    player.SetVoiceGain(pInside ? 0f : DefaultVoiceGain);
            }
        }

        // ── Audio de Mundo ────────────────────────────────────────────────

        private void ApplyWorldAudio(bool localIsInside)
        {
            if (worldAudioSources == null) return;
            foreach (AudioSource src in worldAudioSources)
            {
                if (src == null) continue;
                src.mute = localIsInside;
            }
        }

        // ── Helpers de Rol ────────────────────────────────────────────────

        private int GetLocalRole()
        {
            if (playerRoleManager == null || _localPlayer == null) return -1;
            return playerRoleManager.GetPlayerRole(_localPlayer.playerId);
        }

        private int GetPlayerRole(int playerId)
        {
            if (playerRoleManager == null) return -1;
            return playerRoleManager.GetPlayerRole(playerId);
        }

        private bool RoleHasZoneAccess(int roleIndex)
        {
            if (zoneAudioRoles == null || roleIndex < 0 || roleIndex >= zoneAudioRoles.Length) return false;
            return zoneAudioRoles[roleIndex];
        }

        private bool RoleIsExempt(int roleIndex)
        {
            if (exemptRoles == null || roleIndex < 0 || roleIndex >= exemptRoles.Length) return false;
            return exemptRoles[roleIndex];
        }

        // ── Eventos VRChat ────────────────────────────────────────────────

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!_wasInsideZone || player == null || player.isLocal) return;
            if (zoneArea == null) return;

            int  pRole    = GetPlayerRole(player.playerId);
            bool pExempt  = RoleIsExempt(pRole);

            if (pExempt) return;

            switch (mode)
            {
                case AudioZoneMode.SeparacionUnica:
                    bool pHasAccess = RoleHasZoneAccess(pRole);
                    bool localHas   = RoleHasZoneAccess(_localRole);
                    if (localHas && pHasAccess)
                        player.SetVoiceGain(outsideVoiceGain); // nuevo jugador fuera por defecto
                    break;
                case AudioZoneMode.SeparacionTotal:
                    player.SetVoiceGain(0f);
                    break;
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player) { }
    }
}
