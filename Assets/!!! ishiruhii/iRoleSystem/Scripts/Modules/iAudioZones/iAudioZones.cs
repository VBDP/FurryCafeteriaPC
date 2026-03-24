// ============================================================================
// iRoleSystem v5.7 - iAudioZones Module
// Author: ishiruhii
// Description: Zonas de separación de audio poligonales basadas en iAreaZone.
//              Permite que el audio dentro y fuera de la zona sean completamente
//              independientes, como si fueran canales de voz distintos.
//
//  Modos:
//   · Separación Única  : Los jugadores dentro se oyen entre sí, pero los de
//                         fuera no escuchan a los de dentro ni viceversa.
//   · Separación Total  : La zona es MUTE GLOBAL. Nadie se oye con nadie.
//
//  Audio de Mundo       : Opcionalmente silencia AudioSources específicos
//                         cuando el jugador local entra en la zona.
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

namespace ishiruhii.iRoleSystem.Modules.AudioZones
{
    using ishiruhii.iRoleSystem.Core;

    public enum AudioZoneMode
    {
        SeparacionUnica = 0,   // Dentro ↔ Dentro | Fuera ↔ Fuera (grupos separados)
        SeparacionTotal = 1    // MUTE GLOBAL dentro de la zona
    }

    [AddComponentMenu("iRoleSystem/Modules/iAudioZones")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iAudioZones : UdonSharpBehaviour
    {
        // ── Zona ──────────────────────────────────────────────────────────
        [Header("Zona de Audio")]
        [Tooltip("Zona poligonal 3D que define el área de separación de audio (iAreaZone).")]
        public iAreaZone zoneArea;

        // ── Modo ──────────────────────────────────────────────────────────
        [Header("Modo de Separación")]
        [Tooltip("SeparacionUnica: grupos de audio separados. SeparacionTotal: mute global dentro de la zona.")]
        public AudioZoneMode mode = AudioZoneMode.SeparacionUnica;

        // ── Audio de mundo ────────────────────────────────────────────────
        [Header("Audio de Mundo")]
        [Tooltip("Permite silenciar AudioSources del mundo cuando el jugador local está dentro de la zona.")]
        public bool enableWorldAudio = false;

        [Tooltip("AudioSources del mundo que deben silenciarse al entrar en la zona.\n" +
                 "Solo aplica si enableWorldAudio = true.")]
        public AudioSource[] worldAudioSources;

        // ── Configuración ─────────────────────────────────────────────────
        [Header("Configuración")]
        [Tooltip("Intervalo de chequeo de posición (segundos). Valor recomendado: 0.25–0.5")]
        [Range(0.05f, 2f)]
        public float checkInterval = 0.25f;

        [Tooltip("Nivel de voz para jugadores fuera de la zona cuando el jugador local está dentro.\n" +
                 "0 = silencio total, 1 = volumen normal.")]
        [Range(0f, 1f)]
        public float outsideVoiceGain = 0f;

        [Tooltip("Nivel de voz para jugadores dentro de la zona cuando el jugador local está fuera.\n" +
                 "0 = silencio total, 1 = volumen normal.")]
        [Range(0f, 1f)]
        public float insideVoiceGain = 0f;

        // ── Estado interno ────────────────────────────────────────────────
        private VRCPlayerApi   _localPlayer;
        private bool           _wasInsideZone  = false;
        private VRCPlayerApi[] _playerBuffer   = new VRCPlayerApi[80];

        // Voces originales (se guardan para restaurar al salir)
        private float _defaultVoiceGain        = 15f;  // VRChat default
        private float _defaultVoiceFar         = 25f;
        private float _defaultVoiceNear        = 0f;
        private float _defaultVoiceVolumetric  = 0f;
        private bool  _voicesSaved             = false;

        // ─────────────────────────────────────────────────────────────────

        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;
            if (_localPlayer == null) return;

            // Guardar configuración de voz default del primer jugador remoto encontrado
            // (en VRChat la API de voz es por jugador, los valores default son globales)
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

        // ── Separación Única ──────────────────────────────────────────────
        // El jugador local ESTÁ dentro:
        //   - Jugadores fuera  → su voz baja a outsideVoiceGain
        // El jugador local ESTÁ fuera:
        //   - Jugadores dentro → su voz baja a insideVoiceGain
        // ─────────────────────────────────────────────────────────────────

        private void ApplySeparacionUnica(bool localIsInside)
        {
            int count = VRCPlayerApi.GetPlayerCount();
            VRCPlayerApi.GetPlayers(_playerBuffer);

            for (int i = 0; i < count; i++)
            {
                VRCPlayerApi player = _playerBuffer[i];
                if (player == null || player.isLocal) continue;

                bool playerInside = zoneArea.ContainsPoint(player.GetPosition());

                if (localIsInside)
                {
                    // Local dentro: silenciar a los que están fuera
                    float gain = playerInside ? _defaultVoiceGain : outsideVoiceGain;
                    player.SetVoiceGain(gain);
                }
                else
                {
                    // Local fuera: silenciar a los que están dentro
                    float gain = playerInside ? insideVoiceGain : _defaultVoiceGain;
                    player.SetVoiceGain(gain);
                }
            }
        }

        // ── Separación Total ──────────────────────────────────────────────
        // Dentro de la zona = MUTE GLOBAL (nadie escucha a nadie)
        // ─────────────────────────────────────────────────────────────────

        private void ApplySeparacionTotal(bool localIsInside)
        {
            int count = VRCPlayerApi.GetPlayerCount();
            VRCPlayerApi.GetPlayers(_playerBuffer);

            for (int i = 0; i < count; i++)
            {
                VRCPlayerApi player = _playerBuffer[i];
                if (player == null || player.isLocal) continue;

                if (localIsInside)
                {
                    // Local dentro: silenciar a TODOS
                    player.SetVoiceGain(0f);
                }
                else
                {
                    // Local fuera: restaurar voz de los que están fuera
                    bool playerInside = zoneArea.ContainsPoint(player.GetPosition());
                    float gain = playerInside ? 0f : _defaultVoiceGain;
                    player.SetVoiceGain(gain);
                }
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

        // ── Eventos VRChat ────────────────────────────────────────────────

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            // Cuando entra un jugador nuevo, si el local está en la zona
            // aplicamos la configuración de voz correspondiente
            if (!_wasInsideZone || player == null || player.isLocal) return;

            bool playerInside = zoneArea != null && zoneArea.ContainsPoint(player.GetPosition());

            switch (mode)
            {
                case AudioZoneMode.SeparacionUnica:
                    player.SetVoiceGain(playerInside ? _defaultVoiceGain : outsideVoiceGain);
                    break;
                case AudioZoneMode.SeparacionTotal:
                    player.SetVoiceGain(0f);
                    break;
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            // Nada que hacer, el jugador ya se fue
        }
    }
}
