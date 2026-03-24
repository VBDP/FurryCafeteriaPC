// ============================================================================
// iRoleAdminPanel.cs - v5.6
// Author: ishiruhii
// Description: Panel de administración con soporte multilenguaje completo
// ============================================================================

using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using TMPro;

namespace ishiruhii.iRoleSystem.Modules.AdminPanel
{
    using ishiruhii.iRoleSystem.Core;

    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleAdminPanel : UdonSharpBehaviour
    {
        [Header("=== iRoleAdminPanel ===")]
        [Space(5)]

        // ====================================================================
        // REFERENCIAS
        // ====================================================================
        [Header("Referencias del Sistema")]
        public iRoleSystemCore    core;
        public iPlayerRoleManager roleManager;
        public iRoleDatabase      roleDatabase;

        [Header("Idioma")]
        [Tooltip("Referencia al iLanguageSystem para mensajes localizados")]
        public iLanguageSystem languageSystem;

        // ====================================================================
        // CONFIGURACIÓN DE ADMINS
        // ====================================================================
        [Header("Configuración de Administradores")]
        public string[]      adminNames        = new string[0];
        [HideInInspector]
        public int           maxSlotsPerAdmin  = 8;
        public int[]         perAdminRoleIds   = new int[0];
        public GameObject[]  perAdminPrefabs   = new GameObject[0];
        public GameObject    defaultButtonPrefab;

        [Tooltip("Si está activo, el Master de la instancia puede abrir el panel aunque no esté en la lista de admins")]
        public bool allowMasterAccess = false;

        // ====================================================================
        // CONFIGURACIÓN POR ROL PERMITIDO
        // ====================================================================
        [Header("Roles con Acceso al Panel")]
        [Tooltip("Roles que pueden abrir este panel. Cada rol tiene sus propios botones configurables.")]
        public int[]        allowedRoleIds       = new int[0];
        [HideInInspector]
        public int          maxSlotsPerRole      = 8;
        public int[]        perRoleButtonIds     = new int[0];
        public GameObject[] perRoleButtonPrefabs = new GameObject[0];

        // ====================================================================
        // CANVAS Y UI
        // ====================================================================
        [Header("Canvas y UI — Panel Principal")]
        public GameObject  canvasRoot;
        public Transform   roleButtonsContainer;
        public InputField  targetPlayerInput;
        public Text        feedbackText;
        public Text        playerInfoText;

        [Header("Canvas Selector de Jugadores")]
        [Tooltip("Canvas secundario con botones de jugadores en instancia para rellenar el input fácilmente")]
        public GameObject  playerSelectorCanvas;
        [Tooltip("Contenedor donde se instancian los botones de jugador")]
        public Transform   playerButtonsContainer;
        [Tooltip("Prefab del botón de jugador (necesita iPlayerSelectorButton). Si es null usa defaultButtonPrefab)")]
        public GameObject  playerSelectorButtonPrefab;
        [Tooltip("Si está activo, el selector de jugadores se actualiza automáticamente cada vez que se abre")]
        public bool        autoRefreshOnOpen = true;
        [Tooltip("Máximo de botones de jugador a mostrar")]
        public int         maxPlayerButtons  = 80;

        // ====================================================================
        // CONTROL DE TECLAS
        // ====================================================================
        [Header("Control de Teclas")]
        public bool    enableKeyToggle   = true;
        public KeyCode pcToggleKey       = KeyCode.Tab;
        [Range(0, 4)]
        public int     questToggleButton = 2;

        // ====================================================================
        // POSICIONAMIENTO
        // ====================================================================
        [Header("Posicionamiento del Canvas")]
        public bool    positionInFrontOfPlayer = true;
        public float   canvasDistance          = 2.0f;
        public float   canvasHeightOffset      = 0.0f;
        public Vector3 canvasExtraRotation     = new Vector3(0f, 180f, 0f);
        public Vector3 canvasScale             = new Vector3(1f, 1f, 1f);

        // ====================================================================
        // EFECTOS
        // ====================================================================
        [Header("Efectos de Audio")]
        public AudioSource openSound;
        public AudioSource assignSound;
        public AudioSource errorSound;
        public AudioSource closeSound;

        // ====================================================================
        // OPCIONES
        // ====================================================================
        [Header("Opciones")]
        public int   maxButtons           = 12;
        public bool  clearInputOnClose    = true;
        public bool  showPlayerInfoOnType = true;
        public float infoUpdateInterval   = 2f;
        public bool  autoCloseAfterAssign = true;
        public float autoCloseDelay       = 1.0f;

        [Header("Debug")]
        public bool debugMode = false;

        // ====================================================================
        // PRIVADAS
        // ====================================================================
        private VRCPlayerApi _localPlayer;
        private int          _currentAdminIndex = -1;
        private bool         _panelOpen         = false;
        private GameObject[] _spawnedButtons;
        private int          _spawnedCount = 0;
        private int[]        _buttonRoleIds;
        private bool         _keyWasPressed = false;

        // Selector de jugadores
        private GameObject[] _spawnedPlayerButtons;
        private int          _spawnedPlayerCount = 0;

        // ====================================================================
        // INICIO
        // ====================================================================
        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;

            if (adminNames      == null) adminNames      = new string[0];
            if (perAdminRoleIds == null) perAdminRoleIds = new int[0];
            if (perAdminPrefabs == null) perAdminPrefabs = new GameObject[0];
            if (allowedRoleIds  == null) allowedRoleIds  = new int[0];
            if (perRoleButtonIds == null) perRoleButtonIds = new int[0];
            if (perRoleButtonPrefabs == null) perRoleButtonPrefabs = new GameObject[0];

            _spawnedButtons      = new GameObject[maxButtons];
            _buttonRoleIds       = new int[maxButtons];
            _spawnedPlayerButtons = new GameObject[maxPlayerButtons];

            ValidateSetup();
            HidePanel();
            if (playerSelectorCanvas != null) playerSelectorCanvas.SetActive(false);

            Log("iRoleAdminPanel iniciado | admins=" + adminNames.Length);
        }

        private void ValidateSetup()
        {
            if (core        == null) LogError("iRoleSystemCore no asignado!");
            if (roleManager == null) LogError("iPlayerRoleManager no asignado!");
            if (roleDatabase == null) LogWarning("iRoleDatabase no asignado.");

            if (targetPlayerInput == null)
                LogWarning("targetPlayerInput no asignado.");
        }

        // ====================================================================
        // UPDATE
        // ====================================================================
        private void Update()
        {
            if (!enableKeyToggle || _localPlayer == null) return;

            bool isMasterAllowed = allowMasterAccess && _localPlayer.isMaster;
            bool isAdmin         = GetAdminIndex(_localPlayer.displayName) >= 0;
            bool hasRoleAccess   = false;
            if (!isAdmin && !isMasterAllowed && roleManager != null && allowedRoleIds != null)
            {
                int localRole = roleManager.GetPlayerRole(_localPlayer.playerId);
                hasRoleAccess = GetAllowedRoleIndex(localRole) >= 0;
            }

            if (!isAdmin && !isMasterAllowed && !hasRoleAccess) return;

            bool keyPressed = Input.GetKey(pcToggleKey);
            if (keyPressed && !_keyWasPressed)
            {
                if (_panelOpen) HidePanel();
                else            OnOpenPanel();
            }
            _keyWasPressed = keyPressed;
        }

        // ====================================================================
        // INTERACT
        // ====================================================================
        public override void Interact() { OnOpenPanel(); }

        public void OnOpenPanel()
        {
            if (_localPlayer == null) return;
            string localName  = _localPlayer.displayName;
            int    adminIndex = GetAdminIndex(localName);

            // 1) Acceso por nombre de admin
            if (adminIndex >= 0)
            {
                _currentAdminIndex = adminIndex;
                ShowPanel();
                return;
            }

            // 2) Acceso del Master (si está permitido)
            if (allowMasterAccess && _localPlayer.isMaster)
            {
                _currentAdminIndex = -2;
                ShowPanelForMaster();
                return;
            }

            // 3) Acceso por rol del jugador local
            if (roleManager != null && allowedRoleIds != null && allowedRoleIds.Length > 0)
            {
                int localRole    = roleManager.GetPlayerRole(_localPlayer.playerId);
                int allowedIndex = GetAllowedRoleIndex(localRole);
                if (allowedIndex >= 0)
                {
                    _currentAdminIndex = -3 - allowedIndex;
                    ShowPanelForAllowedRole(allowedIndex);
                    return;
                }
            }

            PlaySound(errorSound);
            Log(">>> Sin acceso: jugador no es admin, Master autorizado ni tiene rol permitido");
        }

        // ====================================================================
        // MOSTRAR / OCULTAR PANEL
        // ====================================================================
        private void ShowPanel()
        {
            if (canvasRoot == null) { LogError("canvasRoot no asignado!"); return; }

            PlaySound(openSound);

            if (positionInFrontOfPlayer)
                PositionCanvasInFrontOfPlayer();

            ClearRoleButtons();
            BuildRoleButtons();

            canvasRoot.SetActive(true);
            _panelOpen = true;

            UpdatePlayerInfo();

            if (infoUpdateInterval > 0f)
                SendCustomEventDelayedSeconds(nameof(_TickInfoUpdate), infoUpdateInterval);
        }

        // Muestra el panel para un rol permitido con sus botones configurados
        private void ShowPanelForAllowedRole(int allowedIndex)
        {
            if (canvasRoot == null) { LogError("canvasRoot no asignado!"); return; }

            PlaySound(openSound);

            if (positionInFrontOfPlayer)
                PositionCanvasInFrontOfPlayer();

            ClearRoleButtons();

            int start = allowedIndex * maxSlotsPerRole;
            for (int s = 0; s < maxSlotsPerRole && _spawnedCount < maxButtons; s++)
            {
                int idx = start + s;
                if (perRoleButtonIds == null || idx >= perRoleButtonIds.Length) break;

                int roleId = perRoleButtonIds[idx];
                if (roleId < 0) continue;

                GameObject prefab = (perRoleButtonPrefabs != null && idx < perRoleButtonPrefabs.Length)
                    ? perRoleButtonPrefabs[idx] : null;
                if (prefab == null) prefab = defaultButtonPrefab;
                if (prefab == null) { LogWarning("Sin prefab para slot de rol " + s); continue; }

                InstantiateButton(prefab, roleId);
            }

            canvasRoot.SetActive(true);
            _panelOpen = true;

            UpdatePlayerInfo();

            if (infoUpdateInterval > 0f)
                SendCustomEventDelayedSeconds(nameof(_TickInfoUpdate), infoUpdateInterval);
        }

        private int GetAllowedRoleIndex(int roleId)
        {
            if (allowedRoleIds == null || roleId < 0) return -1;
            for (int i = 0; i < allowedRoleIds.Length; i++)
                if (allowedRoleIds[i] == roleId) return i;
            return -1;
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

        // Muestra el panel para el Master con todos los roles de la DB
        private void ShowPanelForMaster()
        {
            if (canvasRoot == null) { LogError("canvasRoot no asignado!"); return; }
            if (roleDatabase == null) { LogError("roleDatabase no asignado!"); return; }

            PlaySound(openSound);

            if (positionInFrontOfPlayer)
                PositionCanvasInFrontOfPlayer();

            ClearRoleButtons();

            int roleCount = roleDatabase.roleNames != null ? roleDatabase.roleNames.Length : 0;
            for (int r = 0; r < roleCount && _spawnedCount < maxButtons; r++)
            {
                if (defaultButtonPrefab == null) { LogWarning("defaultButtonPrefab no asignado para el Master."); break; }
                InstantiateButton(defaultButtonPrefab, r);
            }

            canvasRoot.SetActive(true);
            _panelOpen = true;

            UpdatePlayerInfo();

            if (infoUpdateInterval > 0f)
                SendCustomEventDelayedSeconds(nameof(_TickInfoUpdate), infoUpdateInterval);
        }

        public void HidePanel()
        {
            PlaySound(closeSound);
            ClearRoleButtons();
            ClosePlayerSelector();

            if (clearInputOnClose && targetPlayerInput != null)
                targetPlayerInput.text = "";

            SetFeedback("");
            SetPlayerInfo("");

            _panelOpen         = false;
            _currentAdminIndex = -1;

            if (canvasRoot != null) canvasRoot.SetActive(false);
        }

        // ====================================================================
        // SELECTOR DE JUGADORES
        // ====================================================================

        /// <summary>
        /// Abre el canvas secundario con botones de todos los jugadores en instancia.
        /// </summary>
        public void OpenPlayerSelector()
        {
            if (playerSelectorCanvas == null) { LogWarning("playerSelectorCanvas no asignado."); return; }
            if (playerButtonsContainer == null) { LogWarning("playerButtonsContainer no asignado."); return; }

            if (autoRefreshOnOpen) RefreshPlayerButtons();

            playerSelectorCanvas.SetActive(true);
        }

        /// <summary>
        /// Cierra el canvas secundario del selector de jugadores.
        /// </summary>
        public void ClosePlayerSelector()
        {
            if (playerSelectorCanvas != null) playerSelectorCanvas.SetActive(false);
            ClearPlayerButtons();
        }

        /// <summary>
        /// Regenera los botones de jugador con todos los jugadores actualmente en la instancia.
        /// Llamado automáticamente al abrir si autoRefreshOnOpen está activo.
        /// </summary>
        public void RefreshPlayerButtons()
        {
            if (playerButtonsContainer == null) return;

            ClearPlayerButtons();

            GameObject prefabToUse = playerSelectorButtonPrefab != null
                ? playerSelectorButtonPrefab : defaultButtonPrefab;

            if (prefabToUse == null) { LogError("Sin prefab para botones de jugador (playerSelectorButtonPrefab / defaultButtonPrefab)."); return; }

            VRCPlayerApi[] players = new VRCPlayerApi[80];
            VRCPlayerApi.GetPlayers(players);

            for (int i = 0; i < players.Length && _spawnedPlayerCount < maxPlayerButtons; i++)
            {
                if (players[i] == null || !players[i].IsValid()) continue;

                string name = players[i].displayName;
                InstantiatePlayerButton(prefabToUse, name);
            }

            Log("Selector de jugadores actualizado: " + _spawnedPlayerCount + " jugadores.");
        }

        private void InstantiatePlayerButton(GameObject prefab, string name)
        {
            GameObject btn = Instantiate(prefab);
            btn.transform.SetParent(playerButtonsContainer, false);
            btn.SetActive(true);

            _spawnedPlayerButtons[_spawnedPlayerCount] = btn;
            _spawnedPlayerCount++;

            // Auto-texto TMP con el nombre del jugador
            TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
                tmpText.text = name;
            else
            {
                Text legacyText = btn.GetComponentInChildren<Text>();
                if (legacyText != null) legacyText.text = name;
            }

            // Enlazar el script del botón
            iPlayerSelectorButton selectorBtn = btn.GetComponent<iPlayerSelectorButton>();
            if (selectorBtn != null)
            {
                selectorBtn.parentPanel = this;
                selectorBtn.playerName  = name;
            }
            else
            {
                // Si el prefab no tiene iPlayerSelectorButton (p.ej. usa iRoleAdminButton),
                // intentar configurar igual — el clic se gestiona desde OnButtonClick vía UI
                LogWarning("El prefab de jugador no tiene iPlayerSelectorButton en '" + name + "'.");
            }

            Collider col = btn.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }

        private void ClearPlayerButtons()
        {
            for (int i = 0; i < _spawnedPlayerCount; i++)
            {
                if (_spawnedPlayerButtons[i] != null)
                {
                    Destroy(_spawnedPlayerButtons[i]);
                    _spawnedPlayerButtons[i] = null;
                }
            }
            _spawnedPlayerCount = 0;
        }

        /// <summary>
        /// Callback llamado por iPlayerSelectorButton cuando el usuario hace clic en un nombre.
        /// Rellena el input field y cierra el selector.
        /// </summary>
        public void OnPlayerSelectorClicked(string name)
        {
            if (targetPlayerInput != null)
                targetPlayerInput.text = name;

            ClosePlayerSelector();

            // Actualizar info del jugador inmediatamente
            UpdatePlayerInfo();

            Log("Jugador seleccionado desde selector: " + name);
        }

        // ====================================================================
        // POSICIONAMIENTO DEL CANVAS
        // ====================================================================
        private void PositionCanvasInFrontOfPlayer()
        {
            if (_localPlayer == null || canvasRoot == null) return;

            Vector3    headPosition = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            Quaternion headRotation = _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;

            Vector3 forward = headRotation * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3    canvasPosition = headPosition + forward * canvasDistance;
            canvasPosition.y = headPosition.y + canvasHeightOffset;

            Quaternion baseRotation  = Quaternion.LookRotation(-forward);
            Quaternion extraRotation = Quaternion.Euler(canvasExtraRotation);

            canvasRoot.transform.position   = canvasPosition;
            canvasRoot.transform.rotation   = baseRotation * extraRotation;
            canvasRoot.transform.localScale = canvasScale;
        }

        // ====================================================================
        // BOTONES DE ROL
        // ====================================================================
        private void BuildRoleButtons()
        {
            if (_currentAdminIndex < 0) return;

            int start = _currentAdminIndex * maxSlotsPerAdmin;
            for (int s = 0; s < maxSlotsPerAdmin; s++)
            {
                int idx = start + s;
                if (idx >= perAdminRoleIds.Length) break;

                int roleId = perAdminRoleIds[idx];
                if (roleId < 0) continue;

                GameObject prefab = (idx < perAdminPrefabs.Length && perAdminPrefabs[idx] != null)
                    ? perAdminPrefabs[idx] : defaultButtonPrefab;

                if (prefab == null) { LogWarning("No hay prefab para slot " + s); continue; }
                if (_spawnedCount >= maxButtons) { LogWarning("Máximo de botones alcanzado"); break; }

                InstantiateButton(prefab, roleId);
            }
        }

        private void InstantiateButton(GameObject prefab, int roleId)
        {
            GameObject btn = Instantiate(prefab);
            btn.transform.SetParent(roleButtonsContainer, false);
            btn.SetActive(true);

            _spawnedButtons[_spawnedCount] = btn;
            _buttonRoleIds[_spawnedCount]  = roleId;
            _spawnedCount++;

            iRoleAdminButton btnScript = btn.GetComponent<iRoleAdminButton>();
            if (btnScript != null)
            {
                btnScript.parentPanel = this;
                btnScript.roleId      = roleId;

                if (roleDatabase != null)
                {
                    string roleName = roleDatabase.GetRoleName(roleId);

                    // Auto-configurar texto TMP (TextMeshProUGUI)
                    TextMeshProUGUI tmpText = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (tmpText != null)
                    {
                        tmpText.text = roleName;
                    }
                    else
                    {
                        // Fallback a Text legacy
                        Text btnText = btn.GetComponentInChildren<Text>();
                        if (btnText != null) btnText.text = roleName;
                    }
                }
            }
            else { LogError("El prefab '" + prefab.name + "' no tiene iRoleAdminButton!"); }

            Collider col = btn.GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }

        private void ClearRoleButtons()
        {
            for (int i = 0; i < _spawnedCount; i++)
            {
                if (_spawnedButtons[i] != null) { Destroy(_spawnedButtons[i]); _spawnedButtons[i] = null; }
                _buttonRoleIds[i] = 0;
            }
            _spawnedCount = 0;
        }

        // ====================================================================
        // CALLBACK — asigna rol al jugador objetivo
        // ====================================================================
        public void OnRoleButtonClicked(int roleId)
        {
            if (targetPlayerInput == null)
            {
                SetFeedback(L(iLang.INPUT_NOT_ASSIGNED));
                PlaySound(errorSound);
                return;
            }

            string targetName = targetPlayerInput.text.Trim();

            if (string.IsNullOrEmpty(targetName))
            {
                SetFeedback(L(iLang.WRITE_PLAYER_NAME));
                PlaySound(errorSound);
                return;
            }

            VRCPlayerApi targetPlayer = FindPlayerByName(targetName);

            if (targetPlayer == null)
            {
                SetFeedback(L(iLang.PLAYER_NOT_FOUND, targetName));
                PlaySound(errorSound);
                return;
            }

            if (roleManager == null)
            {
                SetFeedback(L(iLang.ROLE_MANAGER_NULL));
                PlaySound(errorSound);
                return;
            }

            roleManager.SetPlayerRole(targetPlayer.playerId, roleId);

            string roleName = roleDatabase != null
                ? roleDatabase.GetRoleName(roleId)
                : roleId.ToString();

            SetFeedback(L(iLang.ROLE_ASSIGNED, roleName) + " → " + targetName);
            PlaySound(assignSound);
            UpdatePlayerInfo();

            if (autoCloseAfterAssign)
            {
                if (autoCloseDelay > 0f) SendCustomEventDelayedSeconds(nameof(_AutoClosePanel), autoCloseDelay);
                else                     HidePanel();
            }
        }

        public void _AutoClosePanel() { if (_panelOpen) HidePanel(); }

        // ====================================================================
        // BÚSQUEDA DE JUGADOR
        // ====================================================================
        private VRCPlayerApi FindPlayerByName(string name)
        {
            VRCPlayerApi[] players = new VRCPlayerApi[80];
            VRCPlayerApi.GetPlayers(players);
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == null || !players[i].IsValid()) continue;
                if (players[i].displayName.Trim() == name) return players[i];
            }
            return null;
        }

        // ====================================================================
        // ACTUALIZACIÓN DE INFO
        // ====================================================================
        public void _TickInfoUpdate()
        {
            if (!_panelOpen) return;
            UpdatePlayerInfo();
            SendCustomEventDelayedSeconds(nameof(_TickInfoUpdate), infoUpdateInterval);
        }

        public void OnInputChanged() { if (showPlayerInfoOnType) UpdatePlayerInfo(); }

        private void UpdatePlayerInfo()
        {
            if (playerInfoText == null || targetPlayerInput == null) return;

            string targetName = targetPlayerInput.text.Trim();

            if (string.IsNullOrEmpty(targetName))
            {
                SetPlayerInfo(L(iLang.WRITE_NAME_PROMPT));
                return;
            }

            VRCPlayerApi player = FindPlayerByName(targetName);

            if (player == null)
            {
                SetPlayerInfo(L(iLang.PLAYER_NOT_IN_WORLD, targetName));
                return;
            }

            int    currentRole = roleManager  != null ? roleManager.GetPlayerRole(player.playerId) : -1;
            string roleName    = roleDatabase != null ? roleDatabase.GetRoleName(currentRole) : currentRole.ToString();
            string isLocal     = player.isLocal ? L(iLang.YOU_LABEL) : "";

            SetPlayerInfo(targetName + isLocal + "  |  " + L(iLang.CURRENT_ROLE, roleName));
        }

        // ====================================================================
        // UTILIDADES DE UI
        // ====================================================================
        private void SetFeedback(string msg)   { if (feedbackText  != null) feedbackText.text  = msg; }
        private void SetPlayerInfo(string msg) { if (playerInfoText != null) playerInfoText.text = msg; }
        private void PlaySound(AudioSource src) { if (src != null) src.Play(); }

        // Shortcut de localización
        private string L(int id)                => languageSystem != null ? languageSystem.Get(id)       : "?";
        private string L(int id, string arg0)   => languageSystem != null ? languageSystem.Get(id, arg0) : arg0;

        // ====================================================================
        // ADMIN INDEX
        // ====================================================================
        private int GetAdminIndex(string playerName)
        {
            if (string.IsNullOrEmpty(playerName) || adminNames == null) return -1;
            for (int i = 0; i < adminNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(adminNames[i]) && adminNames[i].Trim() == playerName.Trim())
                    return i;
            }
            return -1;
        }

        public bool IsAdminAuthorized(string playerName) => GetAdminIndex(playerName) >= 0;

        // ====================================================================
        // API PÚBLICA EDITOR
        // ====================================================================
        public int GetAdminCount() => adminNames != null ? adminNames.Length : 0;

        public int GetActiveSlotsForAdmin(int adminIndex)
        {
            if (adminIndex < 0 || adminIndex >= GetAdminCount()) return 0;
            int start = adminIndex * maxSlotsPerAdmin;
            int count = 0;
            for (int s = 0; s < maxSlotsPerAdmin; s++)
            {
                int idx = start + s;
                if (idx >= perAdminRoleIds.Length) break;
                if (perAdminRoleIds[idx] >= 0) count++;
            }
            return count;
        }

        // ====================================================================
        // LOGGING
        // ====================================================================
        private void Log(string msg)        { if (debugMode) Debug.Log("[iRoleAdminPanel] " + msg); }
        private void LogWarning(string msg) { Debug.LogWarning("[iRoleAdminPanel] " + msg); }
        private void LogError(string msg)   { Debug.LogError("[iRoleAdminPanel] " + msg); }
    }
}
