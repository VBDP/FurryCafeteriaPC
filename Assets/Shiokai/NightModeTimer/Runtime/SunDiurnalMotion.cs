using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.Accessory
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(Light))]
    public class SunDiurnalMotion : UdonSharpBehaviour
    {
        [SerializeField] private TimePointerBase _dawnTimePointer;
        [SerializeField] private TimePointerBase _duskTimePointer;
        private Transform _transform;
        private Vector3 _initialSunUp;
        // 日の出時の太陽のRotation
        // Scene開始時の回転=>X軸を水平(日の出)に
        private Quaternion _dawnSunRotation;


        // d: day, n: night, c: current
        // |     |  day  | night |
        // | --- | ----- | ----- |
        // | === | ===== | ===== |
        // | d<n | d<c<n | d<n<c |
        // |     |       | c<d<n |
        // | === |====== | ===== |
        // | n<d | n<d<c | n<c<d |
        // |     | c<n<d |       |
        // 日の出0、日の入り180
        private float CalculateSunAngle(float time)
        {
            float angle;
            float dawnTime = _dawnTimePointer.PointingTime;
            float duskTime = _duskTimePointer.PointingTime;
            if (dawnTime < duskTime)
            {
                if (time < dawnTime)
                {
                    // 日の入り後は+180
                    angle = 180.0f * (time + 24.0f - duskTime) / (24.0f - (duskTime - dawnTime)) + 180.0f;
                }
                else if (duskTime < time)
                {
                    // 日の入り後は+180
                    angle = 180.0f * (time - duskTime) / (24.0f - (duskTime - dawnTime)) + 180.0f;
                }
                else
                {
                    angle = 180.0f * (time - dawnTime) / (duskTime - dawnTime);
                }
            }
            else
            {
                if (dawnTime < time)
                {
                    angle = 180.0f * (time - dawnTime) / (24.0f - (dawnTime - duskTime));
                }
                else if (time < duskTime)
                {
                    angle = 180.0f * (time + 24.0f - dawnTime) / (24.0f - (dawnTime - duskTime));
                }
                else
                {
                    // 日の入り後は+180
                    angle = 180.0f * (time - duskTime) / (dawnTime - duskTime) + 180.0f;
                }
            }

            return angle;
        }


        private Quaternion CalculateSunRotation(float time)
        {
            float timeAngle = CalculateSunAngle(time) - 90.0f;
            // Scene配置時のSunのY軸を軸として、日の出からの時間分回転させるQuaternion
            Quaternion timeRot = Quaternion.AngleAxis(timeAngle, _initialSunUp);

            // 日の出時の太陽のRotation(Scene開始時の回転=>X軸を水平(日の出)に)=>時間分回転
            Quaternion sunRot = timeRot * _dawnSunRotation;
            return sunRot;
        }
        private void Start()
        {
            _transform = transform;
            _initialSunUp = _transform.up;
            _transform.rotation = Quaternion.identity;
            // Quaternion.identityからSceneに配置された状態に回転させるQuaternion
            var upRot = Quaternion.FromToRotation(Vector3.up, _initialSunUp);

            // SunのScene配置時のY軸方向周りにSunを回転させたとき、X軸が水平になる時のX軸方向のベクトルを求める。
            // X軸がこの向きになる時を日の出とする。
            // Prove:
            // up := (a, b, c)
            // axis := (x, y, z)
            // dot(up, axis) = 0    (1)
            // y = 0    (2)
            // ||up|| = ||axis|| = 1    (3)
            // とする。
            // (1)より
            // a*x + b*y + c*z = 0
            // これと(2)より
            // a*x + c*z = 0
            // よって
            // z = -a*x/c   (4)
            // (3)より
            // x^2 + y^2 + c^2 = 1
            // これと(2)、(4)より
            // x^2 + a^2*x^2/c^2 = 1
            // よって
            // x^2 = c^2/(b^2 + c^2)
            // これと(3)より
            // x^2 = c^2/(1 - a^2)
            // よってx > 0とすると
            // x = c/sqrt(1 - a^2)    (5)
            // (4)、(5)より
            // z = a/sqrt(1 - a^2)
            // よって
            // axis = (c/sqrt(1 - a^2), 0, a/sqrt(1 - a^2)
            var xAxisHoriz = new Vector3(
                _initialSunUp.z / Mathf.Sqrt(1 - _initialSunUp.x * _initialSunUp.x),
                0,
                -_initialSunUp.x / Mathf.Sqrt(1 - _initialSunUp.x * _initialSunUp.x));

            // Scene配置時のX軸の、X軸水平時への回転
            // Scene配置時のX軸方向のベクトルを、X軸水平方向へ回転させるQuaternion
            var xRot = Quaternion.FromToRotation(upRot * Vector3.right, xAxisHoriz);

            _dawnSunRotation = xRot * upRot;
        }

        private void Update()
        {
            if (_dawnTimePointer == null || _duskTimePointer == null)
            {
                return;
            }

            var timeInHour = (float)DateTime.Now.TimeOfDay.TotalMilliseconds / 3600000.0f;
            _transform.rotation = CalculateSunRotation(timeInHour);

        }

    }
}
