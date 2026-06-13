
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DateNTimeStuffs : UdonSharpBehaviour
{
    DateTime dt;
    public TextMeshProUGUI date;
    public TextMeshProUGUI time;
    public TextMeshProUGUI displayN;

    void Start()
    {
        displayN.text = "Welcome, " + Networking.LocalPlayer.displayName;

        _dtUpdate();
    }

    public void _dtUpdate()
    {
        dt = DateTime.Now;

        date.text = dt.ToString("dddd, dd MMMM yyyy");
        time.text = dt.ToString("hh:mm tt");

        SendCustomEventDelayedSeconds(nameof(_dtUpdate), 10);
    }
}
