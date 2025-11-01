
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.SDK3.Data;
using VRC.Udon;
using TMPro;

namespace VRCCoffeeSet
{
    [AddComponentMenu("MiniGreen/Coffee Set/Pad")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
    public class Pad : UdonSharpBehaviour
    {
        MainController m_controller;

        [Header("DO NOT Change variables")]
        [Header("")]
        [SerializeField] public GameObject m_canvas;
        [SerializeField] public Text m_textHolder;
        [SerializeField] public Transform m_recipeBtnContainer;
        [SerializeField] public Transform m_recipeSectionContainer;

        Transform m_currentFont;
        RectTransform m_currentMenu;
        RectTransform m_currentSection;
        RectTransform m_currentMenuBtn;

        float m_time = 0;
        bool m_animMenuOpen, m_animMenuClose, m_animSectionNext, m_animSectionBack;
        bool m_isAnim { get{return m_animMenuOpen || m_animMenuClose || m_animSectionNext || m_animSectionBack;} }
        RectTransform m_animRectstore;
        
        void Start()
        {
            m_controller = GetComponentInParent<MainController>();
            m_textHolder.text = "";
            m_canvas.SetActive(false);
            m_canvas.GetComponent<RectMask2D>().enabled = true;
            SetLang();
        }

        void Update()
        {
            if (m_animMenuOpen) AnimMenuOpen();
            if (m_animMenuClose) AnimMenuClose();
            if (m_animSectionNext) AnimSectionNext();
            if (m_animSectionBack) AnimSectionBack();
        }

        public override void OnPickupUseDown()
        {
            m_canvas.SetActive(!m_canvas.activeSelf);
        }

        #region Button

        public void BtnMenuOpen()
        {
            if (m_isAnim) return;
            // Get MainController and Language Font Name
            if (m_controller == null) return;
            // Start Animation
            m_animMenuOpen = true;
            m_currentMenuBtn = m_controller.FindTransform(m_currentFont.GetChild(0), m_textHolder.text).GetComponent<RectTransform>();
            m_currentMenu = m_controller.FindTransform(m_currentFont.GetChild(1), m_textHolder.text).GetComponent<RectTransform>();
            if (m_currentMenu == null || m_currentMenuBtn == null) return;
            // Menu
            m_currentMenu.gameObject.SetActive(true);
            m_currentMenu.anchoredPosition = m_currentMenuBtn.anchoredPosition;
            m_currentMenu.localScale = Vector3.zero;
            // Menu-Header
            m_animRectstore = m_currentMenu.GetChild(m_currentMenu.childCount-1).GetComponent<RectTransform>();
            m_animRectstore.sizeDelta = new Vector2(m_animRectstore.sizeDelta.x, m_currentMenu.sizeDelta.y);
            // Section
            m_currentSection = GetSection(m_currentMenu, "0");
            for (int i = 0; i < m_currentMenu.childCount -1; i++) // minus 1 because of the last child is the Header
                m_currentMenu.GetChild(i).gameObject.SetActive(i == 0);
        }
        public void BtnMenuClose()
        {
            if (m_isAnim) return;
            m_animMenuClose = true;
        }
        public void BtnSectionNext()
        {
            if (m_isAnim) return;
            m_animSectionNext = true;
            // Section Next
            m_animRectstore = GetSection(m_currentMenu, m_textHolder.text.Split('_')[0]);
            m_animRectstore.gameObject.SetActive(true);
            m_animRectstore.anchoredPosition = new Vector2(m_currentMenu.sizeDelta.x, m_animRectstore.anchoredPosition.y);
            // Section-Sub
            var sectionSub = m_textHolder.text.Split('_')[1];
            GameObject tempGo;
            for (int i = 0; i < m_animRectstore.childCount-1; i++) // minus 1 because of the last child is the return button
            {
                tempGo = m_animRectstore.GetChild(i).gameObject;
                tempGo.SetActive( tempGo.name.Equals(sectionSub) );
            }
        }
        public void BtnSectionBack()
        {
            if (m_isAnim) return;
            m_animSectionBack = true;
            // Section Next
            m_animRectstore = GetSection(m_currentMenu, m_textHolder.text);
            m_animRectstore.gameObject.SetActive(true);
            m_animRectstore.anchoredPosition = new Vector2(-m_currentMenu.sizeDelta.x, m_animRectstore.anchoredPosition.y);
        }

        public void ChangeLang()
        {
            // Get MainController
            if (m_controller == null) return;
            // Change Lang
            m_controller.m_langCurrent += 1;
            if (m_controller.m_langCurrent >= m_controller.m_langJson.Length) m_controller.m_langCurrent = 0;
            SetLang();
        }

        public void Reset_All() { if (m_controller) m_controller.Reset_All(); }
        public void Reset_Tool() { if (m_controller) m_controller.Reset_Tool(); }
        public void Reset_Object() { if (m_controller) m_controller.Reset_Object(); }
        public void Reset_Cup() { if (m_controller) m_controller.Reset_Cup(); }
        public void Reset_Plate() { if (m_controller) m_controller.Reset_Plate(); }

        #endregion

        #region  Animation

        // 0:00 = 0
        // 0:05 = 0.083
        // 0:10 = 0.167
        // 0:15 = 0.25
        // 0:20 = 0.333
        // 0:25 = 0.417
        // 0:30 = 0.5
        // 0:35 = 0.583
        // 0:40 = 0.667
        // 0:45 = 0.75
        // 0:50 = 0.833
        // 0:55 = 0.917
        // 1:00 = 1

        void AnimMenuOpen()
        {
            m_time += Time.deltaTime;
            float t_menu = SmoothStepedTimeClamped(m_time, 0.25f);
            float t_header = SmoothStepedTimeClamped(m_time - 0.5f, 0.25f);
            // Animation
            m_currentMenu.anchoredPosition = Vector2.Lerp(m_currentMenuBtn.anchoredPosition, Vector2.zero, t_menu);
            m_currentMenu.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t_menu);
            m_animRectstore.sizeDelta = new Vector2(m_animRectstore.sizeDelta.x, Mathf.Lerp(m_currentMenu.sizeDelta.y, 30, t_header));
            // Finish
            if (m_time >= 0.75f)
            {
                // Reset variables
                m_animMenuOpen = false;
                m_animRectstore = null;
                m_time = 0;
            }
        }
        void AnimMenuClose()
        {
            m_time += Time.deltaTime;
            float t_menu = SmoothStepedTimeClamped(m_time, 0.25f);
            // Animation
            m_currentMenu.anchoredPosition = Vector2.Lerp(Vector2.zero, m_currentMenuBtn.anchoredPosition, t_menu);
            m_currentMenu.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t_menu);
            // Finish
            if (m_time >= 0.25f)
            {
                // Hide menu
                m_currentMenu.gameObject.SetActive(false);
                // Reset variables
                m_animMenuClose = false;
                m_currentMenu = null;
                m_currentMenuBtn = null;
                m_currentSection = null;
                m_time = 0;
            }
        }
        void AnimSectionNext()
        {
            m_time += Time.deltaTime;
            float t_sec = SmoothStepedTimeClamped(m_time, 0.25f);
            // Animation
            m_currentSection.anchoredPosition = Vector2.Lerp(
                new Vector2(0, m_currentSection.anchoredPosition.y),
                new Vector2(-m_currentMenu.sizeDelta.x, m_currentSection.anchoredPosition.y),
                t_sec
            );
            m_animRectstore.anchoredPosition = Vector2.Lerp(
                new Vector2(m_currentMenu.sizeDelta.x, m_animRectstore.anchoredPosition.y),
                new Vector2(0, m_animRectstore.anchoredPosition.y),
                t_sec
            );
            // Finish
            if (m_time >= 0.25f)
            {
                // Hide previous section but keep sub-section
                m_currentSection.gameObject.SetActive(false);
                // Reset variables
                m_animSectionNext = false;
                m_currentSection = m_animRectstore;
                m_animRectstore = null;
                m_time = 0;
            }
        }
        void AnimSectionBack()
        {
            m_time += Time.deltaTime;
            float t_sec = SmoothStepedTimeClamped(m_time, 0.25f);
            // Animation
            m_currentSection.anchoredPosition = Vector2.Lerp(
                new Vector2(0, m_currentSection.anchoredPosition.y),
                new Vector2(m_currentMenu.sizeDelta.x, m_currentSection.anchoredPosition.y),
                t_sec
            );
            m_animRectstore.anchoredPosition = Vector2.Lerp(
                new Vector2(-m_currentMenu.sizeDelta.x, m_animRectstore.anchoredPosition.y),
                new Vector2(0, m_animRectstore.anchoredPosition.y),
                t_sec
            );
            // Finish
            if (m_time >= 0.25f)
            {
                // Hide previous section and all of sub-section
                m_currentSection.gameObject.SetActive(false);
                for (int i = 0; i < m_currentSection.childCount-1; i++) // minus 1 because of the last child is the return button
                    m_currentSection.GetChild(i).gameObject.SetActive(false);
                // Reset variables
                m_animSectionBack = false;
                m_currentSection = m_animRectstore;
                m_animRectstore = null;
                m_time = 0;
            }
        }
        
        #endregion

        void SetLang()
        {
            if (m_controller == null) return;
            // Get Lang Data
            m_controller.m_langData = m_controller.ConvertJson( m_controller.m_langJson[m_controller.m_langCurrent] );
            var font = m_controller.GetLangValueFromCurrent("langFont");
            // Find UI by font name
            m_currentFont = null;
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
                    if (!m_controller.m_udonDLC[0]) item.text = m_controller.GetLangValueFromCurrent("needDLC");
                    else item.text = m_controller.GetLangValueFromCurrent(item.name.Replace(@"|DLCAde|", ""));
                else if (item.name.Contains("|DLCBoba|"))
                    if (!m_controller.m_udonDLC[1]) item.text = m_controller.GetLangValueFromCurrent("needDLC");
                    else item.text = m_controller.GetLangValueFromCurrent(item.name.Replace(@"|DLCBoba|", ""));
                else item.text = m_controller.GetLangValueFromCurrent(item.name);
            }
            // Set Machine Display
            foreach (var machine in m_controller.m_machine)
            {
                Transform display = machine.m_display.transform;
                bool isMatch = false;
                foreach (Transform child in display)
                {
                    var isFontMatch = child.name.Equals(font);
                    child.gameObject.SetActive(isFontMatch);
                    child.GetComponent<TextMeshProUGUI>().text = null;
                    if (isFontMatch) isMatch = true;
                }
                if (!isMatch) display.GetChild(0).gameObject.SetActive(true);
            }
        }

        RectTransform GetSection(RectTransform menu, string name)
        {
            for (int i = 0; i < menu.childCount; i++)
                if (menu.GetChild(i).name == $"Section_{name}") return menu.GetChild(i).GetComponent<RectTransform>();
            return null;
        }
        float SmoothStepedTimeClamped(float time, float duration)
        {
            float t = Mathf.Clamp(time / duration, 0, 1);
            return t * t * (3f - 2f * t);
        }
    }
}