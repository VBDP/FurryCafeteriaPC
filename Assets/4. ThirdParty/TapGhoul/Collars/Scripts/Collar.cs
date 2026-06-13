//#define TAPGHOUL_COLLAR_DEBUG

using System;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace TapGhoul.Collars
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Collar : UdonSharpBehaviour
    {
        #region Config

        [UdonSynced] private int _currentPlayerId = -1;

        [Tooltip("Length of leash")] public float leashLength = 4f;

        [Tooltip("Distance the handle spawns from the collared player on leashing")]
        public float handleSpawnOffset = 0.25f;

        [Tooltip("Maximum distance to search for a neck on drop")]
        public float maxNeckDistance = 0.25f;

        [Tooltip("Allow a player to unleash themselves by respawning")]
        public bool respawnToUnleash = true;

        public LineRenderer leash;
        public VRCPickup collar;
        public VRCPickup handle;

        #endregion

        #region State

        private VRCObjectSync _handleSync;
        private GenerationTracker _handleGenerationTracker;

        private Rigidbody _collarBody;
        private Rigidbody _handleBody;

        [UdonSynced] private short _handleGeneration = 0;

        private VRCPlayerApi[] _players = new VRCPlayerApi[120];
        private int _playerCount = 0;

        private Vector3 _collarTrackingPosition = Vector3.zero;
        private Quaternion _collarTrackingRotation = Quaternion.identity;

        #endregion

        #region VRChat Events

        private void Start()
        {
            _handleSync = handle.GetComponent<VRCObjectSync>();
            _handleGenerationTracker = handle.GetComponent<GenerationTracker>();
            _collarBody = collar.GetComponent<Rigidbody>();
            _handleBody = handle.GetComponent<Rigidbody>();
        }

        private void OnEnable() => RefreshPlayers();

        public override void OnDeserialization(DeserializationResult result) => RefreshSyncedState();

#if TAPGHOUL_COLLAR_DEBUG
        public void Update()
        {
            if (!(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))) return;

            if (_currentPlayerId == Networking.LocalPlayer.playerId) Unleash();
            else
            {
                _collarBody.position = Networking.LocalPlayer.GetBonePosition(HumanBodyBones.Neck);
                Leash(Networking.LocalPlayer);
            }
        }
#endif

        public override void PostLateUpdate()
        {
            var player = VRCPlayerApi.GetPlayerById(_currentPlayerId);
            // Player unset, or has disconnected
            if (!Utilities.IsValid(player)) return;

            UpdateCollarTarget(player);

            // Increment generation to avoid accidental teleport when position is not yet synced
            if (player.isLocal && _handleGeneration == _handleGenerationTracker.generation)
            {
                var offset = _handleBody.position - _collarTrackingPosition;
                var distanceFromEdge = offset.magnitude - leashLength;

                if (distanceFromEdge > 0)
                {
                    _collarTrackingPosition = TeleportIntoBounds(_collarTrackingPosition);

                    // Avoid fall speed accumulating to where we can no longer access our menu
                    var velocity = player.GetVelocity();
                    var gravityStrength = Physics.gravity.y * player.GetGravityStrength();
                    if (velocity.y <= gravityStrength)
                    {
                        velocity.y = gravityStrength;
                        player.SetVelocity(velocity);
                    }
                }
            }

            // We don't use .Move() because we don't want to interpolate the visual component
            _collarBody.position = _collarTrackingPosition;
            _collarBody.rotation = _collarTrackingRotation;

            UpdateLeash();
        }

        public override void OnPlayerJoined(VRCPlayerApi player) => RefreshPlayers();

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            // Fix to stop error spam if we are leaving the instance
            if (!Utilities.IsValid(player)) return;

            RefreshPlayers();

            // Even if the instance master has this disabled, we don't need to worry.
            // The player validity check will fail anyway.
            if (player.playerId == _currentPlayerId && Networking.IsOwner(gameObject))
            {
                Unleash();
            }
        }

        public override void OnPlayerRespawn(VRCPlayerApi player)
        {
            if (!respawnToUnleash || !player.isLocal || player.playerId != _currentPlayerId) return;
            Networking.SetOwner(player, gameObject);
            Unleash();
        }

        #endregion

        #region Delegate events

        [PublicAPI]
        public void _OnCollarPickup()
        {
            Unleash();
        }

        [PublicAPI]
        public void _OnCollarDrop()
        {
            var searchPos = _collarBody.position;
            for (var i = 0; i < _playerCount; i++)
            {
                var player = _players[i];

                if (!Utilities.IsValid(player) || player.isLocal) continue;

                var position = player.GetBonePosition(HumanBodyBones.Neck);
                var distance = (position - searchPos).sqrMagnitude;
                if (distance > maxNeckDistance) continue;

                Leash(player);
                break;
            }
        }

        /*
         * This is now unused as of v1.1.0, but kept here just in case we decide to use it later.

        [PublicAPI]
        public void _OnLeashPickup()
        {
        }

        [PublicAPI]
        public void _OnLeashDrop()
        {
        }
        */

        #endregion

        #region Actions

        private void Leash(VRCPlayerApi player)
        {
            // This is handled on pickup in PickupDelegator
            // Networking.SetOwner(Networking.LocalPlayer, gameObject);

            _currentPlayerId = player.playerId;
            RequestSerialization();
            RefreshSyncedState();

            // Move handle into place
            Networking.SetOwner(Networking.LocalPlayer, handle.gameObject);
            // Increment generation to avoid accidental teleport when position is not yet synced
            _handleGenerationTracker.generation = ++_handleGeneration;

            var forward = Networking.LocalPlayer.GetPosition() - player.GetPosition();
            forward.y = 0;
            _handleBody.position = _collarBody.position + (forward.normalized * handleSpawnOffset);
            _handleSync.FlagDiscontinuity();
        }

        private void Unleash()
        {
            // This is handled on pickup in PickupDelegator
            // Networking.SetOwner(Networking.LocalPlayer, gameObject);

            _currentPlayerId = -1;
            RequestSerialization();
            RefreshSyncedState();
        }

        private void RefreshPlayers()
        {
            _playerCount = VRCPlayerApi.GetPlayerCount();
            // Idk I'm paranoid ok
            if (_playerCount > _players.Length)
            {
                _players = new VRCPlayerApi[_playerCount];
            }

            VRCPlayerApi.GetPlayers(_players);
        }

        private void RefreshSyncedState()
        {
            var isLeashed = _currentPlayerId != -1;
            var isLocalLeashed = _currentPlayerId == Networking.LocalPlayer.playerId;

            if (isLeashed)
            {
                _PollReadyLeashed();
            }
            else
            {
                leash.enabled = false;
            }

            /*
             * !!! THIS IS BROKEN! !!!
             * As it turns out, we cannot actually enable/disable collar sync here - it breaks the VRCPickup.
             * The problem is if you disable it while held (or while in held/unheld transition?) the VRCPickup
             * ends up in a "limbo" state of pickup.IsHeld is false, but the internal machinery thinks it *is* being held.
             * As a result, remote players can no longer pick it up at any time. This would need to be solved by a custom
             * pickup sync system - not something that is worth doing for this project.
             */
            // Don't network-sync the collar if we are leashed - we manage this manually
            //_collarSync.enabled = !isLeashed;

            // Only show the handle and leash if someone is leashed
            handle.gameObject.SetActive(isLeashed);

            // Don't allow leashed person to take it off
            collar.pickupable = !isLocalLeashed;
            handle.pickupable = !isLocalLeashed;
        }

        private void UpdateCollarTarget(VRCPlayerApi player)
        {
            _collarTrackingPosition = player.GetBonePosition(HumanBodyBones.Neck);
            if (_collarTrackingPosition != Vector3.zero)
            {
                _collarTrackingRotation = player.GetBoneRotation(HumanBodyBones.Neck);
                return;
            }

            _collarTrackingPosition = player.GetBonePosition(HumanBodyBones.Head);
            if (_collarTrackingPosition != Vector3.zero)
            {
                _collarTrackingRotation = player.GetBoneRotation(HumanBodyBones.Head);
                return;
            }

            var trackingData = player.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot);
            _collarTrackingPosition = trackingData.position;
            _collarTrackingRotation = trackingData.rotation;
        }

        /// <summary>
        /// Teleport into bounds
        /// </summary>
        /// <param name="neckPosition">Neck position (optimization)</param>
        /// <returns>New neck position (optimization)</returns>
        private Vector3 TeleportIntoBounds(Vector3 neckPosition)
        {
            var player = Networking.LocalPlayer;

            // Do all calculations based on player position
            var playerRootPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.AvatarRoot).position;

            // We need to keep track of the playspace origin/offset in order to get teleport to teleport within the correct coordinate space
            var playspaceOrigin = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin);
            var playspaceOffset = playspaceOrigin.position - playerRootPosition;

            var neckOffset = neckPosition - playerRootPosition;
            var handlePosition = _handleBody.position - neckOffset;


            var naiveLeashTarget =
                handlePosition + ((playerRootPosition - handlePosition).normalized * leashLength);

            // Perform raycast
            var rayOrigin = naiveLeashTarget;
            rayOrigin.y = handlePosition.y;
            var ray = new Ray(
                rayOrigin + (neckOffset / 2),
                -neckOffset
            );
            var raycastResults = new RaycastHit[8];
            var raycastResultCount = Physics.RaycastNonAlloc(ray, raycastResults, neckOffset.magnitude,
                0b100000000001,
                QueryTriggerInteraction.Ignore);

            if (raycastResultCount == 0)
            {
                // Don't bother grounding, early exit
                player.TeleportTo(naiveLeashTarget + playspaceOffset, playspaceOrigin.rotation,
                    VRC_SceneDescriptor.SpawnOrientation.AlignRoomWithSpawnPoint,
                    true);
                return naiveLeashTarget + neckOffset;
            }

            // Find highest point (closest to ray origin)
            // Why unity doesn't do this internally, I have no idea.
            var targetPoint = raycastResults[0].point;
            for (var i = 1; i < raycastResultCount; i++)
            {
                var point = raycastResults[i].point;
                if (point.y > targetPoint.y) targetPoint = point;
            }

            var targetDistance = (targetPoint - handlePosition).magnitude;

            // Calculate triangle, a = height, b = ?, c = leashLength
            var triangleHeight = targetPoint.y - handlePosition.y;
            var horizontalDistance = (float)Math.Sqrt(leashLength * leashLength - triangleHeight * triangleHeight);
            // 0.002f buffer to avoid re-triggering every frame,
            // causing a slide due to lateral displacement of neck position relative to player position
            var finalTargetDistance = horizontalDistance - 0.002f;

            // If our target distance is smaller by a good margin, just use our smaller one
            if (finalTargetDistance > targetDistance + 0.375f)
            {
                finalTargetDistance = targetDistance - 0.375f;
            }

            var heightAdjustedOrigin = handlePosition;
            heightAdjustedOrigin.y = targetPoint.y;
            var teleportTarget = heightAdjustedOrigin - (heightAdjustedOrigin - targetPoint).normalized *
                finalTargetDistance;

            // Note: This is broken in editor. I'm not sure why. But it works in game.
            player.TeleportTo(teleportTarget + playspaceOffset, playspaceOrigin.rotation,
                VRC_SceneDescriptor.SpawnOrientation.AlignRoomWithSpawnPoint, true);

            return teleportTarget + neckOffset;
        }

        private void UpdateLeash()
        {
            var positions = leash.positionCount;
            var lines = new Vector3[positions];
            var a = _collarBody.position;
            var c = _handleBody.position;

            var dipAmount = leashLength - Vector3.Distance(a, c);

            // This is required because if the distance is higher (due to lag) we don't want to bend upwards
            if (dipAmount <= 0)
            {
                // Straight line lerp
                for (var i = 0; i < positions; i++)
                {
                    var pos = (float)i / (positions - 1);
                    lines[i] = Vector3.Lerp(a, c, pos);
                }
            }
            else
            {
                // Pretty lerp
                var b = Vector3.Lerp(a, c, 0.5f);
                b.y -= (1 - (float)Math.Pow(1 - (dipAmount / leashLength), 2)) * leashLength;

                for (var i = 0; i < positions; i++)
                {
                    var pos = (float)i / (positions - 1);
                    var a_b = Vector3.Lerp(a, b, pos);
                    var b_c = Vector3.Lerp(b, c, pos);
                    var ab_bc = Vector3.Lerp(a_b, b_c, pos);
                    lines[i] = ab_bc;
                }
            }

            leash.SetPositions(lines);
        }

        /// <summary>
        /// Handle functionality that must wait for the system to be fully synced before activating some components
        /// </summary>
        public void _PollReadyLeashed()
        {
            if (_handleGeneration != _handleGenerationTracker.generation)
            {
                SendCustomEventDelayedFrames(nameof(_PollReadyLeashed), 1);
                return;
            }

            leash.enabled = true;
        }

        #endregion
    }
}