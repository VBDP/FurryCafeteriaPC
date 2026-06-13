
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LocalPlayerTime : UdonSharpBehaviour
{
    public TextMeshProUGUI timethingy;
    void Start()
    {
        UpdateLocalPLayerTime();
    }

    public void UpdateLocalPLayerTime() 
    { 
        timethingy.text = DateTime.UtcNow.ToString();
        SendCustomEventDelayedSeconds("UpdateLocalPLayerTime", 1);
    }
}
