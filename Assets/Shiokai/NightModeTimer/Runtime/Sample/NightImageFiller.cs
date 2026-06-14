
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.Orbit
{
    [RequireComponent(typeof(Image))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NightImageFiller : UdonSharpBehaviour
    {
        [SerializeField] private TimePointerBase _dawnTimePointer;
        [SerializeField] private TimePointerBase _duskTimePointer;

        private Image _image;

        private void FillNightImage(float dawnPosAngle, float duskPosAngle)
        {
            // 角度の正負を反転させているので、回転軸は-z方向
            _image.rectTransform.localRotation = Quaternion.AngleAxis(duskPosAngle, Vector3.back);

            _image.fillAmount = duskPosAngle > dawnPosAngle ? 1 - (duskPosAngle - dawnPosAngle) / 360f : (dawnPosAngle - duskPosAngle) / 360f;
        }
        void Start()
        {
            _image = GetComponent<Image>();
            if (_image == null || _dawnTimePointer == null || _duskTimePointer == null)
            {
                return;
            }
            // 0~24を-180~180に。ただし0時が0°
            float dawnPosAngle = Mathf.Repeat((_dawnTimePointer.DefaultTime * 360f / 24f) + 180f, 360f) - 180f;
            float duskPosAngle = Mathf.Repeat((_duskTimePointer.DefaultTime * 360f / 24f) + 180f, 360f) - 180f;
            FillNightImage(dawnPosAngle, duskPosAngle);
        }

        private void Update()
        {
            if (_image == null || _dawnTimePointer == null || _duskTimePointer == null)
            {
                return;
            }

            float dawnPosAngle = -Vector2.SignedAngle(Vector2.down, _dawnTimePointer.transform.localPosition);
            float duskPosAngle = -Vector2.SignedAngle(Vector2.down, _duskTimePointer.transform.localPosition);
            FillNightImage(dawnPosAngle, duskPosAngle);
        }
    }
}

