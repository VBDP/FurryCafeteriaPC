// ============================================================================
// iRoleSystem v4.0.5 - iRoleButton Helper (VERSIÓN SIMPLE)
// Author: ishiruhii
// Description: Simple approach - calls parent directly with stored roleId
// ============================================================================

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;

namespace ishiruhii.iRoleSystem.Modules.iNameGetRole
{
    /// <summary>
    /// Simple helper - stores roleId as int and calls parent directly
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class iRoleButton : UdonSharpBehaviour
    {
        [HideInInspector]
        public iNameGetRole parentModule;

        // Store roleId directly - this should work
        [HideInInspector]
        public int roleId;

        /// <summary>
        /// Initialize with parent and roleId
        /// </summary>
        public void Initialize(iNameGetRole parent, int id)
        {
            parentModule = parent;
            roleId = id;
        }

        /// <summary>
        /// Called when button is clicked
        /// </summary>
        public override void Interact()
        {
            Debug.Log("[iRoleButton] Interact() llamado - roleId=" + roleId + " | parentModule=" + (parentModule != null ? "OK" : "NULL"));

            if (parentModule != null)
            {
                parentModule.OnRoleSelected(roleId);
            }
            else
            {
                Debug.LogError("[iRoleButton] parentModule es NULL!");
            }
        }

        /// <summary>
        /// For OnClick in Unity
        /// </summary>
        public void OnButtonClick()
        {
            Debug.Log("[iRoleButton] OnButtonClick() llamado - roleId=" + roleId + " | parentModule=" + (parentModule != null ? "OK" : "NULL"));

            if (parentModule != null)
            {
                parentModule.OnRoleSelected(roleId);
            }
            else
            {
                Debug.LogError("[iRoleButton] parentModule es NULL!");
            }
        }
    }
}
