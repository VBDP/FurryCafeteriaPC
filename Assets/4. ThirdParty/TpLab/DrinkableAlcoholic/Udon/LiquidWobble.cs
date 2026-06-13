/*Please do support www.bitshiftprogrammer.com by joining the facebook page : fb.com/BitshiftProgrammer
Legal Stuff:
This code is free to use no restrictions but attribution would be appreciated.
Any damage caused either partly or completly due to usage this stuff is not my responsibility*/
using UdonSharp;
using UnityEngine;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体を揺らすスクリプト。
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LiquidWobble : UdonSharpBehaviour
    {
        #region SerializeFields
        [SerializeField]
        [Tooltip("最大揺らし幅")]
        float MaxWobble = 0.03f;

        [SerializeField]
        [Tooltip("揺らす速度")]
        float WobbleSpeed = 5.0f;

        [SerializeField]
        [Tooltip("復元率")]
        float RecoveryRate = 1f;

        [SerializeField]
        [Tooltip("液体のMaterialIndex")]
        int liquidMaterialIndex;
        #endregion

        #region PrivateFields
        Renderer _rend;
        Vector3 _prevPos;
        Vector3 _prevRot;
        float _wobbleAmountToAddX;
        float _wobbleAmountToAddZ;
        #endregion

        /// <summary>
        /// 初期化。
        /// </summary>
        void Start()
        {
            _rend = GetComponent<Renderer>();

            // save the last position
            _prevPos = transform.position;
            _prevRot = transform.rotation.eulerAngles;
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        void Update()
        {
            if (_wobbleAmountToAddX == 0f && _wobbleAmountToAddZ == 0f
                && transform.position == _prevPos && transform.rotation.eulerAngles == _prevRot)
            {
                return;
            }

            // decreases the wobble over time
            _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, Time.deltaTime * RecoveryRate);
            _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, Time.deltaTime * RecoveryRate);

            // limit wobble
            _wobbleAmountToAddX = Mathf.Abs(_wobbleAmountToAddX) < 0.001 ? 0f : _wobbleAmountToAddX;
            _wobbleAmountToAddZ = Mathf.Abs(_wobbleAmountToAddZ) < 0.001 ? 0f : _wobbleAmountToAddZ;

            // make a sine wave of the decreasing wobble
            var wobbleAmountX = _wobbleAmountToAddX * Mathf.Sin(WobbleSpeed * Time.time);
            var wobbleAmountZ = _wobbleAmountToAddZ * Mathf.Sin(WobbleSpeed * Time.time);

            // send it to the shader
            _rend.materials[liquidMaterialIndex].SetFloat("_WobbleX", wobbleAmountX);
            _rend.materials[liquidMaterialIndex].SetFloat("_WobbleZ", wobbleAmountZ);

            // Move Speed
            var moveSpeed = (_prevPos - transform.position) / Time.deltaTime;
            var rotationDelta = transform.rotation.eulerAngles - _prevRot;

            // add clamped speed to wobble
            _wobbleAmountToAddX += Mathf.Clamp((moveSpeed.x + (rotationDelta.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
            _wobbleAmountToAddZ += Mathf.Clamp((moveSpeed.z + (rotationDelta.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

            // save the last position
            _prevPos = transform.position;
            _prevRot = transform.rotation.eulerAngles;
        }
    }
}
