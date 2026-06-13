
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UnderwaterHandler : UdonSharpBehaviour
{
    [Header("Visuals to toggle")]
    //xd
    [Header("Whatever this object's Y position is, anything under will get the water effect")]
    public GameObject visuals;

    [Header("Player Head Position")]
    public Transform headTracker;
    void Start()
    {
        visuals.SetActive(false);
    }

    public override void PostLateUpdate()
    {
        if (headTracker.position.y < transform.position.y)
        {
            visuals.SetActive(true);
        } else
        {
            visuals.SetActive(false);
        }
    }
}
