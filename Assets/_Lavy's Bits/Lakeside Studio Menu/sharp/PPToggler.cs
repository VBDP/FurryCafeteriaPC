
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PPToggler : UdonSharpBehaviour
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
    public GameObject pp2;

    [Header("Sound")]
    public AudioSource tick;
    public void PPToggle()
    {
        if (currentState)
        {
            currentState = false;
            display.text = st2N;
            display.color = st2C;
            pp1.SetActive(false);
            pp2.SetActive(true);
        }
        else
        {
            currentState = true;
            display.text = st1N;
            display.color = st1C;
            pp1.SetActive(true);
            pp2.SetActive(false);
        }
        tick.Play();
    }
}
