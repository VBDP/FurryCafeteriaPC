using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerPositionSyncer : UdonSharpBehaviour
{
    // The place to store the player's position and rotation
    [UdonSynced]
    private Vector3 _Position;
    [UdonSynced]
    private Quaternion _Rotation;
    
    // The player that this script is attached to
    private VRCPlayerApi _owner;
    
    // if the data is restored, teleport the player to the restored position
    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if(!player.isLocal) return;
        
        player.TeleportTo(_Position, _Rotation);
    }

    private void Start()
    {
        if (Networking.LocalPlayer.IsOwner(gameObject))
        {
            _owner = Networking.LocalPlayer;
            SendCustomEventDelayedSeconds(nameof(UpdatePosition), 0.5f);
        }
    }
    
    // get the player's position and rotation every 0.5 seconds and store it in the player data
    public void UpdatePosition()
    {
        if (_owner.IsPlayerGrounded()) // only update the position if the player is grounded
        {
            _Position = _owner.GetPosition();
            _Rotation = _owner.GetRotation();
            RequestSerialization();
        }
        SendCustomEventDelayedSeconds(nameof(UpdatePosition), 0.5f);
    }
}
