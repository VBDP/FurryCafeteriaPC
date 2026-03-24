// ============================================================================
// iRoleSystem v4.0.5 - RankDisplay Module
// Author: ishiruhii
// Description: Sistema de insignias/displays sobre la cabeza de jugadores
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.RankDisplay
{
    /// <summary>
    /// Configuración de un tipo de display de rango.
    /// Define qué prefab mostrar y para qué roles.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRankDisplayConfig : UdonSharpBehaviour
    {
        [Header("Configuración de Display")]
        [Tooltip("Prefab a instanciar sobre la cabeza")]
        public GameObject displayPrefab;

        [Tooltip("Altura sobre la cabeza del jugador")]
        public float heightOffset = 0.35f;

        [Tooltip("Escala del display")]
        public Vector3 displayScale = Vector3.one;

        [Header("Roles Permitidos")]
        [Tooltip("Qué roles mostrarán este display")]
        public bool[] allowedRoles;

        [Header("Animación (Opcional)")]
        [Tooltip("Clip de animación para el display")]
        public AnimationClip animationClip;

        [Tooltip("Loop de animación")]
        public bool loopAnimation = true;

        [Header("Configuración Visual")]
        [Tooltip("Siempre mirar hacia la cámara")]
        public bool billboard = true;

        [Tooltip("Solo eje Y del billboard")]
        public bool billboardYOnly = true;
    }
}
