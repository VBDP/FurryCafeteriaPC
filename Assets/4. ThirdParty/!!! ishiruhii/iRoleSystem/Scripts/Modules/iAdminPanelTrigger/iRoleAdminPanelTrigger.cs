// ============================================================================
// iRoleAdminPanelTrigger.cs
// Author: ishiruhii
// Description: Abre el iRoleAdminPanel con una tecla/botón VR,
//              posicionándolo delante del avatar y escalándolo por su altura.
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.AdminPanel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleAdminPanelTrigger : UdonSharpBehaviour
    {
        [Header("=== iRoleAdminPanelTrigger ===")]
        [Space(5)]

        // ====================================================================
        // REFERENCIAS
        // ====================================================================
        [Header("Referencias")]
        [Tooltip("El script iRoleAdminPanel al que se llamará OnOpenPanel")]
        public iRoleAdminPanel adminPanel;

        [Tooltip("Transform del canvas que se reposicionará frente al jugador")]
        public Transform canvasTransform;

        // ====================================================================
        // WHITELIST
        // ====================================================================
        [Header("Whitelist de Usuarios")]
        [Tooltip("Solo estos DisplayNames podrán abrir el panel con la tecla configurada")]
        public string[] authorizedUsers = new string[0];

        [Tooltip("Si está activo, permite sincronizar automáticamente con la lista de admins del iRoleAdminPanel")]
        public bool syncWithAdminPanel = true;

        // ====================================================================
        // CONFIGURACIÓN DE INPUT
        // ====================================================================
        [Header("Configuración de Input")]
        [Tooltip("0 = Teclado PC  |  1 = Oculus / VR")]
        public int inputMode = 0; // 0 = PC, 1 = Oculus

        // PC - KeyCode almacenado como int para compatibilidad UdonSharp
        [Tooltip("Tecla del teclado (solo se usa si inputMode = PC)")]
        public KeyCode pcKey = KeyCode.Tab;

        // VR - índice del array de botones (ver editor para los nombres)
        [Tooltip("Índice del botón de Oculus (solo se usa si inputMode = Oculus)")]
        public int oculusButtonIndex = 0;

        // Nombres reales de los botones para Input.GetButtonDown
        // ¡No cambiar el orden! El índice debe coincidir con el editor
        [HideInInspector]
        public string[] oculusButtonNames = new string[]
        {
            "Oculus_CrossPlatform_Button1",              // A (mano derecha)
            "Oculus_CrossPlatform_Button2",              // B (mano derecha)
            "Oculus_CrossPlatform_Button3",              // X (mano izquierda)
            "Oculus_CrossPlatform_Button4",              // Y (mano izquierda)
            "Oculus_CrossPlatform_PrimaryHandTrigger",   // Grip derecho
            "Oculus_CrossPlatform_SecondaryHandTrigger", // Grip izquierdo
            "Oculus_CrossPlatform_PrimaryIndexTrigger",  // Index derecho
            "Oculus_CrossPlatform_SecondaryIndexTrigger",// Index izquierdo
            "Oculus_CrossPlatform_PrimaryThumbstick",    // Stick derecho (clic)
            "Oculus_CrossPlatform_SecondaryThumbstick"   // Stick izquierdo (clic)
        };

        // ====================================================================
        // POSICIONAMIENTO DEL CANVAS
        // ====================================================================
        [Header("Posicionamiento del Canvas")]
        [Tooltip("Distancia frente al jugador (multiplicada por altura del avatar)")]
        public float distanceMultiplier = 0.55f;

        [Tooltip("Offset vertical relativo a la cabeza (en metros, escala con avatar)")]
        public float verticalOffset = -0.1f;

        [Tooltip("Altura base de referencia para escalar el canvas (metros)")]
        public float baseAvatarHeight = 1.6f;

        [Tooltip("Escala base del canvas para un avatar de 'baseAvatarHeight' metros")]
        public float baseCanvasScale = 1.0f;

        [Tooltip("¿El canvas debe mirar hacia el jugador al abrirse?")]
        public bool facingPlayer = true;

        [Tooltip("¿Ignorar el eje Y al orientar el canvas? (recomendado para mundos planos)")]
        public bool flatRotation = true;

        [Tooltip("Rotación extra aplicada al canvas (en grados Euler)")]
        public Vector3 extraRotation = new Vector3(0f, 180f, 0f);

        // ====================================================================
        // COOLDOWN
        // ====================================================================
        [Header("Cooldown")]
        [Tooltip("Tiempo mínimo entre pulsaciones (segundos)")]
        public float cooldown = 0.5f;

        // ====================================================================
        // OPCIONES
        // ====================================================================
        [Header("Opciones")]
        [Tooltip("Si está activo, posiciona el canvas al abrirse (si no, usa la posición configurada en el iRoleAdminPanel)")]
        public bool overridePositioning = false;

        // ====================================================================
        // DEBUG
        // ====================================================================
        [Header("Debug")]
        public bool debugMode = false;

        // ====================================================================
        // VARIABLES PRIVADAS
        // ====================================================================
        private VRCPlayerApi _localPlayer;
        private float        _lastTriggerTime = -999f;
        private string       _activeOculusButton = "";

        // ====================================================================
        // INICIO
        // ====================================================================
        private void Start()
        {
            _localPlayer = Networking.LocalPlayer;

            // Cachear el nombre del botón activo para evitar indexación en Update
            if (inputMode == 1 && oculusButtonNames != null &&
                oculusButtonIndex >= 0 && oculusButtonIndex < oculusButtonNames.Length)
            {
                _activeOculusButton = oculusButtonNames[oculusButtonIndex];
            }

            // Sincronizar whitelist con el admin panel si está configurado
            if (syncWithAdminPanel)
            {
                SyncWhitelistFromAdminPanel();
            }

            Log("iRoleAdminPanelTrigger iniciado");
            Log("InputMode: " + (inputMode == 0 ? "PC Teclado" : "Oculus VR"));
            Log("Usuarios autorizados: " + (authorizedUsers != null ? authorizedUsers.Length : 0));
        }

        // ====================================================================
        // UPDATE - DETECTAR INPUT
        // ====================================================================
        private void Update()
        {
            if (_localPlayer == null) return;
            if (Time.time - _lastTriggerTime < cooldown) return;

            bool triggered = false;

            if (inputMode == 0)
            {
                // Modo teclado PC
                triggered = Input.GetKeyDown(pcKey);
            }
            else
            {
                // Modo Oculus VR
                if (!string.IsNullOrEmpty(_activeOculusButton))
                    triggered = Input.GetButtonDown(_activeOculusButton);
            }

            if (triggered)
            {
                OnTriggerActivated();
            }
        }

        // ====================================================================
        // LÓGICA PRINCIPAL
        // ====================================================================
        private void OnTriggerActivated()
        {
            if (_localPlayer == null) return;

            string playerName = _localPlayer.displayName;

            Log(">>> Input detectado por: " + playerName);

            // Comprobar whitelist
            if (!IsAuthorized(playerName))
            {
                Log(">>> No autorizado, ignorando");
                return;
            }

            if (adminPanel == null)
            {
                LogWarning("adminPanel no está asignado!");
                return;
            }

            // Aplicar cooldown
            _lastTriggerTime = Time.time;

            // Verificar si el panel está abierto o cerrado
            bool isPanelOpen = IsPanelOpen();

            if (isPanelOpen)
            {
                // Panel abierto → cerrarlo
                Log(">>> Panel abierto, cerrando...");
                adminPanel.HidePanel();
            }
            else
            {
                // Panel cerrado → abrirlo
                Log(">>> Panel cerrado, abriendo...");
                
                // Reposicionar canvas si está configurado
                if (overridePositioning && canvasTransform != null)
                {
                    PositionCanvasInFrontOfPlayer();
                }
                else if (overridePositioning && canvasTransform == null)
                {
                    LogWarning("overridePositioning está activo pero canvasTransform no asignado.");
                }

                adminPanel.OnOpenPanel();
            }
        }

        // ====================================================================
        // VERIFICAR ESTADO DEL PANEL
        // ====================================================================
        /// <summary>
        /// Verifica si el panel está abierto comprobando si el canvas está activo
        /// </summary>
        private bool IsPanelOpen()
        {
            if (adminPanel == null) return false;
            if (adminPanel.canvasRoot == null) return false;
            
            return adminPanel.canvasRoot.activeSelf;
        }

        // ====================================================================
        // POSICIONAMIENTO
        // ====================================================================
        private void PositionCanvasInFrontOfPlayer()
        {
            // Datos de cabeza del jugador local
            VRCPlayerApi.TrackingData headData =
                _localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

            Vector3    headPos = headData.position;
            Quaternion headRot = headData.rotation;

            // Dirección "hacia delante" del jugador
            Vector3 forward = headRot * Vector3.forward;

            if (flatRotation)
            {
                forward.y = 0f;
                if (forward == Vector3.zero) forward = Vector3.forward;
                forward.Normalize();
            }

            // Altura del avatar para escalar distancia y posición
            float eyeHeight = _localPlayer.GetAvatarEyeHeightAsMeters();
            if (eyeHeight <= 0f) eyeHeight = baseAvatarHeight;

            float heightRatio = eyeHeight / baseAvatarHeight;

            // Posición final del canvas
            Vector3 canvasPos = headPos
                + forward * (distanceMultiplier * eyeHeight)
                + Vector3.up * (verticalOffset * heightRatio);

            canvasTransform.position = canvasPos;

            // Orientación del canvas
            if (facingPlayer)
            {
                if (flatRotation)
                {
                    // El canvas mira hacia el jugador solo en el plano horizontal
                    Vector3 lookDir = canvasPos - new Vector3(headPos.x, canvasPos.y, headPos.z);
                    if (lookDir != Vector3.zero)
                    {
                        Quaternion baseRotation = Quaternion.LookRotation(lookDir);
                        Quaternion extraRot = Quaternion.Euler(extraRotation);
                        canvasTransform.rotation = baseRotation * extraRot;
                    }
                }
                else
                {
                    canvasTransform.LookAt(headPos);
                    Quaternion extraRot = Quaternion.Euler(extraRotation);
                    canvasTransform.rotation = canvasTransform.rotation * extraRot;
                }
            }

            // Escala proporcional a la altura del avatar
            float scaleFactor = baseCanvasScale * heightRatio;
            canvasTransform.localScale = Vector3.one * scaleFactor;

            Log(">>> Canvas posicionado | eyeHeight=" + eyeHeight.ToString("F2") +
                " | pos=" + canvasPos + " | scale=" + scaleFactor.ToString("F3"));
        }

        // ====================================================================
        // WHITELIST
        // ====================================================================
        private bool IsAuthorized(string playerName)
        {
            if (authorizedUsers == null || authorizedUsers.Length == 0) return false;

            for (int i = 0; i < authorizedUsers.Length; i++)
            {
                if (!string.IsNullOrEmpty(authorizedUsers[i]) &&
                    authorizedUsers[i].Trim() == playerName.Trim())
                    return true;
            }
            return false;
        }

        // ====================================================================
        // MÉTODO PÚBLICO - Sincronizar whitelist desde iRoleAdminPanel
        // ====================================================================
        /// <summary>
        /// Copia los adminNames del iRoleAdminPanel a la whitelist de este trigger.
        /// Útil para mantenerlos sincronizados sin duplicar configuración.
        /// </summary>
        public void SyncWhitelistFromAdminPanel()
        {
            if (adminPanel == null || adminPanel.adminNames == null) return;
            
            authorizedUsers = new string[adminPanel.adminNames.Length];
            for (int i = 0; i < adminPanel.adminNames.Length; i++)
                authorizedUsers[i] = adminPanel.adminNames[i];

            Log("Whitelist sincronizada desde iRoleAdminPanel: " + authorizedUsers.Length + " usuarios");
        }

        // ====================================================================
        // MÉTODO PÚBLICO - Alternar panel
        // ====================================================================
        /// <summary>
        /// Alterna entre abrir y cerrar el panel (útil para llamar desde botones UI)
        /// </summary>
        public void TogglePanel()
        {
            if (adminPanel == null) return;

            // Verificar autorización
            if (_localPlayer == null) return;
            string playerName = _localPlayer.displayName;
            if (!IsAuthorized(playerName))
            {
                Log(">>> TogglePanel: No autorizado");
                return;
            }

            // Llamar a la lógica principal de toggle
            OnTriggerActivated();
        }

        // ====================================================================
        // LOGGING
        // ====================================================================
        private void Log(string msg)
        {
            if (debugMode) Debug.Log("[iRoleAdminPanelTrigger] " + msg);
        }

        private void LogWarning(string msg)
        {
            Debug.LogWarning("[iRoleAdminPanelTrigger] " + msg);
        }
    }
}
