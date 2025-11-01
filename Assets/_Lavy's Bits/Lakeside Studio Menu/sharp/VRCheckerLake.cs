
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VRCheckerLake : UdonSharpBehaviour
{
    public UdonBehaviour[] butts;
    void Start()
    {
        if (Networking.LocalPlayer.IsUserInVR())
        {
            for (int i = 0; i < butts.Length; i++)
            {
                butts[i].SetProgramVariable("currentState", true);
                butts[i].SendCustomEvent("PPToggle");
            }
        } else
        {
            for (int i = 0; i < butts.Length; i++)
            {
                butts[i].SetProgramVariable("currentState", false);
                butts[i].SendCustomEvent("PPToggle");
            }
        }
    }
}
