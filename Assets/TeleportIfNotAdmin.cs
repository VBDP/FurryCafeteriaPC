using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class TeleportIfNotAdmin : UdonSharpBehaviour
{
    public Transform teleportLocation;
    public float minVelocity = 2f; // velocidad mínima para considerar "entrada volando"

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        Vector3 velocity = player.GetVelocity();

        if (velocity.magnitude >= minVelocity)
        {
            player.TeleportTo(teleportLocation.position, teleportLocation.rotation);
        }
    }
}