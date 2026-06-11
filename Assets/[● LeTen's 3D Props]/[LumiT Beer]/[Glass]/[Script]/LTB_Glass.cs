using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class LTB_Glass : UdonSharpBehaviour
{
    #region [ Member variable ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    [Header("[ Requirement ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public LTB_Coll LTB_Coll;
    public Animator animator_Glass;

    public VRC_Pickup VRC_Pickup;
    private VRCPlayerApi localPlayer;
    [Header("[ Adjustment ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public bool isClicking;
    public float Liquid_Amount;
    [Header("[ Timer ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    [HideInInspector] public float _timerCount;
    public float Timer;
    [Header("[ Audio ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public AudioSource Audio;
    public AudioClip[] clips;
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ When you get in the world ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ When you get in the world ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    void Start()
    {
        localPlayer = Networking.LocalPlayer;
        _timerCount = Timer;
    }
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ When you Click ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ When you Click ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    public override void OnPickupUseDown()
    {
        isClicking = true;
    }
    public override void OnPickupUseUp()
    {
        isClicking = false;
        _timerCount = Timer;
    }
    #endregion




    void Update()
    {
        /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Timer ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
        #region [ Timer ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
        if (isClicking == true)
        {
            if (_timerCount <= 0)
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Decrease_Liquid_Amount));

                _timerCount = Timer;
            }
            else _timerCount -= Time.deltaTime;
        }
        #endregion
    }



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Adjusting Amount ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ Adjusting Amount ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    public void Decrease_Liquid_Amount()
    {
        if (Liquid_Amount > 0) Liquid_Amount -= 0.01f;
        if (Liquid_Amount < 0) Liquid_Amount = 0;
        LTB_Coll.RequestSerialization();

        animator_Glass.SetFloat("Liquid_Amount", Liquid_Amount);
    }
    public void Increase_Liquid_Amount()
    {
        if (Liquid_Amount < 1) Liquid_Amount += 0.015f;
        if (Liquid_Amount > 1) Liquid_Amount = 1;
        LTB_Coll.RequestSerialization();

        animator_Glass.SetFloat("Liquid_Amount", Liquid_Amount);
    }
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Audio ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ Audio ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    public void CheersSE()
    {
        if (Audio && clips[0] != null) Audio.PlayOneShot(clips[0]);
    }
    public void AcidSE()
    {
        if (Audio && clips[1] != null) Audio.PlayOneShot(clips[1]);
    }
    #endregion
}