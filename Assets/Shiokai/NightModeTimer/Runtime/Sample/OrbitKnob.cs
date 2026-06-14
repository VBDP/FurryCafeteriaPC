
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.Udon;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.Orbit
{
    [RequireComponent(typeof(VRCPickup))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OrbitKnob : TimePointerBase
    {
        private float _radius = 0.2f;
        private float _initialZ = 0.0f;
        private Quaternion _initialRotation;

        private bool _isHeld = false;
        private Transform _transform;

        private float _pointingTime;
        public override float PointingTime => _pointingTime;

        public override void OnPickup() => _isHeld = true;

        public override void OnDrop() => _isHeld = false;

        // localPositionなので_transform.SetPositionAndRotationは使わない
        private void InitializeTransform()
        {
            // 0~24を-180~180に。ただし0時が0°
            float angle = Mathf.Repeat((DefaultTime * 360f / 24f) + 180f, 360f) - 180f;
            // SinとCosの引数はradian
            _transform.localPosition = new Vector3(
                -_radius * Mathf.Sin(Mathf.Deg2Rad * angle),
                -_radius * Mathf.Cos(Mathf.Deg2Rad * angle),
                0);

            AdjustRotation();
        }

        private void AdjustPosition()
        {
            Vector3 localPos = _transform.localPosition;
            localPos.z = _initialZ;
            _transform.localPosition = localPos.normalized * _radius;
        }

        private void AdjustRotation()
        {
            // Quadの表が-Z側なので、それに合わせて-Z側から見たときに時計回りで時間が経過するように。
            // SignedAngleの符号は外積正の方向が正。すなわち+Z側から見て時計回りの方向。
            float angle = -Vector2.SignedAngle(Vector2.down, _transform.localPosition);
            _transform.rotation = _initialRotation * Quaternion.AngleAxis(angle, Vector3.back);
        }

        private float CalculateKnobTime()
        {
            // Quadの表が-Z側なので、それに合わせて-Z側から見たときに時計回りで時間が経過するように。
            // SignedAngleの符号は外積正の方向が正。すなわち+Z側から見て時計回りの方向。
            float angle = -Vector2.SignedAngle(Vector2.down, _transform.localPosition);
            // 24時間にするため0~360に変換
            float angle360 = Mathf.Repeat(angle, 360);
            float time = angle360 * 24f / 360;
            return time;
        }

        // 1分未満を切り捨て
        private float FloorTime(float time)
        {

            int hour = Mathf.FloorToInt(time);
            int minute = Mathf.FloorToInt((time - hour) * 60.0f);
            float floorTime = (float)hour + (float)minute / 60.0f;
            return floorTime;
        }
        void Start()
        {
            _transform = transform;
            float angle = -Vector2.SignedAngle(Vector2.down, _transform.localPosition);
            _initialRotation = _transform.rotation * Quaternion.AngleAxis(angle, Vector3.forward);
            _radius = _transform.localPosition.magnitude;
            _initialZ = _transform.localPosition.z;

            InitializeTransform();
            _pointingTime = CalculateKnobTime();
        }

        private void Update()
        {
            if (!_isHeld)
            {
                return;
            }
            AdjustPosition();
            AdjustRotation();
            var time = CalculateKnobTime();
            _pointingTime = FloorTime(time);

        }
    }
}
