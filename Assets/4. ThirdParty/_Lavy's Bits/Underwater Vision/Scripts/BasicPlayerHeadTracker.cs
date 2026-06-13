
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class BasicPlayerHeadTracker : UdonSharpBehaviour
{
    VRCPlayerApi locl;
    void Start()
    {
        //saving local player, dunno if it's much more performant than using it all the time but ehhh
        locl = Networking.LocalPlayer;
    }

    public override void PostLateUpdate()
    {
        //get the exact head's position and store that in the current gameobject for later reference,
        //while i could just get the pos and check that, i like having the object to reference in other scripts
        gameObject.transform.position = locl.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
    }
}
