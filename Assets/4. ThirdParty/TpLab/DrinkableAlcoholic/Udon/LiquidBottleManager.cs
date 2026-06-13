using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using static VRC.SDKBase.Utilities;

namespace TpLab.DrinkableAlcoholic.Udon
{
    /// <summary>
    /// 液体ボトルの管理スクリプト。
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LiquidBottleManager : UdonSharpBehaviour
    {
        #region SerializeFields
        [SerializeField]
        [Tooltip("ボトルのキャップ")]
        GameObject cap;

        [SerializeField]
        [Tooltip("ボトルのストリーム")]
        GameObject stream;

        [SerializeField]
        [Tooltip("液体のボトル")]
        LiquidBottle liquidBottle;
        #endregion

        #region UdonSynced
        [UdonSynced]
        [FieldChangeCallback(nameof(IsActiveCap))]
        bool isActiveCap;

        public bool IsActiveCap
        {
            get => isActiveCap;
            set
            {
                isActiveCap = value;
                ApplyCap();
            }
        }

        [UdonSynced]
        [FieldChangeCallback(nameof(IsActiveStream))]
        bool isActiveStream;

        public bool IsActiveStream
        {
            get => isActiveStream;
            set
            {
                isActiveStream = value;
                ApplyStream();
            }
        }
        #endregion

        /// <summary>
        /// 初期化。
        /// </summary>
        void Start()
        {
#if UNITY_EDITOR
            CheckSerializeObject(cap, nameof(cap));
            CheckSerializeObject(stream, nameof(stream));
            CheckSerializeObject(liquidBottle, nameof(liquidBottle));
#endif
            if (Networking.IsOwner(gameObject))
            {
                isActiveCap = cap.activeSelf;
                isActiveStream = stream.activeSelf;
            }
            liquidBottle.Setup(this);
        }

        /// <summary>
        /// キャップを切り替える
        /// </summary>
        /// <returns>キャップの状態</returns>
        public void SwitchCap()
        {
            if (TakeOwnership())
            {
                IsActiveCap = !IsActiveCap;
                if (IsActiveCap)
                {
                    // キャップが有効な場合、ストリームを無効にする
                    IsActiveStream = false;
                }
                RequestSerialization();
            }
        }

        /// <summary>
        /// ボトルを落とした際に発生するイベント。
        /// </summary>
        public void OnDropBottle()
        {
            if (TakeOwnership())
            {
                IsActiveCap = true;
                IsActiveStream = false;
                RequestSerialization();
            }
        }

        /// <summary>
        /// ストリームの有効状態を設定する。
        /// </summary>
        /// <param name="isActive">有効状態</param>
        public void SetActiveStream(bool isActive)
        {
            if (IsActiveStream == isActive) return;
            if (TakeOwnership())
            {
                IsActiveStream = isActive;
                RequestSerialization();
            }
        }

        /// <summary>
        /// キャップの状態を反映する。
        /// </summary>
        void ApplyCap()
        {
            cap.SetActive(IsActiveCap);
        }

        /// <summary>
        /// ストリームの状態を反映する。
        /// </summary>
        void ApplyStream()
        {
            stream.SetActive(IsActiveStream);
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