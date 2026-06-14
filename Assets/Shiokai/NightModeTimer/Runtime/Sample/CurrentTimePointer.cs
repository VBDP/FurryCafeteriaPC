
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using System;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.Visual
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class CurrentTimePointer : TimePointerBase
    {
        private float _radius = 0.2f;
        private Quaternion _initialRotation;
        private Transform _transform;
        private float _pointingTime = 0.0f;
        public override float PointingTime => _pointingTime;
        private void SetCurrentTimePoint()
        {
            float time = (float)DateTime.Now.TimeOfDay.TotalMilliseconds / 3600000.0f;
            _pointingTime = time;

            // 午前正、午後負 -180~180
            float currentPosAngle = Mathf.Repeat((time * 360f / 24f) + 180f, 360f) - 180f;

            _transform.localPosition = new Vector3(
                -_radius * Mathf.Sin(Mathf.Deg2Rad * currentPosAngle),
                -_radius * Mathf.Cos(Mathf.Deg2Rad * currentPosAngle),
                0);
            _transform.rotation = _initialRotation * Quaternion.AngleAxis(currentPosAngle, Vector3.back);
        }

        void Start()
        {
            _transform = transform;
            float angle = -Vector2.SignedAngle(Vector2.down, _transform.localPosition);
            _initialRotation = _transform.rotation * Quaternion.AngleAxis(angle, Vector3.forward);
            _radius = _transform.localPosition.magnitude;
        }

        private void Update()
        {
            SetCurrentTimePoint();
        }
    }
}
