// ============================================================================
// iInteractButton - Generic Interact Button for VRChat
// Author: ishiruhii
// Description: Convierte cualquier objeto con collider en un botón interactuable
//              que puede llamar eventos personalizados en otros scripts UdonSharp
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace ishiruhii.iRoleSystem.Utilities
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iInteractButton : UdonSharpBehaviour
    {
        [Header("=== iInteractButton ===")]
        [Space(10)]

        [Header("Target Configuration")]
        [Tooltip("El UdonBehaviour objetivo donde se llamará el evento")]
        public UdonBehaviour targetScript;

        [Tooltip("Nombre del evento/método a llamar (debe ser público)")]
        public string customEventName = "OnInteract";

        [Header("Interaction Settings")]
        [Tooltip("Texto que aparece al apuntar al objeto")]
        public string interactionText = "Interact";

        [Tooltip("Distancia máxima de interacción")]
        public float interactionProximity = 2f;

        [Header("Access Control (Opcional)")]
        [Tooltip("Si está activado, solo ciertos jugadores pueden usar el botón")]
        public bool useWhitelist = false;

        [Tooltip("Lista de nombres de jugador autorizados (solo si useWhitelist = true)")]
        public string[] whitelistedPlayers = new string[0];

        [Header("Feedback (Opcional)")]
        [Tooltip("Sonido al interactuar")]
        public AudioSource interactSound;

        [Tooltip("Animador para efectos visuales")]
        public Animator buttonAnimator;

        [Tooltip("Nombre del trigger de animación")]
        public string animationTrigger = "Press";

        [Header("Cooldown")]
        [Tooltip("Tiempo de espera entre interacciones (segundos)")]
        public float cooldownTime = 0.5f;

        [Header("Debug")]
        [Tooltip("Mostrar mensajes de debug")]
        public bool debugMode = false;

        // Variables internas
        private float lastInteractionTime = 0f;

        private void Start()
        {
            // Configurar texto de interacción
            if (!string.IsNullOrEmpty(interactionText))
            {
                InteractionText = interactionText;
            }

            // La proximidad se configura en el VRC_Pickup o directamente en el Inspector

            if (debugMode)
            {
                Log("iInteractButton inicializado. Target: " +
                    (targetScript != null ? targetScript.gameObject.name : "NO ASIGNADO") +
                    ", Evento: " + customEventName);
            }
        }

        public override void Interact()
        {
            // Verificar cooldown
            if (Time.time - lastInteractionTime < cooldownTime)
            {
                if (debugMode) Log("Cooldown activo, ignorando interacción");
                return;
            }

            // Verificar whitelist si está activada
            if (useWhitelist && !IsPlayerAuthorized())
            {
                if (debugMode) Log("Jugador no autorizado: " + Networking.LocalPlayer.displayName);
                return;
            }

            // Actualizar tiempo de última interacción
            lastInteractionTime = Time.time;

            // Reproducir efectos
            PlayFeedback();

            // Llamar al evento en el script objetivo
            CallTargetEvent();
        }

        /// <summary>
        /// Llama al evento personalizado en el script objetivo
        /// </summary>
        private void CallTargetEvent()
        {
            if (targetScript == null)
            {
                LogWarning("No hay targetScript asignado!");
                return;
            }

            if (string.IsNullOrEmpty(customEventName))
            {
                LogWarning("No hay customEventName configurado!");
                return;
            }

            if (debugMode)
            {
                Log("Llamando evento '" + customEventName + "' en " + targetScript.gameObject.name);
            }

            // Llamar al evento usando SendCustomEvent
            targetScript.SendCustomEvent(customEventName);
        }

        /// <summary>
        /// Verifica si el jugador actual está en la whitelist
        /// </summary>
        private bool IsPlayerAuthorized()
        {
            if (Networking.LocalPlayer == null) return false;
            if (whitelistedPlayers == null || whitelistedPlayers.Length == 0) return false;

            string playerName = Networking.LocalPlayer.displayName;

            for (int i = 0; i < whitelistedPlayers.Length; i++)
            {
                if (!string.IsNullOrEmpty(whitelistedPlayers[i]) &&
                    whitelistedPlayers[i].Trim() == playerName.Trim())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reproduce efectos de feedback
        /// </summary>
        private void PlayFeedback()
        {
            // Sonido
            if (interactSound != null)
            {
                interactSound.Play();
            }

            // Animación
            if (buttonAnimator != null && !string.IsNullOrEmpty(animationTrigger))
            {
                buttonAnimator.SetTrigger(animationTrigger);
            }
        }

        #region Public Methods for External Calls

        /// <summary>
        /// Permite cambiar el target en runtime
        /// </summary>
        public void SetTarget(UdonBehaviour newTarget, string eventName)
        {
            targetScript = newTarget;
            customEventName = eventName;

            if (debugMode)
            {
                Log("Target actualizado: " + newTarget.gameObject.name + " -> " + eventName);
            }
        }

        /// <summary>
        /// Activa o desactiva la interacción
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            // En UdonSharp, desactivar el objeto o collider para "desactivar" interacción
            gameObject.GetComponent<Collider>().enabled = interactable;

            if (debugMode)
            {
                Log("Interacción " + (interactable ? "activada" : "desactivada"));
            }
        }

        /// <summary>
        /// Simula una interacción (para llamar desde otros scripts)
        /// </summary>
        public void SimulateInteract()
        {
            Interact();
        }

        #endregion

        #region Logging

        private void Log(string message)
        {
            Debug.Log("[iInteractButton] " + message);
        }

        private void LogWarning(string message)
        {
            Debug.LogWarning("[iInteractButton] " + message);
        }

        #endregion
    }
}
