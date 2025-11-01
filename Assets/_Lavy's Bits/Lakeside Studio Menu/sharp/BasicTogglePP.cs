
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BasicTogglePP : UdonSharpBehaviour
{
    public bool currentState = false;

    [Header("First State Name & Color")]
    public string st1N;
    public Color st1C;

    [Header("Second State Name & Color")]
    public string st2N;
    public Color st2C;

    [Header("Text to display")]
    public TextMeshProUGUI display;

    [Header("Profiles to toggle between")]
    public GameObject pp1;

    [Header("Sound")]
    public AudioSource tick;
    public void PPToggle()
    {
        currentState = !currentState;
        pp1.SetActive(currentState);
        tick.Play();
        if (currentState)
        {
            display.text = st1N; display.color = st1C;
        } else
        {
            display.text = st2N; display.color = st2C;
        }
    }
}
