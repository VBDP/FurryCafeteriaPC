using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Shiokai.NightModeTimer.Core
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class NightModeSwitcher : UdonSharpBehaviour
    {
        [SerializeField] private TimePointerBase _dawnTimePointer;
        [SerializeField] private TimePointerBase _duskTimePointer;
        [SerializeField][Range(0, 24)] private float _defaultDawnTime = 6.0f;
        [SerializeField][Range(0, 24)] private float _defaultDuskTime = 18.0f;

        [SerializeField] private double _switchingDuration = 5.0;

        [SerializeField] private Renderer _nightModeRenderer;
        [SerializeField] private string _nightModePropertyName = "_NightModeLevel";
        [SerializeField][Range(0, 1)] private float _nighttimeNightModeLevel = 0.1f;


        [SerializeField] private Light[] _lights = new Light[0];
        [SerializeField] private float _nighttimeLightIntensityMultiplier = 0.5f;

        private readonly float _daytimeNightModeLevel = 1.0f;
        private MaterialPropertyBlock _nightModePropertyBlock;
        private int _nightModePropertyID;

        private float[] _defaultLightIntensities = new float[0];

        private float _dawnTime = 6.0f;
        private float _duskTime = 18.0f;

        private DateTime _switchStartTime;
        private DateTime _switchEndTime;

        /// <summary>
        /// 夜間のNight Mode Level。
        /// 値は0~1の範囲にClampされます。
        /// </summary>
        /// <value>夜間のNight Mode Level</value>
        public float NightModeLevel
        {
            get => _nighttimeNightModeLevel;
            set
            {
                _nighttimeNightModeLevel = Mathf.Clamp01(value);
                ApplyNightModeLevel();
            }
        }

        /// <summary>
        /// 夜間のライト強度の倍率。
        /// 0以下の値は0になります。
        /// </summary>
        /// <value>夜間のライト強度の倍率</value>
        public float LightIntensityMultiplier
        {
            get => _nighttimeLightIntensityMultiplier;
            set
            {
                _nighttimeLightIntensityMultiplier = Mathf.Max(value, 0);
                ApplyLightIntensities();
            }
        }

        /// <summary>
        /// 日中/夜間の状態遷移にかかる時間 (秒)
        /// </summary>
        /// <value>状態遷移にかかる時間 (s)</value>
        public double SwitchingDuration
        {
            get => _switchingDuration;
            set
            {
                _switchingDuration = value;
                var currentRate = ElapsedRate;
                _switchStartTime = DateTime.Now.AddSeconds(-_switchingDuration*currentRate);
                _switchEndTime = DateTime.Now.AddSeconds(_switchingDuration*(1.0 - currentRate));
            }
        }

        /// <summary>
        /// 現在が日中か夜間かを示す値です。<br/>
        /// 日中ならtrue、夜間ならfalseを返します。<br/>
        /// 日中と夜間を遷移中の場合、遷移後の状態を返します。
        /// </summary>
        /// <value>現在が日中かどうか</value>
        public bool IsDaytime { get; private set; } = true;

        /// <summary>
        /// 日中と夜間の状態遷移の経過度合。<br/>
        /// 0..1の範囲の値を返します。
        /// 0で未遷移、1で遷移完了、中間値で遷移中を示します。
        /// </summary>
        /// <value>遷移の経過度合</value>
        public float ElapsedRate
        {
            get
            {
                var durationTimeSpan = _switchEndTime - _switchStartTime;

                if (durationTimeSpan.TotalMilliseconds == 0)
                {
                    return 1;
                }

                var elapsedTimeSpan = DateTime.Now - _switchStartTime;

                var elapsedRate = elapsedTimeSpan.TotalMilliseconds / durationTimeSpan.TotalMilliseconds;
                return Mathf.Clamp01((float)elapsedRate);
            }
        }

        private void ApplyLightIntensities()
        {
            if (_lights == null || _defaultLightIntensities.Length == 0)
            {
                return;
            }

            for (int i = 0; i < _lights.Length; i++)
            {
                var light = _lights[i];
                var intensity = _defaultLightIntensities[i];
                if (light == null)
                {
                    continue;
                }

                var nightLevel = IsDaytime ? (1 - ElapsedRate) : ElapsedRate;
                _lights[i].intensity = intensity * Mathf.Lerp(1.0f, _nighttimeLightIntensityMultiplier, nightLevel);

            }
        }

        private void ApplyNightModeLevel()
        {
            if (_nightModeRenderer == null || _nightModePropertyBlock == null)
            {
                return;
            }
            _nightModeRenderer.GetPropertyBlock(_nightModePropertyBlock);

            var nightLevel = IsDaytime ? (1 - ElapsedRate) : ElapsedRate;
            _nightModePropertyBlock.SetFloat(_nightModePropertyID, Mathf.Lerp(_daytimeNightModeLevel, _nighttimeNightModeLevel, nightLevel));
            _nightModeRenderer.SetPropertyBlock(_nightModePropertyBlock);
        }

        private void UpdateElapsed()
        {
            ApplyNightModeLevel();
            ApplyLightIntensities();
        }

        private void InitializeField()
        {

            if (_dawnTimePointer != null)
            {
                _dawnTime = _dawnTimePointer.DefaultTime;
            }
            else
            {
                _dawnTime = _defaultDawnTime;
            }

            if (_duskTimePointer != null)
            {
                _duskTime = _duskTimePointer.DefaultTime;
            }
            else
            {
                _duskTime = _defaultDuskTime;
            }

            if (_nightModeRenderer != null)
            {
                _nightModePropertyBlock = new MaterialPropertyBlock();
                _nightModeRenderer.GetPropertyBlock(_nightModePropertyBlock);
                _nightModePropertyID = VRCShader.PropertyToID(_nightModePropertyName);
            }

            if (_lights != null && _lights.Length != 0)
            {
                _defaultLightIntensities = new float[_lights.Length];
                for (int i = 0; i < _lights.Length; i++)
                {
                    if (_lights[i] == null)
                    {
                        continue;
                    }
                    _defaultLightIntensities[i] = _lights[i].intensity;
                }
            }

            _switchStartTime = DateTime.Now.AddSeconds(-_switchingDuration);
            _switchEndTime = DateTime.Now;

        }

        void Start()
        {
            InitializeField();
        }

        private void Update()
        {
            if (_dawnTimePointer != null)
            {
                _dawnTime = _dawnTimePointer.PointingTime;
            }

            if (_duskTimePointer != null)
            {
                _duskTime = _duskTimePointer.PointingTime;
            }

            var now = DateTime.Now;

            var timeInHour = now.TimeOfDay.TotalMilliseconds / 3600e3;

            // DawnTime から DuskTime までの間に0時をまたがない場合 DuskTime > DawnTime が true となり Diurnal は DawnTime と DuskTime の間。
            // DawnTime から DuskTime までの間に0時をまたぐ場合 DuskTime > DawnTime が false となり Diurnal は DawnTime より後(で24時(0時)より前)の区間と DuskTime より前(で0時より後)の区間。
            // 指定した時刻に「なった」タイミングで切り替わるための、<=と<の使い分けに注意
            // 使い分けを間違えると1分ずれる
            var isDaytime = _duskTime > _dawnTime ?
                            ((_dawnTime <= timeInHour) && (timeInHour < _duskTime)) :
                            ((_dawnTime <= timeInHour) || (timeInHour < _duskTime));

            // 日中/夜間の状態変更がなかった場合
            if (isDaytime == IsDaytime)
            {
                // 状態遷移終了済みの場合
                if (_switchEndTime < now)
                {
                    return;
                }

                // 状態遷移途中の場合
                UpdateElapsed();
                return;
            }

            // 日中/夜間の状態が変化した場合
            IsDaytime = isDaytime;

            // 遷移途中で再度状態が変化した場合
            if (_switchEndTime > now)
            {
                var latestRate = ElapsedRate;
                _switchStartTime = now.AddSeconds(-_switchingDuration * (1.0f - latestRate));
                _switchEndTime = now.AddSeconds(_switchingDuration * latestRate);

                UpdateElapsed();
                return;
            }

            // 遷移終了時に状態が変化した場合
            _switchStartTime = now;
            _switchEndTime = now.AddSeconds(_switchingDuration);

            UpdateElapsed();
        }
    }
}
