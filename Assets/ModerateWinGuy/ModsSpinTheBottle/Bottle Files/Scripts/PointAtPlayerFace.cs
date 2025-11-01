using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

class PointAtPlayerFace : UdonSharpBehaviour
{
    public Transform ObjectToPointAtTarget;
    public Transform StraightForward;
    public float MaxAngle;
    public float TurnSpeed;
    private Vector3 playerHead;

    public Transform Offset;

    void Start()
    {
    }

    private void Update()
    {
        playerHead = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        RotateTowards(playerHead);
    }

    public void RotateTowardsAngle(float angle)
    {
        Vector3 euler = ObjectToPointAtTarget.transform.eulerAngles;
        var totalAngle = Mathf.MoveTowardsAngle(euler.y, angle, TurnSpeed * Time.deltaTime);
        Debug.Log($"TotalAngle:{totalAngle}");
        if (MaxAngle > Mathf.Abs( Mathf.DeltaAngle(StraightForward.forward.y,totalAngle )))
        {
            euler.y = totalAngle;
            ObjectToPointAtTarget.transform.eulerAngles = euler;
        }
        Debug.Log($"TotalAngle:{totalAngle}, deltaAngle: {Mathf.Abs(Mathf.DeltaAngle(StraightForward.forward.y, totalAngle))}");

    }

    public void RotateTowards(Vector3 target)
    {
        // Calculate angle towards target
        Vector3 delta = target - transform.position;

        RotateTowardsAngle(Quaternion.LookRotation(delta, ObjectToPointAtTarget.transform.up).eulerAngles.y);
    }
}

