
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class DoubleWeightSlider : UdonSharpBehaviour
{
    [Header("Slider goes here")]
    public Slider sldr;

    [Header("Profiles, First is High")]
    public PostProcessVolume ppHigh;
    public PostProcessVolume ppLow;

    [Header("number display for slider")]
    public TextMeshProUGUI numberDisplay;
    public override void Interact()
    {
        if (sldr.value > 0)
        {
            ppHigh.weight = sldr.value;
            ppLow.weight = 0;
        }
        else
        {
            ppHigh.weight = 0;
            ppLow.weight = Mathf.Abs(sldr.value);
        }
        numberDisplay.text = sldr.value.ToString("F2");
    }
}
