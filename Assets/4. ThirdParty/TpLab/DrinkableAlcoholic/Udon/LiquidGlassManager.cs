using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using static VRC.SDKBase.Utilities;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体の管理スクリプト。
    /// </summary>
    [DefaultExecutionOrder(20)]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LiquidGlassManager : UdonSharpBehaviour
    {
        #region SerializeFields
        [SerializeField]
        [Tooltip("液体グラス")]
        LiquidGlass liquidGlass;

        [SerializeField]
        [Tooltip("液体グラスの蓋")]
        LiquidGlassLid liquidGlassLid;

        [SerializeField]
        [Tooltip("液体水面")]
        GameObject liquidSurface;

        [SerializeField]
        [Tooltip("液体のRenderer")]
        Renderer liquidRenderer;

        [SerializeField]
        [Tooltip("液体のMaterialIndex")]
        int liquidMaterialIndex = 0;

        [SerializeField]
        [Tooltip("液体コライダー(任意)")]
        LiquidCollider liquidCollider;

        [SerializeField]
        [Tooltip("液体を注ぐ対象のストリーム(任意)")]
        string[] targetStreams;

        [SerializeField]
        [Tooltip("氷(任意)")]
        Renderer iceRenderer;

        [SerializeField]
        [Tooltip("泡パーティクル(任意)")]
        ParticleSystem bubbleParticle;

        [SerializeField]
        [Tooltip("泡エミッターの高さ")]
        float bubbleEmitterHeight;

        [SerializeField]
        [Tooltip("液体の初期蓄積率")]
        [Range(0f, 1f)]
        float initAmountRate = 0f;

        [SerializeField]
        [Tooltip("液体の最小蓄積率")]
        [Range(0f, 1f)]
        float minAmountRate = 0f;

        [SerializeField]
        [Tooltip("液体の最大蓄積率")]
        [Range(0f, 1f)]
        float maxAmountRate = 1f;

        [SerializeField]
        [Tooltip("液体を注ぐ速度")]
        [Range(0f, 1f)]
        float pourSpeed = 0.005f;

        [SerializeField]
        [Tooltip("液体を飲む速度")]
        [Range(0f, 1f)]
        float drinkSpeed = 0.001f;

        [SerializeField]
        [Tooltip("Use時に液体を補充する")]
        bool isUseRefill;
        #endregion

        #region UdonSynced
        /// <summary>
        /// 液体の貯蓄率
        /// </summary>
        [UdonSynced]
        [FieldChangeCallback(nameof(AmountRate))]
        float _amountRate;
        public float AmountRate
        {
            get => _amountRate;
            set
            {
                _amountRate = Mathf.Clamp(value, minAmountRate, maxAmountRate);
                ApplyAmountRate(_amountRate);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 液体の高さ
        /// </summary>
        public float LiquidHeight => liquidRenderer.materials[liquidMaterialIndex].GetFloat("_LiquidHeight");
        #endregion

        /// <summary>
        /// 初期化。
        /// </summary>
        void Start()
        {
#if UNITY_EDITOR
            CheckSerializeObject(liquidGlass, nameof(liquidGlass));
            CheckSerializeObject(liquidGlassLid, nameof(liquidGlassLid));
            CheckSerializeObject(liquidSurface, nameof(liquidSurface));
            CheckSerializeObject(liquidRenderer, nameof(liquidRenderer));
#endif
            liquidGlass.Setup(this);
            if (IsValid(liquidCollider))
            {
                liquidCollider.Setup(this, targetStreams);
            }
            liquidSurface.SetActive(false);
            liquidGlassLid.gameObject.SetActive(true);
            if (Networking.IsOwner(gameObject))
            {
                AmountRate = initAmountRate;
            }
            liquidRenderer.materials[liquidMaterialIndex].SetFloat("_MinAmountRate", minAmountRate);

            if (iceRenderer)
            {
                iceRenderer.material.renderQueue = 3001;
            }
            if (bubbleParticle)
            {
                var rend = bubbleParticle.GetComponent<ParticleSystemRenderer>();
                rend.material.renderQueue = 3002;
            }
        }

        /// <summary>
        /// Pickupを開始する。
        /// </summary>
        public void StartPickup()
        {
            TakeOwnership();
            ApplyActive(true);
        }

        /// <summary>
        /// Pickupを停止する。
        /// </summary>
        public void StopPickup()
        {
            ApplyActive(false);
        }

        /// <summary>
        /// 液体の貯蓄率を更新する。
        /// </summary>
        /// <param name="rate">貯蓄率</param>
        public void UpdateAmountRate(float rate)
        {
            if (TakeOwnership())
            {
                AmountRate += rate;
                RequestSerialization();
            }
        }

        /// <summary>
        /// 液体を注ぐ。
        /// </summary>
        public void PourLiquid() => UpdateAmountRate(pourSpeed);

        /// <summary>
        /// 液体を飲む。
        /// </summary>
        public void DrinkLiquid() => UpdateAmountRate(-drinkSpeed);

        /// <summary>
        /// 液体の貯蓄率をリセットする。
        /// </summary>
        public void ResetAmountRate()
        {
            if (TakeOwnership())
            {
                AmountRate = minAmountRate;
                RequestSerialization();
            }
        }

        /// <summary>
        /// 液体を補充する。
        /// </summary>
        public void Refill()
        {
            if (!isUseRefill) return;
            if (TakeOwnership())
            {
                AmountRate = maxAmountRate;
                RequestSerialization();
            }
        }

        /// <summary>
        /// 液体の貯蓄率を反映する。
        /// </summary>
        void ApplyAmountRate(float rate)
        {
            liquidRenderer.materials[liquidMaterialIndex].SetFloat("_AmountRate", rate);

            if (bubbleParticle)
            {
                if (AmountRate > minAmountRate)
                {
                    var shape = bubbleParticle.shape;
                    shape.position = new Vector3(shape.position.x, bubbleEmitterHeight * (AmountRate - 1f) * 0.5f, shape.position.z);
                    shape.scale = new Vector3(shape.scale.x, AmountRate, shape.scale.z);
                    bubbleParticle.gameObject.SetActive(true);
                }
                else
                {
                    bubbleParticle.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 有効状態を反映する。
        /// </summary>
        void ApplyActive(bool isActive)
        {
            liquidSurface.SetActive(isActive);
            liquidGlassLid.gameObject.SetActive(isActive);
        }

        /// <summary>
        /// オーナーを取得する。
        /// </summary>
        /// <returns>オーナー状態</returns>
        bool TakeOwnership()
        {
            if (Networking.IsOwner(gameObject)) return true;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            return Networking.IsOwner(gameObject);
        }

#if UNITY_EDITOR
        /// <summary>
        /// SerializeObjectの設定状態をチェックする。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <param name="fieldName">変数名</param>
        void CheckSerializeObject(Object target, string fieldName)
        {
            if (!IsValid(target))
            {
                LogError($"{name}: SerializeField[{fieldName}]が設定されていません。");
            }
        }

        /// <summary>
        /// エラーログを出力する。
        /// </summary>
        /// <param name="message">メッセージ</param>
        void LogError(string message)
        {
            Debug.LogError($"[<color=#4EC9B0>{GetUdonTypeName()}</color>] {message}");
        }
#endif
    }
}
