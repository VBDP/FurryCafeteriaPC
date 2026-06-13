// ============================================================================
// iPlayerSelectorButton.cs
// Author: ishiruhii
// Description: Botón que rellena el InputField del panel admin con el nombre
//              de un jugador de la instancia al hacer clic/interact.
// ============================================================================

using UdonSharp;
using UnityEngine;

namespace ishiruhii.iRoleSystem.Modules.AdminPanel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iPlayerSelectorButton : UdonSharpBehaviour
    {
        [HideInInspector] public iRoleAdminPanel parentPanel;
        [HideInInspector] public string          playerName;

        public override void Interact()
        {
            if (parentPanel != null)
                parentPanel.OnPlayerSelectorClicked(playerName);
            else
                Debug.LogError("[iPlayerSelectorButton] parentPanel es NULL!");
        }

        // Para botones UI (OnClick en Canvas UI)
        public void OnButtonClick()
        {
            if (parentPanel != null)
                parentPanel.OnPlayerSelectorClicked(playerName);
        }
    }
}
