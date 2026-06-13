// ============================================================================
// iRoleAdminButton.cs
// Author: ishiruhii
// Description: Botón de rol para el panel de administración
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace ishiruhii.iRoleSystem.Modules.AdminPanel
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleAdminButton : UdonSharpBehaviour
    {
        [HideInInspector] public iRoleAdminPanel parentPanel;
        [HideInInspector] public int roleId;

        public void Initialize(iRoleAdminPanel parent, int id)
        {
            parentPanel = parent;
            roleId      = id;
        }

        public override void Interact()
        {
            if (parentPanel != null)
                parentPanel.OnRoleButtonClicked(roleId);
            else
                Debug.LogError("[iRoleAdminButton] parentPanel es NULL!");
        }

        // Para botones UI (OnClick)
        public void OnButtonClick()
        {
            if (parentPanel != null)
                parentPanel.OnRoleButtonClicked(roleId);
        }
    }
}
