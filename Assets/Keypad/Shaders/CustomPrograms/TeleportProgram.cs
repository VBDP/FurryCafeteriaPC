
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportProgram : UdonSharpBehaviour
{
    public BoxCollider door;
       public void keypadGranted()
    {
        Debug.Log("Player Granted");
        door.enabled = true;
        //Networking.LocalPlayer.TeleportTo(transform.position, transform.rotation);
    }
}

