using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体グラスの蓋を制御するスクリプト。
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LiquidGlassLid : UdonSharpBehaviour
    {
        [SerializeField]
        [Tooltip("液体グラスの管理")]
        LiquidGlassManager manager;

        [SerializeField]
        [Tooltip("衝突対象のGameObject")]
        GameObject target;

        /// <summary>
        /// 物体が接触している間に発生するイベント。
        /// </summary>
        /// <param name="other">コライダー</param>
        void OnTriggerStay(Collider other)
        {
            if (target.GetInstanceID() != other.gameObject.GetInstanceID()) return;
            if (!Networking.IsOwner(manager.gameObject)) return;
            manager.DrinkLiquid();
        }
    }
}
