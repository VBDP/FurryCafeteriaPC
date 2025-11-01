
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class AimText : UdonSharpBehaviour
{
    public Transform ObjectToPointAtTarget;
    private Vector3 playerHead;
    private VRCPlayerApi localPlayer;

    private float yLevel;

    void Start()
    {
        yLevel = ObjectToPointAtTarget.transform.position.y;
        localPlayer = Networking.LocalPlayer;
    }

    private void Update()
    {
        if (localPlayer == null) return;

        //var head = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);       
        //playerHead = head.position;
        var playerHead = localPlayer.GetPosition();

        Vector3 point = playerHead;
        point.y = yLevel;
        ObjectToPointAtTarget.LookAt(point);
    }

}
