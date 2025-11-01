
using UdonSharp;
using UnityEngine;
using VRC.Udon;
using VRC.SDK3.Data;

namespace VRCCoffeeSet
{
    #region Structs

    public enum TypeMachine
    {
        NULL, Grinder, Espresso, Ice
    }

    public enum TypeMachineE
    {
       NULL, G_D, E_DF, E_DC, E_DP, E_W, ICE
    }

    public enum TypeTool
    {
        NULL, Filter, Pitcher, KnockBox, Tamper, Straw, Ingredient, Squeezer
    }
    
    #endregion

    [AddComponentMenu("MiniGreen/Coffee Set/Main Controller")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MainController : UdonSharpBehaviour
    {
        /// <summary> 0_Ade, 1_Boba </summary>
        public UdonBehaviour[] m_udonDLC = new UdonBehaviour[]{null, null};
        public string[] m_langJson;
        public byte m_langCurrent;
        public DataDictionary m_langData;

        public AudioSource[] audios;
        public AudioClip[] clip_PlatePut;
        uint m_soundMuteCount = 0;

        [Header("Settings")]
        public float m_pickupdelay = 0.5f;
        public float m_extractDelay = 5.5f;

        [Header("Machines")]
        public Machine[] m_machine;
        public Machine_Component[] m_machineF;
        public Machine_Component[] m_machineP;

        [Header("Tools")]
        public Tool[] tool_tamper;
        public Tool[] tool_filter;
        public Tool[] tool_pitcher;
        public Tool[] tool_straw;
        public Tool[] tool_other;

        [Header("Dishes")]
        public Cup_Coffee[] dish_espressoCup;
        public Plate[] dish_espressoPlate;
        public Cup_Coffee[] dish_coffeeCup;
        public Plate[] dish_coffeePlate;
        public Cup_Coffee[] dish_Glass;

        #region Material Variables

        /// <summary> 0_Off, 1_Blue, 2_Green </summary>
        [HideInInspector] public Material[] mat_MachineButton;

        /// <summary> 0_Grinded, 1_Extracted </summary>
        [HideInInspector] public Material[] mat_FilterContent;

        [HideInInspector] public Material[] mat_Ring;
        [HideInInspector] public Material[] mat_Straw;

        [HideInInspector] public Material mat_Empty;
        [HideInInspector] public Material mat_Water;
        [HideInInspector] public Material mat_WaterIce;
        /// <summary> 0_Milk <para></para> Froth : 1_Milk, 2_Latte, 3_Cappuccino </summary>
        [HideInInspector] public Material[] mat_Milk;
        [HideInInspector] public Material mat_MilkIce;

        [HideInInspector] public Material[] mat_Top;
        [HideInInspector] public Material mat_Cream;

        /// <summary> 0_Chocolate <para></para> 1_Caramel </summary>
        [HideInInspector] public Material[] mat_Syrup;

        /// <summary> 0_Espresso <para></para> 1_ChocoMix <para></para> 2_CaramelMix </summary>
        [HideInInspector] public Material[] mat_Espresso;
        /// <summary> 0_Espresso <para></para> 1_ChocoMix </summary>
        [HideInInspector] public Material[] mat_EspressoIce;
        [HideInInspector] public Material mat_Americano;
        [HideInInspector] public Material mat_AmericanoIce;
        [HideInInspector] public Material[] mat_Latte;
        [HideInInspector] public Material mat_LatteIce;
        [HideInInspector] public Material[] mat_Cappuccino;
        [HideInInspector] public Material[] mat_Mocha;
        [HideInInspector] public Material[] mat_MochaArt;
        [HideInInspector] public Material mat_MochaIce;
        /// <summary> 0_CaramelMilk <para></para> 1_Macchiato </summary>
        [HideInInspector] public Material[] mat_Macchiato;
        [HideInInspector] public Material mat_MacchiatoIce;

        /// <summary> 0-1_Lemon, 2-3_Grape, 4-5_Grapefruit </summary>
        [HideInInspector] public Material[] mat_Ade;

        /// <summary> 0-9_Pearls </summary>
        [HideInInspector] public Material[] mat_BobaPearls;
        /// <summary> 0_BrownSugar </summary>
        [HideInInspector] public Material[] mat_BobaDrizzle;
        /// <summary> 0_Milk, 1_Coffee </summary>
        [HideInInspector] public Material[] mat_BobaBrownSugar;

        #endregion

        private void Update()
        {
            CheckSoundVolume();
        }

        #region Reset Position

        public void Reset_All()
        {
            Reset_Tool();
            Reset_Object();
            Reset_Cup();
            Reset_Plate();
        }

        public void Reset_Tool()
        {
            foreach (var item in tool_tamper) item.Call_Reset();
            foreach (var item in tool_filter) item.Call_Reset();
            foreach (var item in tool_pitcher) item.Call_Reset();
            foreach (var item in tool_straw) item.Call_Reset();
        }

        public void Reset_Object()
        {
            foreach (var item in tool_other) item.Call_Reset();
        }

        public void Reset_Cup()
        {
            foreach (var item in dish_coffeeCup) item.Call_Reset();
            foreach (var item in dish_espressoCup) item.Call_Reset();
            foreach (var item in dish_Glass) item.Call_Reset();
        }

        public void Reset_Plate()
        {
            foreach (var item in dish_coffeePlate) item.Call_Reset();
            foreach (var item in dish_espressoPlate) item.Call_Reset();
        }

        #endregion

        #region Sound Proof

        public void SoundMute(bool value)
        {
            if (value) m_soundMuteCount++;
            else m_soundMuteCount--;
            Debug.Log(m_soundMuteCount);
        }
        void CheckSoundVolume()
        {
            if (audios == null) return;
            if (audios.Length == 0) return;
            if (m_soundMuteCount > 0 && audios[0].volume > 0)
            {
                float targetVolume = audios[0].volume - Time.deltaTime;
                if (targetVolume < 0) targetVolume = 0;
                foreach (AudioSource item in audios) item.volume = targetVolume;
            }
            else if (m_soundMuteCount == 0 && audios[0].volume < 1)
            {
                float targetVolume = audios[0].volume + Time.deltaTime;
                if (targetVolume > 1) targetVolume = 1;
                foreach (AudioSource item in audios) item.volume = targetVolume;
            }
        }

        #endregion

        #region Language

        public DataDictionary ConvertJson(string json)
        {
            VRCJson.TryDeserializeFromJson(json, out DataToken result);
            return result.DataDictionary;
        }
        public DataDictionary FindLangByCode(string[] jsons, string langCode)
        {
            foreach (var json in jsons)
            {
                var langData = ConvertJson(json);
                if (langData.TryGetValue("langCode", out DataToken result)) if (result.String == langCode) return langData;
            }
            return null;
        }
        public string GetLangValue(DataDictionary data, string key)
        {
            if (data.TryGetValue(key, out DataToken result)) return result.String;
            return null;
        }
        public string GetLangValueFromCurrent(string key)
        {
            if (m_langData.TryGetValue(key, out DataToken result)) return result.String;
            return null;
        }
        public Transform FindTransform(Transform trans, string name, bool preventNull = false)
        {
            foreach (Transform item in trans) if (item.name == name) return item;
            if (preventNull) return trans.GetChild(0);
            else return null;
        }

        #endregion

        #region Functions

        public float MathRemap(float val, float inMin, float inMax, float outMin, float outMax) { return outMin + (val - inMin) * (outMax - outMin) / (inMax - inMin); }
        void AddArray_Tool(ref Tool[] arr1, Tool[] arr2)
        {
            Tool[] newArr = new Tool[arr1.Length + arr2.Length];
            for (int i = 0; i < arr1.Length; i++) newArr[i] = arr1[i];
            for (int i = arr1.Length; i < newArr.Length; i++) newArr[i] = arr2[i - arr1.Length];
            arr1 = newArr;
        }
        //Array.Resize 역할을 하는 함수, array의 type 상관 없이 사용 가능하도록.
        // void AddArray<T>(ref T[] arr1, T[] arr2)
        // {
        //     T[] newArr = new T[arr1.Length + arr2.Length];
        //     for (int i = 0; i < arr1.Length; i++) newArr[i] = arr1[i];
        //     for (int i = arr1.Length; i < newArr.Length; i++) newArr[i] = arr2[i - arr1.Length];
        //     arr1 = newArr;
        // }

        #endregion
    
        #region Editor

        [HideInInspector, Min(0)] public float m_soundRangeMax = 5;
        [HideInInspector] public bool gizmo_visualize;
        [HideInInspector] public Color gizmo_color;

#if !COMPILER_UDONSHARP && UNITY_EDITOR

        [ExecuteInEditMode]
        public void OnDrawGizmosSelected()
        {
            if (gizmo_visualize)
            {
                Gizmos.color = gizmo_color;
                Gizmos.DrawSphere(transform.position, m_soundRangeMax);
                int step = 32;
                for (int i = 0; i < step; i++)
                {
                    float x = Mathf.Cos(Mathf.PI * 2 * i / step) * m_soundRangeMax;
                    float y = Mathf.Sin(Mathf.PI * 2 * i / step) * m_soundRangeMax;
                    float x2 = Mathf.Cos(Mathf.PI * 2 * (i + 1) / step) * m_soundRangeMax;
                    float y2 = Mathf.Sin(Mathf.PI * 2 * (i + 1) / step) * m_soundRangeMax;
                    Color tempColor = gizmo_color;
                    tempColor.a = 1;
                    Debug.DrawLine(transform.position + new Vector3(x, 0, y), transform.position + new Vector3(x2, 0, y2), tempColor);
                    Debug.DrawLine(transform.position + new Vector3(x, y, 0), transform.position + new Vector3(x2, y2, 0), tempColor);
                    Debug.DrawLine(transform.position + new Vector3(0, x, y), transform.position + new Vector3(0, x2, y2), tempColor);
                }

            }
        }
#endif

        #endregion
    }

}