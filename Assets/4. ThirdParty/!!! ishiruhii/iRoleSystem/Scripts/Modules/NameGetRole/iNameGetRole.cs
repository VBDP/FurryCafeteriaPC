// ============================================================================
// iNameGetRole.cs - v5.6
// Author: ishiruhii
// Description: Módulo con configuración de roles individual por jugador
//              Con soporte multilenguaje completo
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using TMPro;

namespace ishiruhii.iRoleSystem.Modules.iNameGetRole
{
    using ishiruhii.iRoleSystem.Core;

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iNameGetRole : UdonSharpBehaviour
    {
        [Header("=== iNameGetRole Module ===")]
        [Space(10)]

        [Header("Referencias del Sistema")]
        public iRoleSystemCore   core;
        public iPlayerRoleManager roleManager;
        public iRoleDatabase     roleDatabase;

        [Header("Idioma")]
        [Tooltip("Referencia al iLanguageSystem para textos localizados")]
        public iLanguageSystem languageSystem;

        // ====================================================================
        // CONFIGURACIÓN POR JUGADOR
        // ====================================================================
        [Header("Configuración de Jugadores")]
        public string[]      playerNames       = new string[0];
        [HideInInspector]
        public int           maxSlotsPerPlayer = 8;
        public int[]         perPlayerRoleIds  = new int[0];
        public GameObject[]  perPlayerPrefabs  = new GameObject[0];
        public GameObject    defaultRolePrefab;

        // ====================================================================
        // CONFIGURACIÓN POR ROL PERMITIDO
        // ====================================================================
        [Header("Roles con Acceso al Canvas")]
        [Tooltip("Roles que pueden abrir este canvas. Cada rol tiene sus propios botones configurables.")]
        public int[]        allowedRoleIds       = new int[0];
        [HideInInspector]
        public int          maxSlotsPerRole      = 8;
        public int[]        perRoleButtonIds     = new int[0];
        public GameObject[] perRoleButtonPrefabs = new GameObject[0];

        [Header("Configuración de UI")]
        public GameObject  roleSelectionCanvas;
        public Transform   roleButtonsContainer;

        [Tooltip("Si está activo, el Master de la instancia puede abrir el canvas aunque no esté en la lista de jugadores")]
        public bool allowMasterAccess = false;

        [Header("Efectos y Feedback")]
        public AudioSource menuOpenSound;
        public AudioSource roleAssignedSound;
        public AudioSource accessDeniedSound;
        public Text        feedbackText;

        [Header("Actualización Automática de Rol")]
        public float roleCheckInterval = 5f;
        public Text  roleDisplayText;

        [Header("Debug")]
        public bool debugMode  = false;
        public int  maxButtons = 10;

        // ====================================================================
        // Variables de Runtime
        // ====================================================================
        private GameObject[] spawnedButtons;
        private int          spawnedButtonCount = 0;
        private int[]        buttonRoleIds;
        private int          _currentPlayerIndex = -1;

        // ====================================================================
        // INICIO
        // ====================================================================
        private void Start()
        {
            if (playerNames       == null) playerNames       = new string[0];
            if (perPlayerRoleIds  == null) perPlayerRoleIds  = new int[0];
            if (perPlayerPrefabs  == null) perPlayerPrefabs  = new GameObject[0];
            if (allowedRoleIds    == null) allowedRoleIds    = new int[0];
            if (perRoleButtonIds  == null) perRoleButtonIds  = new int[0];
            if (perRoleButtonPrefabs == null) perRoleButtonPrefabs = new GameObject[0];

            spawnedButtons = new GameObject[maxButtons];
            buttonRoleIds  = new int[maxButtons];

            ValidateSetup();
            HideMenu();

            if (roleCheckInterval > 0)
                SendCustomEventDelayedSeconds(nameof(UpdateRoleDisplay), roleCheckInterval);

            UpdateRoleDisplayText();

            if (debugMode)
            {
                Log("=== INICIO iNameGetRole ===");
                Log("Jugadores configurados: " + playerNames.Length);
            }
        }

        private void ValidateSetup()
        {
            if (core         == null) LogError("iRoleSystemCore no asignado!");
            if (roleManager  == null) LogError("iPlayerRoleManager no asignado!");
            if (roleDatabase == null) LogError("iRoleDatabase no asignado!");

            int expectedSize = playerNames.Length * maxSlotsPerPlayer;
            if (perPlayerRoleIds.Length != expectedSize)
                LogWarning("Tamaño de array perPlayerRoleIds incorrecto. Usa el Inspector para corregirlo.");
        }

        // ====================================================================
        // INTERACCIÓN PRINCIPAL
        // ====================================================================
        public void OnInteract()
        {
            if (Networking.LocalPlayer == null) { LogWarning("No hay jugador local"); return; }

            string playerName  = Networking.LocalPlayer.displayName;
            int    playerIndex = GetPlayerIndex(playerName);

            // 1) Acceso por nombre de jugador
            if (playerIndex >= 0)
            {
                _currentPlayerIndex = playerIndex;
                ShowRoleSelectionMenu();
                return;
            }

            // 2) Acceso del Master (si está permitido)
            if (allowMasterAccess && Networking.LocalPlayer.isMaster)
            {
                _currentPlayerIndex = -2;
                ShowRoleSelectionMenuForMaster();
                return;
            }

            // 3) Acceso por rol del jugador local
            if (roleManager != null && allowedRoleIds != null && allowedRoleIds.Length > 0)
            {
                int localRole     = roleManager.GetPlayerRole(Networking.LocalPlayer.playerId);
                int allowedIndex  = GetAllowedRoleIndex(localRole);
                if (allowedIndex >= 0)
                {
                    _currentPlayerIndex = -3 - allowedIndex; // encode: -3 = allowedRole[0], -4 = [1], etc.
                    ShowRoleSelectionMenuForAllowedRole(allowedIndex);
                    return;
                }
            }

            DenyAccess(playerName);
        }

        // ====================================================================
        // MENÚ DE SELECCIÓN DE ROL
        // ====================================================================
        public void ShowRoleSelectionMenu()
        {
            if (roleSelectionCanvas == null) { LogError("Canvas de selección no asignado!"); return; }
            if (_currentPlayerIndex < 0 || _currentPlayerIndex >= playerNames.Length) { LogError("playerIndex inválido"); return; }

            PlaySound(menuOpenSound);
            ClearRoleButtons();

            int start        = _currentPlayerIndex * maxSlotsPerPlayer;
            int createdCount = 0;

            for (int s = 0; s < maxSlotsPerPlayer; s++)
            {
                int idx = start + s;
                if (idx >= perPlayerRoleIds.Length) break;

                int roleId = perPlayerRoleIds[idx];
                if (roleId < 0) continue;

                GameObject prefab = (perPlayerPrefabs != null && idx < perPlayerPrefabs.Length)
                    ? perPlayerPrefabs[idx] : null;
                if (prefab == null) prefab = defaultRolePrefab;

                if (prefab == null) { LogError("Slot " + s + " sin prefab asignado!"); continue; }

                CreateRoleButton(roleId, prefab);
                createdCount++;
            }

            if (createdCount == 0)
                ShowFeedback(L(iLang.NO_ROLES_AVAILABLE));

            roleSelectionCanvas.SetActive(true);
        }

        private void CreateRoleButton(int roleId, GameObject prefab)
        {
            if (roleButtonsContainer == null) { LogError("Contenedor de botones no asignado!"); return; }
            if (spawnedButtonCount >= maxButtons) { LogWarning("Máximo de botones alcanzado"); return; }

            GameObject buttonObj = Instantiate(prefab, roleButtonsContainer);
            spawnedButtons[spawnedButtonCount] = buttonObj;
            buttonRoleIds[spawnedButtonCount]  = roleId;
            spawnedButtonCount++;

            // Auto-configurar texto TMP si el prefab es el fallback o si no hay prefab específico
            if (roleDatabase != null)
            {
                string roleName = roleDatabase.GetRoleName(roleId);
                // Buscar TMPro.TextMeshProUGUI en el botón
                TMPro.TextMeshProUGUI tmpText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (tmpText != null) tmpText.text = roleName;
            }

            iRoleButton roleButton = buttonObj.GetComponent<iRoleButton>();
            if (roleButton != null) roleButton.Initialize(this, roleId);

            Collider col = buttonObj.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            buttonObj.SetActive(true);
        }

        // ====================================================================
        // MENÚ DEL MASTER (muestra todos los roles disponibles en la DB)
        // ====================================================================
        public void ShowRoleSelectionMenuForMaster()
        {
            if (roleSelectionCanvas == null) { LogError("Canvas de selección no asignado!"); return; }
            if (roleDatabase == null) { LogError("roleDatabase no asignado!"); return; }

            PlaySound(menuOpenSound);
            ClearRoleButtons();

            int roleCount = roleDatabase.roleNames != null ? roleDatabase.roleNames.Length : 0;
            for (int r = 0; r < roleCount; r++)
            {
                GameObject prefab = defaultRolePrefab;
                if (prefab == null) { LogError("defaultRolePrefab no asignado para el Master!"); break; }
                CreateRoleButton(r, prefab);
            }

            if (roleCount == 0)
                ShowFeedback(L(iLang.NO_ROLES_AVAILABLE));

            roleSelectionCanvas.SetActive(true);
        }

        // ====================================================================
        // MENÚ PARA ROL PERMITIDO
        // ====================================================================
        public void ShowRoleSelectionMenuForAllowedRole(int allowedIndex)
        {
            if (roleSelectionCanvas == null) { LogError("Canvas de selección no asignado!"); return; }

            PlaySound(menuOpenSound);
            ClearRoleButtons();

            int start = allowedIndex * maxSlotsPerRole;
            int created = 0;

            for (int s = 0; s < maxSlotsPerRole; s++)
            {
                int idx = start + s;
                if (perRoleButtonIds == null || idx >= perRoleButtonIds.Length) break;

                int roleId = perRoleButtonIds[idx];
                if (roleId < 0) continue;

                GameObject prefab = (perRoleButtonPrefabs != null && idx < perRoleButtonPrefabs.Length)
                    ? perRoleButtonPrefabs[idx] : null;
                if (prefab == null) prefab = defaultRolePrefab;
                if (prefab == null) { LogError("Sin prefab para slot de rol " + s); continue; }

                CreateRoleButton(roleId, prefab);
                created++;
            }

            if (created == 0)
                ShowFeedback(L(iLang.NO_ROLES_AVAILABLE));

            roleSelectionCanvas.SetActive(true);
        }

        private int GetAllowedRoleIndex(int roleId)
        {
            if (allowedRoleIds == null || roleId < 0) return -1;
            for (int i = 0; i < allowedRoleIds.Length; i++)
                if (allowedRoleIds[i] == roleId) return i;
            return -1;
        }

        // ====================================================================
        // SELECCIÓN DE ROL
        // ====================================================================
        public void OnRoleSelected(int selectedRoleId)
        {
            if (Networking.LocalPlayer == null) { LogError("LocalPlayer es NULL"); return; }
            if (roleManager == null) { LogError("roleManager es NULL!"); return; }

            int playerId = Networking.LocalPlayer.playerId;
            roleManager.SetPlayerRole(playerId, selectedRoleId);
            PlaySound(roleAssignedSound);

            string roleName = roleDatabase != null
                ? roleDatabase.GetRoleName(selectedRoleId)
                : selectedRoleId.ToString();

            ShowFeedback(L(iLang.ROLE_ASSIGNED, roleName));
            HideMenu();
        }

        // ====================================================================
        // OCULTAR MENÚ
        // ====================================================================
        public void HideMenu()
        {
            ClearRoleButtons();
            _currentPlayerIndex = -1;
            if (roleSelectionCanvas != null) roleSelectionCanvas.SetActive(false);
        }

        // ====================================================================
        // ACTUALIZACIÓN DE TEXTO DE ROL
        // ====================================================================
        public void UpdateRoleDisplay()
        {
            UpdateRoleDisplayText();
            if (roleCheckInterval > 0)
                SendCustomEventDelayedSeconds(nameof(UpdateRoleDisplay), roleCheckInterval);
        }

        private void UpdateRoleDisplayText()
        {
            if (roleDisplayText == null || roleManager == null || roleDatabase == null) return;
            if (Networking.LocalPlayer == null) return;

            int roleIndex = roleManager.GetPlayerRole(Networking.LocalPlayer.playerId);

            if (roleIndex >= 0)
            {
                roleDisplayText.text = roleDatabase.GetRoleName(roleIndex);
            }
            else
            {
                roleDisplayText.text = L(iLang.NO_ROLE);
            }
        }

        public void ForceRoleUpdate() { UpdateRoleDisplayText(); }

        // ====================================================================
        // UTILIDADES
        // ====================================================================
        private int GetPlayerIndex(string playerName)
        {
            if (string.IsNullOrEmpty(playerName) || playerNames == null) return -1;
            for (int i = 0; i < playerNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(playerNames[i]) && playerNames[i].Trim() == playerName.Trim())
                    return i;
            }
            return -1;
        }

        private void ClearRoleButtons()
        {
            for (int i = 0; i < spawnedButtonCount; i++)
            {
                if (spawnedButtons[i] != null) { Destroy(spawnedButtons[i]); spawnedButtons[i] = null; }
                buttonRoleIds[i] = 0;
            }
            spawnedButtonCount = 0;
        }

        private void DenyAccess(string playerName)
        {
            PlaySound(accessDeniedSound);
            ShowFeedback(L(iLang.ACCESS_DENIED));
            Log(">>> Acceso denegado: '" + playerName + "'");
        }

        private void ShowFeedback(string message)
        {
            if (feedbackText != null) feedbackText.text = message;
            Log(">>> Feedback: " + message);
        }

        private void PlaySound(AudioSource source) { if (source != null) source.Play(); }

        // Shortcut de localización
        private string L(int id) => languageSystem != null ? languageSystem.Get(id) : "?";
        private string L(int id, string arg0) => languageSystem != null ? languageSystem.Get(id, arg0) : arg0;

        // API pública
        public bool IsPlayerAuthorized(string playerName) => GetPlayerIndex(playerName) >= 0;
        public bool CanLocalPlayerAccess()
        {
            if (Networking.LocalPlayer == null) return false;
            return IsPlayerAuthorized(Networking.LocalPlayer.displayName);
        }
        public int GetPlayerCount() => playerNames != null ? playerNames.Length : 0;
        public int GetWhitelistCount() => GetPlayerCount();
        public int GetAllowedRolesCount()    => allowedRoleIds != null ? allowedRoleIds.Length : 0;
        public int GetCustomRoleSlotsCount() => 0;

        public int GetActiveSlotsForPlayer(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= playerNames.Length) return 0;
            int start = playerIndex * maxSlotsPerPlayer;
            int count = 0;
            for (int s = 0; s < maxSlotsPerPlayer; s++)
            {
                int idx = start + s;
                if (idx >= perPlayerRoleIds.Length) break;
                if (perPlayerRoleIds[idx] >= 0) count++;
            }
            return count;
        }

        public int GetActiveSlotsForAllowedRole(int allowedIndex)
        {
            if (allowedIndex < 0 || allowedRoleIds == null || allowedIndex >= allowedRoleIds.Length) return 0;
            int start = allowedIndex * maxSlotsPerRole;
            int count = 0;
            for (int s = 0; s < maxSlotsPerRole; s++)
            {
                int idx = start + s;
                if (perRoleButtonIds == null || idx >= perRoleButtonIds.Length) break;
                if (perRoleButtonIds[idx] >= 0) count++;
            }
            return count;
        }

        // ====================================================================
        // LOGGING
        // ====================================================================
        private void Log(string message)        { if (debugMode) Debug.Log("[iNameGetRole] " + message); }
        private void LogWarning(string message) { Debug.LogWarning("[iNameGetRole] " + message); }
        private void LogError(string message)   { Debug.LogError("[iNameGetRole] " + message); }
    }
}
