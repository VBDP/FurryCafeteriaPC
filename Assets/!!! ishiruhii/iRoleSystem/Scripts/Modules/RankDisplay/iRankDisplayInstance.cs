// ============================================================================
// iRoleSystem v4.0.5 - RankDisplay Module
// Author: ishiruhii
// Description: Instancia individual de display de rango sobre un jugador
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.RankDisplay
{
    /// <summary>
    /// Instancia de display que sigue a un jugador específico.
    /// Se crea dinámicamente por el iRankDisplayManager.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class iRankDisplayInstance : UdonSharpBehaviour
    {
        [UdonSynced] private int syncedPlayerId = -1;
        [UdonSynced] private int syncedRoleIndex = -1;
        [UdonSynced] private float syncedHeightOffset = 0.35f;

        [Header("Configuración Runtime")]
        public bool billboardEnabled = true;
        public bool billboardYOnly = true;

        private VRCPlayerApi targetPlayer;
        private VRCPlayerApi localPlayer;
        private int frameCounter;
        private int updateRate = 2;

        /// <summary>
        /// Inicializa la instancia para seguir a un jugador
        /// </summary>
        public void Initialize(VRCPlayerApi player, int roleIndex, float heightOffset, int updateEveryXFrames, bool billboard, bool yOnly)
        {
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            syncedPlayerId = player.playerId;
            syncedRoleIndex = roleIndex;
            syncedHeightOffset = heightOffset;
            updateRate = Mathf.Max(1, updateEveryXFrames);

            billboardEnabled = billboard;
            billboardYOnly = yOnly;

            targetPlayer = player;

            RequestSerialization();
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Actualiza el rol mostrado
        /// </summary>
        public void UpdateRole(int newRoleIndex, float newHeightOffset)
        {
            if (!Networking.IsOwner(gameObject))
                Networking.SetOwner(Networking.LocalPlayer, gameObject);

            syncedRoleIndex = newRoleIndex;
            syncedHeightOffset = newHeightOffset;
            RequestSerialization();
        }

        /// <summary>
        /// Obtiene el ID del jugador asociado
        /// </summary>
        public int GetPlayerId()
        {
            return syncedPlayerId;
        }

        /// <summary>
        /// Desactiva y limpia la instancia
        /// </summary>
        public void Deactivate()
        {
            syncedPlayerId = -1;
            syncedRoleIndex = -1;
            targetPlayer = null;
            gameObject.SetActive(false);
        }

        public override void OnDeserialization()
        {
            targetPlayer = VRCPlayerApi.GetPlayerById(syncedPlayerId);

            if (targetPlayer == null || !targetPlayer.IsValid())
            {
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            // Obtener el jugador local usando VRC Player API
            localPlayer = Networking.LocalPlayer;
        }

        private void LateUpdate()
        {
            if (targetPlayer == null || !targetPlayer.IsValid())
            {
                return;
            }

            // Optimización: actualizar cada X frames
            frameCounter++;
            if (frameCounter < updateRate)
                return;

            frameCounter = 0;

            // Posicionar sobre la cabeza
            Vector3 headPosition = targetPlayer.GetBonePosition(HumanBodyBones.Head);

            // Fallback si no hay hueso de cabeza
            if (headPosition == Vector3.zero)
            {
                headPosition = targetPlayer.GetPosition() + Vector3.up * 1.7f;
            }

            transform.position = headPosition + Vector3.up * syncedHeightOffset;

            // Billboard hacia la cámara del jugador local usando VRC Player API
            if (billboardEnabled && localPlayer != null && localPlayer.IsValid())
            {
                // Obtener la posición de la cabeza del jugador local desde tracking data
                Vector3 cameraPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

                if (billboardYOnly)
                {
                    Vector3 lookDir = cameraPosition - transform.position;
                    lookDir.y = 0;
                    if (lookDir != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(-lookDir);
                    }
                }
                else
                {
                    transform.LookAt(cameraPosition);
                    transform.Rotate(0, 180, 0);
                }
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }
        }
    }
}
