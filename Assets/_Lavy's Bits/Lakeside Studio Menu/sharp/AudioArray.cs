
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class AudioArray : UdonSharpBehaviour
{
    public AudioSource[] sources;
    public Slider vol;
    public TextMeshProUGUI display;
    public override void Interact()
    {
        for (int i = 0; i < sources.Length; i++) 
        {
            sources[i].volume = vol.value;
        }
        display.text = vol.value.ToString("F2");
    }
}
