using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体ボトルのスクリプト。
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LiquidBottle : UdonSharpBehaviour
    {
        #region SerializeFields
        [SerializeField]
        [Tooltip("液体ボトルのキャップ")]
        GameObject cap;

        [SerializeField]
        [Tooltip("液体を表示する閾値")]
        float threshold = 90f;
        #endregion

        #region PrivateFields
        /// <summary>
        /// 液体ボトルの管理
        /// </summary>
        LiquidBottleManager _manager;

        /// <summary>
        /// VRCPickup
        /// </summary>
        VRCPickup _pickup;
        #endregion

        /// <summary>
        /// 初期化。
        /// </summary>
        void Start()
        {
            _pickup = (VRCPickup)GetComponent(typeof(VRCPickup));
        }

        /// <summary>
        /// セットアップ。
        /// </summary>
        /// <param name="manager">液体ボトル管理</param>
        public void Setup(LiquidBottleManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        void Update()
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (cap.activeSelf) return;
            if (!_pickup.IsHeld) return;

            // normalize angle
            var qrot = Quaternion.LookRotation(transform.position - cap.transform.position);
            var angle = qrot.eulerAngles.x <= 90f ? 90f - qrot.eulerAngles.x : 90f;
            _manager.SetActiveStream(angle >= threshold);
        }

        /// <summary>
        /// Drop時に発生するイベント。
        /// </summary>
        public override void OnDrop()
        {
            _manager.OnDropBottle();
        }

        /// <summary>
        /// Pickup中にUseした際に発生するイベント。
        /// </summary>
        public override void OnPickupUseDown()
        {
            _manager.SwitchCap();
        }
    }
}