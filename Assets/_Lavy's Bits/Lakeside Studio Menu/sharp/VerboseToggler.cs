
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VerboseToggler : UdonSharpBehaviour
{
    public bool currentState = true;

    [Header("Objects to Toggle")]
    public GameObject[] toToggle;
    [Header("Objects to turn ON")]
    public GameObject[] toOn;
    [Header("Objects to turn OFF")]
    public GameObject[] toOff;

    [Header("First State Name & Color")]
    public string st1N;
    public Color st1C;

    [Header("Second State Name & Color")]
    public string st2N;
    public Color st2C;

    [Header("Text to display")]
    public TextMeshProUGUI display;

    [Header("Sound")]
    public AudioSource tick;
    public override void Interact()
    {
        if (toToggle != null)
        {
            for (int i = 0; i < toToggle.Length; i++)
            {
                toToggle[i].SetActive(!toToggle[i].activeSelf);
            }
        }

        if (toOn != null)
        {
            for (int i = 0; i < toOn.Length; i++)
            {
                toOn[i].SetActive(true);
            }
        }

        if (toOff != null)
        {
            for (int i = 0; i < toOff.Length; i++)
            {
                toOff[i].SetActive(false);
            }
        }
        tick.Play();
        currentState = !currentState;
        if (currentState)
        {
            display.text = st1N; display.color = st1C;
        }
        else
        {
            display.text = st2N; display.color = st2C;
        }
    }
}
