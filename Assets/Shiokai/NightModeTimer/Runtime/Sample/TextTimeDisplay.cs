
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using Shiokai.NightModeTimer.Core;

namespace Shiokai.NightModeTimer.Visual
{
    [RequireComponent(typeof(Text))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TextTimeDisplay : UdonSharpBehaviour
    {
        [SerializeField] private TimePointerBase _timePointer;
        private Text _display;

        private string FormatTime(float time)
        {
            int hour = Mathf.FloorToInt(time);
            int minute = Mathf.FloorToInt((time - hour) * 60.0f);
            // D2により最小桁数を2桁に
            string text = hour.ToString("D2") + ":" + minute.ToString("D2");
            return text;
        }
        void Start()
        {
            _display = GetComponent<Text>();
        }

        private void Update()
        {
            if (_display == null)
            {
                return;
            }
            _display.text = FormatTime(_timePointer.PointingTime);
        }
    }
}

