using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using static VRC.SDKBase.Utilities;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体とストリームの衝突を制御するスクリプト。
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LiquidCollider : UdonSharpBehaviour
    {
        /// <summary>
        /// 液体グラスの管理
        /// </summary>
        LiquidGlassManager _manager;

        /// <summary>
        /// 対象のストリーム
        /// </summary>
        string[] _targetStreams;

        /// <summary>
        /// セットアップ。
        /// </summary>
        /// <param name="manager">液体グラスの管理</param>
        /// <param name="targetStreams">対象のストリーム</param>
        public void Setup(LiquidGlassManager manager, string[] targetStreams)
        {
            this._manager = manager;
            this._targetStreams = targetStreams;
        }

        /// <summary>
        /// パーティクルがコリジョンに衝突した際に呼ばれるイベント。
        /// </summary>
        /// <param name="other">パーティクルのGameObject</param>
        void OnParticleCollision(GameObject other)
        {
            if (!IsTargetStream(GetTargetName(other))) return;
            if (!Networking.IsOwner(_manager.gameObject)) return;
            _manager.PourLiquid();
        }

        /// <summary>
        /// 対象オブジェクトの名前を取得する。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <returns>名前</returns>
        string GetTargetName(GameObject target)
        {
            return IsValid(target) ? target.name : "";
        }

        /// <summary>
        /// 対象のストリームかどうか判定する。
        /// </summary>
        /// <param name="name">ストリーム名</param>
        /// <returns>対象の場合はtrue、それ以外はfalse</returns>
        bool IsTargetStream(string name)
        {
            if (!IsValid(_targetStreams) || _targetStreams.Length == 0) return true;
            foreach (var target in _targetStreams)
            {
                if (target == name) return true;
            }
            return false;
        }
    }
}
