
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TwoPositionSwampper : UdonSharpBehaviour
{
    public Transform objectToTp;

    [Header("")]
    public Transform positionOne;
    public Transform positionTwo;

    [Header("Should the object be rotated when teleporting?")]
    public bool rotateOnTeleport = true;

    [Header("Start Position, will teleport to one if false, otherwise to two")]
    public bool defaultPosition = false;

    [Header("Optional Callback to call other scipts")]
    public UdonBehaviour callback;
    bool hasCallback = false;

    public string posOneEvent = "";
    public string posTwoEvent = "";
    public string teleportEvent = "";

    bool hasOneEvent = false;
    bool hasTwoEvent = false;
    bool hasTeleportEvent = false;
    void Start()
    {
        if (callback != null)
        {
            hasCallback = true;

            if (posOneEvent != "") hasOneEvent = true;
            if (posTwoEvent != "") hasTwoEvent = true;
            if (teleportEvent != "") hasTeleportEvent = true;
        }

        if (defaultPosition)
        {
            _TpToOne();
        } else
        {
            _TpToTwo();
        }
    }

    public void _TpToOne()
    {
        objectToTp.position = positionOne.position;
        if (rotateOnTeleport) objectToTp.rotation = positionOne.rotation;

        defaultPosition = false;

        if (hasCallback)
        {
            if (hasOneEvent) callback.SendCustomEvent(posOneEvent);
            if (hasTeleportEvent) callback.SendCustomEvent(teleportEvent);
        }
    }

    public void _TpToTwo()
    {
        objectToTp.position = positionTwo.position;
        if (rotateOnTeleport) objectToTp.rotation = positionTwo.rotation;

        defaultPosition = true;

        if (hasCallback)
        {
            if (hasTwoEvent) callback.SendCustomEvent(posOneEvent);
            if (hasTeleportEvent) callback.SendCustomEvent(teleportEvent);
        }
    }

    public void _TogglePosition()
    {
        defaultPosition = !defaultPosition;

        if (defaultPosition) 
        {
            _TpToTwo();
        } else
        {
            _TpToOne();
        }
    }
}
