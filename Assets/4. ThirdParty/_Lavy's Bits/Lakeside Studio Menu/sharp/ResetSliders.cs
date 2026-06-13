
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ResetSliders : UdonSharpBehaviour
{
    public Slider sldr;
    public float defaultValue;
    public override void Interact()
    {
        sldr.value = defaultValue;
    }
}
