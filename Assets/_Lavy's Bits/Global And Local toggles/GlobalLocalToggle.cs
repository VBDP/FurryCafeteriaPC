
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GlobalLocalToggle : UdonSharpBehaviour
{
    [UdonSynced]
    bool globalSync = false;

    bool localSync = false;

    [Header("To toggle locally, call _LocalToggle, otherwise call _GlobalToggle")]
    public GameObject[] toOn;
    public GameObject[] toOff;

    [Header("Global Only Toggles")]
    public GameObject[] toOnGlobal;
    public GameObject[] toOffGlobal;

    [Header("Local Only Toggles")]
    public GameObject[] toOnLocal;
    public GameObject[] toOffLocal;

    [Header("Animator with a bool parameter")]
    public Animator anim;
    public string paramName;
    void Start()
    {
        _ProcessToggle();
    }

    void _ProcessToggle()
    {
        bool orOp = false;

        if (globalSync || localSync)
        {
            orOp = true;
        }

        //both
        for (int i = 0; i < toOn.Length; i++)
        {
            toOn[i].SetActive(orOp);
        }

        for (int i = 0; i < toOff.Length; i++)
        {
            toOff[i].SetActive(!orOp);
        }

        //lcl
        for (int i = 0; i < toOnLocal.Length; i++)
        {
            toOnLocal[i].SetActive(localSync);
        }

        for (int i = 0; i < toOffLocal.Length; i++)
        {
            toOffLocal[i].SetActive(!localSync);
        }

        //glb
        for (int i = 0; i < toOnGlobal.Length; i++)
        {
            toOnGlobal[i].SetActive(globalSync);
        }

        for (int i = 0; i < toOffGlobal.Length; i++)
        {
            toOffGlobal[i].SetActive(!globalSync);
        }

        anim.SetBool(paramName, orOp);
    }

    public void _LocalToggle()
    {
        localSync = !localSync;
        _ProcessToggle();
    }

    public void _GlobalToggle()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);

        globalSync = !globalSync;

        RequestSerialization();
        _ProcessToggle();
    }

    public override void OnDeserialization()
    {
        _ProcessToggle();
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        RequestSerialization();
    }
}
