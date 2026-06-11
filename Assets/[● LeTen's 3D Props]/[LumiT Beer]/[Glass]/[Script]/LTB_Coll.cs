using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class LTB_Coll : UdonSharpBehaviour
{
    #region [ Member variable ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    [Header("[ Requirement ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    public LTB_Glass LTB_Glass;
    public Animator animator_Glass;

    public bool isFilling;
    public bool IsSounding;
    private VRCPlayerApi localPlayer;
    [Header("[ Timer ]")]/*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    [HideInInspector] public float _timerCount;
    public float Timer;
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ When you get in the world ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ When you get in the world ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        _timerCount = Timer;
    }
    #endregion



    /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ When Particle Hits ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
    #region [ When Particle Hits ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
    void OnParticleCollision(GameObject other)
    {
        if (LTB_Glass.Liquid_Amount < 1)                   // Amount isn't full
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(LTB_SendTo_Increase));
        }
    }




    void Update()
    {
        /*ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ Timer ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ*/
        #region [ Timer ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ ■ ]
        if (LTB_Glass.Liquid_Amount >= 1) isFilling = false;

        if (isFilling == true)
        {
            if (_timerCount <= 0) _timerCount = Timer;
            else _timerCount -= Time.deltaTime;
        } 

        if (IsSounding == true)
        {
            if (LTB_Glass.Liquid_Amount <= 0) IsSounding = false;
        }
        #endregion
    }


    public void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponent<LTB_Glass>() == true || other.GetComponent<LumiT_Bottle>() == true) && isFilling == false)
        {
            LTB_Glass.CheersSE();
        }
    }



    public void LTB_SendTo_Increase()
    {
        isFilling = true;

        LTB_Glass.Increase_Liquid_Amount();
        if (IsSounding == false)
        {
            IsSounding = true;
            LTB_Glass.AcidSE();
        }
    }
    #endregion
}