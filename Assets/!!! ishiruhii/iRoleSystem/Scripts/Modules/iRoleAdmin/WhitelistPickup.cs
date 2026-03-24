using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class WhitelistPickup : UdonSharpBehaviour
{
    [Header("Pickups controlados por whitelist")]
    public VRCPickup[] pickups;

    [Header("Whitelist de DisplayNames (exacto, case-sensitive)")]
    public string[] allowedPlayers = new string[]
    {
        "JugadorUno",
        "JugadorDos",
        "OtroJugador"
    };

    private bool localPlayerAllowed = false;

    void Start()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null) return;

        string localName = localPlayer.displayName;

        foreach (string name in allowedPlayers)
        {
            if (localName == name)
            {
                localPlayerAllowed = true;
                break;
            }
        }

        ApplyPickupAccess();
    }

    private void ApplyPickupAccess()
    {
        foreach (VRCPickup pickup in pickups)
        {
            if (pickup == null) continue;
            pickup.pickupable = localPlayerAllowed;
        }
    }
}