using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class LumiT_Dispenser : UdonSharpBehaviour
{
    #region [ Member variable ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    [Header("[ Requirement ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public Animator animator_Dispenser;
    public GameObject Particle_Drink;
    [Header("[ Adjustment ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public bool isOn;
    [Header("[ Audio ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public AudioSource Audio;
    public AudioClip[] clips;
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ When you get in the world ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ When you get in the world ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    void Start()
    {

    }
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ When you Click ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ When you Click ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    public override void Interact()
    {
        if (isOn == false) SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TurnON));
        else SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TurnOFF));
    }

    public void TurnON()
    {
        LeverSE();
        isOn = true;

        Particle_Drink.SetActive(isOn);
        animator_Dispenser.SetBool("Turn On", isOn);
    }
    public void TurnOFF()
    {
        isOn = false;

        Particle_Drink.SetActive(isOn);
        animator_Dispenser.SetBool("Turn On", isOn);
    }
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Audio ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ Audio ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    public void LeverSE()
    {
        if (Audio && clips[0] != null) Audio.PlayOneShot(clips[0]);
    }
    #endregion
}