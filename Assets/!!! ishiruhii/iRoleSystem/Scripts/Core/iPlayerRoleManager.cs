// ============================================================================
// iRoleSystem v4.0.5 - Core Module
// Author: ishiruhii
// Description: Gestor sincronizado de roles de jugadores
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Core
{
    /// <summary>
    /// Gestiona la asignación y sincronización de roles de jugadores.
    /// Maneja automáticamente la entrada/salida de jugadores.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class iPlayerRoleManager : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia a la base de datos de roles")]
        public iRoleDatabase roleDatabase;

        [Header("Configuración")]
        [Tooltip("Rol por defecto para nuevos jugadores (-1 = sin rol)")]
        public int defaultRoleIndex = -1;

        [Tooltip("Segundos de espera antes de asignar el rol por defecto al jugador que entra (0 = inmediato)")]
        public float defaultRoleDelay = 0f;

        [Tooltip("Si está activo, no sobreescribe el rol de un jugador que ya tiene uno asignado")]
        public bool skipIfAlreadyHasRole = true;

        [Header("Configuración del Master")]
        [Tooltip("Si está activo, se asignará un rol especial al Master de la instancia")]
        public bool enableMasterRole = false;

        [Tooltip("Rol a asignar al Master de la instancia (-1 = sin rol)")]
        public int masterRoleIndex = -1;

        [Tooltip("Segundos de espera antes de asignar el rol al Master (0 = inmediato)")]
        public float masterRoleDelay = 0f;

        [Tooltip("Si está activo, no sobreescribe el rol del Master si ya tiene uno asignado")]
        public bool masterSkipIfAlreadyHasRole = true;

        [Tooltip("Capacidad máxima de jugadores")]
        public int maxPlayers = 80;

        [Header("Eventos")]
        [Tooltip("Behaviours a notificar cuando cambia un rol")]
        public UdonSharpBehaviour[] onRoleChangedListeners;

        // Arrays sincronizados
        [UdonSynced] private int[] syncedPlayerIds;
        [UdonSynced] private int[] syncedPlayerRoles;

        // Cache local
        private int localPlayerId = -1;
        private int localPlayerRole = -1;

        // Cola de asignación con delay
        private int[]   _pendingPlayerIds  = new int[80];
        private int[]   _pendingRoles      = new int[80];
        private float[] _pendingTimers     = new float[80];
        private int     _pendingCount      = 0;

        private void Start()
        {
            InitializeArrays();

            _pendingPlayerIds = new int[maxPlayers];
            _pendingRoles     = new int[maxPlayers];
            _pendingTimers    = new float[maxPlayers];
            for (int i = 0; i < maxPlayers; i++)
                _pendingPlayerIds[i] = -1;

            if (Networking.LocalPlayer != null)
            {
                localPlayerId = Networking.LocalPlayer.playerId;
            }
        }

        private void InitializeArrays()
        {
            if (syncedPlayerIds == null || syncedPlayerIds.Length != maxPlayers)
            {
                syncedPlayerIds = new int[maxPlayers];
                syncedPlayerRoles = new int[maxPlayers];

                for (int i = 0; i < maxPlayers; i++)
                {
                    syncedPlayerIds[i] = -1;
                    syncedPlayerRoles[i] = -1;
                }
            }
        }

        /// <summary>
        /// Asigna un rol a un jugador específico
        /// </summary>
        public void SetPlayerRole(int playerId, int roleIndex)
        {
            if (!TakeOwnership()) return;

            int existingIndex = FindPlayerIndex(playerId);
            int emptyIndex = FindEmptySlot();

            if (existingIndex != -1)
            {
                // Actualizar rol existente
                syncedPlayerRoles[existingIndex] = roleIndex;
            }
            else if (emptyIndex != -1)
            {
                // Nuevo jugador
                syncedPlayerIds[emptyIndex] = playerId;
                syncedPlayerRoles[emptyIndex] = roleIndex;
            }
            else
            {
                Debug.LogWarning("[iRoleSystem] No hay espacio para más jugadores");
                return;
            }

            RequestSerialization();
            NotifyRoleChanged(playerId, roleIndex);

            // Actualizar cache local si es el jugador local
            if (playerId == localPlayerId)
            {
                localPlayerRole = roleIndex;
            }
        }

        /// <summary>
        /// Obtiene el rol de un jugador específico
        /// </summary>
        public int GetPlayerRole(int playerId)
        {
            // Usar cache para el jugador local
            if (playerId == localPlayerId && localPlayerRole != -1)
            {
                return localPlayerRole;
            }

            int index = FindPlayerIndex(playerId);
            return index != -1 ? syncedPlayerRoles[index] : -1;
        }

        /// <summary>
        /// Obtiene el rol del jugador local
        /// </summary>
        public int GetLocalPlayerRole()
        {
            if (Networking.LocalPlayer == null) return -1;
            return GetPlayerRole(Networking.LocalPlayer.playerId);
        }

        /// <summary>
        /// Verifica si un jugador tiene un rol específico
        /// </summary>
        public bool PlayerHasRole(int playerId, int roleIndex)
        {
            return GetPlayerRole(playerId) == roleIndex;
        }

        /// <summary>
        /// Verifica si el jugador local tiene un rol específico
        /// </summary>
        public bool LocalPlayerHasRole(int roleIndex)
        {
            return GetLocalPlayerRole() == roleIndex;
        }

        /// <summary>
        /// Remueve el rol de un jugador
        /// </summary>
        public void RemovePlayerRole(int playerId)
        {
            if (!TakeOwnership()) return;

            int index = FindPlayerIndex(playerId);
            if (index != -1)
            {
                syncedPlayerIds[index] = -1;
                syncedPlayerRoles[index] = -1;
                RequestSerialization();
                NotifyRoleChanged(playerId, -1);
            }

            if (playerId == localPlayerId)
            {
                localPlayerRole = -1;
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsMaster) return;

            bool isMaster = player.isMaster;

            // Asignación de rol al Master
            if (isMaster && enableMasterRole && masterRoleIndex >= 0)
            {
                if (!masterSkipIfAlreadyHasRole || GetPlayerRole(player.playerId) < 0)
                {
                    if (masterRoleDelay <= 0f)
                        SetPlayerRole(player.playerId, masterRoleIndex);
                    else
                        EnqueuePendingRole(player.playerId, masterRoleIndex, masterRoleDelay);
                }
                // Si el master tiene rol propio y defaultRoleIndex también está activo,
                // el master role tiene prioridad; salimos para no aplicar ambos.
                return;
            }

            // Asignación de rol por defecto (no-master)
            if (defaultRoleIndex < 0) return;
            if (skipIfAlreadyHasRole && GetPlayerRole(player.playerId) >= 0) return;

            if (defaultRoleDelay <= 0f)
                SetPlayerRole(player.playerId, defaultRoleIndex);
            else
                EnqueuePendingRole(player.playerId, defaultRoleIndex, defaultRoleDelay);
        }

        private void EnqueuePendingRole(int playerId, int roleIndex, float delay)
        {
            // Si ya está en cola, actualizar su timer y rol
            for (int i = 0; i < _pendingCount; i++)
            {
                if (_pendingPlayerIds[i] == playerId)
                {
                    _pendingRoles[i]  = roleIndex;
                    _pendingTimers[i] = delay;
                    return;
                }
            }

            if (_pendingCount >= _pendingPlayerIds.Length) return;

            _pendingPlayerIds[_pendingCount] = playerId;
            _pendingRoles[_pendingCount]     = roleIndex;
            _pendingTimers[_pendingCount]    = delay;
            _pendingCount++;
        }

        private void Update()
        {
            if (_pendingCount == 0) return;

            float dt = Time.deltaTime;
            int i = 0;

            while (i < _pendingCount)
            {
                _pendingTimers[i] -= dt;

                if (_pendingTimers[i] <= 0f)
                {
                    int pid     = _pendingPlayerIds[i];
                    int roleId  = _pendingRoles[i];

                    // Comprobar que el jugador sigue en la instancia
                    VRCPlayerApi player = VRCPlayerApi.GetPlayerById(pid);
                    if (player != null && player.IsValid())
                    {
                        // Para el master usamos masterSkipIfAlreadyHasRole, para el resto skipIfAlreadyHasRole
                        bool skip = player.isMaster ? masterSkipIfAlreadyHasRole : skipIfAlreadyHasRole;
                        if (!skip || GetPlayerRole(pid) < 0)
                        {
                            SetPlayerRole(pid, roleId);
                        }
                    }

                    // Eliminar de la cola compactando
                    _pendingCount--;
                    _pendingPlayerIds[i] = _pendingPlayerIds[_pendingCount];
                    _pendingRoles[i]     = _pendingRoles[_pendingCount];
                    _pendingTimers[i]    = _pendingTimers[_pendingCount];
                    _pendingPlayerIds[_pendingCount] = -1;
                }
                else
                {
                    i++;
                }
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsMaster) return;

            int playerId = player.playerId;
            int index = FindPlayerIndex(playerId);

            if (index != -1)
            {
                if (!TakeOwnership()) return;

                syncedPlayerIds[index] = -1;
                syncedPlayerRoles[index] = -1;
                RequestSerialization();
            }
        }

        public override void OnDeserialization()
        {
            // Actualizar cache local después de deserialización
            if (localPlayerId != -1)
            {
                int index = FindPlayerIndex(localPlayerId);
                localPlayerRole = index != -1 ? syncedPlayerRoles[index] : -1;
            }
        }

        #region Utilidades Privadas

        private int FindPlayerIndex(int playerId)
        {
            if (syncedPlayerIds == null) return -1;

            for (int i = 0; i < syncedPlayerIds.Length; i++)
            {
                if (syncedPlayerIds[i] == playerId)
                    return i;
            }
            return -1;
        }

        private int FindEmptySlot()
        {
            if (syncedPlayerIds == null) return -1;

            for (int i = 0; i < syncedPlayerIds.Length; i++)
            {
                if (syncedPlayerIds[i] == -1)
                    return i;
            }
            return -1;
        }

        private bool TakeOwnership()
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            return true;
        }

        private void NotifyRoleChanged(int playerId, int newRole)
        {
            if (onRoleChangedListeners == null) return;

            for (int i = 0; i < onRoleChangedListeners.Length; i++)
            {
                if (onRoleChangedListeners[i] != null)
                {
                    onRoleChangedListeners[i].SetProgramVariable("_lastChangedPlayerId", playerId);
                    onRoleChangedListeners[i].SetProgramVariable("_lastChangedRole", newRole);
                    onRoleChangedListeners[i].SendCustomEvent("_OnPlayerRoleChanged");
                }
            }
        }

        #endregion

        #region Debug

        /// <summary>
        /// Obtiene información de debug sobre los jugadores con rol
        /// </summary>
        public string GetDebugInfo()
        {
            string info = "=== iRoleSystem Debug ===\n";

            if (syncedPlayerIds == null) return info + "Arrays no inicializados";

            for (int i = 0; i < syncedPlayerIds.Length; i++)
            {
                if (syncedPlayerIds[i] != -1)
                {
                    VRCPlayerApi player = VRCPlayerApi.GetPlayerById(syncedPlayerIds[i]);
                    string playerName = player != null ? player.displayName : "Desconectado";
                    string roleName = roleDatabase != null ?
                        roleDatabase.GetRoleName(syncedPlayerRoles[i]) :
                        syncedPlayerRoles[i].ToString();

                    info += $"[{syncedPlayerIds[i]}] {playerName}: {roleName}\n";
                }
            }

            return info;
        }

        #endregion
    }
}
