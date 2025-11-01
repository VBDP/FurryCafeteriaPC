using System.ComponentModel;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace TapGhoul.RespawnObjects
{
    public class ResetPickups : UdonSharpBehaviour
    {
        [SerializeField, Description("Synced objects to reset")]
        private VRCObjectSync[] syncedObjects = new VRCObjectSync[0];

        public void _ResetPickups()
        {
            var localPlayer = Networking.LocalPlayer;
            foreach (var obj in syncedObjects)
            {
                Networking.SetOwner(localPlayer, obj.gameObject);
                obj.Respawn();
            }
        }
    }
}