
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class localFps : UdonSharpBehaviour
{
    public TextMeshProUGUI fps;
    private void Update()
    {
        int fpsValue = (int)(1f / Time.deltaTime);
        fps.text = fpsValue.ToString() + " fps";
    }
}
