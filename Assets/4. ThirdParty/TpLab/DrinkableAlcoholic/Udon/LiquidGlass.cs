using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using static VRC.SDKBase.Utilities;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体のグラスを監視するスクリプト。
    /// </summary>
    [DefaultExecutionOrder(10)]
    [RequireComponent(typeof(VRCPickup))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LiquidGlass : UdonSharpBehaviour
    {
        #region PrivateFields
        /// <summary>
        /// 液体グラスの管理
        /// </summary>
        LiquidGlassManager _manager;

        /// <summary>
        /// VRCPickup
        /// </summary>
        VRCPickup _pickup;

        /// <summary>
        /// 液体のTransform
        /// </summary>
        Transform _liquid;

        /// <summary>
        /// グラスの蓋を示すTransform
        /// </summary>
        Transform _liquidGlassLid;

        /// <summary>
        /// 液体の水面を示すTransform
        /// </summary>
        Transform _liquidSurface;
        #endregion

        /// <summary>
        /// 初期化。
        /// </summary>
        void Start()
        {
            _pickup = (VRCPickup)GetComponent(typeof(VRCPickup));
            _liquid = transform.Find("Liquid");
            if (!_liquid)
            {
                _liquid = transform;
            }
            _liquidGlassLid = transform.Find("LiquidGlassLid");
            _liquidSurface = transform.Find("LiquidSurface");
#if UNITY_EDITOR
            CheckObject(_liquid, nameof(_liquid));
            CheckObject(_liquidGlassLid, nameof(_liquidGlassLid));
#endif
        }

        /// <summary>
        /// セットアップ。
        /// </summary>
        /// <param name="manager">液体グラスの管理</param>
        public void Setup(LiquidGlassManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// 更新処理。
        /// </summary>
        void Update()
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (!IsValid(_manager)) return;
#if !UNITY_EDITOR
            if (!_pickup.IsHeld) return;
#endif
            if (_manager.AmountRate <= 0f) return;

            _liquidSurface.position = new Vector3(
                _liquidGlassLid.position.x,
                _liquid.position.y + (-_manager.LiquidHeight / 2) + _manager.LiquidHeight * _manager.AmountRate,
                _liquidGlassLid.position.z
            );
            _liquidSurface.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }

        /// <summary>
        /// Pickupした際に発生するイベント。
        /// </summary>
        public override void OnPickup() => _manager.StartPickup();

        /// <summary>
        /// Dropした際に発生するイベント。
        /// </summary>
        public override void OnDrop() => _manager.StopPickup();

        /// <summary>
        /// Pickup中にUseした際に発生するイベント。
        /// </summary>
        public override void OnPickupUseDown() => _manager.Refill();

#if UNITY_EDITOR
        /// <summary>
        /// Objectの設定状態をチェックする。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <param name="fieldName">変数名</param>
        void CheckObject(Object target, string fieldName)
        {
            if (!IsValid(target))
            {
                LogError($"{name}: [{fieldName}]が取得できませんでした。");
            }
        }

        /// <summary>
        /// エラーログを出力する。
        /// </summary>
        /// <param name="message">メッセージ</param>
        void LogError(string message)
        {
            Debug.LogError($"[<color=#4EC9B0>{GetType().Name}</color>] {message}");
        }
#endif
    }
}
