
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SimpleTeleport : UdonSharpBehaviour
{
    [Header("Usable with interact, or through a callback with \"_TpPlayer\"")]
    public Transform tpPosition;
    public override void Interact()
    {
        _TpPlayer();
    }

    public void _TpPlayer()
    {
        Networking.LocalPlayer.TeleportTo(tpPosition.position, tpPosition.rotation);
    }
}
