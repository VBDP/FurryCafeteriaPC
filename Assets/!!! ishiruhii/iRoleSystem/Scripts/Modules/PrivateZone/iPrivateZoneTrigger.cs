// ============================================================================
// iRoleSystem v5.6 - iPrivateZoneTrigger
// Author: ishiruhii
// DEPRECADO: Este componente ya no es necesario con iAreaZone.
// iPrivateZoneManager ahora detecta la presencia del jugador internamente
// mediante un bucle de polling (CheckZone). Puedes eliminar este script
// y el GameObject que lo contenía de tu escena.
//
// Si necesitas mantenerlo por compatibilidad con prefabs existentes,
// simplemente déjalo en el proyecto — no hace nada.
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.PrivateZone
{
    /// <summary>
    /// DEPRECADO: Ya no necesario. iPrivateZoneManager detecta zonas
    /// internamente via iAreaZone. Este script es un stub vacío para
    /// evitar errores de compilación si el prefab antiguo aún lo referencia.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iPrivateZoneTrigger : UdonSharpBehaviour
    {
        [Header("⚠ DEPRECADO")]
        [Tooltip("Este componente ya no es necesario. El iPrivateZoneManager con iAreaZone detecta zonas por polling interno. Puedes eliminar este componente y su GameObject.")]
        public iPrivateZoneManager zoneManager;

        // Stub vacío — sin lógica. El nuevo sistema no usa triggers.
    }
}
