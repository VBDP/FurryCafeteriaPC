
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Shiokai.NightModeTimer.Core
{
    public abstract class TimePointerBase : UdonSharpBehaviour
    {
        /// <summary>
        /// このTimePointerが現在示している時刻。
        /// 単位はHour。
        /// </summary>
        /// <value>時刻(Hour)</value>
        public abstract float PointingTime { get; }
        /// <summary>
        /// このTimerPointerが初期状態で示している時刻
        /// </summary>
        public float DefaultTime => _defaultTime;
        /// <summary>
        /// このTimerPointerが初期状態で示している時刻<br/>
        /// このTimePointerがNightModeSwitcherに与えられている場合、NightModeSwitcherに指定されたDefault Dawn/Dusk Timeが与えられます。<br/>
        /// Runtimeでの値の変更は想定していません。
        /// </summary>
        [Range(0, 24)]
        [SerializeField] private float _defaultTime;

    }
}
