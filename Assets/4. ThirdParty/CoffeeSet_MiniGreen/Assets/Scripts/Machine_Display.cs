
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Machine Display")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Machine_Display : UdonSharpBehaviour
    {
        public float time_Delay = 5;
        float time_Next = 0;

        [UdonSynced] string strDisplay = "";

        void Update()
        {
            if (strDisplay == "") return;
            
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;

            if (Time.time < time_Next) return;

            strDisplay = "";
            RequestSerialization();
            LocalUpdate();
        }

        public override void OnDeserialization() => LocalUpdate();
        void LocalUpdate()
        {
            GetComponentInChildren<TextMeshProUGUI>().text = strDisplay;
        }

        /// <summary> Should be called by Owner only </summary>
        public void SetText(string value, float time)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) Networking.SetOwner(Networking.LocalPlayer, gameObject);
            strDisplay = value;
            time_Next = Time.time + time;
            RequestSerialization();
            LocalUpdate();
        }
    }
}