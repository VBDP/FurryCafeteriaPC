// ============================================================================
// iRoleSystem v4.0.5 - RankDisplay Module
// Author: ishiruhii
// Description: Gestor principal de displays de rango sobre jugadores
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.RankDisplay
{
    using ishiruhii.iRoleSystem.Core;

    /// <summary>
    /// Gestiona la creación y actualización de displays de rango
    /// sobre las cabezas de los jugadores según su rol.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRankDisplayManager : UdonSharpBehaviour
    {
        [Header("Referencias del Sistema")]
        [Tooltip("Referencia al sistema core")]
        public iRoleSystemCore roleSystem;

        [Tooltip("Referencia directa al PlayerRoleManager")]
        public iPlayerRoleManager playerRoleManager;

        [Header("Configuración de Displays")]
        [Tooltip("Configuraciones de display disponibles")]
        public iRankDisplayConfig[] displayConfigs;

        [Tooltip("Prefabs directos correspondientes a cada config")]
        public GameObject[] displayPrefabs;

        [Header("Configuración")]
        [Tooltip("Frames entre actualizaciones de posición")]
        [Range(1, 10)]
        public int updateEveryXFrames = 2;

        [Tooltip("Intervalo de chequeo de roles en segundos (0 = desactivado)")]
        [Range(0, 60)]
        public float roleCheckInterval = 5f;

        [Tooltip("Capacidad máxima de jugadores")]
        public int maxPlayers = 80;

        // Pool de instancias por jugador y tipo de display
        private iRankDisplayInstance[][] instancePool;
        private int[] currentDisplayByPlayer;
        private int[] lastKnownRoleByPlayer;

        private void Start()
        {
            InitializePool();
            SendCustomEventDelayedSeconds(nameof(RefreshAllPlayers), 1f);

            // Iniciar el loop de chequeo de roles si está habilitado
            if (roleCheckInterval > 0f)
            {
                SendCustomEventDelayedSeconds(nameof(StartRoleCheckLoop), roleCheckInterval);
            }
        }

        /// <summary>
        /// Inicia el loop de chequeo de roles
        /// </summary>
        public void StartRoleCheckLoop()
        {
            if (roleCheckInterval > 0f)
            {
                CheckAllPlayerRoles();
                SendCustomEventDelayedSeconds(nameof(CheckAllPlayerRoles), roleCheckInterval);
            }
        }

        /// <summary>
        /// Chequea los roles de todos los jugadores y actualiza los displays si hay cambios
        /// </summary>
        public void CheckAllPlayerRoles()
        {
            if (roleCheckInterval <= 0f) return;

            VRCPlayerApi[] players = new VRCPlayerApi[maxPlayers];
            VRCPlayerApi.GetPlayers(players);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].IsValid())
                {
                    CheckAndUpdatePlayerRole(players[i]);
                }
            }

            // Continuar el loop
            SendCustomEventDelayedSeconds(nameof(CheckAllPlayerRoles), roleCheckInterval);
        }

        /// <summary>
        /// Chequea y actualiza el rol de un jugador específico si ha cambiado
        /// </summary>
        public void CheckAndUpdatePlayerRole(VRCPlayerApi player)
        {
            if (player == null || !player.IsValid()) return;

            int playerId = player.playerId;
            if (playerId < 0 || playerId >= maxPlayers) return;

            // Obtener el rol actual del jugador
            int currentRole = playerRoleManager != null ?
                playerRoleManager.GetPlayerRole(playerId) : -1;

            // Verificar si el rol ha cambiado
            if (currentRole != lastKnownRoleByPlayer[playerId])
            {
                // Actualizar el registro del último rol conocido
                lastKnownRoleByPlayer[playerId] = currentRole;

                // Forzar actualización del display
                currentDisplayByPlayer[playerId] = -1;
                UpdatePlayerDisplay(player);
            }
        }

        private void InitializePool()
        {
            instancePool = new iRankDisplayInstance[maxPlayers][];
            currentDisplayByPlayer = new int[maxPlayers];
            lastKnownRoleByPlayer = new int[maxPlayers];

            for (int i = 0; i < maxPlayers; i++)
            {
                instancePool[i] = new iRankDisplayInstance[displayConfigs.Length];
                currentDisplayByPlayer[i] = -1;
                lastKnownRoleByPlayer[i] = -1;
            }
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            SendCustomEventDelayedSeconds(nameof(RefreshAllPlayers), 0.5f);
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            int id = player.playerId;
            if (id >= 0 && id < maxPlayers)
            {
                DeactivateAllForPlayer(id);
                currentDisplayByPlayer[id] = -1;
                lastKnownRoleByPlayer[id] = -1;
            }
        }

        public override void OnAvatarChanged(VRCPlayerApi player)
        {
            UpdatePlayerDisplay(player);
        }

        /// <summary>
        /// Llamado cuando cambia el rol de un jugador (evento)
        /// </summary>
        [HideInInspector] public int _lastChangedPlayerId;
        [HideInInspector] public int _lastChangedRole;

        public void _OnPlayerRoleChanged()
        {
            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(_lastChangedPlayerId);
            if (player != null && player.IsValid())
            {
                UpdatePlayerDisplay(player);
            }
        }

        /// <summary>
        /// Refresca los displays de todos los jugadores
        /// </summary>
        public void RefreshAllPlayers()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[maxPlayers];
            VRCPlayerApi.GetPlayers(players);

            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].IsValid())
                {
                    // Actualizar el tracking del último rol conocido
                    int playerId = players[i].playerId;
                    int currentRole = playerRoleManager != null ?
                        playerRoleManager.GetPlayerRole(playerId) : -1;
                    lastKnownRoleByPlayer[playerId] = currentRole;

                    UpdatePlayerDisplay(players[i]);
                }
            }
        }

        /// <summary>
        /// Actualiza el display de un jugador específico
        /// </summary>
        public void UpdatePlayerDisplay(VRCPlayerApi player)
        {
            if (player == null || !player.IsValid()) return;

            int playerId = player.playerId;
            if (playerId < 0 || playerId >= maxPlayers) return;

            int roleIndex = playerRoleManager != null ?
                playerRoleManager.GetPlayerRole(playerId) : -1;

            int configIndex = GetConfigIndexForRole(roleIndex);

            // Sin display necesario
            if (configIndex == -1)
            {
                DeactivateAllForPlayer(playerId);
                currentDisplayByPlayer[playerId] = -1;
                return;
            }

            // Ya tiene el display correcto
            if (currentDisplayByPlayer[playerId] == configIndex)
            {
                return;
            }

            // Cambiar display
            DeactivateAllForPlayer(playerId);

            iRankDisplayInstance instance = instancePool[playerId][configIndex];

            // Crear instancia si no existe
            if (instance == null && displayPrefabs[configIndex] != null)
            {
                GameObject obj = VRCInstantiate(displayPrefabs[configIndex]);
                obj.transform.SetParent(transform);
                instance = obj.GetComponent<iRankDisplayInstance>();
                instancePool[playerId][configIndex] = instance;
            }

            if (instance != null)
            {
                iRankDisplayConfig config = displayConfigs[configIndex];
                instance.Initialize(
                    player,
                    roleIndex,
                    config.heightOffset,
                    updateEveryXFrames,
                    config.billboard,
                    config.billboardYOnly
                );

                instance.transform.localScale = config.displayScale;
            }

            currentDisplayByPlayer[playerId] = configIndex;
        }

        private void DeactivateAllForPlayer(int playerId)
        {
            if (playerId < 0 || playerId >= maxPlayers) return;

            for (int i = 0; i < displayConfigs.Length; i++)
            {
                if (instancePool[playerId][i] != null)
                {
                    instancePool[playerId][i].Deactivate();
                }
            }
        }

        private int GetConfigIndexForRole(int roleIndex)
        {
            if (roleIndex < 0) return -1;

            for (int i = 0; i < displayConfigs.Length; i++)
            {
                if (displayConfigs[i] == null) continue;

                bool[] allowed = displayConfigs[i].allowedRoles;
                if (allowed != null &&
                    roleIndex < allowed.Length &&
                    allowed[roleIndex])
                {
                    return i;
                }
            }

            return -1;
        }
    }
}