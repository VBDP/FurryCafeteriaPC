
using UnityEngine;
using UdonSharp;
using VRC.Udon;
using VRC.SDKBase;
using VRC.SDK3.Data;
using TMPro;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Menu Plate")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class Menu_Plate : UdonSharpBehaviour
    {
        [Header("DO NOT Change variables")]
        [Header("")]
        public GameObject m_canvas;

        [UdonSynced, HideInInspector] public int m_currentLang = 0;
        int m_currentLangOld = 0;

        void Start()
        {
            SetLang();
        }

        public override void OnDeserialization()
        {
            if (m_currentLang != m_currentLangOld) SetLang();
        }

        public override void OnPickupUseDown()
        {
            var controller = GetComponentInParent<MainController>();
            if (controller == null) return;
            m_currentLang += 1;
            if (m_currentLang >= controller.m_langJson.Length) m_currentLang = 0;
            SetLang();
        }

        void SetLang()
        {
            m_currentLangOld = m_currentLang;
            var controller = GetComponentInParent<MainController>();
            if (controller == null) return;
            DataDictionary langData = controller.ConvertJson( controller.m_langJson[m_currentLang] );
            var font = controller.GetLangValue(langData, "langFont");
            // Find UI by font name
            Transform m_currentFont = null;
            foreach (Transform item in m_canvas.transform)
            {
                var isFontMatch = item.name.Equals(font);
                item.gameObject.SetActive(isFontMatch);
                if (isFontMatch) m_currentFont = item;
            }
            if (!m_currentFont)
            {
                var temp = m_canvas.transform.GetChild(0);
                temp.gameObject.SetActive(true);
                m_currentFont = temp;
            }
            // Set Text
            foreach (var item in m_currentFont.GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (item.name.Contains("|IGNORE|")) continue;
                if (item.name.Contains("|DLCAde|"))
                    if (!controller.m_udonDLC[0]) item.text = controller.GetLangValue(langData, "needDLC");
                    else item.text = controller.GetLangValue(langData, item.name.Replace(@"|DLCAde|", ""));
                else if (item.name.Contains("|DLCBoba|"))
                    if (!controller.m_udonDLC[1]) item.text = controller.GetLangValue(langData, "needDLC");
                    else item.text = controller.GetLangValue(langData, item.name.Replace(@"|DLCBoba|", ""));
                else item.text = controller.GetLangValue(langData, item.name);
            }
        }
    }
}