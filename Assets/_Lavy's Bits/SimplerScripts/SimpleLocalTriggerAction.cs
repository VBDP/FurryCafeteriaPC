
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SimpleLocalTriggerAction : UdonSharpBehaviour
{
    [Header("Optional Callback to call other scipts")]
    public UdonBehaviour callback;
    public string OnPlayerEnterName = "";
    public string OnPlayerExitName = "";

    bool hasEnter = false;
    bool hasExit = false;

    public bool isLocal = true;

    void Start()
    {
        if (OnPlayerEnterName != "") hasEnter = true;
        if (OnPlayerExitName != "") hasExit = true;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!hasEnter) return;

        if (isLocal)
        {
            if (player != Networking.LocalPlayer) return;
        }

         callback.SendCustomEvent(OnPlayerEnterName);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (!hasExit) return;

        if (isLocal)
        {
            if (player != Networking.LocalPlayer) return;
        }

        callback.SendCustomEvent(OnPlayerExitName);
    }
}
