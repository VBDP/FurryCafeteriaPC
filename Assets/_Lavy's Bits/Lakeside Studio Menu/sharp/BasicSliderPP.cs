
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class BasicSliderPP : UdonSharpBehaviour
{
    [Header("Slider goes here")]
    public Slider sldr;

    [Header("Profiles")]
    public PostProcessVolume pp1;

    [Header("number display for slider")]
    public TextMeshProUGUI numberDisplay;
    public override void Interact()
    {
        pp1.weight = sldr.value;
        numberDisplay.text = sldr.value.ToString("F2");
    }
}
