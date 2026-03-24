// ============================================================================
// iNameSetter.cs - v5.6
// Author: ishiruhii
// Description: Asignación automática de roles por nombre con soporte multilenguaje
// ============================================================================

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.NameSetter
{
    using ishiruhii.iRoleSystem.Core;

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iNameSetter : UdonSharpBehaviour
    {
        [Header("=== iNameSetter Module ===")]
        [Space(10)]

        [Header("Referencias del Sistema")]
        public iRoleSystemCore    core;
        public iPlayerRoleManager roleManager;
        public iRoleDatabase      roleDatabase;

        [Header("Idioma")]
        [Tooltip("Referencia al iLanguageSystem para mensajes localizados")]
        public iLanguageSystem languageSystem;

        [Header("Configuración de Asignación")]
        public bool  assignOnJoin = true;
        public float assignDelay  = 2f;

        [Header("Lista de Jugadores y Roles")]
        public string[] playerDisplayNames;
        public int[]    playerRoleIndices;
        public GameObject feedbackPrefab;

        [Header("Configuración de Feedback")]
        public AudioSource roleAssignedSound;
        public Text        feedbackText;

        [Header("Debug")]
        public bool debugMode = false;

        // Variables para asignación diferida
        [HideInInspector] public int    _pendingPlayerId;
        [HideInInspector] public int    _pendingRoleIndex;
        [HideInInspector] public string _pendingPlayerName;

        private void Start()
        {
            if (playerDisplayNames == null) playerDisplayNames = new string[0];
            if (playerRoleIndices  == null) playerRoleIndices  = new int[0];

            ValidateSetup();

            if (debugMode)
                Log("=== INICIO iNameSetter === Jugadores: " + playerDisplayNames.Length);
        }

        private void ValidateSetup()
        {
            if (core        == null) LogWarning("iRoleSystemCore no asignado!");
            if (roleManager == null) LogError("iPlayerRoleManager no asignado!");
            if (roleDatabase == null) LogWarning("iRoleDatabase no asignado!");
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!assignOnJoin) return;

            string playerName = player.displayName;
            int    roleIndex  = GetRoleIndexForPlayer(playerName);

            if (roleIndex >= 0)
            {
                SetProgramVariable("_pendingPlayerId",    player.playerId);
                SetProgramVariable("_pendingRoleIndex",   roleIndex);
                SetProgramVariable("_pendingPlayerName",  playerName);
                SendCustomEventDelayedSeconds(nameof(AssignPendingRole), assignDelay);
            }
        }

        public void AssignPendingRole()
        {
            int    playerId   = (int)GetProgramVariable("_pendingPlayerId");
            int    roleIndex  = (int)GetProgramVariable("_pendingRoleIndex");
            string playerName = (string)GetProgramVariable("_pendingPlayerName");

            VRCPlayerApi player = VRCPlayerApi.GetPlayerById(playerId);
            if (player == null || !player.IsValid()) return;
            if (player.displayName != playerName) return;

            int currentRole = roleManager.GetPlayerRole(playerId);
            if (currentRole >= 0) return;

            AssignRole(playerId, roleIndex, playerName);
        }

        public void AssignRole(int playerId, int roleIndex, string playerName)
        {
            if (roleManager == null) { LogError("roleManager es NULL!"); return; }
            if (roleIndex < 0) { LogWarning("Índice de rol inválido: " + roleIndex); return; }

            roleManager.SetPlayerRole(playerId, roleIndex);
            PlaySound(roleAssignedSound);

            string roleName = roleDatabase != null ? roleDatabase.GetRoleName(roleIndex) : roleIndex.ToString();
            ShowFeedback(L(iLang.ROLE_ASSIGNED, roleName) + " → " + playerName);

            if (debugMode) Log(">>> Rol '" + roleName + "' asignado a " + playerName);
        }

        public int GetRoleIndexForPlayer(string playerName)
        {
            if (string.IsNullOrEmpty(playerName) || playerDisplayNames == null || playerRoleIndices == null)
                return -1;

            for (int i = 0; i < playerDisplayNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(playerDisplayNames[i]))
                {
                    if (playerDisplayNames[i].Trim().Equals(playerName.Trim(), System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (i < playerRoleIndices.Length)
                            return playerRoleIndices[i];
                    }
                }
            }
            return -1;
        }

        public bool IsPlayerInSetterList(string playerName) => GetRoleIndexForPlayer(playerName) >= 0;

        public void ForceCheckAllPlayers()
        {
            VRCPlayerApi[] players = new VRCPlayerApi[80];
            VRCPlayerApi.GetPlayers(players);
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].IsValid())
                {
                    string playerName = players[i].displayName;
                    int    roleIndex  = GetRoleIndexForPlayer(playerName);
                    if (roleIndex >= 0 && roleManager.GetPlayerRole(players[i].playerId) < 0)
                        AssignRole(players[i].playerId, roleIndex, playerName);
                }
            }
        }

        public void AssignRoleByName(string playerName)
        {
            int roleIndex = GetRoleIndexForPlayer(playerName);
            if (roleIndex < 0) { ShowFeedback(L(iLang.PLAYER_NOT_FOUND, playerName)); return; }

            VRCPlayerApi targetPlayer = FindPlayerByName(playerName);
            if (targetPlayer == null) { ShowFeedback(L(iLang.PLAYER_NOT_IN_WORLD, playerName)); return; }

            AssignRole(targetPlayer.playerId, roleIndex, playerName);
        }

        private VRCPlayerApi FindPlayerByName(string playerName)
        {
            VRCPlayerApi[] players = new VRCPlayerApi[80];
            VRCPlayerApi.GetPlayers(players);
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] != null && players[i].IsValid())
                    if (players[i].displayName.Equals(playerName, System.StringComparison.OrdinalIgnoreCase))
                        return players[i];
            }
            return null;
        }

        private void ShowFeedback(string message)
        {
            if (feedbackText != null) feedbackText.text = message;
            if (debugMode) Log(">>> Feedback: " + message);
        }

        private void PlaySound(AudioSource audioSource) { if (audioSource != null) audioSource.Play(); }

        // Shortcut de localización
        private string L(int id)              => languageSystem != null ? languageSystem.Get(id)       : "?";
        private string L(int id, string arg0) => languageSystem != null ? languageSystem.Get(id, arg0) : arg0;

        public int    GetConfiguredPlayersCount() => playerDisplayNames != null ? playerDisplayNames.Length : 0;

        private void Log(string message)        { if (debugMode) Debug.Log("[iNameSetter] " + message); }
        private void LogWarning(string message) { Debug.LogWarning("[iNameSetter] " + message); }
        private void LogError(string message)   { Debug.LogError("[iNameSetter] " + message); }
    }
}
