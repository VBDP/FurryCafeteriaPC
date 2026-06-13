using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class LumiT_Bottle : UdonSharpBehaviour
{
    #region [ Member variable ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    [Header("[ Requirement ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public GameObject Particle_Drink;
    public GameObject Bottle_Lid;
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
    public override void OnPickupUseDown()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TurnON));
    }
    public override void OnPickupUseUp()
    {
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TurnOFF));
    }

    public void TurnON()
    {
        Particle_Drink.SetActive(true);
        Bottle_Lid.SetActive(false);
        if (Bottle_Lid.activeSelf == true) LidOpenSE();
    }
    public void TurnOFF()
    {
        Particle_Drink.SetActive(false);
    }
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Audio ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ Audio ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    public void LidOpenSE()
    {
        if (Audio && clips[0] != null) Audio.PlayOneShot(clips[0]);
    }
    #endregion
}