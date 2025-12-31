using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportOnClick : UdonSharpBehaviour
{
    [Header("Punto de destino")]
    public Transform destino;

    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;

        if (player == null || destino == null)
            return;

        // Teletransportar al jugador local
        player.TeleportTo(destino.position, destino.rotation);
    }
}
