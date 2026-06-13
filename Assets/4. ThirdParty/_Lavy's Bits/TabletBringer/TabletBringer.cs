
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class TabletBringer : UdonSharpBehaviour
{
    [Header("Put tablet here")]
    public Transform objToTp;

    [Header("You don't have to care about this")]
    public Transform canvas;
    public Image fillViz;
    public Image fade;
    public Color fadeCol;
    public Animator vizActivity;
    public GameObject vrIndic;
    public GameObject desktopIndic;

    bool isInVr = false;

    float filltracker = 0;
    bool fillCalc = false;
    private void Start()
    {
        isInVr = Networking.LocalPlayer.IsUserInVR();

        if (isInVr)
        {
            vrIndic.SetActive(true);
            desktopIndic.SetActive(false);
        } else
        {
            vrIndic.SetActive(false);
            desktopIndic.SetActive(true);
        }
    }
    public override void PostLateUpdate()
    {
        canvas.rotation = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        canvas.position = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position + canvas.forward;

        if (!isInVr && Input.GetKey(KeyCode.DownArrow))
        {
            _FillCalcAction();
        }

        if (fillCalc)
        {
            filltracker -= Time.deltaTime * .5f;

            fillViz.fillAmount = filltracker;
            fade.color = fadeCol * new Color(1, 1, 1, filltracker);

            if (filltracker >= 1)
            {
                filltracker = 1;

                objToTp.position = Vector3.Lerp(objToTp.position, canvas.position, Time.deltaTime * 8);
                objToTp.rotation = Quaternion.Lerp(objToTp.rotation, canvas.rotation, Time.deltaTime * 8);
            }

            if (filltracker <= 0)
            {
                filltracker = 0;
                fillCalc = false;
                vizActivity.SetBool("IsActive", false);
            }
        }
    }

    public override void InputLookVertical(float value, UdonInputEventArgs args)
    {
        if (isInVr && value <= -.6f)
        {
            _FillCalcAction();
        }
    }

    public void _FillCalcAction()
    {
        fillCalc = true;
        vizActivity.SetBool("IsActive", true);
        filltracker += Time.deltaTime * 2;
    }
}
