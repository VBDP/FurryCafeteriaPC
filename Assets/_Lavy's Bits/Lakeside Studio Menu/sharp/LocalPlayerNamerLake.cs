
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LocalPlayerNamerLake : UdonSharpBehaviour
{
    public TextMeshProUGUI namer;
    void Start()
    {
        namer.text = "Welcome, " + Networking.LocalPlayer.displayName;
    }
}
