
using UdonSharp;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PresetSetter : UdonSharpBehaviour
{
    bool isinvr;
    [Header("Only have one as default!")]
    public bool isDefault = false;

    [Header("All of the button/slider refs")]
    public UdonBehaviour[] allSettings;
    public Slider[] allSliders;

    [Header("True = ACES, False = Neutral")]
    public bool tonemapper = true;

    [Header("Exposure Slider")]
    [Range(-1f, 1f)]
    public float exposure = 0;

    [Header("Saturation Slider")]
    [Range(-1f, 1f)]
    public float saturation = 0;

    [Header("Contrast Slider")]
    [Range(-1f, 1f)]
    public float contrast = 0;

    [Header("Temperature Slider, -1 = Cold, +1 = Hot")]
    [Range(-1f, 1f)]
    public float temperature = 0;

    [Header("Tint Slider, -1 = Green, +1 = Purple")]
    [Range(-1f, 1f)]
    public float tint = 0;

    [Header("Hue Slider, all hell let loose.")]
    [Range(-1f, 1f)]
    public float hue = 0;

    [Header("True = JP, False = Realistic")]
    public bool bloomType = true;
    [Range(0, 1f)]
    public float bloomIntensity = 0.5f;

    [Header("True = MSVO, False = SAO")]
    public bool aoType = true;
    [Range(0, 1f)]
    public float aoIntensity = 0;

    [Header("Auto Exposure, it's wild")]
    public bool autoExposure = false;

    [Header("Film Grain, Dizzy vr!")]
    public bool filmGrain = false;
    [Range(0, 1f)]
    public float filmGrainIntensity = .2f;

    [Header("Chromatic Aberration")]
    public bool chromaticAberration = true;
    [Range(0, 1f)]
    public float caIntensity = .2f;

    [Header("Motion Blur")]
    public bool motionBlur = true;

    [Header("Vignette")]
    public bool vignette = true;
    [Range(0, 1f)]
    public float vignetteIntensity = .2f;

    [Header("Visual Dot")]
    public RectTransform dot;
    public RectTransform pos;
    void Start()
    {
        isinvr = Networking.LocalPlayer.IsUserInVR();
        if (isDefault)
        {
            SendCustomEventDelayedSeconds("SendIt", 2);
        }
    }

    public void SendIt()
    {
        allSettings[0].SetProgramVariable("currentState", !tonemapper);
        allSettings[0].SendCustomEvent("PPToggle");

        allSliders[0].value = exposure;

        allSliders[1].value = saturation;

        allSliders[2].value = contrast;

        allSliders[3].value = temperature;

        allSliders[4].value = tint;

        allSliders[5].value = hue;

        allSettings[1].SetProgramVariable("currentState", !bloomType);
        allSettings[1].SendCustomEvent("PPToggle");
        allSliders[6].value = bloomIntensity;

        allSettings[2].SetProgramVariable("currentState", !aoType);
        allSettings[2].SendCustomEvent("PPToggle");
        allSliders[7].value = aoIntensity;

        allSettings[3].SetProgramVariable("currentState", !autoExposure);
        allSettings[3].SendCustomEvent("PPToggle");

        allSettings[4].SetProgramVariable("currentState", !filmGrain);
        allSettings[4].SendCustomEvent("PPToggle");
        allSliders[8].value = filmGrainIntensity;

        if (!isinvr)
        {
            allSettings[5].SetProgramVariable("currentState", !chromaticAberration);
            allSettings[5].SendCustomEvent("PPToggle");
            allSliders[9].value = caIntensity;

            allSettings[6].SetProgramVariable("currentState", !motionBlur);
            allSettings[6].SendCustomEvent("PPToggle");

            allSettings[7].SetProgramVariable("currentState", !vignette);
            allSettings[7].SendCustomEvent("PPToggle");
            allSliders[10].value = vignetteIntensity;
        }

        dot.position = pos.position;
    }
}
