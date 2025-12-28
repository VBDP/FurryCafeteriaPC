using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon.Serialization.OdinSerializer;

namespace TapGhoul.Collars
{
    [RequireComponent(typeof(VRCPickup))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PickupDelegator : UdonSharpBehaviour
    {
        public UdonSharpBehaviour delegateTarget;
        public string pickupEvent;

        // We use PreviouslySerializedAs here for compatibility for v1.0.0 -> v1.1.0
        [PreviouslySerializedAs("dropEvent")]
        public string intentionalDropEvent;

        public string stolenDropEvent;

        public override void OnPickup()
        {
            if (delegateTarget == null || pickupEvent == null || pickupEvent == "") return;

            Networking.SetOwner(Networking.LocalPlayer, delegateTarget.gameObject);
            delegateTarget.SendCustomEvent(pickupEvent);
        }

        public override void OnDrop()
        {
            var eventToDispatch = Networking.IsOwner(gameObject) ? intentionalDropEvent : stolenDropEvent;
            if (delegateTarget == null || eventToDispatch == null || eventToDispatch == "") return;

            // Probably redundant, but just in case.
            Networking.SetOwner(Networking.LocalPlayer, delegateTarget.gameObject);
            delegateTarget.SendCustomEvent(eventToDispatch);
        }
    }
}