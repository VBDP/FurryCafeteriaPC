
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BasicToggleV2 : UdonSharpBehaviour
{
    [Header("If Default State is \"True\", these objects will be on")]
    public GameObject[] toOn;
    [Header("If Default State is \"True\", these objects will be off")]
    public GameObject[] toOff;

    [Header("Should we set the default state on start?")]
    public bool shouldStateDefault = true;

    [Header("The state that should be applied on start")]
    [UdonSynced]
    public bool defaultState = false;

    bool hasOn = false;
    bool hasOff = false;

    [Header("Should this be a global toggle?")]
    public bool isSync = false;

    [Header("Optional Callback to call other scipts")]
    public UdonBehaviour callback;
    public string OnEventName;
    public string OffEventName;

    bool hasCallback = false;
    void Start()
    {
        if (toOn.Length > 0) hasOn = true;
        if (toOff.Length > 0) hasOff = true;
        if (callback != null) hasCallback = true;

        if (!shouldStateDefault) return;
        if (defaultState)
        {
            SendCustomEventDelayedFrames(nameof(_ApplyState), 3);
        }
        else
        {
            _ApplyState();
        }
    }

    public void _SetOff()
    {
        defaultState = false;

        _SyncCheck();

        _ApplyState();
    }

    public void _SetOn()
    {
        defaultState = true;

        _SyncCheck();

        _ApplyState();
    }

    public void _Toggle()
    {
        defaultState = !defaultState;

        _SyncCheck();

        _ApplyState();
    }

    public void _SyncCheck()
    {
        if (isSync)
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }

    public void _ApplyState()
    {
        if (hasOn)
        {
            for (int i = 0; i < toOn.Length; i++)
            {
                toOn[i].SetActive(defaultState);
            }
        }

        if (hasOff)
        {
            for (int i = 0; i < toOff.Length; i++)
            {
                toOff[i].SetActive(!defaultState);
            }
        }

        if (hasCallback)
        {
            if (defaultState)
            {
                callback.SendCustomEvent(OnEventName);
            }
            else
            {
                callback.SendCustomEvent(OffEventName);
            }
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (isSync) RequestSerialization();
    }

    public override void OnDeserialization()
    {
        if (isSync) _ApplyState();
    }
}
