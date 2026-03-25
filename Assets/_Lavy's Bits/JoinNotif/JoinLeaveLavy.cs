
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class JoinLeaveLavy : UdonSharpBehaviour
{
    public TextMeshProUGUI pname;
    public TextMeshProUGUI announcement;

    public string joinText = "Has arrived!";
    public string leaveText = "Has left.";

    public Transform theThingy;
    public Animator anim;

    public ParticleSystem partJoin;
    public ParticleSystem partLeave;
    public AudioSource audioSource;
    public AudioClip joinClip;
    public AudioClip leaveClip;
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        pname.text = player.displayName;
        announcement.text = joinText;
        theThingy.position = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        theThingy.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        theThingy.position = theThingy.position + theThingy.forward;
        partJoin.Play();
        anim.SetTrigger("cum");
        audioSource.PlayOneShot(joinClip);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        pname.text = player.displayName;
        announcement.text = leaveText;
        theThingy.position = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        theThingy.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        theThingy.position = theThingy.position + theThingy.forward;
        partLeave.Play();
        anim.SetTrigger("cum");
        audioSource.PlayOneShot(leaveClip);
    }
}
