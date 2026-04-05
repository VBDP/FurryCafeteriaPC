using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TeleportOnClick : UdonSharpBehaviour
{
    [Header("Punto de destino")]
    public Transform destino;
    public Keypad keypad;

    public override void Interact()
    {
        VRCPlayerApi player = Networking.LocalPlayer;

        if (player == null || destino == null || keypad == null)
            return;

        if (!keypad.IsLocalPlayerAdmin())
            return;

        player.TeleportTo(destino.position, destino.rotation);
    }
}
