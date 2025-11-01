
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class MakeClickableInteract : UdonSharpBehaviour
{
    [Header("Behavior to activate interact on")]
    public UdonBehaviour callback;
    public string eventName;
    public override void Interact()
    {
        callback.SendCustomEvent(eventName);
    }
}
