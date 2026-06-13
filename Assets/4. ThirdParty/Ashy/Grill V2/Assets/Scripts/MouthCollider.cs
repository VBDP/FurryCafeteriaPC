using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;


public class MouthCollider : UdonSharpBehaviour
{
    private void LateUpdate()
    {

        //Find the player's position
        Vector3 playerPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        //Move from center to head area
        transform.position=playerPos - new Vector3(0, 0.1f, 0);
        
            
    }
}


