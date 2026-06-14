
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.Accessory
{
    public enum VariableTypeEnum
    {
        NightModeLevel,
        LightIntensityMultiplier
    }

    [RequireComponent(typeof(Slider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VariableSlider : UdonSharpBehaviour
    {
        [SerializeField] private NightModeSwitcher _nightModeSwitcher;
        [SerializeField] private Text _display;
        [SerializeField] private VariableTypeEnum _variableType = VariableTypeEnum.NightModeLevel;
        [SerializeField] private float _maxValue = 1.0f;
        [SerializeField] private float _minValue = 0.0f;
        [SerializeField] private float _defaultValue = 0.5f;
        private Slider _slider;

        private void SetTargetVariable()
        {
            if (_nightModeSwitcher == null || _slider == null)
            {
                return;
            }

            if (_display != null)
            {
                _display.text = _slider.value.ToString("F2");
            }

            switch (_variableType)
            {
                case VariableTypeEnum.NightModeLevel:
                    _nightModeSwitcher.NightModeLevel = _slider.value;
                    break;
                case VariableTypeEnum.LightIntensityMultiplier:
                    _nightModeSwitcher.LightIntensityMultiplier = _slider.value;
                    break;
                default:
                    break;
            }
        }

        private void InitializeSlider()
        {
            if (_slider == null)
            {
                return;
            }

            float clampedValue = Mathf.Min(Mathf.Max(_defaultValue, _minValue), _maxValue);

            _slider.maxValue = _maxValue;
            _slider.minValue = _minValue;
            _slider.value = clampedValue;
        }

        public void OnSliderValueChanged()
        {
            SetTargetVariable();
        }

        void Start()
        {
            _slider = GetComponent<Slider>();

            InitializeSlider();

            SetTargetVariable();
        }
    }
}
